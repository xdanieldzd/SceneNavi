using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.ActorRendering
{
    public static class MIPS
    {
        public enum Opcode : byte
        {
            JAL = 3,
            SLLV = 4,
            ADDIU = 9,
            ANDI = 12,
            ORI = 13,
            LUI = 15,
            SW = 43,
            LH = 0x21,
            LW = 35,
            LHU = 37,
            TYPE_R = 0
        };

        public enum Opcode_R : byte
        {
            SLL = 0x0,
            SRL = 0x2,
            SRA = 0x3,
            SLLV = 0x4,
            SRLV = 0x6,
            SRAV = 0x7,
            JR = 0x8,
            JALR = 0x9,
            SYSCALL = 0xC,
            BREAK = 0xD,
            SYNC = 0xF,
            MFHI = 0x10,
            MTHI = 0x11,
            MFLO = 0x12,
            MTLO = 0x13,
            DSLLV = 0x14,
            DSRLV = 0x16,
            DSRAV = 0x17,
            MULT = 0x18,
            MULTU = 0x19,
            DIV = 0x1A,
            DIVU = 0x1B,
            DMULT = 0x1C,
            DMULTU = 0x1D,
            DDIV = 0x1E,
            DDIVU = 0x1F,
            ADD = 0x20,
            ADDU = 0x21,
            SUB = 0x22,
            SUBU = 0x23,
            AND = 0x24,
            OR = 0x25,
            XOR = 0x26,
            NOR = 0x27,
            SLT = 0x2A,
            SLTU = 0x2B,
            DADD = 0x2C,
            DADDU = 0x2D,
            DSUB = 0x2E,
            DSUBU = 0x2F,
            TGE = 0x30,
            TGEU = 0x31,
            TLT = 0x32,
            TLTU = 0x33,
            TEQ = 0x34,
            TNE = 0x36,
            DSLL = 0x38,
            DSRL = 0x3A,
            DSRA = 0x3B,
            DSLL32 = 0x3C,
            DSRL32 = 0x3E,
            DSRA32 = 0x3F
        };

        public enum Register : byte
        {
            R0 = 0,
            AT = 1,
            V0 = 2,
            V1 = 3,
            A0 = 4,
            A1 = 5,
            A2 = 6,
            A3 = 7,
            T0 = 8,
            T1 = 9,
            T2 = 10,
            T3 = 11,
            T4 = 12,
            T5 = 13,
            T6 = 14,
            T7 = 15,
            S0 = 16,
            S1 = 17,
            S2 = 18,
            S3 = 19,
            S4 = 20,
            S5 = 21,
            S6 = 22,
            S7 = 23,
            T8 = 24,
            T9 = 25,
            K0 = 26,
            K1 = 27,
            GP = 28,
            SP = 29,
            FP = 30,
            RA = 31
        }

        public const uint SafetyVal = 4;

        public static uint OP(uint x) { return ((x) << 26); }
        public static uint OF(uint x) { return (((uint)(x) >> 2) & 0xFFFF); }
        public static uint SA(uint x) { return (((x) & 0x1F) << 6); }
        public static uint RD(uint x) { return (((x) & 0x1F) << 11); }
        public static uint RT(uint x) { return (((x) & 0x1F) << 16); }
        public static uint RS(uint x) { return (((x) & 0x1F) << 21); }
        public static uint IM(uint x) { return ((uint)(x) & 0xFFFF); }
        public static uint JT(uint x) { return (((uint)(x) >> 2) & 0x3FFFFFF); }

        public static uint ADD(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x20); }
        public static uint ADDU(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x21); }
        public static uint ADDI(uint rt, uint rs, uint immd) { return (OP(0x08) | RS(rs) | RT(rt) | IM(immd)); }
        public static uint ADDIU(uint rt, uint rs, uint immd) { return (OP(0x09) | RS(rs) | RT(rt) | IM(immd)); }
        public static uint ANDI(uint rt, uint rs, uint immd) { return (OP(0x0C) | RS(rs) | RT(rt) | IM(immd)); }
        public static uint BC1F(uint off) { return (OP(0x11) | RS(0x08) | OF(off)); }
        public static uint BC1FL(uint off) { return (OP(0x11) | RS(0x08) | RT(0x02) | OF(off)); }
        public static uint BC1T(uint off) { return (OP(0x11) | RS(0x08) | RT(0x01) | OF(off)); }
        public static uint BC1TL(uint off) { return (OP(0x11) | RS(0x08) | RT(0x03) | OF(off)); }
        public static uint BEQ(uint rs, uint rt, uint off) { return (OP(0x04) | RS(rs) | RT(rt) | OF(off)); }
        public static uint BEQL(uint rs, uint rt, uint off) { return (OP(0x14) | RS(rs) | RT(rt) | OF(off)); }
        public static uint BGEZ(uint rs, uint off) { return (OP(0x01) | RS(rs) | RT(0x01) | OF(off)); }
        public static uint BGEZAL(uint rs, uint off) { return (OP(0x01) | RS(rs) | RT(0x11) | OF(off)); }
        public static uint BGEZALL(uint rs, uint off) { return (OP(0x01) | RS(rs) | RT(0x13) | OF(off)); }
        public static uint BGEZL(uint rs, uint off) { return (OP(0x01) | RS(rs) | RT(0x03) | OF(off)); }
        public static uint BGTZ(uint rs, uint off) { return (OP(0x07) | RS(rs) | OF(off)); }
        public static uint BGTZL(uint rs, uint off) { return (OP(0x17) | RS(rs) | OF(off)); }
        public static uint BLEZ(uint rs, uint off) { return (OP(0x06) | RS(rs) | OF(off)); }
        public static uint BLEZL(uint rs, uint off) { return (OP(0x16) | RS(rs) | OF(off)); }
        public static uint BLTZ(uint rs, uint off) { return (OP(0x01) | RS(rs) | OF(off)); }
        public static uint BLTZAL(uint rs, uint off) { return (OP(0x01) | RS(rs) | RT(0x10) | OF(off)); }
        public static uint BLTZALL(uint rs, uint off) { return (OP(0x01) | RS(rs) | RT(0x12) | OF(off)); }
        public static uint BLTZL(uint rs, uint off) { return (OP(0x01) | RS(rs) | RT(0x02) | OF(off)); }
        public static uint BNE(uint rs, uint rt, uint off) { return (OP(0x05) | RS(rs) | RT(rt) | OF(off)); }
        public static uint BNEL(uint rs, uint rt, uint off) { return (OP(0x15) | RS(rs) | RT(rt) | OF(off)); }
        public static uint BREAK(uint code) { return ((code) << 6 | 0x0D); }
        public static uint CACHE(uint cbase, uint op, uint off) { return (OP(0x2F) | RS(cbase) | RT(op) | OF(off)); }
        public static uint CFC1(uint cbase, uint rt, uint rd) { return (OP(0x11) | RS(0x02) | RT(cbase) | RD(rd)); }
        public static uint COP1(uint cofun) { return (OP(0x11) | (1 << 25) | ((cofun) & 0x1FFFFFF)); }
        public static uint CTC1(uint cbase, uint rt, uint rd) { return (OP(0x11) | RS(0x06) | RT(cbase) | RD(rd)); }
        public static uint DADD(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x2C); }
        public static uint DADDI(uint rt, uint rs, uint immd) { return (OP(0x18) | RS(rs) | RT(rt) | IM(immd)); }
        public static uint DADDIU(uint rt, uint rs, uint immd) { return (OP(0x19) | RS(rs) | RT(rt) | IM(immd)); }
        public static uint DADDU(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x2D); }
        public static uint DDIV(uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | 0x1E); }
        public static uint DDIVU(uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | 0x1F); }
        public static uint DIV(uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | 0x1A); }
        public static uint DIVU(uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | 0x1B); }
        public static uint DMFC0(uint rt, uint rd) { return (OP(0x10) | RS(0x01) | RT(rt) | RD(rd)); }
        public static uint DMTC0(uint rt, uint rd) { return (OP(0x10) | RS(0x05) | RT(rt) | RD(rd)); }
        public static uint DMULT(uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | 0x1C); }
        public static uint DMULTU(uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | 0x1D); }
        public static uint DSLL(uint rd, uint rt, uint sa) { return (OP(0x00) | RT(rt) | RD(rd) | SA(sa) | 0x38); }
        public static uint DSLLV(uint rd, uint rt, uint rs) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x14); }
        public static uint DSLL32(uint rd, uint rt, uint sa) { return (OP(0x00) | RT(rt) | RD(rd) | SA(sa) | 0x3C); }
        public static uint DSRA(uint rd, uint rt, uint sa) { return (OP(0x00) | RT(rt) | RD(rd) | SA(sa) | 0x3B); }
        public static uint DSRAV(uint rd, uint rt, uint rs) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x17); }
        public static uint DSRA32(uint rd, uint rt, uint sa) { return (OP(0x00) | RT(rt) | RD(rd) | SA(sa) | 0x3F); }
        public static uint DSRL(uint rd, uint rt, uint sa) { return (OP(0x00) | RT(rt) | RD(rd) | SA(sa) | 0x3A); }
        public static uint DSRLV(uint rd, uint rt, uint rs) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x16); }
        public static uint DSRL32(uint rd, uint rt, uint sa) { return (OP(0x00) | RT(rt) | RD(rd) | SA(sa) | 0x3E); }
        public static uint DSUB(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x2E); }
        public static uint DSUBU(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x2F); }
        public static uint ERET() { return (OP(0x10) | 1 << 25 | 0x18); }
        public static uint J(uint target) { return (OP(0x02) | JT(target)); }
        public static uint JAL(uint target) { return (OP(0x03) | JT(target)); }
        public static uint JALR(uint rd, uint rs) { return (OP(0x00) | RS(rs) | RD(rd) | 0x09); }
        public static uint JR(uint rs) { return (OP(0x00) | RS(rs) | 0x08); }
        public static uint LB(uint rt, uint off, uint cbase) { return (OP(0x20) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LBU(uint rt, uint off, uint cbase) { return (OP(0x24) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LD(uint rt, uint off, uint cbase) { return (OP(0x37) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LDC1(uint rt, uint off, uint cbase) { return (OP(0x35) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LDL(uint rt, uint off, uint cbase) { return (OP(0x1A) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LDR(uint rt, uint off, uint cbase) { return (OP(0x1B) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LH(uint rt, uint off, uint cbase) { return (OP(0x21) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LHU(uint rt, uint off, uint cbase) { return (OP(0x25) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LL(uint rt, uint off, uint cbase) { return (OP(0x30) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LLD(uint rt, uint off, uint cbase) { return (OP(0x34) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LUI(uint rt, uint immd) { return (OP(0x0F) | RT(rt) | IM(immd)); }
        public static uint LW(uint rt, uint off, uint cbase) { return (OP(0x23) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LWC1(uint rt, uint off, uint cbase) { return (OP(0x31) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LWL(uint rt, uint off, uint cbase) { return (OP(0x22) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LWR(uint rt, uint off, uint cbase) { return (OP(0x26) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint LWU(uint rt, uint off, uint cbase) { return (OP(0x27) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint MFC0(uint rt, uint rd) { return (OP(0x10) | RT(rt) | RD(rd)); }
        public static uint MFC1(uint rt, uint rd) { return (OP(0x11) | RT(rt) | RD(rd)); }
        public static uint MFHI(uint rd) { return (OP(0x00) | RD(rd) | 0x10); }
        public static uint MFLO(uint rd) { return (OP(0x00) | RD(rd) | 0x12); }
        public static uint MTC0(uint rt, uint rd) { return (OP(0x10) | RS(0x04) | RT(rt) | RD(rd)); }
        public static uint MTC1(uint rt, uint rd) { return (OP(0x11) | RS(0x04) | RT(rt) | RD(rd)); }
        public static uint MTHI(uint rd) { return (OP(0x00) | RD(rd) | 0x11); }
        public static uint MTLO(uint rd) { return (OP(0x00) | RD(rd) | 0x13); }
        public static uint MULT(uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | 0x18); }
        public static uint MULTU(uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | 0x19); }
        public static uint NOR(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x27); }
        public static uint OR(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x25); }
        public static uint ORI(uint rt, uint rs, uint immd) { return (OP(0x0D) | RS(rs) | RT(rt) | IM(immd)); }
        public static uint SB(uint rt, uint off, uint cbase) { return (OP(0x28) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SC(uint rt, uint off, uint cbase) { return (OP(0x38) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SCD(uint rt, uint off, uint cbase) { return (OP(0x3C) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SD(uint rt, uint off, uint cbase) { return (OP(0x3F) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SDC1(uint rt, uint off, uint cbase) { return (OP(0x3D) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SDL(uint rt, uint off, uint cbase) { return (OP(0x2C) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SDR(uint rt, uint off, uint cbase) { return (OP(0x2D) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SH(uint rt, uint off, uint cbase) { return (OP(0x29) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SLL(uint rd, uint rt, uint sa) { return (OP(0x00) | RT(rt) | RD(rd) | SA(sa)); }
        public static uint SLLV(uint rd, uint rt, uint rs) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x04); }
        public static uint SLT(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x2A); }
        public static uint SLTI(uint rt, uint rs, uint immd) { return (OP(0x0A) | RS(rs) | RT(rt) | IM(immd)); }
        public static uint SLTIU(uint rt, uint rs, uint immd) { return (OP(0x0B) | RS(rs) | RT(rt) | IM(immd)); }
        public static uint SLTU(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x2B); }
        public static uint SRA(uint rd, uint rt, uint sa) { return (OP(0x00) | RT(rt) | RD(rd) | SA(sa) | 0x03); }
        public static uint SRAV(uint rd, uint rt, uint rs) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x07); }
        public static uint SRL(uint rd, uint rt, uint sa) { return (OP(0x00) | RT(rt) | RD(rd) | SA(sa) | 0x02); }
        public static uint SRLV(uint rd, uint rt, uint rs) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x06); }
        public static uint SUB(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x22); }
        public static uint SUBU(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x23); }
        public static uint SW(uint rt, uint off, uint cbase) { return (OP(0x2B) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SWC1(uint rt, uint off, uint cbase) { return (OP(0x39) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SWL(uint rt, uint off, uint cbase) { return (OP(0x2A) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SWR(uint rt, uint off, uint cbase) { return (OP(0x2E) | RS(cbase) | RT(rt) | IM(off)); }
        public static uint SYNC() { return (0x0F); }
        public static uint SYSCALL(uint code) { return ((code) << 6 | 0x0C); }
        public static uint TEQ(uint rs, uint rt, uint code) { return (OP(0x00) | RS(rs) | RT(rt) | ((code) & 0x3FF) << 6 | 0x34); }
        public static uint TEQI(uint rs, uint immd) { return (OP(0x01) | RS(rs) | RT(0x0C) | IM(immd)); }
        public static uint TGE(uint rs, uint rt, uint code) { return (OP(0x00) | RS(rs) | RT(rt) | ((code) & 0x3FF) << 6 | 0x34); }
        public static uint TGEI(uint rs, uint immd) { return (OP(0x01) | RS(rs) | RT(0x08) | IM(immd)); }
        public static uint TGEIU(uint rs, uint immd) { return (OP(0x01) | RS(rs) | RT(0x09) | IM(immd)); }
        public static uint TGEU(uint rs, uint rt, uint code) { return (OP(0x00) | RS(rs) | RT(rt) | ((code) & 0x3FF) << 6 | 0x31); }
        public static uint TLBP() { return (OP(0x10) | 1 << 25 | 0x08); }
        public static uint TLBR() { return (OP(0x10) | 1 << 25 | 0x01); }
        public static uint TLBWI() { return (OP(0x10) | 1 << 25 | 0x02); }
        public static uint TLBWR() { return (OP(0x10) | 1 << 25 | 0x06); }
        public static uint TLT(uint rs, uint rt, uint code) { return (OP(0x00) | RS(rs) | RT(rt) | ((code) & 0x3FF) << 6 | 0x32); }
        public static uint TLTI(uint rs, uint immd) { return (OP(0x01) | RS(rs) | RT(0x0A) | IM(immd)); }
        public static uint TLTIU(uint rs, uint immd) { return (OP(0x01) | RS(rs) | RT(0x0B) | IM(immd)); }
        public static uint TLTU(uint rs, uint rt, uint code) { return (OP(0x00) | RS(rs) | RT(rt) | ((code) & 0x3FF) << 6 | 0x33); }
        public static uint TNE(uint rs, uint rt, uint code) { return (OP(0x00) | RS(rs) | RT(rt) | ((code) & 0x3FF) << 6 | 0x36); }
        public static uint TNEI(uint rs, uint immd) { return (OP(0x01) | RS(rs) | RT(0x0E) | IM(immd)); }
        public static uint XOR(uint rd, uint rs, uint rt) { return (OP(0x00) | RS(rs) | RT(rt) | RD(rd) | 0x26); }
        public static uint XORI(uint rt, uint rs, uint immd) { return (OP(0x0E) | RS(rs) | RT(rt) | IM(immd)); }

        public static uint GetRS(uint word)
        {
            return (uint)(uint)((word >> 21) & 31);
        }

        public static uint GetRT(uint word)
        {
            return (uint)((word >> 16) & 31);
        }

        public static uint GetRD(uint word)
        {
            return (uint)((word >> 11) & 31);
        }

        public static uint GetSA(uint word)
        {
            return (uint)((word >> 6) & 31);
        }

        public static uint GetFT(uint word)
        {
            return (uint)(uint)((word >> 21) & 31);
        }

        public static uint GetFS(uint word)
        {
            return (uint)((word >> 16) & 31);
        }

        public static uint GetFD(uint word)
        {
            return (uint)((word >> 11) & 31);
        }

        public static uint GetBASE(uint word)
        {
            return (uint)((word >> 21) & 31);
        }

        public static uint GetIMM(uint word)
        {
            return (uint)(word & 0xFFFF);
        }

        public static int GetSIMM(uint word)
        {
            return (int)(word & 0xFFFF);
        }

        public static int GetOFFSET(uint word)
        {
            return ((short)((word & 0xFFFF)) * 4);
        }

        public static uint GetTARGET(uint word)
        {
            return (uint)((word & 0x3FFFFFF) << 2);
        }
    }
}
