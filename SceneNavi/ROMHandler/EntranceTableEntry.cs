using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SceneNavi.ROMHandler
{
    public class EntranceTableEntry
    {
        [ReadOnly(true)]
        public ushort Number { get; set; }

        [Browsable(false)]
        public int Offset { get; private set; }
        [Browsable(false)]
        public bool IsOffsetRelative { get; private set; }

        [DisplayName("Scene #")]
        public byte SceneNumber { get; set; }
        [DisplayName("Entrance #")]
        public byte EntranceNumber { get; set; }
        [DisplayName("Variable")]
        public byte Variable { get; set; }
        [DisplayName("Fade")]
        public byte Fade { get; set; }

        [DisplayName("Scene Name")]
        public string SceneName
        {
            get
            {
                return (SceneNumber < ROM.Scenes.Count ? ROM.Scenes[SceneNumber].Name : "(invalid?)");
            }

            set
            {
                int scnidx = ROM.Scenes.FindIndex(x => x.Name.ToLowerInvariant() == value.ToLowerInvariant());
                if (scnidx != -1)
                    SceneNumber = (byte)scnidx;
                else
                    System.Media.SystemSounds.Hand.Play();
            }
        }

        ROMHandler ROM;

        public EntranceTableEntry(ROMHandler rom, int ofs, bool isrel)
        {
            ROM = rom;
            Offset = ofs;
            IsOffsetRelative = isrel;

            SceneNumber = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs];
            EntranceNumber = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 1];
            Variable = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 2];
            Fade = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 3];
        }

        public void SaveTableEntry()
        {
            (IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset] = SceneNumber;
            (IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 1] = EntranceNumber;
            (IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 2] = Variable;
            (IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 3] = Fade;
        }
    }
}
