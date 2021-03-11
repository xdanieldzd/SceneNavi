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

			public Color AmbientLightColor { get; set; }
			public Vector4 Light1Direction { get; set; }
			public Color Light1Color { get; set; }
			public Vector4 Light2Direction { get; set; }
			public Color Light2Color { get; set; }
			public Color FogColor { get; set; }
			public ushort LightBlendRate { get; set; }      // ???
			public ushort FogNear { get; set; }             // 0 to 1000, max is 999?
			public short DrawDistance { get; set; }         // ??? to 32767

			ROMHandler.ROMHandler ROM;

			public Entry() { }

			public Entry(ROMHandler.ROMHandler rom, uint adr)
			{
				ROM = rom;
				Address = adr;

				byte[] segdata = (byte[])ROM.SegmentMapping[(byte)(adr >> 24)];
				if (segdata == null) return;

				adr &= 0xFFFFFF;

				AmbientLightColor = Color.FromArgb(segdata[adr], segdata[adr + 1], segdata[adr + 2]);
				Light1Direction = new Vector4(((sbyte)segdata[adr + 3] / 255.0f), ((sbyte)segdata[adr + 4] / 255.0f), ((sbyte)segdata[adr + 5] / 255.0f), 0.0f);
				Light1Color = Color.FromArgb(segdata[adr + 6], segdata[adr + 7], segdata[adr + 8]);
				Light2Direction = new Vector4(((sbyte)segdata[adr + 9] / 255.0f), ((sbyte)segdata[adr + 10] / 255.0f), ((sbyte)segdata[adr + 11] / 255.0f), 0.0f);
				Light2Color = Color.FromArgb(segdata[adr + 12], segdata[adr + 13], segdata[adr + 14]);
				FogColor = Color.FromArgb(segdata[adr + 15], segdata[adr + 16], segdata[adr + 17]);
				LightBlendRate = (ushort)((Endian.SwapUInt16(BitConverter.ToUInt16(segdata, (int)(adr + 18))) & 0xFC00) >> 10);
				FogNear = (ushort)(Endian.SwapUInt16(BitConverter.ToUInt16(segdata, (int)(adr + 18))) & 0x03FF);
				DrawDistance = (short)Endian.SwapUInt16(BitConverter.ToUInt16(segdata, (int)(adr + 20)));
			}

			public void InitializeLighting()
			{
				// TODO  make correct, look up in SDK docs, etc, etc!!!

				GL.PushMatrix();
				GL.LoadIdentity();
				GL.Light(LightName.Light0, LightParameter.Ambient, AmbientLightColor);
				GL.Light(LightName.Light1, LightParameter.Diffuse, Light1Color);
				GL.Light(LightName.Light1, LightParameter.Position, Light1Direction);
				GL.Light(LightName.Light2, LightParameter.Diffuse, Light2Color);
				GL.Light(LightName.Light2, LightParameter.Position, Light2Direction);
				GL.PopMatrix();

				GL.Enable(EnableCap.Light0);
				GL.Enable(EnableCap.Light1);
				GL.Enable(EnableCap.Light2);
			}

			public void InitializeFog(float scale)
			{
				GL.Fog(FogParameter.FogMode, (int)FogMode.Exp2);
				GL.Hint(HintTarget.FogHint, HintMode.Nicest);
				GL.Fog(FogParameter.FogColor, new float[] { FogColor.R / 255.0f, FogColor.G / 255.0f, FogColor.B / 255.0f });
				GL.Fog(FogParameter.FogDensity, 1.0f - (FogNear / 1000.0f));
			}
		}
	}
}
