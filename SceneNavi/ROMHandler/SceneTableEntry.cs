using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SceneNavi.ROMHandler
{
    public class SceneTableEntry : IHeaderParent
    {
        #region Pseudo-code for validity
        /*
         * xxxxxxxx yyyyyyyy aaaaaaaa bbbbbbbb cc dd ee ff
         * 
         * MUST
         * || (yyyyyyyy > xxxxxxxx)
         * 
         * MUST THIS
         * || (aaaaaaaa AND bbbbbbbb != 0)
         * || --> ((bbbbbbbb > aaaaaaaa) AND (bbbbbbbb == aaaaaaaa + 0x2880 || bbbbbbbb == aaaaaaaa + 0x1B00))
         * OR THIS
         * || (aaaaaaaa AND bbbbbbbb == 0)
         * 
         * MUST
         * || ((cc & 0xF0) == 0)
         * 
         * MUST
         * || ((ee & 0xF0) == 0)
         * 
         * MUST
         * || (ff == 0)
         */
        #endregion

        [Browsable(false)]
        public string DMAFilename { get; private set; }
        [ReadOnly(true)]
        public string Name { get; private set; }
        [Browsable(false)]
        public int Offset { get; private set; }
        [Browsable(false)]
        public bool IsOffsetRelative { get; private set; }

        [Browsable(false)]
        public uint SceneStartAddress { get; private set; }
        [Browsable(false)]
        public uint SceneEndAddress { get; private set; }

        [DisplayName("Label start address")]
        public uint LabelStartAddress { get; set; }
        [DisplayName("Label end address")]
        public uint LabelEndAddress { get; set; }

        [DisplayName("Unknown 1")]
        public byte Unknown1 { get; set; }
        [DisplayName("Configuration #")]
        public byte ConfigurationNo { get; set; }
        [DisplayName("Unknown 3")]
        public byte Unknown3 { get; set; }
        [DisplayName("Unknown 4")]
        public byte Unknown4 { get; set; }

        [Browsable(false)]
        public bool IsValid { get; private set; }
        [Browsable(false)]
        public byte[] Data { get; private set; }

        [Browsable(false)]
        public List<HeaderLoader> SceneHeaders { get; private set; }

        [Browsable(false)]
        public bool InROM { get; private set; }
        [Browsable(false)]
        public bool IsNameExternal { get; private set; }

        [Browsable(false)]
        public HeaderLoader CurrentSceneHeader { get; set; }
        [Browsable(false)]
        public HeaderCommands.Actors ActiveTransitionData
        {
            get { return (CurrentSceneHeader == null ? null : CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Transitions) as HeaderCommands.Actors); }
        }
        [Browsable(false)]
        public HeaderCommands.Actors ActiveSpawnPointData
        {
            get { return (CurrentSceneHeader == null ? null : CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Spawns) as HeaderCommands.Actors); }
        }
        [Browsable(false)]
        public HeaderCommands.SpecialObjects ActiveSpecialObjs
        {
            get { return (CurrentSceneHeader == null ? null : CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.SpecialObjects) as HeaderCommands.SpecialObjects); }
        }
        [Browsable(false)]
        public HeaderCommands.Waypoints ActiveWaypoints
        {
            get { return (CurrentSceneHeader == null ? null : CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Waypoints) as HeaderCommands.Waypoints); }
        }
        [Browsable(false)]
        public HeaderCommands.Collision ActiveCollision
        {
            get { return (CurrentSceneHeader == null ? null : CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Collision) as HeaderCommands.Collision); }
        }
        [Browsable(false)]
        public HeaderCommands.SettingsSoundScene ActiveSettingsSoundScene
        {
            get { return (CurrentSceneHeader == null ? null : CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.SettingsSoundScene) as HeaderCommands.SettingsSoundScene); }
        }
        [Browsable(false)]
        public HeaderCommands.EnvironmentSettings ActiveEnvSettings
        {
            get { return (CurrentSceneHeader == null ? null : CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.EnvironmentSettings) as HeaderCommands.EnvironmentSettings); }
        }

        ROMHandler ROM;

        public SceneTableEntry(ROMHandler rom, string fn)
        {
            ROM = rom;
            InROM = false;

            Offset = -1;
            IsOffsetRelative = false;

            SceneStartAddress = SceneEndAddress = LabelStartAddress = LabelEndAddress = 0;
            Unknown1 = ConfigurationNo = Unknown3 = Unknown4 = 0;

            IsValid = true;

            System.IO.FileStream fs = new System.IO.FileStream(fn, System.IO.FileMode.Open);
            Data = new byte[fs.Length];
            fs.Read(Data, 0, (int)fs.Length);
            fs.Close();

            Name = System.IO.Path.GetFileNameWithoutExtension(fn);
        }

        public SceneTableEntry(ROMHandler rom, int ofs, bool isrel)
        {
            ROM = rom;
            InROM = true;

            Offset = ofs;
            IsOffsetRelative = isrel;

            SceneStartAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs));
            SceneEndAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 4));
            LabelStartAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 8));
            LabelEndAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 12));
            Unknown1 = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 16];
            ConfigurationNo = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 17];
            Unknown3 = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 18];
            Unknown4 = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 19];

            IsValid =
                /*(SceneStartAddress > rom.Code.VStart) &&*/
                (SceneStartAddress < rom.Size) && (SceneEndAddress < rom.Size) && (LabelStartAddress < rom.Size) && (LabelEndAddress < rom.Size) &&
                ((SceneStartAddress & 0xF) == 0) && ((SceneEndAddress & 0xF) == 0) && ((LabelStartAddress & 0xF) == 0) && ((LabelEndAddress & 0xF) == 0) &&
                (SceneEndAddress > SceneStartAddress) &&
                (((LabelStartAddress != 0) && (LabelEndAddress != 0) && (LabelEndAddress > LabelStartAddress) &&
                    (LabelEndAddress == LabelStartAddress + 0x2880 || LabelEndAddress == LabelStartAddress + 0x1B00)) || (LabelStartAddress == 0 && LabelEndAddress == 0))/* &&
                ((Unknown1 & 0xF0) == 0) && ((Unknown3 & 0xF0) == 0) && (Unknown4 == 0)*/;

            if (IsValid == true)
            {
                DMATableEntry dma = rom.Files.Find(x => x.PStart == SceneStartAddress);
                if (dma != null) DMAFilename = dma.Name;

                if ((Name = (ROM.XMLSceneNames.Names[SceneStartAddress] as string)) == null)
                {
                    IsNameExternal = false;

                    if (dma != null)
                        Name = DMAFilename;
                    else
                        Name = string.Format("S{0:X}_L{1:X}", SceneStartAddress, LabelStartAddress);
                }
                else
                    IsNameExternal = true;

                Data = new byte[SceneEndAddress - SceneStartAddress];
                Array.Copy(ROM.Data, SceneStartAddress, Data, 0, SceneEndAddress - SceneStartAddress);
            }
        }

        public void SaveTableEntry()
        {
            if (!InROM) throw new Exception("Trying to save scene table entry for external scene file");

            byte[] tmpbuf = null;

            tmpbuf = BitConverter.GetBytes(Endian.SwapUInt32(SceneStartAddress));
            Buffer.BlockCopy(tmpbuf, 0, (IsOffsetRelative ? ROM.CodeData : ROM.Data), Offset, tmpbuf.Length);

            tmpbuf = BitConverter.GetBytes(Endian.SwapUInt32(SceneEndAddress));
            Buffer.BlockCopy(tmpbuf, 0, (IsOffsetRelative ? ROM.CodeData : ROM.Data), Offset + 4, tmpbuf.Length);

            tmpbuf = BitConverter.GetBytes(Endian.SwapUInt32(LabelStartAddress));
            Buffer.BlockCopy(tmpbuf, 0, (IsOffsetRelative ? ROM.CodeData : ROM.Data), Offset + 8, tmpbuf.Length);

            tmpbuf = BitConverter.GetBytes(Endian.SwapUInt32(LabelEndAddress));
            Buffer.BlockCopy(tmpbuf, 0, (IsOffsetRelative ? ROM.CodeData : ROM.Data), Offset + 12, tmpbuf.Length);

            (IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 16] = Unknown1;
            (IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 17] = ConfigurationNo;
            (IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 18] = Unknown3;
            (IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 19] = Unknown4;
        }

        public void ReadScene(HeaderCommands.Rooms forcerooms = null)
        {
            Program.Status.Message = string.Format("Reading scene '{0}'...", this.Name);

            ROM.SegmentMapping.Remove((byte)0x02);
            ROM.SegmentMapping.Add((byte)0x02, Data);

            SceneHeaders = new List<HeaderLoader>();

            HeaderLoader newheader = null;
            HeaderCommands.Rooms rooms = null;
            HeaderCommands.Collision coll = null;

            if (Data[0] == (byte)HeaderLoader.CommandTypeIDs.SettingsSoundScene || Data[0] == (byte)HeaderLoader.CommandTypeIDs.Rooms ||
                BitConverter.ToUInt32(Data, 0) == (byte)HeaderLoader.CommandTypeIDs.SubHeaders)
            {
                /* Get rooms & collision command from first header */
                newheader = new HeaderLoader(ROM, this, (byte)0x02, 0, 0);

                /* If external rooms should be forced, overwrite command in header */
                if (forcerooms != null)
                {
                    int roomidx = newheader.Commands.FindIndex(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms);
                    if (roomidx != -1) newheader.Commands[roomidx] = forcerooms;
                }

                rooms = newheader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms) as HeaderCommands.Rooms;
                coll = newheader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Collision) as HeaderCommands.Collision;
                SceneHeaders.Add(newheader);

                if (BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)0x02]), 0) == 0x18)
                {
                    int hnum = 1;
                    uint aofs = Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)0x02]), 4));
                    while (true)
                    {
                        uint rofs = Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)0x02]), (int)(aofs & 0x00FFFFFF)));
                        if (rofs != 0)
                        {
                            if ((rofs & 0x00FFFFFF) > ((byte[])ROM.SegmentMapping[(byte)0x02]).Length || (rofs >> 24) != 0x02) break;
                            newheader = new HeaderLoader(ROM, this, (byte)0x02, (int)(rofs & 0x00FFFFFF), hnum++);

                            /* Get room command index... */
                            int roomidx = newheader.Commands.FindIndex(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms);

                            /* If external rooms should be forced, overwrite command in header */
                            if (roomidx != -1 && forcerooms != null) newheader.Commands[roomidx] = forcerooms;

                            /* If rooms were found in first header, force using these! */
                            if (roomidx != -1 && rooms != null) newheader.Commands[roomidx] = rooms;

                            /* If collision was found in header, force */
                            int collidx = newheader.Commands.FindIndex(x => x.Command == HeaderLoader.CommandTypeIDs.Collision);
                            if (collidx != -1 && coll != null) newheader.Commands[collidx] = coll;

                            SceneHeaders.Add(newheader);
                        }
                        aofs += 4;
                    }
                }

                CurrentSceneHeader = SceneHeaders[0];
            }
        }
    }
}
