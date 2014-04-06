using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using SceneNavi.ROMHandler;

namespace SceneNavi.HeaderCommands
{
    public class SettingsSoundScene : Generic, IStoreable
    {
        // https://www.the-gcn.com/topic/2471-the-beginners-guide-to-music-antiqua-teasers/?p=40641

        //Just to complement this info, the yy above referred as "music playback" option stands for the scene's night bgm.
        //The night bgm uses a different audio type, that plays nature sounds.
        //A setting of 0x00 is found into scenes with the complete day-night cycle and will play the standard night noises.
        //The 0x13 setting is found in dungeons and indoors, so the music will be always playing, independent of the time of the day.
        //01 - Standard night [Kakariko]
        //02 - Distant storm [Graveyard]
        //03 - Howling wind and cawing [Ganon's Castle]
        //04 - Wind + night birds [Kokiri]
        //05, 08, 09, 0D, 0E, 10, 12 - Wind + crickets
        //06,0C - Wind
        //07 - Howling wind
        //0A - Tubed howling wind [Wasteland]
        //0B - Tubed howling wind [Colossus]
        //0F - Wind + birds
        //14, 16, 18, 19, 1B, 1E - silence
        //1C - Rain
        //17, 1A, 1D, 1F - high tubed wind + rain

        public byte Reverb { get; set; }
        public byte NightSfxID { get; set; }
        public byte TrackID { get; set; }

        public SettingsSoundScene(Generic basecmd)
            : base(basecmd)
        {
            Reverb = (byte)((this.Data >> 48) & 0xFF);
            NightSfxID = (byte)((this.Data >> 8) & 0xFF);
            TrackID = (byte)(this.Data & 0xFF);
        }

        public void Store(byte[] databuf, int baseadr)
        {
            databuf[(int)(baseadr + (this.Offset & 0xFFFFFF) + 1)] = Reverb;
            databuf[(int)(baseadr + (this.Offset & 0xFFFFFF) + 6)] = NightSfxID;
            databuf[(int)(baseadr + (this.Offset & 0xFFFFFF) + 7)] = TrackID;
        }
    }
}
