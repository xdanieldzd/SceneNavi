using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using SceneNavi.SimpleF3DEX2;

namespace SceneNavi.ROMHandler
{
    public class ROMHandler
    {
        public class ROMHandlerException : Exception
        {
            public ROMHandlerException(string errorMessage) : base(errorMessage) { }
            public ROMHandlerException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
        };

        public const int MinROMSize = 0x400000;
        public const uint CodeRAMAddress = 0x8001CE60;
        const int CodeUcodeThreshold = 0x2000;
        const int Z64TablesAdrOffset = 0xB9E6C8;

        public enum ByteOrder { BigEndian, MiddleEndian, LittleEndian };
        public ByteOrder DetectedByteOrder { get; private set; }
        public class ByteOrderException : ROMHandlerException
        {
            public ByteOrderException(string errorMessage) : base(errorMessage) { }
            public ByteOrderException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
        };

        public string Filename { get; private set; }
        public byte[] Data { get; private set; }

        public string Title { get; private set; }
        public string GameID { get; private set; }
        public byte Version { get; private set; }
        public int Size { get { return Data.Length; } }

        public bool HasZ64TablesHack { get; private set; }

        public string Creator { get; private set; }
        public string BuildDateString { get; private set; }
        public DateTime BuildDate
        {
            get { return DateTime.ParseExact(BuildDateString, "yy-MM-dd HH:mm:ss", null); }
        }

        public int DMATableAddress { get; private set; }
        public List<DMATableEntry> Files { get; private set; }

        public int FileNameTableAddress { get; private set; }
        public bool HasFileNameTable
        {
            get
            {
                return FileNameTableAddress != -1;
            }
        }

        public DMATableEntry Code { get; private set; }
        public byte[] CodeData { get; private set; }

        public List<SceneTableEntry> Scenes { get; private set; }
        public int SceneTableAddress { get; private set; }
        public System.Windows.Forms.AutoCompleteStringCollection SceneNameACStrings { get; private set; }

        public List<ActorTableEntry> Actors { get; private set; }
        public int ActorTableAddress { get; private set; }

        public List<ObjectTableEntry> Objects { get; private set; }
        public int ObjectTableAddress { get; private set; }
        public ushort ObjectCount { get; private set; }
        public System.Windows.Forms.AutoCompleteStringCollection ObjectNameACStrings { get; private set; }

        public List<ExitTableEntry> Exits { get; private set; }
        public int ExitTableAddress { get; private set; }

        public Hashtable SegmentMapping { get; set; }

        public F3DEX2Interpreter Renderer { get; private set; }

        public XMLActorDefinitionReader XMLActorDefReader { get; private set; }

        public XMLHashTableReader XMLActorNames { get; private set; }
        public XMLHashTableReader XMLObjectNames { get; private set; }
        public XMLHashTableReader XMLSongNames { get; private set; }

        public XMLHashTableReader XMLSceneNames { get; private set; }
        public XMLHashTableReader XMLRoomNames { get; private set; }
        public XMLHashTableReader XMLStageDescriptions { get; private set; }

        public bool Loaded { get; private set; }

        public static int GetTerminatedString(byte[] bytes, int index, out string str)
        {
            int nullidx = Array.FindIndex(bytes, index, (x) => x == 0);

            if (nullidx >= 0)
                str = Encoding.ASCII.GetString(bytes, index, nullidx - index);
            else
                str = Encoding.ASCII.GetString(bytes, index, bytes.Length - index);

            int nextidx = Array.FindIndex(bytes, nullidx, (x) => x != 0);

            return nextidx;
        }

        public ROMHandler() { }

