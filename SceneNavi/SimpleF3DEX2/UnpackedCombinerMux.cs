using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.SimpleF3DEX2
{
    internal class UnpackedCombinerMux
    {
        public enum ComponentsC32 : byte
        {
            CCMUX_COMBINED = 0,
            CCMUX_TEXEL0 = 1,
            CCMUX_TEXEL1 = 2,
            CCMUX_PRIMITIVE = 3,
            CCMUX_SHADE = 4,
            CCMUX_ENVIRONMENT = 5,
            CCMUX_1 = 6,
            CCMUX_COMBINED_ALPHA = 7,
            CCMUX_TEXEL0_ALPHA = 8,
            CCMUX_TEXEL1_ALPHA = 9,
            CCMUX_PRIMITIVE_ALPHA = 10,
            CCMUX_SHADE_ALPHA = 11,
            CCMUX_ENV_ALPHA = 12,
            CCMUX_LOD_FRACTION = 13,
            CCMUX_PRIM_LOD_FRAC = 14,
            CCMUX_K5 = 15,
            CCMUX_0 = 31
        }

        public enum ComponentsC16 : byte
        {
            CCMUX_COMBINED = 0,
            CCMUX_TEXEL0 = 1,
            CCMUX_TEXEL1 = 2,
            CCMUX_PRIMITIVE = 3,
            CCMUX_SHADE = 4,
            CCMUX_ENVIRONMENT = 5,
            CCMUX_1 = 6,
            CCMUX_COMBINED_ALPHA = 7,
            CCMUX_TEXEL0_ALPHA = 8,
            CCMUX_TEXEL1_ALPHA = 9,
            CCMUX_PRIMITIVE_ALPHA = 10,
            CCMUX_SHADE_ALPHA = 11,
            CCMUX_ENV_ALPHA = 12,
            CCMUX_LOD_FRACTION = 13,
            CCMUX_PRIM_LOD_FRAC = 14,
            CCMUX_0 = 15
        }

        public enum ComponentsC8 : byte
        {
            CCMUX_COMBINED = 0,
            CCMUX_TEXEL0 = 1,
            CCMUX_TEXEL1 = 2,
            CCMUX_PRIMITIVE = 3,
            CCMUX_SHADE = 4,
            CCMUX_ENVIRONMENT = 5,
            CCMUX_1 = 6,
            CCMUX_0 = 7
        }

        public enum ComponentsA8 : byte
        {
            ACMUX_COMBINED = 0,
            ACMUX_TEXEL0 = 1,
            ACMUX_TEXEL1 = 2,
            ACMUX_PRIMITIVE = 3,
            ACMUX_SHADE = 4,
            ACMUX_ENVIRONMENT = 5,
            //ACMUX_PRIM_LOD_FRAC = 6,      //TODO check
            ACMUX_1 = 6,
            ACMUX_0 = 7
        }

        public ComponentsC16[] cA { get; set; }
        public ComponentsC16[] cB { get; set; }
        public ComponentsC32[] cC { get; set; }
        public ComponentsC8[] cD { get; set; }
        public ComponentsA8[] aA { get; set; }
        public ComponentsA8[] aB { get; set; }
        public ComponentsA8[] aC { get; set; }
        public ComponentsA8[] aD { get; set; }

        public UnpackedCombinerMux(ulong mux) : this((uint)(mux >> 16), (uint)(mux & 0xFFFFFFFF)) { }

        public UnpackedCombinerMux(uint m0, uint m1)
        {
            cA = new ComponentsC16[2];
            cB = new ComponentsC16[2];
            cC = new ComponentsC32[2];
            cD = new ComponentsC8[2];
            aA = new ComponentsA8[2];
            aB = new ComponentsA8[2];
            aC = new ComponentsA8[2];
            aD = new ComponentsA8[2];

            cA[0] = (ComponentsC16)(byte)((m0 >> 20) & 0x0F);
            cB[0] = (ComponentsC16)(byte)((m1 >> 28) & 0x0F);
            cC[0] = (ComponentsC32)(byte)((m0 >> 15) & 0x1F);
            cD[0] = (ComponentsC8)(byte)((m1 >> 15) & 0x07);

            aA[0] = (ComponentsA8)(byte)((m0 >> 12) & 0x07);
            aB[0] = (ComponentsA8)(byte)((m1 >> 12) & 0x07);
            aC[0] = (ComponentsA8)(byte)((m0 >> 9) & 0x07);
            aD[0] = (ComponentsA8)(byte)((m1 >> 9) & 0x07);

            cA[1] = (ComponentsC16)(byte)((m0 >> 5) & 0x0F);
            cB[1] = (ComponentsC16)(byte)((m1 >> 24) & 0x0F);
            cC[1] = (ComponentsC32)(byte)((m0 >> 0) & 0x1F);
            cD[1] = (ComponentsC8)(byte)((m1 >> 6) & 0x07);

            aA[1] = (ComponentsA8)(byte)((m1 >> 21) & 0x07);
            aB[1] = (ComponentsA8)(byte)((m1 >> 3) & 0x07);
            aC[1] = (ComponentsA8)(byte)((m1 >> 18) & 0x07);
            aD[1] = (ComponentsA8)(byte)((m1 >> 0) & 0x07);
        }
    }
}
