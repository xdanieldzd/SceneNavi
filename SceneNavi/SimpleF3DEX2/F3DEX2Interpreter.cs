using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SceneNavi.SimpleF3DEX2.CombinerEmulation;

namespace SceneNavi.SimpleF3DEX2
{
	public class F3DEX2Interpreter
	{
		public delegate void UcodeCommandDelegate(uint w0, uint w1);

		class Macro
		{
			public delegate void MacroDelegate(uint[] w0, uint[] w1);

			public MacroDelegate Function { get; private set; }
			public General.UcodeCmds[] Commands { get; private set; }

			public Macro(MacroDelegate func, General.UcodeCmds[] cmds)
			{
				Function = func;
				Commands = cmds;
			}
		}

		UcodeCommandDelegate[] ucodecmds;
		List<Macro> macros;
		bool inmacro;

		internal List<SimpleTriangle> LastTriList { get; private set; }

		internal Vertex[] VertexBuffer { get; private set; }
		uint rdphalf1, texaddress;
		internal uint LastComb0 { get; private set; }
		internal uint LastComb1 { get; private set; }
		public uint GeometryMode { get; private set; }
		public uint OtherModeH { get; private set; }
		public OtherModeL OtherModeL { get; private set; }
		internal Color4 PrimColor { get; private set; }
		internal Color4 EnvColor { get; private set; }
		Stack<Matrix4d> mtxstack;
		internal Texture[] Textures { get; private set; }
		readonly Color4[] palette;
		List<TextureCache> texcache;
		int activetex;
		bool multitex;
		public float[] ScaleS { get; private set; }
		public float[] ScaleT { get; private set; }

		ICombinerManager combinerManager;

		ROMHandler.ROMHandler ROM;
		Stack<OpenGLHelpers.DisplayListEx> ActiveGLDL;

		public F3DEX2Interpreter(ROMHandler.ROMHandler rom)
		{
			ROM = rom;
			ActiveGLDL = new Stack<OpenGLHelpers.DisplayListEx>();

			InitializeParser();
			InitializeMacros();

			LastTriList = new List<SimpleTriangle>();

			VertexBuffer = new Vertex[32];
			rdphalf1 = GeometryMode = OtherModeH = LastComb0 = LastComb1 = 0;
			OtherModeL = OtherModeL.Empty;
			mtxstack = new Stack<Matrix4d>();
			mtxstack.Push(Matrix4d.Identity);

			Textures = new Texture[2];
			Textures[0] = new Texture();
			Textures[1] = new Texture();
			palette = new Color4[256];
			texcache = new List<TextureCache>();
			activetex = 0;
			multitex = false;
			ScaleS = new float[2];
			ScaleT = new float[2];

			InitCombiner();
		}

		public void InitCombiner()
		{
			if (Configuration.CombinerType == CombinerTypes.ArbCombiner)
			{
				Program.Status.Message = "Initializing ARB combiner...";
				combinerManager = new ArbCombineManager();
			}
			else if (Configuration.CombinerType == CombinerTypes.GLSLCombiner)
			{
				Program.Status.Message = "Initializing GLSL combiner...";
				combinerManager = new GLSLCombineManager(this);
			}
		}

		public void ResetCaches()
		{
			ResetTextureCache();

			if (Configuration.CombinerType == CombinerTypes.ArbCombiner && combinerManager is ArbCombineManager arbCombiner) arbCombiner.ResetFragmentCache();

			if (LastTriList != null) LastTriList.Clear();
		}

		public void ResetTextureCache()
		{
			if (texcache != null)
			{
				foreach (TextureCache tc in texcache) if (GL.IsTexture(tc.GLID)) GL.DeleteTexture(tc.GLID);
				texcache.Clear();
			}
		}

