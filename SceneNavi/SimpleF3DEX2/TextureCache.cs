using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.SimpleF3DEX2
{
    internal class TextureCache
    {
        public object Tag { get; set; }
        public byte Format { get; set; }
        public uint Address { get; set; }
        public int RealWidth { get; set; }
        public int RealHeight { get; set; }
        public int GLID { get; set; }

        public TextureCache(object tag, Texture tex, int glid)
        {
            Tag = tag;
            Format = tex.Format;
            Address = tex.Address;
            RealWidth = tex.RealWidth;
            RealHeight = tex.RealHeight;
            GLID = glid;
        }
    }
}
