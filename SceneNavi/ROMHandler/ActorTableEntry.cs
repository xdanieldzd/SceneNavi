using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.ROMHandler
{
    public class ActorTableEntry
    {
        public int Offset { get; private set; }
        public bool IsOffsetRelative { get; private set; }

        public uint StartAddress { get; private set; }
        public uint EndAddress { get; private set; }
        public uint RAMStartAddress { get; private set; }
        public uint RAMEndAddress { get; private set; }
        public uint Unknown1 { get; private set; }
        public uint ActorInfoRAMAddress { get; private set; }
        public uint ActorNameRAMAddress { get; private set; }
        public uint Unknown2 { get; private set; }

        public bool IsValid { get; private set; }
        public bool IsIncomplete { get; private set; }
        public bool IsComplete { get; private set; }
        public bool IsEmpty { get; private set; }

        public ushort ActorNumber { get; private set; }
        public byte ActorType { get; private set; }
        public ushort ObjectNumber { get; private set; }

        public string Name { get; private set; }
        public string Filename { get; private set; }

        ROMHandler ROM;

        public ActorTableEntry(ROMHandler rom, int ofs, bool isrel)
        {
            ROM = rom;
            Offset = ofs;
            IsOffsetRelative = isrel;

            StartAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs));
            EndAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 4));
            RAMStartAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 8));
            RAMEndAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 12));
            Unknown1 = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 16));
            ActorInfoRAMAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 20));
            ActorNameRAMAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 24));
            Unknown2 = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 28));

            IsValid =
                (ActorInfoRAMAddress >> 24 == 0x80) &&
                (((ActorNameRAMAddress >> 24 == 0x80) && (ActorNameRAMAddress - ROMHandler.CodeRAMAddress) < rom.CodeData.Length) || (ActorNameRAMAddress == 0));

            IsIncomplete = (StartAddress == 0 && EndAddress == 0 && RAMStartAddress == 0 && RAMEndAddress == 0);

            IsComplete =
                (StartAddress > rom.Code.VStart && StartAddress < rom.Size) &&
                (EndAddress > StartAddress && EndAddress > rom.Code.VStart && EndAddress < rom.Size) &&
                (RAMStartAddress >> 24 == 0x80) &&
                (RAMEndAddress > RAMStartAddress && RAMEndAddress >> 24 == 0x80) &&
                (ActorInfoRAMAddress >> 24 == 0x80) &&
                (((ActorNameRAMAddress >> 24 == 0x80) && (ActorNameRAMAddress - ROMHandler.CodeRAMAddress) < rom.CodeData.Length) || (ActorNameRAMAddress == 0));

            IsEmpty = (StartAddress == 0 && EndAddress == 0 && RAMStartAddress == 0 && RAMEndAddress == 0 && Unknown1 == 0 && ActorInfoRAMAddress == 0 && ActorNameRAMAddress == 0 && Unknown2 == 0);

            Name = Filename = "N/A";

            if (IsValid == true && IsEmpty == false)
            {
                if (ActorNameRAMAddress != 0)
                {
                    string tmp = string.Empty;
                    ROMHandler.GetTerminatedString(rom.CodeData, (int)(ActorNameRAMAddress - ROMHandler.CodeRAMAddress), out tmp);
                    Name = tmp;
                }
                else
                    Name = string.Format("RAM Start 0x{0:X}", RAMStartAddress);

                if (RAMStartAddress != 0 && RAMEndAddress != 0)
                {
                    DMATableEntry dma = rom.Files.Find(x => x.PStart == StartAddress);
                    if (dma != null)
                    {
                        Filename = dma.Name;

                        byte[] tmp = new byte[dma.VEnd - dma.VStart];
                        Array.Copy(rom.Data, dma.PStart, tmp, 0, dma.VEnd - dma.VStart);

                        uint infoadr = (ActorInfoRAMAddress - RAMStartAddress);
                        if (infoadr >= tmp.Length) return;

                        ActorNumber = Endian.SwapUInt16(BitConverter.ToUInt16(tmp, (int)infoadr));
                        ActorType = tmp[infoadr + 2];
                        ObjectNumber = Endian.SwapUInt16(BitConverter.ToUInt16(tmp, (int)infoadr + 8));
                    }
                    else
                        Filename = Name;
                }
            }
        }
    }
}
