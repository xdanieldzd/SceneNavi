using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using SceneNavi.ROMHandler;

namespace SceneNavi.HeaderCommands
{
    public class SpecialObjects : Generic, IStoreable
    {
        public class SpecialObjectTypes
        {
            public ushort ObjectNumber { get; private set; }
            public string Name { get; private set; }

            public SpecialObjectTypes(ushort number, string name)
            {
                ObjectNumber = number;
                Name = name;
            }
        }

        public static List<SpecialObjectTypes> Types = new List<SpecialObjectTypes>()
        {
            new SpecialObjectTypes(0x0002, "Field objects (0x02)"),
            new SpecialObjectTypes(0x0003, "Dungeon objects (0x03)")
        };

        public ushort SelectedSpecialObjects { get; set; }

        public SpecialObjects(Generic basecmd)
            : base(basecmd)
        {
            SelectedSpecialObjects = (ushort)(GetAddressGeneric() & 0xFFFF);
        }

        public void Store(byte[] databuf, int baseadr)
        {
            byte[] objbytes = BitConverter.GetBytes(Endian.SwapUInt16(this.SelectedSpecialObjects));
            Buffer.BlockCopy(objbytes, 0, databuf, (int)(baseadr + (this.Offset & 0xFFFFFF) + 6), objbytes.Length);
        }
    }
}
