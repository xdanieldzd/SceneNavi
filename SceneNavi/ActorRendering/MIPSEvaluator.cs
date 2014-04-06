/*
 * Heavily based on mips-eval.c by spinout (2010-06-13) as used in ZSaten
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.ActorRendering
{
    public class MIPSEvaluator
    {
        public class OvlSections
        {
            public byte[] text { get; private set; }
            public byte[] data { get; private set; }
            public byte[] rodata { get; private set; }
            public byte[] bss { get; private set; }
            public uint textVA { get; private set; }
            public uint dataVA { get; private set; }
            public uint rodataVA { get; private set; }
            public uint bssVA { get; private set; }

            public OvlSections(ROMHandler.ROMHandler rom, ROMHandler.DMATableEntry dma, uint vstart)
            {
                byte[] ovl = new byte[dma.PEnd - dma.PStart];
                Buffer.BlockCopy(rom.Data, (int)dma.PStart, ovl, 0, ovl.Length);

                int indent, secaddr;
                indent = (int)Endian.SwapUInt32(BitConverter.ToUInt32(ovl, ovl.Length - 4));
                secaddr = (ovl.Length - indent);

                text = new byte[Endian.SwapUInt32(BitConverter.ToUInt32(ovl, secaddr))];
                textVA = vstart;
                Buffer.BlockCopy(ovl, (int)textVA, text, 0, text.Length);

                data = new byte[Endian.SwapUInt32(BitConverter.ToUInt32(ovl, secaddr + 4))];
                dataVA = (uint)(textVA + text.Length);
                Buffer.BlockCopy(ovl, (int)dataVA, data, 0, data.Length);

                rodata = new byte[Endian.SwapUInt32(BitConverter.ToUInt32(ovl, secaddr + 8))];
                rodataVA = (uint)(dataVA + data.Length);
                Buffer.BlockCopy(ovl, (int)rodataVA, rodata, 0, rodata.Length);

                bss = new byte[Endian.SwapUInt32(BitConverter.ToUInt32(ovl, secaddr + 12))];
                bssVA = (uint)(rodataVA + rodata.Length);
                Buffer.BlockCopy(ovl, (int)bssVA, bss, 0, bss.Length);
            }
        }

        public class MemoryRegion
        {
            public byte[] Data { get; private set; }
            public uint Address { get; private set; }

            public MemoryRegion(byte[] data, uint adr)
            {
                Address = adr;
                Data = data;
            }
        }

        public class SpecialOp
        {
            public uint Opcode { get; private set; }
            public uint Mask { get; private set; }
            public uint Value { get; private set; }

            public SpecialOp(uint op, uint mask, uint val)
            {
                Opcode = op;
                Mask = mask;
                Value = val;
            }
        }

        public enum ResultLocation { Internal, External, CodeFile };

        public class Result
        {
            public uint OpcodeAddress { get; set; }
            public uint TargetAddress { get; set; }
            public uint[] Arguments { get; set; }

            public Result(uint opadr, uint tgtadr, uint[] args)
            {
                OpcodeAddress = opadr;
                TargetAddress = tgtadr;
                Arguments = args;
            }

            public override string ToString()
            {
                return string.Format("(Loc: {0:X8} Tgt: {1:X8} Args: {2:X8} {3:X8} {4:X8} {5:X8})", OpcodeAddress, TargetAddress, Arguments[0], Arguments[1], Arguments[2], Arguments[3]);
            }
        }

        uint BaseAddress;
        uint[] Registers;
        uint[] Stack;
        int StackPos;
        OvlSections Sections;
        List<MemoryRegion> MemoryMap;
        List<SpecialOp> SpecialOps;

        public List<uint> Watches { get; private set; }
        public List<Result> Results { get; private set; }

        public delegate void RegisterHookDelegate(uint[] regs);
        private RegisterHookDelegate RegisterHook;

        public MIPSEvaluator(ROMHandler.ROMHandler rom, ROMHandler.DMATableEntry dma, uint ramadr, RegisterHookDelegate reghook = null, ushort var = 0)
        {
            BaseAddress = ramadr;

            Registers = new uint[32];
            Stack = new uint[256 * MIPS.SafetyVal];
            StackPos = (int)(128 * MIPS.SafetyVal);

            Sections = new OvlSections(rom, dma, 0);

            MemoryMap = new List<MemoryRegion>();
            MemoryMap.Add(new MemoryRegion(Sections.text, ramadr + Sections.textVA));
            MemoryMap.Add(new MemoryRegion(Sections.data, ramadr + Sections.dataVA));
            MemoryMap.Add(new MemoryRegion(Sections.rodata, ramadr + Sections.rodataVA));
            MemoryMap.Add(new MemoryRegion(Sections.bss, ramadr + Sections.bssVA));

            RegisterHook = reghook;

            SpecialOps = new List<SpecialOp>();
            SpecialOps.Add(new SpecialOp(MIPS.LH((uint)MIPS.Register.R0, 0x1C, (uint)MIPS.Register.A0), MIPS.LH((uint)MIPS.Register.R0, 0x1C, (uint)MIPS.Register.A0), var));
            SpecialOps.Add(new SpecialOp(MIPS.LH((uint)MIPS.Register.R0, 0x1C, (uint)MIPS.Register.S0), MIPS.LH((uint)MIPS.Register.R0, 0x1C, (uint)MIPS.Register.A0), var));

            Watches = new List<uint>();
        }

        public void BeginEvaluation()
        {
            Results = new List<Result>();

            for (int i = 0; i < Sections.text.Length; i += 4)
            {
                Evaluate(Sections.text, i);
                Registers[0] = 0;
            }
        }

        private void Evaluate(byte[] words, int pos)
        {
            uint word = Endian.SwapUInt32(BitConverter.ToUInt32(words, pos));
            uint imm = 0, calcadr = 0, target = 0;

            foreach (SpecialOp sop in SpecialOps)
            {
                if ((word & sop.Mask) == sop.Opcode)
                {
                    if ((word & 0xFC000000) == 0)
                        Registers[MIPS.GetRD(word)] = sop.Value;
                    else
                        Registers[MIPS.GetRT(word)] = sop.Value;
                    return;
                }
            }

            switch ((MIPS.Opcode)((word >> 26) & 0x3F))
            {
                case MIPS.Opcode.JAL:
                    target = MIPS.GetTARGET(word);
                    Evaluate(words, pos + 4);
                    ReportResult(target, pos);
                    Array.Clear(Registers, 1, 15);
                    Array.Clear(Registers, 24, 4);

                    Registers[(int)MIPS.Register.RA] = (BaseAddress & 0xFFFFFF) + (uint)(pos + 4);

                    foreach (MemoryRegion mem in MemoryMap)
                    {
                        if (target > (mem.Address & 0xFFFFFF) && mem.Data.Length + (mem.Address & 0xFFFFFF) > target)
                        {
                            pos = (int)(target - (BaseAddress & 0xFFFFFF));
                            break;
                        }
                    }
                    break;

                case MIPS.Opcode.SLLV:
                    Registers[MIPS.GetRD(word)] = Registers[MIPS.GetRT(word)] << (int)MIPS.GetRS(word);
                    break;

                case MIPS.Opcode.ADDIU:
                    imm = MIPS.GetIMM(word);
                    if ((imm & 0x8000) != 0) imm |= 0xFFFF0000;
                    Registers[MIPS.GetRT(word)] = Registers[MIPS.GetRS(word)] + imm;
                    if (MIPS.GetRT(word) == MIPS.GetRS(word) && MIPS.GetRT(word) == (uint)MIPS.Register.SP) StackPos += (short)imm;
                    break;

                case MIPS.Opcode.LUI:
                    Registers[MIPS.GetRT(word)] = MIPS.GetIMM(word) << 16;
                    break;

                case MIPS.Opcode.ANDI:
                    Registers[MIPS.GetRT(word)] = Registers[MIPS.GetRS(word)] & MIPS.GetIMM(word);
                    break;

                case MIPS.Opcode.ORI:
                    Registers[MIPS.GetRT(word)] = Registers[MIPS.GetRS(word)] | MIPS.GetIMM(word);
                    break;

                case MIPS.Opcode.SW:
                    if (MIPS.GetRS(word) == (uint)MIPS.Register.SP) Stack[StackPos + MIPS.GetIMM(word)] = Registers[MIPS.GetRT(word)];
                    break;
                /*
            case MIPS.Opcode.LH:
                imm = MIPS.GetIMM(word);
                calcadr = imm + Registers[MIPS.GetRS(word)];

                if (MIPS.GetRS(word) == (uint)MIPS.Register.SP)
                {
                    Registers[MIPS.GetRT(word)] = Stack[StackPos + imm];
                    break;
                }

                foreach (MemoryRegion mem in MemoryMap)
                {
                    if (calcadr > mem.Address && mem.Data.Length + mem.Address > calcadr)
                    {
                        Registers[MIPS.GetRT(word)] = (uint)Endian.SwapInt16(BitConverter.ToInt16(mem.Data, (int)(calcadr - mem.Address)));
                        break;
                    }
                }
                break;
                */
                case MIPS.Opcode.LW:
                    imm = MIPS.GetIMM(word);
                    if ((imm & 0x8000) != 0) imm |= 0xFFFF0000;
                    calcadr = imm + Registers[MIPS.GetRS(word)];

                    if (MIPS.GetRS(word) == (uint)MIPS.Register.SP)
                    {
                        Registers[MIPS.GetRT(word)] = Stack[StackPos + imm];
                        break;
                    }

                    foreach (MemoryRegion mem in MemoryMap)
                    {
                        if (calcadr > mem.Address && mem.Data.Length + mem.Address > calcadr)
                        {
                            Registers[MIPS.GetRT(word)] = Endian.SwapUInt32(BitConverter.ToUInt32(mem.Data, (int)(calcadr - mem.Address)));
                            break;
                        }
                    }
                    break;
                /*
            case MIPS.Opcode.LHU:
                imm = MIPS.GetIMM(word);
                if ((imm & 0x8000) != 0) imm |= 0xFFFF0000; //????
                calcadr = imm + Registers[MIPS.GetRS(word)];

                if (MIPS.GetRS(word) == (uint)MIPS.Register.SP)
                {
                    Registers[MIPS.GetRT(word)] = Stack[StackPos + imm];
                    break;
                }

                foreach (MemoryRegion mem in MemoryMap)
                {
                    if (calcadr > mem.Address && mem.Data.Length + mem.Address > calcadr)
                    {
                        Registers[MIPS.GetRT(word)] = Endian.SwapUInt16(BitConverter.ToUInt16(mem.Data, (int)(calcadr - mem.Address)));
                        break;
                    }
                }
                break;
                */
                case MIPS.Opcode.TYPE_R:
                    {
                        switch ((MIPS.Opcode_R)(word & 0x3F))
                        {
                            case MIPS.Opcode_R.SLL:
                                Registers[MIPS.GetRD(word)] = Registers[MIPS.GetRT(word)] << (int)MIPS.GetSA(word);
                                break;
                            case MIPS.Opcode_R.SRA:
                            case MIPS.Opcode_R.SRL: /*test!*/
                                Registers[MIPS.GetRD(word)] = Registers[MIPS.GetRT(word)] >> (int)MIPS.GetSA(word);
                                break;
                            case MIPS.Opcode_R.ADDU:
                                Registers[MIPS.GetRD(word)] = Registers[MIPS.GetRT(word)] + Registers[MIPS.GetRS(word)];
                                break;
                            case MIPS.Opcode_R.SUBU:
                                Registers[MIPS.GetRD(word)] = Registers[MIPS.GetRT(word)] - Registers[MIPS.GetRS(word)];
                                break;
                            case MIPS.Opcode_R.AND:
                                Registers[MIPS.GetRD(word)] = Registers[MIPS.GetRT(word)] & Registers[MIPS.GetRS(word)];
                                break;
                            case MIPS.Opcode_R.OR:
                                Registers[MIPS.GetRD(word)] = Registers[MIPS.GetRT(word)] | Registers[MIPS.GetRS(word)];
                                break;
                            case MIPS.Opcode_R.XOR:
                                Registers[MIPS.GetRD(word)] = Registers[MIPS.GetRT(word)] ^ Registers[MIPS.GetRS(word)];
                                break;
                            case MIPS.Opcode_R.JR:
                                if (MIPS.GetRS(word) == (uint)MIPS.Register.RA)
                                {
                                    Array.Clear(Registers, 1, 15);
                                    Array.Clear(Registers, 24, 4);
                                    pos = (int)Registers[(int)MIPS.Register.RA];
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;

                default:
                    break;
            }

            if (RegisterHook != null) RegisterHook(Registers);
        }

        private void ReportResult(uint target, int pos)
        {
            if (Watches.Count != 0 && Watches.Find(x => (x & 0x0FFFFFFF) == (target & 0x0FFFFFFF)) == 0) return;

            uint[] args = new uint[4];
            for (int i = 0; i < args.Length; i++) args[i] = Registers[(int)MIPS.Register.A0 + i];

            Results.Add(new Result((uint)pos, (target & 0x0FFFFFFF), args));
        }
    }
}
