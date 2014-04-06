using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.ROMHandler
{
    public class DMATableEntry
    {
        public enum FileTypes { Undefined, General, Empty, Scene, Room, Overlay, Object, Invalid };

        const int HeaderScanThreshold = 0x200;

        public uint VStart { get; private set; }
        public uint VEnd { get; private set; }
        public uint PStart { get; private set; }
        public uint PEnd { get; private set; }

        public string Name { get; set; }

        public bool IsValid { get; private set; }
        public bool IsCompressed { get; private set; }

        public FileTypes FileType { get; private set; }
        public byte AssumedSegment { get; private set; }

        public DMATableEntry(ROMHandler rom, int idx)
        {
            int readofs = (rom.DMATableAddress + (idx * 0x10));

            VStart = Endian.SwapUInt32(BitConverter.ToUInt32(rom.Data, readofs));
            VEnd = Endian.SwapUInt32(BitConverter.ToUInt32(rom.Data, readofs + 4));
            PStart = Endian.SwapUInt32(BitConverter.ToUInt32(rom.Data, readofs + 8));
            PEnd = Endian.SwapUInt32(BitConverter.ToUInt32(rom.Data, readofs + 12));

            if (PStart == 0xFFFFFFFF || PEnd == 0xFFFFFFFF)
                IsValid = false;
            else
            {
                IsValid = true;
                if (PEnd != 0 && Encoding.ASCII.GetString(rom.Data, (int)PStart, 4) == "Yaz0") IsCompressed = true;
                else IsCompressed = false;
            }

            Name = string.Format("File #{0}", idx);

            FileType = FileTypes.Undefined;
            AssumedSegment = 0x00;
        }

        public void Identify(ROMHandler rom)
        {
            FileTypes fnassumed = FileTypes.General;

            if (rom.FileNameTableAddress != -1)
            {
                if (Name.EndsWith("_scene") == true) fnassumed = FileTypes.Scene;
                else if (Name.Contains("_room_") == true) fnassumed = FileTypes.Room;
                else if (Name.StartsWith("ovl_") == true) fnassumed = FileTypes.Overlay;
                else if (Name.StartsWith("object_") == true) fnassumed = FileTypes.Object;
            }

            /* Invalid file? */
            if (!IsValid || VEnd - VStart == 0)
            {
                FileType = FileTypes.Invalid;
                return;
            }

            if (!IsCompressed)
            {
                byte[] data = new byte[VEnd - VStart];
                Buffer.BlockCopy(rom.Data, (int)PStart, data, 0, data.Length);

                /* Room file? */
                if (BitConverter.ToUInt32(data, (int)0) == 0x16 || ((BitConverter.ToUInt32(data, (int)0) == 0x18) && data[4] == 0x03 && BitConverter.ToUInt32(data, (int)8) == 0x16))
                {
                    for (int i = 8; i < HeaderScanThreshold; i += 8)
                        if (BitConverter.ToUInt32(data, i) == 0x14 && (fnassumed == FileTypes.General || fnassumed == FileTypes.Room))
                        {
                            AssumedSegment = 0x03;
                            FileType = FileTypes.Room;
                            return;
                        }
                }

                /* Scene file? */
                if ((BitConverter.ToUInt32(data, (int)0) & 0xFFFF00FF) == 0x15 || ((BitConverter.ToUInt32(data, (int)0) == 0x18) && data[4] == 0x02 && (BitConverter.ToUInt32(data, (int)8) & 0xFFFF00FF) == 0x15))
                {
                    for (int i = 8; i < HeaderScanThreshold; i += 8)
                        if (BitConverter.ToUInt32(data, i) == 0x14 && (fnassumed == FileTypes.General || fnassumed == FileTypes.Scene))
                        {
                            AssumedSegment = 0x02;
                            FileType = FileTypes.Scene;
                            return;
                        }
                }

                /* Overlay file? */
                uint ovlheader = ((uint)data.Length - Endian.SwapUInt32(BitConverter.ToUInt32(data, (data.Length - 4))));
                if ((ovlheader + 16) < data.Length)
                {
                    uint btext = Endian.SwapUInt32(BitConverter.ToUInt32(data, (int)ovlheader));
                    uint bdata = Endian.SwapUInt32(BitConverter.ToUInt32(data, (int)ovlheader + 4));
                    uint brodata = Endian.SwapUInt32(BitConverter.ToUInt32(data, (int)ovlheader + 8));
                    uint bssdata = Endian.SwapUInt32(BitConverter.ToUInt32(data, (int)ovlheader + 12));

                    if ((btext + bdata + brodata == data.Length || btext + bdata + brodata == ovlheader) && (fnassumed == FileTypes.General || fnassumed == FileTypes.Overlay))
                    {
                        FileType = FileTypes.Overlay;
                        return;
                    }
                }

                /* Object file? */
                bool indl, hassync, hasvtx, hasdlend;
                int[] segcount = new int[16];
                indl = hassync = hasvtx = hasdlend = false;
                for (int i = 0; i < data.Length; i += 8)
                {
                    if (BitConverter.ToUInt32(data, i) == 0xE7 && BitConverter.ToUInt32(data, i + 4) == 0x0)
                    {
                        hassync = true;
                        indl = true;
                    }
                    else if (indl && data[i] == 0x01 && data[i + 4] <= 0x0F)
                    {
                        hasvtx = true;
                        segcount[data[i + 4]]++;
                    }
                    else if (BitConverter.ToUInt32(data, i) == 0xDF && BitConverter.ToUInt32(data, i + 4) == 0x0)
                    {
                        hasdlend = true;
                        indl = false;
                    }
                }
                if (hassync && hasvtx && hasdlend && (fnassumed == FileTypes.General || fnassumed == FileTypes.Object))
                {
                    AssumedSegment = (byte)segcount.ToList().IndexOf(segcount.Max());
                    FileType = FileTypes.Object;
                    return;
                }

                /* Empty file? */
                if (data.Length < 0x100)
                {
                    int isempty = data.Count(x => x != 0);
                    if (isempty == 0)
                    {
                        FileType = FileTypes.Empty;
                        return;
                    }
                }
            }

            /* Use assumption */
            FileType = fnassumed;
        }
    }
}