        public ROMHandler(string fn)
        {
#if !DEBUG
            try
#endif
            {
            reload:
                DMATableAddress = FileNameTableAddress = SceneTableAddress = -1;

                Filename = fn;

                /* Initialize segment and rendering systems */
                SegmentMapping = new Hashtable();
                Renderer = new F3DEX2Interpreter(this);

                /* Read ROM */
                System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.Open(fn, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read));
                if (br.BaseStream.Length < MinROMSize) throw new ROMHandlerException(string.Format("File size is less than {0}MB; ROM appears to be invalid.", (MinROMSize / 0x100000)));
                Data = new byte[br.BaseStream.Length];
                br.Read(Data, 0, (int)br.BaseStream.Length);
                br.Close();

                /* Detect byte order */
                DetectByteOrder();

                if (DetectedByteOrder != ByteOrder.BigEndian)
                {
                    if (MessageBox.Show("The ROM file you have selected uses an incompatible byte order, and needs to be converted to Big Endian format to be used." + Environment.NewLine + Environment.NewLine +
                        "Convert the ROM now? (You will be asked for the target filename; the converted ROM will also be reloaded.)", "Byte Order Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        /* Ask for new filename */
                        string fnnew = GUIHelpers.ShowSaveFileDialog("Nintendo 64 ROMs (*.z64;*.bin)|*.z64;*.bin|All Files (*.*)|*.*");
                        if (fnnew != string.Empty)
                        {
                            fn = fnnew;

                            /* Perform byte order conversion */
                            byte[] datanew = new byte[Data.Length];
                            byte[] conv = null;
                            for (int i = 0, j = 0; i < Data.Length; i += 4, j += 4)
                            {
                                if (DetectedByteOrder == ByteOrder.MiddleEndian) conv = new byte[4] { Data[i + 1], Data[i], Data[i + 3], Data[i + 2] };
                                else if (DetectedByteOrder == ByteOrder.LittleEndian) conv = new byte[4] { Data[i + 3], Data[i + 2], Data[i + 1], Data[i] };
                                Buffer.BlockCopy(conv, 0, datanew, j, conv.Length);
                            }

                            /* Save converted ROM, then reload it */
                            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(System.IO.File.Create(fn));
                            bw.Write(datanew);
                            bw.Close();

                            goto reload;
                        }
                    }
                    else
                    {
                        /* Wrong byte order, no conversion performed */
                        throw new ByteOrderException(string.Format("Incompatible byte order {0} detected; ROM cannot be used.", DetectedByteOrder));
                    }
                }
                else
                {
                    /* Read header */
                    ReadROMHeader();

                    /* Create XML actor definition reader */
                    XMLActorDefReader = new XMLActorDefinitionReader(System.IO.Path.Combine("XML", "ActorDefinitions", GameID.Substring(1, 2)));

                    if (XMLActorDefReader.Definitions.Count > 0)
                    {
                        /* Create remaining XML-related objects */
                        XMLActorNames = new XMLHashTableReader(System.IO.Path.Combine("XML", "GameDataGeneric", GameID.Substring(1, 2)), "ActorNames.xml");
                        XMLObjectNames = new XMLHashTableReader(System.IO.Path.Combine("XML", "GameDataGeneric", GameID.Substring(1, 2)), "ObjectNames.xml");
                        XMLSongNames = new XMLHashTableReader(System.IO.Path.Combine("XML", "GameDataGeneric", GameID.Substring(1, 2)), "SongNames.xml");

                        XMLSceneNames = new XMLHashTableReader(System.IO.Path.Combine("XML", "GameDataSpecific", string.Format("{0}{1:X1}", GameID, Version)), "SceneNames.xml");
                        XMLRoomNames = new XMLHashTableReader(System.IO.Path.Combine("XML", "GameDataSpecific", string.Format("{0}{1:X1}", GameID, Version)), "RoomNames.xml");
                        XMLStageDescriptions = new XMLHashTableReader(System.IO.Path.Combine("XML", "GameDataSpecific", string.Format("{0}{1:X1}", GameID, Version)), "StageDescriptions.xml");

                        /* Determine if ROM uses z64tables hack */
                        HasZ64TablesHack = (Version == 15 && Endian.SwapUInt32(BitConverter.ToUInt32(Data, 0x1238)) != 0x0C00084C);

                        /* Find and read build information, DMA table, etc. */
                        FindBuildInfo();
                        FindFileNameTable();
                        ReadDMATable();
                        ReadFileNameTable();

                        /* Try to identify files */
                        foreach (DMATableEntry dte in Files) dte.Identify(this);

                        /* Find the code file */
                        FindCodeFile();

                        /* Find other Zelda-specific stuff */
                        FindActorTable();
                        FindObjectTable();
                        FindSceneTable();
                        ReadExitTable();

                        /* Some sanity checking & exception handling*/
                        if (Scenes == null || Scenes.Count == 0) throw new ROMHandlerException("No valid scenes could be recognized in the ROM.");

                        /* Done */
                        Loaded = true;
                    }
                }
            }
#if !DEBUG
            catch (Exception ex)
            {
                Loaded = false;
                if (MessageBox.Show(ex.Message + "\n\n" + "Show detailed information?", "Exception", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    MessageBox.Show(ex.ToString(), "Exception Details", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
#endif
        }

        public bool IsAddressSupported(uint address)
        {
            if (address >> 24 != 0x80)
            {
                if ((address >> 24) > 0x0F || SegmentMapping[(byte)(address >> 24)] == null) return false;
                if ((address & 0xFFFFFF) > ((byte[])SegmentMapping[(byte)(address >> 24)]).Length && ((byte[])SegmentMapping[(byte)(address >> 24)]).Length != 0) return false;
            }
            else
                return false;

            return true;

        }

        private void DetectByteOrder()
        {
            if (Data[0x0] == 0x80 && Data[0x8] == 0x80) DetectedByteOrder = ByteOrder.BigEndian;
            else if (Data[0x1] == 0x80 && Data[0x9] == 0x80) DetectedByteOrder = ByteOrder.MiddleEndian;
            else if (Data[0x3] == 0x80 && Data[0xB] == 0x80) DetectedByteOrder = ByteOrder.LittleEndian;
        }

        private void ReadROMHeader()
        {
            Title = Encoding.ASCII.GetString(Data, 0x20, 0x14).TrimEnd(new char[] { '\0', ' ' });
            GameID = Encoding.ASCII.GetString(Data, 0x3B, 0x4).TrimEnd(new char[] { '\0' });
            Version = Data[0x3F];

            if (Title.Contains("MAJORA") || Title.Contains("MUJURA")) throw new ROMHandlerException("Majora's Mask is not supported.");
        }

        private void FindBuildInfo()
        {
            for (int i = 0; i < MinROMSize; i++)
            {
                if (Encoding.ASCII.GetString(Data, i, 4) == "@srd")
                {
                    i -= (i % 8);
                    string tmp = string.Empty;

                    int next = GetTerminatedString(Data, i, out tmp);
                    Creator = tmp;
                    int next2 = GetTerminatedString(Data, next, out tmp);
                    BuildDateString = tmp;

                    next2 -= (next2 % 8);
                    DMATableAddress = next2;
                    return;
                }
            }

            if (DMATableAddress == -1) throw new ROMHandlerException("Could not find DMA table.");
        }

        private void FindFileNameTable()
        {
            for (int i = 0; i < MinROMSize; i += 4)
            {
                if (Encoding.ASCII.GetString(Data, i, 7) == "makerom")
                {
                    FileNameTableAddress = i;
                    return;
                }
            }
        }

        private void ReadDMATable()
        {
            Files = new List<DMATableEntry>();

            int idx = 0;
            while (true)
            {
                if (DMATableAddress + (idx * 0x10) > Data.Length) throw new ROMHandlerException("Went out of range while reading DMA table.");
                DMATableEntry ndma = new DMATableEntry(this, idx);
                if (ndma.VStart == 0 && ndma.VEnd == 0 && ndma.PStart == 0) break;
                Files.Add(ndma);
                idx++;
            }
        }

        private void ReadFileNameTable()
        {
            if (FileNameTableAddress == -1) return;

            int nofs = FileNameTableAddress;
            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].Name = Encoding.ASCII.GetString(Data, nofs, 50).TrimEnd('\0');
                int index = Files[i].Name.IndexOf('\0');
                if (index >= 0) Files[i].Name = Files[i].Name.Remove(index);
                nofs += Files[i].Name.Length;
                while (Data[nofs] == 0) nofs++;
            }
        }

        private void FindCodeFile()
        {
            Code = null;

            foreach (DMATableEntry dma in Files)
            {
                if (dma.IsValid == false) continue;

                byte[] fdata = new byte[dma.VEnd - dma.VStart];
                if (dma.IsCompressed == true) throw new ROMHandlerException("Compressed ROMs are not supported.");
                Array.Copy(Data, dma.PStart, fdata, 0, dma.VEnd - dma.VStart);

                for (int i = (fdata.Length - 8); i > Math.Min((uint)(fdata.Length - CodeUcodeThreshold), fdata.Length); i -= 8)
                {
                    if (Encoding.ASCII.GetString(fdata, i, 8) == "RSP Gfx ")
                    {
                        Code = dma;
                        CodeData = fdata;
                        return;
                    }
                }
            }

            if (Code == null) throw new ROMHandlerException("Could not find code file.");
        }

        private void FindSceneTable()
        {
            Scenes = new List<SceneTableEntry>();

            if (!HasZ64TablesHack)
            {
                int inc = 16;
                for (int i = 0; i < CodeData.Length - (16 * 16); i += inc)
                {
                    SceneTableEntry scn1 = new SceneTableEntry(this, i, true);
                    SceneTableEntry scn2 = new SceneTableEntry(this, i + 20, true);
                    if (scn1.IsValid == false && scn2.IsValid == false && Scenes.Count > 0) break;
                    if (scn1.IsValid == true && scn2.IsValid == true && Scenes.Count == 0)
                    {
                        SceneTableAddress = i;
                    }
                    if (scn1.IsValid == true && (scn2.IsValid == true || Scenes.Count > 0)) { inc = 20; Scenes.Add(scn1); }
                }
            }
            else
            {
                SceneTableAddress = Endian.SwapInt32(BitConverter.ToInt32(Data, Z64TablesAdrOffset));
                int cnt = Endian.SwapInt32(BitConverter.ToInt32(Data, Z64TablesAdrOffset + 4));
                for (int i = 0; i < cnt; i++)
                {
                    Scenes.Add(new SceneTableEntry(this, SceneTableAddress + (i * 20), false));
                }
            }

            SceneNameACStrings = new System.Windows.Forms.AutoCompleteStringCollection();
            foreach (SceneTableEntry ste in Scenes)
            {
                ste.ReadScene();
                SceneNameACStrings.Add(ste.Name);
            }
        }

        private void FindActorTable()
        {
            Actors = new List<ActorTableEntry>();

            if (!HasZ64TablesHack)
            {
                int inc = 16;
                for (int i = 0; i < CodeData.Length - (16 * 16); i += inc)
                {
                    ActorTableEntry act1 = new ActorTableEntry(this, i, true);
                    ActorTableEntry act2 = new ActorTableEntry(this, i + 32, true);
                    ActorTableEntry act3 = new ActorTableEntry(this, i + 64, true);
                    if (act1.IsComplete == false && act1.IsEmpty == false && act2.IsComplete == false && act2.IsEmpty == false && Actors.Count > 0) break;
                    if ((act1.IsValid == true && act1.IsIncomplete == true) && (act2.IsComplete == true || act2.IsEmpty == true) && Actors.Count == 0)
                    {
                        ActorTableAddress = i;
                        break;
                    }
                }

                for (int i = (int)ActorTableAddress; i < CodeData.Length - 32; i += 32)
                {
                    ActorTableEntry nact = new ActorTableEntry(this, i, true);

                    if (nact.IsEmpty || nact.IsValid) Actors.Add(nact);
                    else break;
                }
            }
            else
            {
                ActorTableAddress = Endian.SwapInt32(BitConverter.ToInt32(Data, Z64TablesAdrOffset + 24));
                int cnt = Endian.SwapInt32(BitConverter.ToInt32(Data, Z64TablesAdrOffset + 28));
                for (int i = 0; i < cnt; i++)
                {
                    Actors.Add(new ActorTableEntry(this, ActorTableAddress + i * 32, false));
                }
            }
        }

        private void FindObjectTable()
        {
            Objects = new List<ObjectTableEntry>();
            ObjectTableAddress = 0;
            ObjectCount = 0;

            if (!HasZ64TablesHack)
            {
                int inc = 8;
                for (int i = (int)ActorTableAddress; i < CodeData.Length - (8 * 8); i += inc)
                {
                    ObjectCount = Endian.SwapUInt16(BitConverter.ToUInt16(CodeData, i - 2));
                    if (ObjectCount < 0x100 || ObjectCount > 0x300) continue;

                    ObjectTableEntry obj1 = new ObjectTableEntry(this, i, true);
                    ObjectTableEntry obj2 = new ObjectTableEntry(this, i + 8, true);
                    ObjectTableEntry obj3 = new ObjectTableEntry(this, i + 16, true);

                    if (obj1.IsEmpty == true && obj2.IsValid == true && obj3.IsValid == true && Objects.Count == 0)
                    {
                        ObjectTableAddress = i;
                        break;
                    }
                }

                if (ObjectTableAddress != 0 && ObjectCount != 0)
                {
                    int i, j = 0;
                    for (i = ObjectTableAddress; i < (ObjectTableAddress + (ObjectCount * 8)); i += 8)
                    {
                        Objects.Add(new ObjectTableEntry(this, i, true, (ushort)j));
                        j++;
                    }

                    ExitTableAddress = i + (i % 16);
                }
            }
            else
            {
                ObjectTableAddress = Endian.SwapInt32(BitConverter.ToInt32(Data, Z64TablesAdrOffset + 8));
                ObjectCount = (ushort)Endian.SwapInt32(BitConverter.ToInt32(Data, Z64TablesAdrOffset + 12));
                for (int i = 0; i < ObjectCount; i++)
                {
                    Objects.Add(new ObjectTableEntry(this, ObjectTableAddress + i * 8, false, (ushort)i));
                }
            }

            ObjectNameACStrings = new System.Windows.Forms.AutoCompleteStringCollection();
            foreach (ObjectTableEntry obj in Objects)
            {
                ObjectNameACStrings.Add(obj.Name);
            }
        }

        private void ReadExitTable()
        {
            Exits = new List<ExitTableEntry>();

            if (!HasZ64TablesHack)
            {
                int i = ExitTableAddress;
                while (i < SceneTableAddress)
                {
                    Exits.Add(new ExitTableEntry(this, i, true));
                    i += 4;
                }
            }
            else
            {
                ExitTableAddress = Endian.SwapInt32(BitConverter.ToInt32(Data, Z64TablesAdrOffset + 16));
                int cnt = Endian.SwapInt32(BitConverter.ToInt32(Data, Z64TablesAdrOffset + 20));
                for (int i = 0; i < cnt; i++)
                {
                    Exits.Add(new ExitTableEntry(this, ExitTableAddress + i * 4, false));
                }
            }
        }
    }
}
