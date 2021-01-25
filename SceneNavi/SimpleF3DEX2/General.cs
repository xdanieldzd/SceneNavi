using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.SimpleF3DEX2
{
	class General
	{
		public static float[] Fixed2Float =
		{
			1.0f,
			0.5f, 0.25f, 0.125f, 0.0625f, 0.03125f, 0.015625f, 0.0078125f, 0.00390625f,
			0.001953125f, 0.0009765625f, 0.00048828125f, 0.00024414063f, 0.00012207031f, 6.1035156e-05f, 3.0517578e-05f, 1.5258789e-05f
		};

		public enum UcodeCmds : byte
		{
			/* Generic */
			SETCIMG = 0xFF,
			SETZIMG = 0xFE,
			SETTIMG = 0xFD,
			SETCOMBINE = 0xFC,
			SETENVCOLOR = 0xFB,
			SETPRIMCOLOR = 0xFA,
			SETBLENDCOLOR = 0xF9,
			SETFOGCOLOR = 0xF8,
			SETFILLCOLOR = 0xF7,
			FILLRECT = 0xF6,
			SETTILE = 0xF5,
			LOADTILE = 0xF4,
			LOADBLOCK = 0xF3,
			SETTILESIZE = 0xF2,
			LOADTLUT = 0xF0,
			RDPSETOTHERMODE = 0xEF,
			SETPRIMDEPTH = 0xEE,
			SETSCISSOR = 0xED,
			SETCONVERT = 0xEC,
			SETKEYR = 0xEB,
			SETKEYGB = 0xEA,
			RDPFULLSYNC = 0xE9,
			RDPTILESYNC = 0xE8,
			RDPPIPESYNC = 0xE7,
			RDPLOADSYNC = 0xE6,
			TEXRECTFLIP = 0xE5,
			TEXRECT = 0xE4,

			/* F3DEX2 */
			VTX = 0x01,
			MODIFYVTX = 0x02,
			CULLDL = 0x03,
			BRANCH_Z = 0x04,
			TRI1 = 0x05,
			TRI2 = 0x06,
			QUAD = 0x07,
			SPECIAL_3 = 0xD3,
			SPECIAL_2 = 0xD4,
			SPECIAL_1 = 0xD5,
			DMA_IO = 0xD6,
			TEXTURE = 0xD7,
			POPMTX = 0xD8,
			GEOMETRYMODE = 0xD9,
			MTX = 0xDA,
			MOVEWORD = 0xDB,
			MOVEMEM = 0xDC,
			LOAD_UCODE = 0xDD,
			DL = 0xDE,
			ENDDL = 0xDF,
			SPNOOP = 0xE0,
			RDPHALF_1 = 0xE1,
			SETOTHERMODE_L = 0xE2,
			SETOTHERMODE_H = 0xE3,
			RDPHALF_2 = 0xF1
		}

		public enum GeometryMode : uint
		{
			ZBUFFER = 0x01,
			SHADE = 0x04,
			CULL_FRONT = 0x0200,
			CULL_BACK = 0x0400,
			CULL_BOTH = 0x0600,
			FOG = 0x010000,
			LIGHTING = 0x020000,
			TEXTURE_GEN = 0x040000,
			TEXTURE_GEN_LINEAR = 0x080000,
			LOD = 0x0100000,
			SHADING_SMOOTH = 0x0200000,
			CLIPPING = 0x0800000
		}

		public enum OtherModeHShifts : ushort
		{
			ALPHADITHER = 4,
			RGBDITHER = 6,
			COMBKEY = 8,
			TEXTCONV = 9,
			TEXTFILT = 12,
			TEXTLUT = 14,
			TEXTLOD = 16,
			TEXTDETAIL = 17,
			TEXTPERSP = 19,
			CYCLETYPE = 20,
			PIPELINE = 23,
		}

		public enum OtherModeL : ushort
		{
			AA_EN = 0x0008,
			Z_CMP = 0x0010,
			Z_UPD = 0x0020,
			IM_RD = 0x0040,
			CLR_ON_CVG = 0x0080,
			CVG_DST_WRAP = 0x0100,
			CVG_DST_FULL = 0x0200,
			CVG_DST_SAVE = 0x0300,
			ZMODE_INTER = 0x0400,
			ZMODE_XLU = 0x0800,
			ZMODE_DEC = 0x0C00,
			CVG_X_ALPHA = 0x1000,
			ALPHA_CVG_SEL = 0x2000,
			FORCE_BL = 0x4000
		}

		internal static void PerformModeChanges(F3DEX2Interpreter f3dex2)
		{
			/* Textures */
			if (Configuration.RenderTextures)
			{
				GL.Enable(EnableCap.Texture2D);

				/* Texgen */
				if (Convert.ToBoolean(f3dex2.GeometryMode & (uint)GeometryMode.TEXTURE_GEN))
				{
					GL.TexGen(TextureCoordName.S, TextureGenParameter.TextureGenMode, (int)TextureGenMode.NormalMap);
					GL.TexGen(TextureCoordName.T, TextureGenParameter.TextureGenMode, (int)TextureGenMode.NormalMap);
					GL.Enable(EnableCap.TextureGenS);
					GL.Enable(EnableCap.TextureGenT);
				}
				else
				{
					GL.Disable(EnableCap.TextureGenS);
					GL.Disable(EnableCap.TextureGenT);
				}
			}
			else
				GL.Disable(EnableCap.Texture2D);

			/* Culling */
			if (Convert.ToBoolean(f3dex2.GeometryMode & (uint)GeometryMode.CULL_BOTH))
			{
				GL.Enable(EnableCap.CullFace);
				if (Convert.ToBoolean(f3dex2.GeometryMode & (uint)GeometryMode.CULL_BACK))
					GL.CullFace(CullFaceMode.Back);
				else
					GL.CullFace(CullFaceMode.Front);
			}
			else
				GL.Disable(EnableCap.CullFace);

			/* Lighting */
			if (Convert.ToBoolean(f3dex2.GeometryMode & (uint)GeometryMode.LIGHTING))
			{
				GL.Enable(EnableCap.Lighting);
				GL.Enable(EnableCap.Normalize);
			}
			else
			{
				GL.Disable(EnableCap.Lighting);
				GL.Disable(EnableCap.Normalize);
			}

			/* Fog */
			if (Convert.ToBoolean(f3dex2.GeometryMode & (uint)GeometryMode.FOG))
				GL.Enable(EnableCap.Fog);
			else
				GL.Disable(EnableCap.Fog);

			/* Decal */
			if (f3dex2.OtherModeL.ZModeDec)
			{
				GL.Enable(EnableCap.PolygonOffsetFill);
				GL.PolygonOffset(-1.0f, -1.0f);
			}
			else
				GL.Disable(EnableCap.PolygonOffsetFill);

			/* Alpha test */
			if ((f3dex2.OtherModeL.CvgXAlpha || f3dex2.OtherModeL.AlphaCvgSel))
			{
				GL.Enable(EnableCap.AlphaTest);
				GL.Disable(EnableCap.Blend);
				if (f3dex2.OtherModeL.AlphaCvgSel)
					GL.AlphaFunc(AlphaFunction.Gequal, 0.125f);
				else
					GL.AlphaFunc(AlphaFunction.Greater, 0.0f);
			}
			else
				GL.Disable(EnableCap.AlphaTest);

			/* Force blending */
			if (f3dex2.OtherModeL.ForceBl)
			{
				GL.Disable(EnableCap.AlphaTest);
				GL.Enable(EnableCap.Blend);
			}
			else
				GL.Disable(EnableCap.Blend);

			/* Z-compare */
			if (f3dex2.OtherModeL.ZCmp)
				GL.DepthFunc(DepthFunction.Lequal);
			else
				GL.DepthFunc(DepthFunction.Always);

			/* Z-update */
			if (f3dex2.OtherModeL.ZUpd)
				GL.DepthMask(true);
			else
				GL.DepthMask(false);

			/* Blend modes */
			switch (f3dex2.OtherModeL.Data >> 16)
			{
				case 0x0448:
				case 0x055A:
					GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
					break;
				case 0x0382:
				case 0x0091:
				case 0x0C08:
				case 0x0F0A:
				case 0x0302:
					GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
					break;
				case 0xAF50:
				case 0x0F5A:
				case 0x0FA5:
				case 0x5055:
					GL.BlendFunc(BlendingFactor.Zero, BlendingFactor.One);
					break;
				case 0x5F50:
					GL.BlendFunc(BlendingFactor.Zero, BlendingFactor.OneMinusSrcAlpha);
					break;
				default:
					GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
					break;
			}
		}

		internal static void RenderTriangles(F3DEX2Interpreter f3dex2, int[] idx)
		{
			GL.Begin(PrimitiveType.Triangles);

			foreach (int i in idx)
			{
				if (i >= f3dex2.VertexBuffer.Length) continue;

				double[] S = new double[2], T = new double[2];

				for (int j = 0; j < (OpenGLHelpers.Initialization.SupportsFunction("glActiveTextureARB") ? f3dex2.Textures.Length : 1); j++)
				{
					if (f3dex2.Textures[j].MaskS != 0)
						S[j] = (f3dex2.VertexBuffer[i].TexCoord.X * f3dex2.Textures[j].ShiftScaleS * f3dex2.ScaleS[j] -
							((f3dex2.Textures[j].ULS * Fixed2Float[2]) % (1 << f3dex2.Textures[j].MaskS))) * f3dex2.Textures[j].ScaleS;
					else
						S[j] = (f3dex2.VertexBuffer[i].TexCoord.X * f3dex2.Textures[j].ShiftScaleS * f3dex2.ScaleS[j] -
							(f3dex2.Textures[j].ULS * Fixed2Float[2])) * f3dex2.Textures[j].ScaleS;

					if (f3dex2.Textures[j].MaskT != 0)
						T[j] = (f3dex2.VertexBuffer[i].TexCoord.Y * f3dex2.Textures[j].ShiftScaleT * f3dex2.ScaleT[j] -
							((f3dex2.Textures[j].ULT * Fixed2Float[2]) % (1 << f3dex2.Textures[j].MaskT))) * f3dex2.Textures[j].ScaleT;
					else
						T[j] = (f3dex2.VertexBuffer[i].TexCoord.Y * f3dex2.Textures[j].ShiftScaleT * f3dex2.ScaleT[j] -
							(f3dex2.Textures[j].ULT * Fixed2Float[2])) * f3dex2.Textures[j].ScaleT;

					OpenGLHelpers.Initialization.MultiTexCoord2Checked(TextureUnit.Texture0 + j, S[j], T[j]);
				}

				if (!Convert.ToBoolean(f3dex2.GeometryMode & (uint)GeometryMode.LIGHTING)) GL.Color4(f3dex2.VertexBuffer[i].Colors);
				GL.Normal3(f3dex2.VertexBuffer[i].Normals);
				GL.Vertex3(f3dex2.VertexBuffer[i].Position);
			}

			GL.End();
		}

		internal static int Pow2(int dim)
		{
			int i = 1;

			while (i < dim) i <<= 1;

			return i;
		}

		internal static int PowOf(int dim)
		{
			int num = 1;
			int i = 0;

			while (num < dim)
			{
				num <<= 1;
				i++;
			}

			return i;
		}

		internal static int ShiftL(uint v, int s, int w)
		{
			return (int)((v & (((uint)0x01 << w) - 1)) << s);
		}

		internal static int ShiftR(uint v, int s, int w)
		{
			return (int)((v >> s) & ((0x01 << w) - 1));
		}
	}
}
