using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.HeaderCommands
{
    public class EnvironmentSettings : Generic
    {
        public List<Entry> EnvSettingList { get; set; }

        public EnvironmentSettings(Generic basecmd)
            : base(basecmd)
        {
            EnvSettingList = new List<Entry>();
            for (int i = 0; i < GetCountGeneric(); i++) EnvSettingList.Add(new Entry(ROM, (uint)(GetAddressGeneric() + i * 22)));
        }

        public class Entry
        {
            public uint Address { get; set; }

            public Color AmbientColor { get; set; }
            public Color Diffuse1Color { get; set; }
            public Vector4 Diffuse1Direction { get; set; }
            public Color Diffuse2Color { get; set; }
            public Vector4 Diffuse2Direction { get; set; }
            public Color FogColor { get; set; }
            public short ForFarDistance { get; set; }
            public short FogNearDistance { get; set; }

            ROMHandler.ROMHandler ROM;

            public Entry() { }

            public Entry(ROMHandler.ROMHandler rom, uint adr)
            {
                ROM = rom;
                Address = adr;

                byte[] segdata = (byte[])ROM.SegmentMapping[(byte)(adr >> 24)];
                if (segdata == null) return;

                adr &= 0xFFFFFF;

                AmbientColor = Color.FromArgb(segdata[adr], segdata[adr + 1], segdata[adr + 2]);
                Diffuse1Direction = new Vector4(((sbyte)segdata[adr + 3] / 255.0f), ((sbyte)segdata[adr + 4] / 255.0f), ((sbyte)segdata[adr + 5] / 255.0f), 0.0f);
                Diffuse1Color = Color.FromArgb(segdata[adr + 6], segdata[adr + 7], segdata[adr + 8]);
                Diffuse2Direction = new Vector4(((sbyte)segdata[adr + 9] / 255.0f), ((sbyte)segdata[adr + 10] / 255.0f), ((sbyte)segdata[adr + 11] / 255.0f), 0.0f);
                Diffuse2Color = Color.FromArgb(segdata[adr + 12], segdata[adr + 13], segdata[adr + 14]);
                FogColor = Color.FromArgb(segdata[adr + 15], segdata[adr + 16], segdata[adr + 17]);
                FogNearDistance = Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)(adr + 18)));
                ForFarDistance = Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)(adr + 20)));
            }

            public void CreateLighting()
            {
                // TODO  make correct, look up in SDK docs, etc, etc!!!

                GL.PushMatrix();
                GL.LoadIdentity();
                GL.Light(LightName.Light0, LightParameter.Ambient, AmbientColor);
                GL.Light(LightName.Light1, LightParameter.Diffuse, Diffuse1Color);
                GL.Light(LightName.Light1, LightParameter.Position, Diffuse1Direction);
                GL.Light(LightName.Light2, LightParameter.Diffuse, Diffuse2Color);
                GL.Light(LightName.Light2, LightParameter.Position, Diffuse2Direction);
                GL.PopMatrix();

                GL.Enable(EnableCap.Light0);
                GL.Enable(EnableCap.Light1);
                GL.Enable(EnableCap.Light2);
                /*
                GL.Fog(FogParameter.FogMode, (int)FogMode.Linear);
                GL.Hint(HintTarget.FogHint, HintMode.Nicest);
                GL.Fog(FogParameter.FogColor, new float[] { FogColor.R / 255.0f, FogColor.G / 255.0f, FogColor.B / 255.0f });

                float fstart = (float)Near * 0.02f;
                float fend = (float)Far * 0.02f;
                GL.Fog(FogParameter.FogStart, fstart);
                GL.Fog(FogParameter.FogEnd, fend);

                GL.Enable(EnableCap.Fog);*/
            }
        }
    }
}
