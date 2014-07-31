using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SceneNavi.ROMHandler;

namespace SceneNavi.HeaderCommands
{
    public class Generic
    {
        public HeaderLoader.CommandTypeIDs Command { get; private set; }
        public int Offset { get; private set; }
        public ulong Data { get; private set; }
        public IHeaderParent Parent { get; private set; }

        public string Description
        {
            get { return (string)HeaderLoader.CommandHumanNames[Command]; }
        }

        public string ByteString
        {
            get { return string.Format("0x{0:X8} 0x{1:X8}", (Data >> 32), (Data & 0xFFFFFFFF)); }
        }

        public ROMHandler.ROMHandler ROM { get; private set; }
        public bool InROM { get; private set; }

        public Generic(ROMHandler.ROMHandler rom, IHeaderParent parent, HeaderLoader.CommandTypeIDs cmdid)
        {
            ROM = rom;
            InROM = false;
            Command = cmdid;
            Offset = -1;
            Data = ulong.MaxValue;
            Parent = parent;
        }

        public Generic(Generic basecmd)
        {
            ROM = basecmd.ROM;
            InROM = basecmd.InROM;
            Command = basecmd.Command;
            Offset = basecmd.Offset;
            Data = basecmd.Data;
            Parent = basecmd.Parent;
        }

        public Generic(ROMHandler.ROMHandler rom, IHeaderParent parent, byte seg, ref int ofs)
        {
            ROM = rom;
            Command = (HeaderLoader.CommandTypeIDs)((byte[])rom.SegmentMapping[seg])[ofs];
            Offset = ofs;
            Data = Endian.SwapUInt64(BitConverter.ToUInt64(((byte[])rom.SegmentMapping[seg]), ofs));
            Parent = parent;
            ofs += 8;

            if (parent is HeaderCommands.Rooms.RoomInfoClass && (parent as HeaderCommands.Rooms.RoomInfoClass).Parent is SceneTableEntryOcarina)
            {
                SceneTableEntryOcarina ste = ((parent as HeaderCommands.Rooms.RoomInfoClass).Parent as SceneTableEntryOcarina);
                InROM = ste.InROM;
            }
            else if (parent is SceneTableEntryOcarina)
            {
                InROM = (parent as SceneTableEntryOcarina).InROM;
            }
        }

        public int GetCountGeneric()
        {
            return (int)((Data >> 48) & 0xFF);
        }

        public int GetAddressGeneric()
        {
            return (int)(Data & 0xFFFFFFFF);
        }
    }
}
