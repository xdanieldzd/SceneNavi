using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.SimpleF3DEX2
{
    internal class Texture
    {
        public uint Address { get; set; }
        public byte Format { get; set; }
        public uint CMS { get; set; }
        public uint CMT { get; set; }
        public int LineSize { get; set; }
        public int Palette { get; set; }
        public int ShiftS { get; set; }
        public int ShiftT { get; set; }
        public int MaskS { get; set; }
        public int MaskT { get; set; }
        public int Tile { get; set; }
        public int ULS { get; set; }
        public int ULT { get; set; }
        public int LRS { get; set; }
        public int LRT { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int RealWidth { get; set; }
        public int RealHeight { get; set; }

        public float ScaleS { get; set; }
        public float ScaleT { get; set; }
        public float ShiftScaleS { get; set; }
        public float ShiftScaleT { get; set; }

        public int GLID { get; set; }

        public Texture()
        {
            ScaleS = ScaleT = 1.0f;
            ShiftScaleS = ShiftScaleT = 1.0f;
        }
    }
}
