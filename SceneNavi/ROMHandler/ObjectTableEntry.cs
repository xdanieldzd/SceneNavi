using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.ROMHandler
{
    public class ObjectTableEntry
    {
        public int Offset { get; private set; }
        public bool IsOffsetRelative { get; private set; }

        public uint StartAddress { get; private set; }
        public uint EndAddress { get; private set; }

        public bool IsValid { get; private set; }
        public bool IsEmpty { get; private set; }

        public string Name { get; private set; }
        public DMATableEntry DMA { get; private set; }

        ROMHandler ROM;

        public ObjectTableEntry(ROMHandler rom, int ofs, bool isrel, ushort number = 0)
        {
            ROM = rom;
            Offset = ofs;
            IsOffsetRelative = isrel;

            StartAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs));
            EndAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 4));

            IsValid =
                ((StartAddress > rom.Code.VStart) &&
                (StartAddress < rom.Size) && (EndAddress < rom.Size) &&
                ((StartAddress & 0xF) == 0) && ((EndAddress & 0xF) == 0) &&
                (EndAddress > StartAddress));

            IsEmpty = (StartAddress == 0 && EndAddress == 0);

            Name = "N/A";

            if (IsValid == true && IsEmpty == false)
            {
                if ((Name = (ROM.XMLObjectNames.Names[number] as string)) == null)
                {
                    DMA = rom.Files.Find(x => x.PStart == StartAddress);
                    if (DMA != null)
                        Name = DMA.Name;
                    else
                        Name = string.Format("S{0:X}_E{1:X}", StartAddress, EndAddress);
                }
            }
        }
    }
}
