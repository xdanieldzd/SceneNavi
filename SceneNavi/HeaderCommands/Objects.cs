using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.HeaderCommands
{
    public class Objects : Generic, IStoreable
    {
        public List<Entry> ObjectList { get; set; }

        public Objects(Generic basecmd)
            : base(basecmd)
        {
            ObjectList = new List<Entry>();
            for (int i = 0; i < GetCountGeneric(); i++) ObjectList.Add(new Entry(ROM, (uint)(GetAddressGeneric() + i * 2)));
        }

        public void Store(byte[] databuf, int baseadr)
        {
            foreach (HeaderCommands.Objects.Entry obj in this.ObjectList)
            {
                byte[] objbytes = BitConverter.GetBytes(Endian.SwapUInt16(obj.Number));
                Buffer.BlockCopy(objbytes, 0, databuf, (int)(baseadr + (obj.Address & 0xFFFFFF)), objbytes.Length);
            }
        }

        public class Entry
        {
            public uint Address { get; set; }
            public ushort Number { get; set; }
            public string Name
            {
                get
                {
                    return (Number < ROM.Objects.Count ? ROM.Objects[Number].Name : "(invalid?)");
                }

                set
                {
                    if (value == null) return;
                    int objidx = ROM.Objects.FindIndex(x => x.Name.ToLowerInvariant() == value.ToLowerInvariant());
                    if (objidx != -1)
                        Number = (ushort)objidx;
                    else
                        System.Media.SystemSounds.Hand.Play();
                }
            }

            ROMHandler.ROMHandler ROM;

            public Entry() { }

            public Entry(ROMHandler.ROMHandler rom, uint adr)
            {
                ROM = rom;
                Address = adr;

                byte[] segdata = (byte[])ROM.SegmentMapping[(byte)(adr >> 24)];
                if (segdata == null) return;

                Number = Endian.SwapUInt16(BitConverter.ToUInt16(segdata, (int)(adr & 0xFFFFFF)));
            }
        }
    }
}
