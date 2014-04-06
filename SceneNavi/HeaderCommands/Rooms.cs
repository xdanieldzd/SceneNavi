using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SceneNavi.ROMHandler;

namespace SceneNavi.HeaderCommands
{
    public class Rooms : Generic
    {
        public class RoomInfoClass : IHeaderParent
        {
            ROMHandler.ROMHandler ROM;

            public uint Start { get; set; }
            public uint End { get; set; }
            public List<HeaderLoader> Headers { get; set; }

            public string DMAFilename { get; private set; }
            public string Description { get; set; }

            public byte[] Data { get; private set; }
            public IHeaderParent Parent { get; private set; }

            public ulong Number { get; private set; }

            public HeaderLoader CurrentRoomHeader { get; set; }
            public HeaderCommands.MeshHeader ActiveMeshHeader
            {
                get { return (CurrentRoomHeader == null ? null : CurrentRoomHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.MeshHeader) as HeaderCommands.MeshHeader); }
            }
            public HeaderCommands.Actors ActiveRoomActorData
            {
                get { return (CurrentRoomHeader == null ? null : CurrentRoomHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Actors) as HeaderCommands.Actors); }
            }
            public HeaderCommands.Objects ActiveObjects
            {
                get { return (CurrentRoomHeader == null ? null : CurrentRoomHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Objects) as HeaderCommands.Objects); }
            }

            public RoomInfoClass() { }

            public RoomInfoClass(ROMHandler.ROMHandler rom, IHeaderParent parent, int num, uint s = 0, uint e = 0)
            {
                ROM = rom;
                Start = s;
                End = e;
                Parent = parent;
                Number = (ulong)num;

                if (Start != 0 && End != 0 && Start < rom.Data.Length && End < rom.Data.Length)
                {
                    ROMHandler.DMATableEntry dma = ROM.Files.Find(x => x.PStart == Start);
                    if (dma != null) DMAFilename = dma.Name;

                    Data = new byte[End - Start];
                    Array.Copy(ROM.Data, Start, Data, 0, End - Start);

                    if ((Description = (ROM.XMLRoomNames.Names[Start] as string)) == null)
                    {
                        ROMHandler.SceneTableEntry parentste = (parent as ROMHandler.SceneTableEntry);
                        if (parentste.IsNameExternal)
                        {
                            Description = string.Format("Room {0}", (Number + 1));
                        }
                        else
                        {
                            if (dma != null)
                                Description = DMAFilename;
                            else
                                Description = string.Format("S{0:X}-E{1:X}", Start, End);
                        }
                    }

                    Load();
                }
            }

            public RoomInfoClass(ROMHandler.ROMHandler rom, IHeaderParent parent, string fn)
            {
                ROM = rom;
                Parent = parent;

                System.IO.FileStream fs = new System.IO.FileStream(fn, System.IO.FileMode.Open);
                Data = new byte[fs.Length];
                fs.Read(Data, 0, (int)fs.Length);
                fs.Close();

                Description = System.IO.Path.GetFileNameWithoutExtension(fn);

                Load();
            }

            private void Load()
            {
                ROM.SegmentMapping.Remove((byte)0x03);
                ROM.SegmentMapping.Add((byte)0x03, Data);

                Headers = new List<HeaderLoader>();

                if (Data[0] == (byte)HeaderLoader.CommandTypeIDs.SettingsSoundRoom || Data[0] == (byte)HeaderLoader.CommandTypeIDs.RoomBehavior ||
                    BitConverter.ToUInt32(Data, 0) == (byte)HeaderLoader.CommandTypeIDs.SubHeaders)
                {
                    Headers.Add(new HeaderLoader(ROM, this, 0x03, 0, 0));

                    if (BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)0x03]), 0) == 0x18)
                    {
                        int hnum = 1;
                        uint aofs = Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)0x03]), 4));
                        while (true)
                        {
                            uint rofs = Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)0x03]), (int)(aofs & 0x00FFFFFF)));
                            if (rofs != 0)
                            {
                                if ((rofs & 0x00FFFFFF) > ((byte[])ROM.SegmentMapping[(byte)0x03]).Length || (rofs >> 24) != 0x03) break;
                                Headers.Add(new HeaderLoader(ROM, this, 0x03, (int)(rofs & 0x00FFFFFF), hnum++));
                            }
                            aofs += 4;
                        }
                    }

                    CurrentRoomHeader = Headers[0];
                }
            }
        }

        public List<RoomInfoClass> RoomInformation { get; private set; }

        public Rooms(ROMHandler.ROMHandler rom, IHeaderParent parent, string fn)
            : base(rom, parent, HeaderLoader.CommandTypeIDs.Rooms)
        {
            RoomInformation = new List<RoomInfoClass>();
            RoomInformation.Add(new RoomInfoClass(rom, parent, fn));
        }

        public Rooms(Generic basecmd)
            : base(basecmd)
        {
            RoomInformation = new List<RoomInfoClass>();

            byte seg = (byte)(GetAddressGeneric() >> 24);

            for (int i = 0; i < GetCountGeneric(); i++)
            {
                RoomInfoClass roomadr = new RoomInfoClass(ROM, basecmd.Parent, i,
                    Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[seg]), (int)((GetAddressGeneric() & 0xFFFFFF) + i * 8))),
                    Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[seg]), (int)((GetAddressGeneric() & 0xFFFFFF) + i * 8) + 4)));
                RoomInformation.Add(roomadr);
            }
        }
    }
}