		private void InitializeParser()
		{
			ucodecmds = new UcodeCommandDelegate[256];
			for (int i = 0; i < ucodecmds.Length; i++) ucodecmds[i] = new UcodeCommandDelegate((w0, w1) => { });
			ucodecmds[(byte)General.UcodeCmds.VTX] = CommandVtx;
			ucodecmds[(byte)General.UcodeCmds.TRI1] = CommandTri1;
			ucodecmds[(byte)General.UcodeCmds.TRI2] = CommandTri2;
			ucodecmds[(byte)General.UcodeCmds.DL] = CommandDL;
			ucodecmds[(byte)General.UcodeCmds.RDPHALF_1] = CommandRDPHalf1;
			ucodecmds[(byte)General.UcodeCmds.BRANCH_Z] = CommandBranchZ;
			ucodecmds[(byte)General.UcodeCmds.GEOMETRYMODE] = CommandGeometryMode;
			ucodecmds[(byte)General.UcodeCmds.MTX] = CommandMtx;
			ucodecmds[(byte)General.UcodeCmds.POPMTX] = CommandPopMtx;
			ucodecmds[(byte)General.UcodeCmds.SETOTHERMODE_H] = CommandSetOtherModeH;
			ucodecmds[(byte)General.UcodeCmds.SETOTHERMODE_L] = CommandSetOtherModeL;
			ucodecmds[(byte)General.UcodeCmds.TEXTURE] = CommandTexture;
			ucodecmds[(byte)General.UcodeCmds.SETTIMG] = CommandSetTImage;
			ucodecmds[(byte)General.UcodeCmds.SETTILE] = CommandSetTile;
			ucodecmds[(byte)General.UcodeCmds.SETTILESIZE] = CommandSetTileSize;
			ucodecmds[(byte)General.UcodeCmds.LOADBLOCK] = CommandLoadBlock;
			ucodecmds[(byte)General.UcodeCmds.SETCOMBINE] = CommandSetCombine;
			ucodecmds[(byte)General.UcodeCmds.SETPRIMCOLOR] = CommandSetPrimColor;
			ucodecmds[(byte)General.UcodeCmds.SETENVCOLOR] = CommandSetEnvColor;
		}

		private void InitializeMacros()
		{
			macros = new List<Macro>
			{
				new Macro(MacroLoadTextureBlock, new General.UcodeCmds[]
				{
					General.UcodeCmds.SETTIMG, General.UcodeCmds.SETTILE, General.UcodeCmds.RDPLOADSYNC, General.UcodeCmds.LOADBLOCK, General.UcodeCmds.RDPPIPESYNC, General.UcodeCmds.SETTILE, General.UcodeCmds.SETTILESIZE
				}),
				new Macro(MacroLoadTLUT, new General.UcodeCmds[]
				{
					General.UcodeCmds.SETTIMG, General.UcodeCmds.RDPTILESYNC, General.UcodeCmds.SETTILE, General.UcodeCmds.RDPLOADSYNC, General.UcodeCmds.LOADTLUT, General.UcodeCmds.RDPPIPESYNC
				})
			};
		}

		public void Render(uint adr, bool call = false, OpenGLHelpers.DisplayListEx gldl = null)
		{
			try
			{
				ActiveGLDL.Push(gldl);

				/* Set some defaults */
				if (!call)
				{
					GL.DepthMask(true);
					if (OpenGLHelpers.Initialization.SupportsFunction("glGenProgramsARB")) GL.Disable((EnableCap)All.FragmentProgram);
					if (OpenGLHelpers.Initialization.SupportsFunction("glCreateShader")) GL.UseProgram(0);

					PrimColor = EnvColor = new Color4(0.5f, 0.5f, 0.5f, 0.5f);

					/* If emulating combiner, set more defaults / load values */
					if (Configuration.CombinerType == CombinerTypes.ArbCombiner)
					{
						GL.Arb.BindProgram(AssemblyProgramTargetArb.FragmentProgram, 0);
						GL.Arb.ProgramEnvParameter4(AssemblyProgramTargetArb.FragmentProgram, 0, PrimColor.R, PrimColor.G, PrimColor.B, PrimColor.A);
						GL.Arb.ProgramEnvParameter4(AssemblyProgramTargetArb.FragmentProgram, 1, EnvColor.R, EnvColor.G, EnvColor.B, EnvColor.A);
					}

					/* Clear out texture units */
					for (int i = 0; i < (OpenGLHelpers.Initialization.SupportsFunction("glActiveTextureARB") ? 2 : 1); i++)
					{
						OpenGLHelpers.Initialization.ActiveTextureChecked(TextureUnit.Texture0 + i);
						GL.BindTexture(TextureTarget.Texture2D, OpenGLHelpers.MiscDrawingHelpers.DummyTextureID);
					}
				}

				/* Ucode interpreter starts here */
				byte seg = (byte)(adr >> 24);
				adr &= 0xFFFFFF;

				byte[] segdata = (byte[])ROM.SegmentMapping[seg];

				while (adr < segdata.Length)
				{
					byte cmd = segdata[adr];

					/* EndDL */
					if (cmd == (byte)General.UcodeCmds.ENDDL) break;

					/* Try to detect macros if any are defined */
					inmacro = false;
					if (macros != null)
					{
						foreach (Macro m in macros)
						{
							if (adr + ((m.Commands.Length + 3) * 8) > segdata.Length) break;

							General.UcodeCmds[] nextcmd = new General.UcodeCmds[m.Commands.Length];
							uint[] nextw0 = new uint[nextcmd.Length + 2];
							uint[] nextw1 = new uint[nextcmd.Length + 2];

							for (int i = 0; i < nextw0.Length; i++)
							{
								nextw0[i] = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)adr + (i * 8)));
								nextw1[i] = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)adr + (i * 8) + 4));
								if (i < m.Commands.Length) nextcmd[i] = (General.UcodeCmds)(nextw0[i] >> 24);
							}

