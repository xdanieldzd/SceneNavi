using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using SceneNavi.HeaderCommands;

namespace SceneNavi.ROMHandler
{
    public interface IHeaderParent { /* Marker interface is A-OK */ }

    public class HeaderLoader
    {
        /* Used to simplify association of room headers with scene header, for grouping headers by "stage" */
        public class HeaderPair
        {
            public HeaderLoader SceneHeader { get; private set; }
            public List<HeaderLoader> RoomHeaders { get; private set; }

            public string Description { get; set; }

            public HeaderPair(HeaderLoader sh, List<HeaderLoader> rhs)
            {
                SceneHeader = sh;
                RoomHeaders = rhs;

                Description = null;
            }
        }

        /* Speaking of stages, some conversion stuff... */
        [TypeConverter(typeof(StageKeyConverter))]
        public class StageKey
        {
            public uint SceneAddress { get; set; }
            public int HeaderNumber { get; set; }
            public string Format
            {
                get
                {
                    return "0x" + SceneAddress.ToString("X8") + ", " + HeaderNumber;
                }
            }

            public StageKey(string format)
            {
                var parts = format.Split(',');
                if (parts.Length != 2)
                {
                    throw new Exception("Invalid format");
                }

                SceneAddress = uint.Parse(parts[0].Substring(2), System.Globalization.NumberStyles.HexNumber);
                HeaderNumber = int.Parse(parts[1]);
            }

            public StageKey(uint sa, int hn)
            {
                SceneAddress = sa;
                HeaderNumber = hn;
            }
        }

        public class StageKeyConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                return new StageKey((string)value);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(string);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                var val = (StageKey)value;
                return val.SceneAddress + ", " + val.HeaderNumber;
            }
        }

        /* Command IDs */
        public enum CommandTypeIDs : byte
        {
            Spawns = 0x00,
            Actors = 0x01,
            Unknown0x02 = 0x02,
            Collision = 0x03,
            Rooms = 0x04,
            WindSettings = 0x05,
            Entrances = 0x06,
            SpecialObjects = 0x07,
            RoomBehavior = 0x08,
            Unknown0x09 = 0x09,
            MeshHeader = 0x0A,
            Objects = 0x0B,
            Unknown0x0C = 0x0C,
            Waypoints = 0x0D,
            Transitions = 0x0E,
            EnvironmentSettings = 0x0F,
            SettingsTime = 0x10,
            SettingsSkyboxScene = 0x11,
            SettingsSkyboxRoom = 0x12,
            Exits = 0x13,
            EndOfHeader = 0x14,
            SettingsSoundScene = 0x15,
            SettingsSoundRoom = 0x16,
            Cutscenes = 0x17,
            SubHeaders = 0x18,
            SceneBehavior = 0x19
        }

        /* Translation table for commands */
        public static System.Collections.Hashtable CommandHumanNames = new System.Collections.Hashtable()
        {
            { CommandTypeIDs.Spawns, "Spawn points" },
            { CommandTypeIDs.Actors, "Actors" },
            { CommandTypeIDs.Unknown0x02, "Unknown 0x02" },
            { CommandTypeIDs.Collision, "Collision" },
            { CommandTypeIDs.Rooms, "Rooms" },
            { CommandTypeIDs.WindSettings, "Wind settings" },
            { CommandTypeIDs.Entrances, "Entrances" },
            { CommandTypeIDs.SpecialObjects, "Special objects" },
            { CommandTypeIDs.RoomBehavior, "Room behavior" },
            { CommandTypeIDs.Unknown0x09, "Unknown 0x09" },
            { CommandTypeIDs.MeshHeader, "Mesh header" },
            { CommandTypeIDs.Objects, "Objects" },
            { CommandTypeIDs.Unknown0x0C, "Unknown 0x0C" },
            { CommandTypeIDs.Waypoints, "Waypoints" },
            { CommandTypeIDs.Transitions, "Transition actors" },
            { CommandTypeIDs.EnvironmentSettings, "Enviroments settings" },
            { CommandTypeIDs.SettingsTime, "Time settings" },
            { CommandTypeIDs.SettingsSkyboxScene, "Skybox settings (scene)" },
            { CommandTypeIDs.SettingsSkyboxRoom, "Skybox settings (room)" },
            { CommandTypeIDs.Exits, "Exits" },
            { CommandTypeIDs.EndOfHeader, "End of header" },
            { CommandTypeIDs.SettingsSoundScene, "Sound settings (scene)" },
            { CommandTypeIDs.SettingsSoundRoom, "Sound settings (room)" },
            { CommandTypeIDs.Cutscenes, "Cutscenes" },
            { CommandTypeIDs.SubHeaders, "Sub-headers" },
            { CommandTypeIDs.SceneBehavior, "Scene behavior" }
        };

        /* Command ID to implementing class associations; add here to add new header command classes! */
        public static System.Collections.Hashtable CommandTypes = new System.Collections.Hashtable()
        {
            { CommandTypeIDs.Rooms, typeof(Rooms) },
            { CommandTypeIDs.MeshHeader, typeof(MeshHeader) },
            { CommandTypeIDs.Actors, typeof(Actors) },
            { CommandTypeIDs.Transitions, typeof(Actors) },
            { CommandTypeIDs.Spawns, typeof(Actors) },
            { CommandTypeIDs.Objects, typeof(Objects) },
            { CommandTypeIDs.SpecialObjects, typeof(SpecialObjects) },
            { CommandTypeIDs.Waypoints, typeof(Waypoints) },
            { CommandTypeIDs.Collision, typeof(Collision) },
            { CommandTypeIDs.SettingsSoundScene, typeof(SettingsSoundScene) },
            { CommandTypeIDs.EnvironmentSettings, typeof(EnvironmentSettings) },
        };

        public List<Generic> Commands { get; private set; }

        public int Offset { get; private set; }
        public byte Segment { get; private set; }
        public int Number { get; private set; }

        public string Description { get { return string.Format("0x{0:X8}", (Offset | (Segment << 24))); } }

        public IHeaderParent Parent { get; private set; }

        public HeaderLoader(ROMHandler rom, IHeaderParent parent, byte seg, int ofs, int number)
        {
            Parent = parent;

            Offset = ofs;
            Segment = seg;
            Number = number;

            Commands = new List<Generic>();
            Generic cmd = null;
            while ((cmd = new Generic(rom, parent, seg, ref ofs)).Command != CommandTypeIDs.EndOfHeader)
            {
                Type cmdtype = (Type)CommandTypes[cmd.Command];
                object inst = Activator.CreateInstance((cmdtype == null ? typeof(Generic) : cmdtype), new object[] { cmd });
                Commands.Add((Generic)inst);
            }
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
