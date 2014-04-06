using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.SimpleF3DEX2
{
    public class OtherModeL
    {
        public static OtherModeL Empty = new OtherModeL(0);

        public uint Data { get; private set; }

        public bool AAEn { get { return (Data & (uint)General.OtherModeL.AA_EN) != 0; } }
        public bool ZCmp { get { return (Data & (uint)General.OtherModeL.Z_CMP) != 0; } }
        public bool ZUpd { get { return (Data & (uint)General.OtherModeL.Z_UPD) != 0; } }
        public bool ImRd { get { return (Data & (uint)General.OtherModeL.IM_RD) != 0; } }
        public bool ClrOnCvg { get { return (Data & (uint)General.OtherModeL.CLR_ON_CVG) != 0; } }
        public bool CvgDstWrap { get { return (Data & (uint)General.OtherModeL.CVG_DST_WRAP) != 0; } }
        public bool CvgDstFull { get { return (Data & (uint)General.OtherModeL.CVG_DST_FULL) != 0; } }
        public bool CvgDstSave { get { return (Data & (uint)General.OtherModeL.CVG_DST_SAVE) != 0; } }
        public bool ZModeInter { get { return (Data & (uint)General.OtherModeL.ZMODE_INTER) != 0; } }
        public bool ZModeXlu { get { return (Data & (uint)General.OtherModeL.ZMODE_XLU) != 0; } }
        public bool ZModeDec { get { return (Data & (uint)General.OtherModeL.ZMODE_DEC) != 0; } }
        public bool CvgXAlpha { get { return (Data & (uint)General.OtherModeL.CVG_X_ALPHA) != 0; } }
        public bool AlphaCvgSel { get { return (Data & (uint)General.OtherModeL.ALPHA_CVG_SEL) != 0; } }
        public bool ForceBl { get { return (Data & (uint)General.OtherModeL.FORCE_BL) != 0; } }

        public OtherModeL(uint data)
        {
            Data = data;
        }
    }
}