							if (inmacro = (Enumerable.SequenceEqual(m.Commands, nextcmd)))
							{
								m.Function(nextw0, nextw1);
								adr += (uint)(m.Commands.Length * 8);
								break;
							}
						}
					}

					/* No macro detected */
					if (!inmacro)
					{
						/* Execute command */
						ucodecmds[cmd](Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)adr)), Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)adr + 4)));
						adr += 8;

						/* Texture loading hack; if SetCombine OR LoadBlock command detected, try loading textures again (fixes Water Temple 1st room, borked walls; SM64toZ64 conversions?) */
						if (Configuration.RenderTextures && (cmd == (byte)General.UcodeCmds.SETCOMBINE || cmd == (byte)General.UcodeCmds.LOADBLOCK) && Textures[0] != null) LoadTextures();
					}
				}
			}
			catch (EntryPointNotFoundException)
			{
				//TODO handle this?
			}
			finally
			{
				ActiveGLDL.Pop();
			}
		}

		private void MacroLoadTextureBlock(uint[] w0, uint[] w1)
		{
			if (!Configuration.RenderTextures) return;

			activetex = (int)((w1[6] >> 24) & 0x01);
			multitex = activetex == 1;

			CommandSetTImage(w0[0], w1[0]);
			CommandSetTile(w0[5], w1[5]);
			CommandSetTileSize(w0[6], w1[6]);

			if ((Textures[activetex].Format == 0x40 || Textures[activetex].Format == 0x48 || Textures[activetex].Format == 0x50) &&
				((w0[7] >> 24) == (byte)General.UcodeCmds.SETTIMG) || ((w0[8] >> 24) == (byte)General.UcodeCmds.SETTIMG)) return;

			LoadTextures();
		}

		private void MacroLoadTLUT(uint[] w0, uint[] w1)
		{
			if (!Configuration.RenderTextures) return;

			uint adr = w1[0];
			byte seg = (byte)(adr >> 24);
			adr &= 0xFFFFFF;

			uint psize = ((w1[4] & 0x00FFF000) >> 14) + 1;

			byte[] segdata = (byte[])ROM.SegmentMapping[seg];
			if (segdata == null) return;

			for (int i = 0; i < psize; i++)
			{
				ushort r = (ushort)((segdata[adr] << 8) | segdata[adr + 1]);

				palette[i].R = (byte)((r & 0xF800) >> 8);
				palette[i].G = (byte)(((r & 0x07C0) << 5) >> 8);
				palette[i].B = (byte)(((r & 0x003E) << 18) >> 16);
				palette[i].A = 0;
				if ((r & 0x0001) == 1) palette[i].A = 0xFF;

				adr += 2;
			}

			LoadTextures();
		}

		private void CommandVtx(uint w0, uint w1)
		{
			/* Vtx */
			byte N = (byte)((w0 >> 12) & 0xFF);
			byte V0 = (byte)(((w0 >> 1) & 0x7F) - N);
			if (N > VertexBuffer.Length || V0 > VertexBuffer.Length) return;

			for (int i = 0; i < N; i++) VertexBuffer[V0 + i] = new Vertex(ROM, (byte[])ROM.SegmentMapping[(byte)(w1 >> 24)], (uint)(w1 + i * 16), mtxstack.Peek());
		}

		private void CommandTri1(uint w0, uint w1)
		{
			/* Tri1 */
			int[] idxs = new int[] { (int)((w0 & 0x00FF0000) >> 16) >> 1, (int)((w0 & 0x0000FF00) >> 8) >> 1, (int)(w0 & 0x000000FF) >> 1 };

			foreach (int idx in idxs) if (idx >= VertexBuffer.Length) return;
			General.RenderTriangles(this, idxs);

			if (ActiveGLDL.Peek() != null) ActiveGLDL.Peek().Triangles.Add(new OpenGLHelpers.DisplayListEx.Triangle(VertexBuffer[idxs[0]], VertexBuffer[idxs[1]], VertexBuffer[idxs[2]]));

			LastTriList.Add(new SimpleTriangle(VertexBuffer[idxs[0]].Position, VertexBuffer[idxs[1]].Position, VertexBuffer[idxs[2]].Position));
		}

		private void CommandTri2(uint w0, uint w1)
		{
			/* Tri2 */
			int[] idxs = new int[]
			{
				(int)((w0 & 0x00FF0000) >> 16) >> 1, (int)((w0 & 0x0000FF00) >> 8) >> 1, (int)(w0 & 0x000000FF) >> 1,
				(int)((w1 & 0x00FF0000) >> 16) >> 1, (int)((w1 & 0x0000FF00) >> 8) >> 1, (int)(w1 & 0x000000FF) >> 1
			};

			foreach (int idx in idxs) if (idx >= VertexBuffer.Length) return;
			General.RenderTriangles(this, idxs);

			if (ActiveGLDL != null)
			{
				if (ActiveGLDL.Peek() != null) ActiveGLDL.Peek().Triangles.Add(new OpenGLHelpers.DisplayListEx.Triangle(VertexBuffer[idxs[0]], VertexBuffer[idxs[1]], VertexBuffer[idxs[2]]));
				if (ActiveGLDL.Peek() != null) ActiveGLDL.Peek().Triangles.Add(new OpenGLHelpers.DisplayListEx.Triangle(VertexBuffer[idxs[3]], VertexBuffer[idxs[4]], VertexBuffer[idxs[5]]));
			}

			LastTriList.Add(new SimpleTriangle(VertexBuffer[idxs[0]].Position, VertexBuffer[idxs[1]].Position, VertexBuffer[idxs[2]].Position));
			LastTriList.Add(new SimpleTriangle(VertexBuffer[idxs[3]].Position, VertexBuffer[idxs[4]].Position, VertexBuffer[idxs[5]].Position));
		}

		private void CommandDL(uint w0, uint w1)
		{
			/* DL */
			if ((byte[])ROM.SegmentMapping[(byte)(w1 >> 24)] != null) Render(w1, true, ActiveGLDL.Peek());
		}

		private void CommandRDPHalf1(uint w0, uint w1)
		{
			/* RDPHalf_1 */
			rdphalf1 = w1;
		}

		private void CommandBranchZ(uint w0, uint w1)
		{
			/* Branch_Z */
			if ((byte[])ROM.SegmentMapping[(byte)(rdphalf1 >> 24)] != null) Render(rdphalf1, true, ActiveGLDL.Peek());
		}

		private void CommandGeometryMode(uint w0, uint w1)
		{
			/* GeometryMode */
			uint clr = ~(w0 & 0xFFFFFF);
			GeometryMode = (GeometryMode & ~clr) | w1;
			General.PerformModeChanges(this);

			if (Configuration.CombinerType != CombinerTypes.None)
				combinerManager?.BindCombiner(LastComb0, LastComb1, Configuration.RenderTextures);
		}

		private void CommandMtx(uint w0, uint w1)
		{
			/* Mtx */
			byte mseg = (byte)(w1 >> 24);
			uint madr = w1 & 0xFFFFFF;
			byte[] msegdata = (byte[])ROM.SegmentMapping[mseg];

			if (mseg == 0x80) mtxstack.Pop();
			if (msegdata == null) return;

			ushort mt1, mt2;
			double[] matrix = new double[16];

			for (int x = 0; x < 4; x++)
			{
				for (int y = 0; y < 4; y++)
				{
					mt1 = Endian.SwapUInt16(BitConverter.ToUInt16(msegdata, (int)madr));
					mt2 = Endian.SwapUInt16(BitConverter.ToUInt16(msegdata, (int)madr + 32));
					matrix[(x * 4) + y] = ((mt1 << 16) | mt2) * (1.0f / 65536.0f);
					madr += 2;
				}
			}

			Matrix4d glmatrix = new Matrix4d(
				matrix[0], matrix[1], matrix[2], matrix[3],
				matrix[4], matrix[5], matrix[6], matrix[7],
				matrix[8], matrix[9], matrix[10], matrix[11],
				matrix[12], matrix[13], matrix[14], matrix[15]);

			mtxstack.Push(glmatrix);
		}

		private void CommandPopMtx(uint w0, uint w1)
		{
			/* PopMtx */
			mtxstack.Pop();
		}

		private void CommandSetOtherModeH(uint w0, uint w1)
		{
			/* SetOtherMode_H */
			/* useless~ */
			switch ((General.OtherModeHShifts)(32 - General.ShiftR(w0, 8, 8) - (General.ShiftR(w0, 0, 8) + 1)))
			{
				case General.OtherModeHShifts.TEXTLUT:
					uint tlutmode = (w1 >> (int)General.OtherModeHShifts.TEXTLUT);
					break;
				default:
					uint length = (uint)(General.ShiftR(w0, 0, 8) + 1);
					uint shift = (uint)(32 - General.ShiftR(w0, 8, 8) - length);
					uint mask = (uint)(((1 << (int)length) - 1) << (int)shift);

					OtherModeH &= ~mask;
					OtherModeH |= w1 & mask;
					break;
			}

			General.PerformModeChanges(this);

			if (Configuration.CombinerType != CombinerTypes.None)
				combinerManager?.BindCombiner(LastComb0, LastComb1, Configuration.RenderTextures);
		}

		private void CommandSetOtherModeL(uint w0, uint w1)
		{
			/* SetOtherMode_L */
			if ((32 - ((w0 & 0x00FFFFFF) << 4 >> 4) - 1) == 3)
			{
				uint data = OtherModeL.Data;
				data &= 0x00000007;
				data |= (w1 & 0xCCCCFFFF | w1 & 0x3333FFFF);
				OtherModeL = new OtherModeL(data);
				General.PerformModeChanges(this);

				if (Configuration.CombinerType != CombinerTypes.None)
					combinerManager?.BindCombiner(LastComb0, LastComb1, Configuration.RenderTextures);
			}
		}

		private void CommandTexture(uint w0, uint w1)
		{
			/* Texture */
			activetex = 0;
			multitex = false;

			Textures[0] = new Texture();
			Textures[1] = new Texture();

			int s = General.ShiftR(w1, 16, 16);
			int t = General.ShiftR(w1, 0, 16);

			ScaleS[0] = ScaleS[1] = (s + 1) / 65536.0f;
			ScaleT[0] = ScaleT[1] = (t + 1) / 65536.0f;
		}

		private void CommandSetTImage(uint w0, uint w1)
		{
			/* SetTImage */
			if (inmacro)
				texaddress = w1;
			else
				Textures[activetex].Address = w1;
		}

		private void CommandSetTile(uint w0, uint w1)
		{
			/* SetTile */
			if (inmacro)
				Textures[activetex].Address = texaddress;

			Textures[activetex].Format = (byte)((w0 & 0xFF0000) >> 16);
			Textures[activetex].CMS = (uint)General.ShiftR(w1, 8, 2);
			Textures[activetex].CMT = (uint)General.ShiftR(w1, 18, 2);
			Textures[activetex].LineSize = General.ShiftR(w0, 9, 9);
			Textures[activetex].Palette = General.ShiftR(w1, 20, 4);
			Textures[activetex].ShiftS = General.ShiftR(w1, 0, 4);
			Textures[activetex].ShiftT = General.ShiftR(w1, 10, 4);
			Textures[activetex].MaskS = General.ShiftR(w1, 4, 4);
			Textures[activetex].MaskT = General.ShiftR(w1, 14, 4);
		}

		private void CommandSetTileSize(uint w0, uint w1)
		{
			/* SetTileSize */
			uint ULS = (uint)General.ShiftR(w0, 12, 12);
			uint ULT = (uint)General.ShiftR(w0, 0, 12);
			uint LRS = (uint)General.ShiftR(w1, 12, 12);
			uint LRT = (uint)General.ShiftR(w1, 0, 12);

			Textures[activetex].Tile = General.ShiftR(w1, 24, 3);
			Textures[activetex].ULS = General.ShiftR(ULS, 2, 10);
			Textures[activetex].ULT = General.ShiftR(ULT, 2, 10);
			Textures[activetex].LRS = General.ShiftR(LRS, 2, 10);
			Textures[activetex].LRT = General.ShiftR(LRT, 2, 10);
		}

		private void CommandLoadBlock(uint w0, uint w1)
		{
			/* LoadBlock */
			CommandSetTileSize(w0, w1);
		}

		private void CommandSetCombine(uint w0, uint w1)
		{
			/* SetCombine */
			LastComb0 = w0 & 0xFFFFFF;
			LastComb1 = w1;

			if (Configuration.CombinerType != CombinerTypes.None)
				combinerManager?.BindCombiner(LastComb0, LastComb1, Configuration.RenderTextures);
		}

		private void CommandSetPrimColor(uint w0, uint w1)
		{
			/* SetPrimColor */
			PrimColor = new Color4(
				General.ShiftR(w1, 24, 8) * 0.0039215689f,
				General.ShiftR(w1, 16, 8) * 0.0039215689f,
				General.ShiftR(w1, 8, 8) * 0.0039215689f,
				General.ShiftR(w1, 0, 8) * 0.0039215689f);

			if (Configuration.CombinerType == CombinerTypes.ArbCombiner || Configuration.CombinerType == CombinerTypes.GLSLCombiner)
			{
				float m = General.ShiftL(w0, 8, 8);
				float l = General.ShiftL(w0, 0, 8) * 0.0039215689f;

				GL.Arb.ProgramEnvParameter4(AssemblyProgramTargetArb.FragmentProgram, 0, PrimColor.R, PrimColor.G, PrimColor.B, PrimColor.A);
				GL.Arb.ProgramEnvParameter4(AssemblyProgramTargetArb.FragmentProgram, 2, l, l, l, l);

				combinerManager?.BindCombiner(LastComb0, LastComb1, Configuration.RenderTextures);
			}
			else
			{
				/* Super-simple colorization faking */
				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, new Color4(PrimColor.R, PrimColor.G, PrimColor.B, PrimColor.A));
				//GL.Light(LightName.Light0, LightParameter.Diffuse, new Color4(PrimColor.R, PrimColor.G, PrimColor.B, PrimColor.A));
			}
		}

		private void CommandSetEnvColor(uint w0, uint w1)
		{
			/* SetEnvColor */
			EnvColor = new Color4(
				General.ShiftR(w1, 24, 8) * 0.0039215689f,
				General.ShiftR(w1, 16, 8) * 0.0039215689f,
				General.ShiftR(w1, 8, 8) * 0.0039215689f,
				General.ShiftR(w1, 0, 8) * 0.0039215689f);

			if (Configuration.CombinerType == CombinerTypes.ArbCombiner)
				GL.Arb.ProgramEnvParameter4(AssemblyProgramTargetArb.FragmentProgram, 1, EnvColor.R, EnvColor.G, EnvColor.B, EnvColor.A);

			if (Configuration.CombinerType != CombinerTypes.None)
				combinerManager?.BindCombiner(LastComb0, LastComb1, Configuration.RenderTextures);
		}

		#region Texturing functions

		private void LoadTextures()
		{
			switch (Configuration.CombinerType)
			{
				case CombinerTypes.None:
					{
						CalculateTextureSize(0);
						OpenGLHelpers.Initialization.ActiveTextureChecked(TextureUnit.Texture0);
						GL.Enable(EnableCap.Texture2D);
						GL.BindTexture(TextureTarget.Texture2D, CheckTextureCache(0));
					}
					break;

				case CombinerTypes.GLSLCombiner:
					{
						CalculateTextureSize(0);
						OpenGLHelpers.Initialization.ActiveTextureChecked(TextureUnit.Texture0);
						GL.Enable(EnableCap.Texture2D);
						GL.BindTexture(TextureTarget.Texture2D, CheckTextureCache(0));

						if (OpenGLHelpers.Initialization.SupportsFunction("glActiveTextureARB"))
						{
							CalculateTextureSize(1);
							GL.ActiveTexture(TextureUnit.Texture1);
							GL.Enable(EnableCap.Texture2D);
							GL.BindTexture(TextureTarget.Texture2D, 0);

							if (multitex) GL.BindTexture(TextureTarget.Texture2D, CheckTextureCache(1));
						}
					}
					break;

				case CombinerTypes.ArbCombiner:
					{
						OpenGLHelpers.Initialization.ActiveTextureChecked(TextureUnit.Texture0);
						GL.Disable(EnableCap.Texture2D);
						GL.BindTexture(TextureTarget.Texture2D, 0);
						OpenGLHelpers.Initialization.ActiveTextureChecked(TextureUnit.Texture1);
						GL.Disable(EnableCap.Texture2D);
						GL.BindTexture(TextureTarget.Texture2D, 0);

						CalculateTextureSize(0);
						OpenGLHelpers.Initialization.ActiveTextureChecked(TextureUnit.Texture0);
						GL.Enable(EnableCap.Texture2D);
						GL.BindTexture(TextureTarget.Texture2D, CheckTextureCache(0));

						if (multitex)
						{
							CalculateTextureSize(1);
							OpenGLHelpers.Initialization.ActiveTextureChecked(TextureUnit.Texture1);
							GL.Enable(EnableCap.Texture2D);
							GL.BindTexture(TextureTarget.Texture2D, CheckTextureCache(1));

							GL.Disable(EnableCap.Texture2D);
						}
						else
						{
							OpenGLHelpers.Initialization.ActiveTextureChecked(TextureUnit.Texture1);
							GL.Disable(EnableCap.Texture2D);
						}

						OpenGLHelpers.Initialization.ActiveTextureChecked(TextureUnit.Texture0);
						GL.Disable(EnableCap.Texture2D);
					}
					break;
			}
		}

		private int CheckTextureCache(int tx)
		{
			object tag = ROM.SegmentMapping[(byte)(Textures[tx].Address >> 24)];

			foreach (TextureCache cached in texcache)
			{
				if (cached.Tag == tag && cached.Format == Textures[tx].Format && cached.Address == Textures[tx].Address &&
					cached.RealHeight == Textures[tx].RealHeight && cached.RealWidth == Textures[tx].RealWidth)
					return cached.GLID;
			}

			TextureCache newcached = new TextureCache(tag, Textures[tx], LoadTexture(tx));
			texcache.Add(newcached);
			return newcached.GLID;
		}

		private int LoadTexture(int tx)
		{
			uint adr = Textures[tx].Address;
			byte seg = (byte)(adr >> 24);
			adr &= 0xFFFFFF;

			byte[] texbuf = new byte[Textures[tx].RealWidth * Textures[tx].RealHeight * 4];
			byte[] segdata = (byte[])ROM.SegmentMapping[seg];

			if (segdata == null)
				texbuf.Fill(new byte[] { 0xFF, 0xFF, 0x00, 0xFF });
			else
				ImageHelper.Convert(
					Textures[tx].Format,
					segdata,
					(int)adr,
					ref texbuf,
					Textures[tx].Width,
					Textures[tx].Height,
					Textures[tx].LineSize,
					Textures[tx].Palette,
					palette);

			int glid = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, glid);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Textures[tx].RealWidth, Textures[tx].RealHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, texbuf);

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

			if (Textures[tx].CMS == 2 || Textures[tx].CMS == 3)
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
			else if (Textures[tx].CMS == 1)
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.MirroredRepeatArb);
			else
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);

			if (Textures[tx].CMT == 2 || Textures[tx].CMT == 3)
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
			else if (Textures[tx].CMT == 1)
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.MirroredRepeatArb);
			else
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);

			return glid;
		}

		private void CalculateTextureSize(int tx)
		{
			int MaxTexel = 0, LineShift = 0;

			switch (Textures[tx].Format)
			{
				/* 4-bit */
				case 0x00:
					// RGBA
					MaxTexel = 4096; LineShift = 4;
					break;
				case 0x40:
					// CI
					MaxTexel = 4096; LineShift = 4;
					break;
				case 0x60:
					// IA
					MaxTexel = 8192; LineShift = 4;
					break;
				case 0x80:
					// I
					MaxTexel = 8192; LineShift = 4;
					break;

				/* 8-bit */
				case 0x08:
					// RGBA
					MaxTexel = 2048; LineShift = 3;
					break;
				case 0x48:
					// CI
					MaxTexel = 2048; LineShift = 3;
					break;
				case 0x68:
					// IA
					MaxTexel = 4096; LineShift = 3;
					break;
				case 0x88:
					// I
					MaxTexel = 4096; LineShift = 3;
					break;

				/* 16-bit */
				case 0x10:
					// RGBA
					MaxTexel = 2048; LineShift = 2;
					break;
				case 0x50:
					// CI
					MaxTexel = 2048; LineShift = 0;
					break;
				case 0x70:
					// IA
					MaxTexel = 2048; LineShift = 2;
					break;
				case 0x90:
					// I
					MaxTexel = 2048; LineShift = 0;
					break;

				/* 32-bit */
				case 0x18:
					// RGBA
					MaxTexel = 1024; LineShift = 2;
					break;

				default:
					return;
			}

			int Line_Width = Textures[tx].LineSize << LineShift;

			int Tile_Width = Textures[tx].LRS - Textures[tx].ULS + 1;
			int Tile_Height = Textures[tx].LRT - Textures[tx].ULT + 1;

			int Mask_Width = 1 << Textures[tx].MaskS;
			int Mask_Height = 1 << Textures[tx].MaskT;

			int Line_Height = 0;
			if (Line_Width > 0)
				Line_Height = Math.Min(MaxTexel / Line_Width, Tile_Height);

			if ((Textures[tx].MaskS > 0) && ((Mask_Width * Mask_Height) <= MaxTexel))
				Textures[tx].Width = Mask_Width;
			else if ((Tile_Width * Tile_Height) <= MaxTexel)
				Textures[tx].Width = Tile_Width;
			else
				Textures[tx].Width = Line_Width;

			if ((Textures[tx].MaskT > 0) && ((Mask_Width * Mask_Height) <= MaxTexel))
				Textures[tx].Height = Mask_Height;
			else if ((Tile_Width * Tile_Height) <= MaxTexel)
				Textures[tx].Height = Tile_Height;
			else
				Textures[tx].Height = Line_Height;

			int Clamp_Width = 0;
			int Clamp_Height = 0;

			if (Textures[tx].CMS == 1)
				Clamp_Width = Tile_Width;
			else
				Clamp_Width = Textures[tx].Width;

			if (Textures[tx].CMT == 1)
				Clamp_Height = Tile_Height;
			else
				Clamp_Height = Textures[tx].Height;

			if (Clamp_Width > 256) Textures[tx].CMS &= ~(uint)0x01;
			if (Clamp_Height > 256) Textures[tx].CMT &= ~(uint)0x01;

			if (Mask_Width > Textures[tx].Width)
			{
				Textures[tx].MaskS = General.PowOf(Textures[tx].Width);
				Mask_Width = 1 << Textures[tx].MaskS;
			}
			if (Mask_Height > Textures[tx].Height)
			{
				Textures[tx].MaskT = General.PowOf(Textures[tx].Height);
				Mask_Height = 1 << Textures[tx].MaskT;
			}

			if (Textures[tx].CMS == 2 || Textures[tx].CMS == 3)
				Textures[tx].RealWidth = General.Pow2(Clamp_Width);
			else if (Textures[tx].CMS == 1)
				Textures[tx].RealWidth = General.Pow2(Mask_Width);
			else
				Textures[tx].RealWidth = General.Pow2(Textures[tx].Width);

			if (Textures[tx].CMT == 2 || Textures[tx].CMT == 3)
				Textures[tx].RealHeight = General.Pow2(Clamp_Height);
			else if (Textures[tx].CMT == 1)
				Textures[tx].RealHeight = General.Pow2(Mask_Height);
			else
				Textures[tx].RealHeight = General.Pow2(Textures[tx].Height);

			Textures[tx].ScaleS = 1.0f / Textures[tx].RealWidth;
			Textures[tx].ScaleT = 1.0f / Textures[tx].RealHeight;

			Textures[tx].ShiftScaleS = 1.0f;
			Textures[tx].ShiftScaleT = 1.0f;

			if (Textures[tx].ShiftS > 10)
				Textures[tx].ShiftScaleS = 1 << (16 - Textures[tx].ShiftS);
			else if (Textures[tx].ShiftS > 0)
				Textures[tx].ShiftScaleS /= 1 << Textures[tx].ShiftS;

			if (Textures[tx].ShiftT > 10)
				Textures[tx].ShiftScaleT = 1 << (16 - Textures[tx].ShiftT);
			else if (Textures[tx].ShiftT > 0)
				Textures[tx].ShiftScaleT /= 1 << Textures[tx].ShiftT;
		}

		#endregion
	}
}
