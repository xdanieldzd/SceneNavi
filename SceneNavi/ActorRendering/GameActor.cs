/*
 * 
 * 
 * DOES NOT YET COMPILE; TODO TODO TODO TODO TODO...
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SceneNavi.ROMHandler;
using SceneNavi.OpenGLHelpers;
using SceneNavi.SimpleF3DEX2;

namespace SceneNavi.ActorRendering
{
    public class GameActor
    {
        public const uint FunctionDisplayList1 = 0x80035260;
        public const uint FunctionDisplayList2 = 0x80093D18;
        public const uint FunctionDisplayList3 = 0x800D0984;
        public const uint FunctionObjectNumber = 0x8009812C;
        public const uint FunctionHierarchyAnim1 = 0x800A457C;
        public const uint FunctionHierarchyAnim2 = 0x800A46F8;

        public ROMHandler.ROMHandler ROM { get; private set; }
        public ActorTableEntry ATEntry { get; private set; }
        public MIPSEvaluator MIPS { get; private set; }

        public class LimbClass
        {
            public Vector3d Translation { get; set; }
            public Vector3d Rotation { get; set; }
            public sbyte Child { get; set; }
            public sbyte Sibling { get; set; }
            public uint DisplayList { get; set; }

            public LimbClass()
            {
                Translation = Vector3d.Zero;
                Rotation = Vector3d.Zero;
                Child = -1;
                Sibling = -1;
                DisplayList = 0;
            }
        }

        public class AddressClass
        {
            public uint Address { get; set; }

            public AddressClass(uint adr)
            {
                Address = adr;
            }

            public override string ToString()
            {
                return string.Format("0x{0:X8}", Address);
            }
        }

        public List<AddressClass> HierarchyAddresses { get; private set; }
        public List<AddressClass> AnimationAddresses { get; private set; }
        public List<LimbClass[]> Hierarchies { get; private set; }
        public List<byte[]> HierarchyMatrices { get; private set; }
        public Vector3d GlobalTranslation { get; private set; }

        public List<ObjectTableEntry> ObjectsUsed { get; private set; }

        public List<DisplayListEx> DisplayLists { get; private set; }

        public int TotalFrames { get; private set; }
        public int CurrentHierarchy { get; set; }
        public int CurrentAnimation { get; set; }
        public int CurrentFrame { get; set; }

        List<ulong> gSPSegmentCmds = new List<ulong>();

        System.IO.TextWriter LogWriter;

        public GameActor(ROMHandler.ROMHandler rom, ActorTableEntry ate)
        {
            ROM = rom;
            ATEntry = ate;

            /* General stuff */
            HierarchyAddresses = new List<AddressClass>();
            AnimationAddresses = new List<AddressClass>();
            Hierarchies = new List<LimbClass[]>();
            HierarchyMatrices = new List<byte[]>();
            GlobalTranslation = Vector3d.Zero;

            /* Object list */
            ObjectsUsed = new List<ObjectTableEntry>();
            ObjectsUsed.Add(ROM.Objects[1]);
            ObjectsUsed.Add(ROM.Objects[2]);
            ObjectsUsed.Add(ROM.Objects[3]);
            ObjectsUsed.Add(ROM.Objects[ATEntry.ObjectNumber]);

            /* Display list list */
            DisplayLists = new List<DisplayListEx>();

            /* Numbers */
            TotalFrames = CurrentHierarchy = CurrentAnimation = CurrentFrame = 0;

            /* Temp DL address list */
            List<uint> dladrs = new List<uint>();

            /* MIPS evaluation of overlay */
            if (ATEntry.StartAddress != 0)
            {
                DMATableEntry ovldma = ROM.Files.Find(x => x.VStart == ATEntry.StartAddress);
                if (ovldma != null)
                {
                    try
                    {
                        /* Create evaluator and begin */
                        //LogWriter = System.IO.File.CreateText("E:\\temp\\sorata\\log.txt");
                        MIPS = new MIPSEvaluator(rom, ovldma, ATEntry.RAMStartAddress, new MIPSEvaluator.RegisterHookDelegate(RegisterChecker), 0);
                        MIPS.BeginEvaluation();
                        //LogWriter.Close();

                        /* Parse results for DL addresses & additional objects */
                        foreach (MIPSEvaluator.Result res in MIPS.Results)
                        {
                            if (res.TargetAddress == (FunctionDisplayList1 & 0xFFFFFF) && res.Arguments[1] != 0)
                                dladrs.Add(res.Arguments[1]);
                            else if (res.TargetAddress == (FunctionDisplayList2 & 0xFFFFFF) && res.Arguments[3] != 0)
                                dladrs.Add(res.Arguments[3]);
                            else if (res.TargetAddress == (FunctionDisplayList3 & 0xFFFFFF) && res.Arguments[0] != 0)
                                dladrs.Add(res.Arguments[0]);

                            if (res.TargetAddress == (FunctionObjectNumber & 0xFFFFFF) && res.Arguments[1] != 0 && (ushort)res.Arguments[1] < ROM.ObjectCount)
                                ObjectsUsed.Add(ROM.Objects[(ushort)res.Arguments[1]]);
                        }

                        /* Log results with address arguments in segments 0x04, 0x05 or 0x06 */
                        //LogSuspiciousResults();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Exception in MIPS evaluator!\n\n" + ex.ToString() + "\n\nPlease notify the developers of this error!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    }
                }
            }

            /* If no hierarchy/animation addresses found, use addresses from MIPS evaluation if any */
            if (MIPS != null && (HierarchyAddresses.Count == 0 || AnimationAddresses.Count == 0))
            {
                foreach (MIPSEvaluator.Result res in MIPS.Results)
                {
                    if (res.TargetAddress == (FunctionHierarchyAnim1 & 0xFFFFFF) ||
                        res.TargetAddress == (FunctionHierarchyAnim2 & 0xFFFFFF))
                    {
                        HierarchyAddresses.Add(new AddressClass(res.Arguments[2]));
                        AnimationAddresses.Add(new AddressClass(res.Arguments[3]));
                    }
                }
            }

            /* Remove garbage zeroes from address results */
            HierarchyAddresses.RemoveAll(x => x.Address == 0);
            AnimationAddresses.RemoveAll(x => x.Address == 0);

            /* Parse objects & scan for hierarchies and animations if needed */
            for (int i = 0; i < ObjectsUsed.Count; i++)
            {
                if (ObjectsUsed[i].DMA != null)
                {
                    byte seg = (byte)(ObjectsUsed[i].DMA.AssumedSegment == 0 ? 0x06 : ObjectsUsed[i].DMA.AssumedSegment);

                    byte[] loadsegdata = new byte[ObjectsUsed[i].DMA.PEnd - ObjectsUsed[i].DMA.PStart];
                    Buffer.BlockCopy(rom.Data, (int)ObjectsUsed[i].DMA.PStart, loadsegdata, 0, loadsegdata.Length);

                    ROM.SegmentMapping.Remove((byte)0x06);
                    ROM.SegmentMapping.Add((byte)0x06, loadsegdata);

                    if (seg == 0x06/* && HierarchyAddresses.Count == 0*/) ScanForHierarchies(seg);
                    if (seg == 0x06/* && AnimationAddresses.Count == 0*/) ScanForAnimations(seg);
                }
            }

            /* Hackish RAM segment generation for facial textures */
            ProcessFacialTextureHack();

            /* Ensure display list address validity */
            dladrs = dladrs.Where(x => ROM.IsAddressSupported(x) == true).ToList();

            /* If still no hierarchy/animation and display list addresses found, search for display lists in segment 0x06 */
            if (HierarchyAddresses.Count == 0 && AnimationAddresses.Count == 0 && dladrs.Count == 0)
                dladrs = ScanForDisplayLists(0x06);

            /* Execute display lists */
            if (dladrs != null)
            {
                foreach (uint dla in dladrs)
                {
                    DisplayListEx dlex = new DisplayListEx(ListMode.Compile);
                    ROM.Renderer.Render(dla);
                    dlex.End();
                    DisplayLists.Add(dlex);
                }
            }

            /* Clean up address results */
            if (HierarchyAddresses.Count != 0 && AnimationAddresses.Count != 0)
            {
                HierarchyAddresses = HierarchyAddresses.Where(x => ROM.IsAddressSupported(x.Address) == true).GroupBy(x => x.Address).Select(x => x.First()).ToList();
                AnimationAddresses = AnimationAddresses.Where(x => ROM.IsAddressSupported(x.Address) == true).GroupBy(x => x.Address).Select(x => x.First()).ToList();
            }

            /* Initial data read */
            ReadHierarchies();
            ReadAnimation(CurrentHierarchy, CurrentAnimation, CurrentFrame);
        }

        public List<uint> ScanForDisplayLists(byte seg)
        {
            if (ROM.SegmentMapping[seg] == null) return null;

            byte[] segdata = (byte[])ROM.SegmentMapping[seg];

            List<uint> dladr = new List<uint>();
            uint ofs, adr;

            ofs = adr = 0;

            /* Find by room+mesh header */
            if (BitConverter.ToUInt32(segdata, 0) == 0x16 || BitConverter.ToUInt32(segdata, 0) == 0x18)
            {
                uint cmd = 0;
                while (ofs < 0x200 && cmd != 0x14)
                {
                    cmd = BitConverter.ToUInt32(segdata, (int)ofs);
                    if (cmd == 0x0A)
                    {
                        uint madr = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)(ofs + 4)));
                        if (ROM.IsAddressSupported(madr) == false) break;
                        byte type = segdata[(madr & 0xFFFFFF)];
                        byte count = segdata[(madr & 0xFFFFFF) + 1];
                        madr = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)((madr & 0xFFFFFF) + 4)));
                        if (type == 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                adr = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)((madr & 0xFFFFFF) + (i * 4))));
                                if (ROM.IsAddressSupported(adr) == true) dladr.Add(adr);
                            }
                        }
                        else if (type == 2)
                        {
                            List<uint> subadr = new List<uint>();
                            for (int i = 0; i < count; i++)
                            {
                                adr = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)((madr & 0xFFFFFF) + (i * 16) + 8)));
                                if (ROM.IsAddressSupported(adr) == true) dladr.Add(adr);
                                adr = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)((madr & 0xFFFFFF) + (i * 16) + 12)));
                                if (ROM.IsAddressSupported(adr) == true) subadr.Add(adr);
                            }
                            dladr.AddRange(subadr);
                        }
                        break;
                    }
                    ofs += 8;
                }
            }

            /* Find by DL calls */
            if (dladr.Count == 0)
            {
                while (ofs < segdata.Length)
                {
                    adr = (uint)(((uint)seg << 24) | ofs);
                    ulong raw = Endian.SwapUInt64(BitConverter.ToUInt64(segdata, (int)ofs));
                    if (((raw >> 56) == (byte)General.UcodeCmds.ENDDL) && ROM.IsAddressSupported((uint)(raw & 0xFFFFFFFF))) dladr.Add((uint)(raw & 0xFFFFFFFF));
                    ofs += 8;
                }
            }

            /* Find by pattern search algorithm */
            if (dladr.Count == 0)
            {
                ofs = 0;
                while (ofs < segdata.Length)
                {
                    adr = (uint)(((uint)seg << 24) | ofs);
                    ulong raw = Endian.SwapUInt64(BitConverter.ToUInt64(segdata, (int)ofs));

                    if (raw == ((ulong)(byte)General.UcodeCmds.RDPPIPESYNC << 56) ||
                        raw == ((ulong)(byte)General.UcodeCmds.RDPTILESYNC << 56) ||
                        (raw >> 32) == ((ulong)(byte)General.UcodeCmds.SETENVCOLOR << 24) ||
                        (raw >> 40) == ((ulong)(byte)General.UcodeCmds.SETPRIMCOLOR << 16) ||
                        (raw >> 40) == ((ulong)(byte)General.UcodeCmds.TEXTURE << 16) ||
                        ((raw >> 56) == ((byte)(byte)General.UcodeCmds.SETTIMG) && ((raw >> 32) & 0xFFFF) == 0) && ROM.IsAddressSupported((uint)(raw & 0xFFFFFFFF)))
                    {
                        if (dladr.Count == 0 || (IsDLEndInBetween(seg, (dladr[dladr.Count - 1] & 0x00FFFFFF), ofs) && IsDLEndInBetween(seg, ofs, (uint)segdata.Length)))
                            dladr.Add(adr);
                    }

                    ofs += 8;
                }
            }

            return dladr;
        }

        bool IsDLEndInBetween(byte seg, uint from, uint to)
        {
            byte[] segdata = (byte[])ROM.SegmentMapping[seg];

            while (from < to)
            {
                if (segdata[from] == (byte)General.UcodeCmds.ENDDL) return true;
                from += 8;
            }

            return false;
        }

        private void LogSuspiciousResults()
        {
            LogWriter = System.IO.File.CreateText("E:\\temp\\sorata\\result.txt");
            foreach (MIPSEvaluator.Result res in MIPS.Results)
            {
                bool log = false;
                for (int i = 0; i < res.Arguments.Length; i++)
                    if ((res.Arguments[i] >> 24) == 0x04 || (res.Arguments[i] >> 24) == 0x05 || (res.Arguments[i] >> 24) == 0x06)
                        log = true;

                if (log) LogWriter.WriteLine(res.ToString());
            }
            LogWriter.Close();
        }

        private void RegisterChecker(uint[] regs)
        {
            for (int i = 0; i < regs.Length; i++)
            {
                uint r = regs[i];

                /* If reg contains MoveWord cmd, param G_MW_SEGMENT... */
                if ((r >> 8) == 0xDB0600)
                {
                    foreach (uint r2 in regs)
                    {
                        /* If other reg contains address in object seg... */
                        if ((r2 >> 24) == 0x06)
                        {
                            /* ...assume we found a gSPSegment cmd w/ base being inside object seg! */
                            ulong cmd = (((ulong)r << 32) | r2);
                            gSPSegmentCmds.Add(cmd);
                            //Console.WriteLine("cmd=={0:X16}", cmd);
                        }
                    }
                }
                //LogWriter.Write("{0}:{1:X8} ", (MIPSHandling.MIPS.Register)i, r);
            }
            //LogWriter.Write('\n');
        }

        private void ProcessFacialTextureHack()
        {
            gSPSegmentCmds = gSPSegmentCmds.Distinct().ToList();
            gSPSegmentCmds.RemoveAll(x => (byte)(x >> 32) == 0);

            foreach (ulong gspseg in gSPSegmentCmds)
            {
                if (ROM.IsAddressSupported((uint)(gspseg & 0xFFFFFFFF)) == false) continue;

                /*byte seg = (byte)((gspseg >> 32) >> 2);
                byte[] data = new byte[Nanami.Segments[(gspseg & 0xFF000000) >> 24].Length - (int)(gspseg & 0xFFFFFF)];
                Buffer.BlockCopy(Nanami.RDRAM, (int)Nanami.Segments[(gspseg & 0xFF000000) >> 24].RAMAddress + (int)(gspseg & 0xFFFFFF), data, 0, data.Length);
                Nanami.Segments[seg] = new NanamiRCP.Components.RAMSegment(Nanami, data, string.Empty, false, ramadr: (uint)(Nanami.RDRAM.Length - 0x10000 - data.Length));*/
            }
        }

        public void Render(bool skeleton = false, uint dladr = 0)
        {
            try
            {
                GL.PushMatrix();
                GL.Scale(0.05, 0.05, 0.05);

                if (Hierarchies.Count > 0)
                {
                    int drawn = 0;
                    DrawLimb(CurrentHierarchy, 0, -1, ref drawn, dladr);

                    if (skeleton == true)
                    {
                        GL.PushMatrix();
                        GL.Translate(GlobalTranslation);
                        DrawSkeleton(CurrentHierarchy, 0, -1);
                        GL.PopMatrix();
                    }
                }
                else if (DisplayLists.Count > 0)
                {
                    foreach (DisplayList dl in DisplayLists) if (dl != null) dl.Render();
                }

                GL.PopMatrix();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        #region Scanning functions

        public void ScanForHierarchies(byte seg)
        {
            int j;
            for (int i = 0; i < Nanami.Segments[seg].Length; i += 4)
            {
                if ((Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i] == seg) && (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 3] & 3) == 0 && (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 4]) != 0)
                {
                    int offset = (int)((Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 1] << 16) | (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 2] << 8) | (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 3]));
                    if (offset < Nanami.Segments[seg].Length)
                    {
                        byte NoPts = Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 4];
                        int offset_end = offset + (NoPts << 2);
                        if (offset_end < Nanami.Segments[seg].Length)
                        {
                            for (j = offset; j < offset_end; j += 4)
                            {
                                if ((Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + j] != seg) || ((Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + j + 3] & 3) != 0) ||
                                    ((int)((Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + j + 1] << 16) | (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + j + 2] << 8) | (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + j + 3])) > Nanami.Segments[seg].Length))
                                    break;
                            }

                            uint adr = (uint)((seg << 24) | i);
                            if (j == i && ROM.IsAddressSupported(adr)) HierarchyAddresses.Add(new AddressClass(adr));
                        }
                    }
                }
            }
        }

        public void ScanForAnimations(byte seg)
        {
            for (int i = 0; i < Nanami.Segments[seg].Length; i += 4)
            {
                if ((i + 15 < Nanami.Segments[seg].Length) &&
                    (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i] == 0) &&
                    (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 1] > 1) &&
                    (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 2] == 0) &&
                    (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 3] == 0) &&
                    (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 4] == seg) &&
                    ((int)((Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 5] << 16) | (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 6] << 8) | (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 7])) < Nanami.Segments[seg].Length) &&
                    (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 8] == seg) &&
                    ((int)((Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 9] << 16) | (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 10] << 8) | (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 11])) < Nanami.Segments[seg].Length) &&
                    (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 12] == 0) &&
                    (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 14] == 0) &&
                    (Nanami.RDRAM[Nanami.Segments[seg].RAMAddress + i + 15] == 0))
                {
                    uint adr = (uint)((seg << 24) | i);
                    if (ROM.IsAddressSupported(adr)) AnimationAddresses.Add(new AddressClass(adr));
                }
            }
        }

        #endregion

        public void ReadHierarchies()
        {
            foreach (AddressClass ac in HierarchyAddresses)
            {
                uint adr = ac.Address;
                if (ROM.IsAddressSupported(adr) == false) continue;

                uint blladr = Endian.SwapUInt32(BitConverter.ToUInt32(Nanami.RDRAM, (int)Nanami.Segments[adr >> 24].RAMAddress + (int)(adr & 0xFFFFFF)));
                if (ROM.IsAddressSupported(blladr) == false) return;

                byte lcount = Nanami.RDRAM[Nanami.Segments[adr >> 24].RAMAddress + (adr & 0xFFFFFF) + 4];
                LimbClass[] limbs = new LimbClass[lcount];

                byte dlcount = lcount;
                if (ROM.IsAddressSupported((adr & 0xFFFFFF) + 8) == true)
                {
                    dlcount = Nanami.RDRAM[Nanami.Segments[adr >> 24].RAMAddress + (adr & 0xFFFFFF) + 8];
                    if (dlcount == 0) dlcount = lcount;
                }
                HierarchyMatrices.Add(new byte[0x40 * dlcount]);

                for (int i = 0; i < limbs.Length; i++)
                {
                    uint cbadr = Endian.SwapUInt32(BitConverter.ToUInt32(Nanami.RDRAM, (int)(Nanami.Segments[blladr >> 24].RAMAddress + (blladr & 0xFFFFFF) + (i << 2))));
                    if (ROM.IsAddressSupported(cbadr) == false) continue;

                    byte cbseg = (byte)(cbadr >> 24);
                    limbs[i] = new LimbClass();
                    limbs[i].Translation = new Vector3d(
                        (double)Endian.SwapInt16(BitConverter.ToInt16(Nanami.RDRAM, (int)(Nanami.Segments[cbseg].RAMAddress + (cbadr & 0xFFFFFF)))),
                        (double)Endian.SwapInt16(BitConverter.ToInt16(Nanami.RDRAM, (int)(Nanami.Segments[cbseg].RAMAddress + (cbadr & 0xFFFFFF) + 2))),
                        (double)Endian.SwapInt16(BitConverter.ToInt16(Nanami.RDRAM, (int)(Nanami.Segments[cbseg].RAMAddress + (cbadr & 0xFFFFFF) + 4))));
                    limbs[i].Child = (sbyte)Nanami.RDRAM[Nanami.Segments[cbseg].RAMAddress + (cbadr & 0xFFFFFF) + 6];
                    limbs[i].Sibling = (sbyte)Nanami.RDRAM[Nanami.Segments[cbseg].RAMAddress + (cbadr & 0xFFFFFF) + 7];
                    limbs[i].DisplayList = Endian.SwapUInt32(BitConverter.ToUInt32(Nanami.RDRAM, (int)(Nanami.Segments[cbseg].RAMAddress + (cbadr & 0xFFFFFF) + 8)));
                }

                Hierarchies.Add(limbs);
            }
        }

        public void ReadAnimation()
        {
            ReadAnimation(CurrentHierarchy, CurrentAnimation, CurrentFrame);
        }

        public void ReadAnimation(int hnumber, int anumber, int fnumber)
        {
            if (hnumber >= Hierarchies.Count || anumber >= AnimationAddresses.Count) return;

            uint AnimationOffset;
            uint RotIndexOffset, RotValOffset;
            int Limit;

            AnimationOffset = AnimationAddresses[anumber].Address;
            if (ROM.IsAddressSupported(AnimationOffset) == false) return;

            RotIndexOffset = Endian.SwapUInt32(BitConverter.ToUInt32(Nanami.RDRAM, (int)(Nanami.Segments[AnimationOffset >> 24].RAMAddress + (AnimationOffset & 0xFFFFFF) + 8)));
            RotValOffset = Endian.SwapUInt32(BitConverter.ToUInt32(Nanami.RDRAM, (int)(Nanami.Segments[AnimationOffset >> 24].RAMAddress + (AnimationOffset & 0xFFFFFF) + 4)));
            TotalFrames = (Endian.SwapUInt16(BitConverter.ToUInt16(Nanami.RDRAM, (int)(Nanami.Segments[AnimationOffset >> 24].RAMAddress + (AnimationOffset & 0xFFFFFF)))) - 1);
            Limit = Endian.SwapUInt16(BitConverter.ToUInt16(Nanami.RDRAM, (int)(Nanami.Segments[AnimationOffset >> 24].RAMAddress + (AnimationOffset & 0xFFFFFF) + 12)));

            if (ROM.IsAddressSupported(RotIndexOffset) == false || ROM.IsAddressSupported(RotValOffset) == false) return;

            ushort gtxidx = Endian.SwapUInt16(BitConverter.ToUInt16(Nanami.RDRAM, (int)(Nanami.Segments[RotIndexOffset >> 24].RAMAddress + (RotIndexOffset & 0xFFFFFF))));
            ushort gtyidx = Endian.SwapUInt16(BitConverter.ToUInt16(Nanami.RDRAM, (int)(Nanami.Segments[RotIndexOffset >> 24].RAMAddress + (RotIndexOffset & 0xFFFFFF) + 2)));
            ushort gtzidx = Endian.SwapUInt16(BitConverter.ToUInt16(Nanami.RDRAM, (int)(Nanami.Segments[RotIndexOffset >> 24].RAMAddress + (RotIndexOffset & 0xFFFFFF) + 4)));

            GlobalTranslation = new Vector3d(
                (double)Endian.SwapInt16(BitConverter.ToInt16(Nanami.RDRAM, (int)(Nanami.Segments[RotValOffset >> 24].RAMAddress + (RotValOffset + (gtxidx * 2) & 0xFFFFFF)))),
                (double)Endian.SwapInt16(BitConverter.ToInt16(Nanami.RDRAM, (int)(Nanami.Segments[RotValOffset >> 24].RAMAddress + (RotValOffset + (gtyidx * 2) & 0xFFFFFF)))),
                (double)Endian.SwapInt16(BitConverter.ToInt16(Nanami.RDRAM, (int)(Nanami.Segments[RotValOffset >> 24].RAMAddress + (RotValOffset + (gtzidx * 2) & 0xFFFFFF)))));

            RotIndexOffset += 6;

            for (int i = 0; i < Hierarchies[hnumber].Length; i++)
            {
                if (ROM.IsAddressSupported((uint)(RotIndexOffset + (i * 6) + 4)) == true)
                {
                    ushort rxidx = Endian.SwapUInt16(BitConverter.ToUInt16(Nanami.RDRAM, (int)(Nanami.Segments[RotIndexOffset >> 24].RAMAddress + (RotIndexOffset & 0xFFFFFF) + (i * 6))));
                    ushort ryidx = Endian.SwapUInt16(BitConverter.ToUInt16(Nanami.RDRAM, (int)(Nanami.Segments[RotIndexOffset >> 24].RAMAddress + (RotIndexOffset & 0xFFFFFF) + (i * 6) + 2)));
                    ushort rzidx = Endian.SwapUInt16(BitConverter.ToUInt16(Nanami.RDRAM, (int)(Nanami.Segments[RotIndexOffset >> 24].RAMAddress + (RotIndexOffset & 0xFFFFFF) + (i * 6) + 4)));

                    if (rxidx >= Limit) rxidx += (ushort)fnumber;
                    if (ryidx >= Limit) ryidx += (ushort)fnumber;
                    if (rzidx >= Limit) rzidx += (ushort)fnumber;

                    uint rxadr = (uint)(RotValOffset + (rxidx * 2));
                    uint ryadr = (uint)(RotValOffset + (ryidx * 2));
                    uint rzadr = (uint)(RotValOffset + (rzidx * 2));

                    if (ROM.IsAddressSupported(rxadr) == true && ROM.IsAddressSupported(ryadr) == true && ROM.IsAddressSupported(rzadr) == true)
                    {
                        Hierarchies[hnumber][i].Rotation = new Vector3d(
                            (double)Endian.SwapInt16(BitConverter.ToInt16(Nanami.RDRAM, (int)(Nanami.Segments[rxadr >> 24].RAMAddress + (rxadr & 0xFFFFFF)))),
                            (double)Endian.SwapInt16(BitConverter.ToInt16(Nanami.RDRAM, (int)(Nanami.Segments[ryadr >> 24].RAMAddress + (ryadr & 0xFFFFFF)))),
                            (double)Endian.SwapInt16(BitConverter.ToInt16(Nanami.RDRAM, (int)(Nanami.Segments[rzadr >> 24].RAMAddress + (rzadr & 0xFFFFFF)))));
                    }
                }
            }

            PrepareMatrixSegment(0x0D, hnumber, anumber);
        }

        public void PrepareMatrixSegment(byte seg, int hnumber, int anumber)
        {
            Matrix4d testmtx = Matrix4d.Identity;
            testmtx = Matrix4d.Mult(Matrix4d.CreateTranslation(GlobalTranslation), testmtx);
            int ofs = 0;
            WriteNextMtx(hnumber, 0, -1, testmtx, ref ofs);

            Nanami.Segments[seg] = new NanamiRCP.Components.RAMSegment(Nanami, HierarchyMatrices[hnumber], string.Empty, false, ramadr: (uint)(Nanami.RDRAM.Length - HierarchyMatrices[hnumber].Length));
        }

        private void WriteNextMtx(int hnumber, sbyte bnumber, sbyte pnumber, Matrix4d testmtx, ref int ofs)
        {
            sbyte child = Hierarchies[hnumber][bnumber].Child;
            sbyte sibling = Hierarchies[hnumber][bnumber].Sibling;

            Matrix4d localmtx = testmtx;
            localmtx = Matrix4d.Mult(Matrix4d.CreateTranslation(Hierarchies[hnumber][bnumber].Translation), testmtx);
            localmtx = Matrix4d.Mult(Matrix4d.CreateRotationZ(MathHelper.DegreesToRadians((float)Hierarchies[hnumber][bnumber].Rotation.Z / 182.0444444f)), localmtx);
            localmtx = Matrix4d.Mult(Matrix4d.CreateRotationY(MathHelper.DegreesToRadians((float)Hierarchies[hnumber][bnumber].Rotation.Y / 182.0444444f)), localmtx);
            localmtx = Matrix4d.Mult(Matrix4d.CreateRotationX(MathHelper.DegreesToRadians((float)Hierarchies[hnumber][bnumber].Rotation.X / 182.0444444f)), localmtx);

            if (Hierarchies[hnumber][bnumber].DisplayList != 0)
            {
                WriteMtxData(localmtx.M11, hnumber, ref ofs);
                WriteMtxData(localmtx.M12, hnumber, ref ofs);
                WriteMtxData(localmtx.M13, hnumber, ref ofs);
                WriteMtxData(localmtx.M14, hnumber, ref ofs);
                WriteMtxData(localmtx.M21, hnumber, ref ofs);
                WriteMtxData(localmtx.M22, hnumber, ref ofs);
                WriteMtxData(localmtx.M23, hnumber, ref ofs);
                WriteMtxData(localmtx.M24, hnumber, ref ofs);
                WriteMtxData(localmtx.M31, hnumber, ref ofs);
                WriteMtxData(localmtx.M32, hnumber, ref ofs);
                WriteMtxData(localmtx.M33, hnumber, ref ofs);
                WriteMtxData(localmtx.M34, hnumber, ref ofs);
                WriteMtxData(localmtx.M41, hnumber, ref ofs);
                WriteMtxData(localmtx.M42, hnumber, ref ofs);
                WriteMtxData(localmtx.M43, hnumber, ref ofs);
                WriteMtxData(localmtx.M44, hnumber, ref ofs);
                ofs += 0x20;
            }

            if (child > -1 && child != bnumber) WriteNextMtx(hnumber, child, bnumber, localmtx, ref ofs);
            if (sibling > -1 && sibling != bnumber) WriteNextMtx(hnumber, sibling, pnumber, testmtx, ref ofs);
        }

        private void WriteMtxData(double val, int hnumber, ref int ofs)
        {
            uint tmp = (uint)(val * 65536.0);
            HierarchyMatrices[hnumber][ofs + 0] = (byte)(tmp >> 24);
            HierarchyMatrices[hnumber][ofs + 1] = (byte)((tmp & 0xFFFFFF) >> 16);
            HierarchyMatrices[hnumber][ofs + 32] = (byte)((tmp & 0xFFFF) >> 8);
            HierarchyMatrices[hnumber][ofs + 33] = (byte)((tmp & 0xFF));
            ofs += 2;
        }

        public void DrawLimb(int hnumber, int bnumber, int pnumber, ref int drawn, uint dladr)
        {
            if (hnumber >= Hierarchies.Count || bnumber >= Hierarchies[hnumber].Length) return;

            if (Hierarchies[hnumber][bnumber].DisplayList != 0)
            {
                ForceMatrixToNanamiLLE((uint)((0x0D << 24) | ((uint)drawn * 0x40)));
                if (dladr == 0 || Hierarchies[hnumber][bnumber].DisplayList == dladr)
                {
                    NanamiRCP.Components.DisplayList dl = Nanami.FindDisplayList(Hierarchies[hnumber][bnumber].DisplayList);
                    if (dl == null)
                        Nanami.ExecuteDisplayList(Hierarchies[hnumber][bnumber].DisplayList);
                    else
                        dl.Render();
                }
                drawn++;
            }

            if (Hierarchies[hnumber][bnumber].Child > -1 && Hierarchies[hnumber][bnumber].Child != bnumber) DrawLimb(hnumber, Hierarchies[hnumber][bnumber].Child, bnumber, ref drawn, dladr);
            if (Hierarchies[hnumber][bnumber].Sibling > -1 && Hierarchies[hnumber][bnumber].Sibling != bnumber) DrawLimb(hnumber, Hierarchies[hnumber][bnumber].Sibling, pnumber, ref drawn, dladr);
        }

        public void ForceMatrixToNanamiLLE(uint adr)
        {
            Matrix4d glmatrix = Matrix4d.Identity;

            if (ROM.IsAddressSupported(adr) == true)
            {
                int ofs = (int)(adr & 0xFFFFFF);
                ushort temp1 = 0, temp2 = 0;
                double[] matrix = new double[16];
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        temp1 = Endian.SwapUInt16(BitConverter.ToUInt16(Nanami.RDRAM, (int)Nanami.Segments[adr >> 24].RAMAddress + ofs));
                        temp2 = Endian.SwapUInt16(BitConverter.ToUInt16(Nanami.RDRAM, (int)Nanami.Segments[adr >> 24].RAMAddress + ofs + 32));
                        matrix[(x * 4) + y] = (double)(((temp1 << 16) | temp2) * (1.0 / 65536.0));
                        ofs += 2;
                    }
                }

                glmatrix = new Matrix4d(
                    matrix[0], matrix[1], matrix[2], matrix[3],
                    matrix[4], matrix[5], matrix[6], matrix[7],
                    matrix[8], matrix[9], matrix[10], matrix[11],
                    matrix[12], matrix[13], matrix[14], matrix[15]);

                Nanami.LLEState.MatrixStack.Push(glmatrix);
            }
        }

        public void DrawSkeleton(int hnumber, int bnumber, int pnumber)
        {
            if (hnumber >= Hierarchies.Count || bnumber >= Hierarchies[hnumber].Length) return;

            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Normalize);
            GL.DepthRange(0.0, 0.0);
            if (pnumber != -1)
            {
                GL.LineWidth(8.0f);
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(1.0, 1.0, 1.0);
                GL.Vertex3(Vector3d.Zero);
                GL.Vertex3(Hierarchies[hnumber][bnumber].Translation);
                GL.End();
                GL.DepthRange(0.0, -0.5);
            }
            GL.PointSize(10.0f);
            GL.Begin(PrimitiveType.Points);
            GL.Color3(0.0, 0.0, 0.0);
            GL.Vertex3(Hierarchies[hnumber][bnumber].Translation);
            GL.End();
            GL.PointSize(7.0f);
            GL.Begin(PrimitiveType.Points);
            GL.Color3(1.0, 0.0, 0.0);
            GL.Vertex3(Hierarchies[hnumber][bnumber].Translation);
            GL.End();
            GL.PopAttrib();

            GL.PushMatrix();
            GL.Translate(Hierarchies[hnumber][bnumber].Translation);
            GL.Rotate(Hierarchies[hnumber][bnumber].Rotation.Z / 182.0444444, 0.0, 0.0, 1.0);
            GL.Rotate(Hierarchies[hnumber][bnumber].Rotation.Y / 182.0444444, 0.0, 1.0, 0.0);
            GL.Rotate(Hierarchies[hnumber][bnumber].Rotation.X / 182.0444444, 1.0, 0.0, 0.0);

            if (Hierarchies[hnumber][bnumber].Child > -1 && Hierarchies[hnumber][bnumber].Child != bnumber) DrawSkeleton(hnumber, Hierarchies[hnumber][bnumber].Child, bnumber);
            GL.PopMatrix();
            if (Hierarchies[hnumber][bnumber].Sibling > -1 && Hierarchies[hnumber][bnumber].Sibling != bnumber) DrawSkeleton(hnumber, Hierarchies[hnumber][bnumber].Sibling, pnumber);
        }
    }
}
