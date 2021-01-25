using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.SimpleF3DEX2.CombinerEmulation
{
	internal class ArbCombineProgram
	{
		public UnpackedCombinerMux Unpacked { get; private set; }
		public uint Mux0 { get; private set; }
		public uint Mux1 { get; private set; }

		public bool Textured { get; private set; }
		public int GLID { get; private set; }

		public ArbCombineProgram(uint m0, uint m1, bool tex)
		{
			Mux0 = m0;
			Mux1 = m1;
			Unpacked = new UnpackedCombinerMux(m0, m1);

			Textured = tex;

			string pstring =
				"!!ARBfp1.0\n" +
				"\n" +
				"TEMP Tex0; TEMP Tex1;\n" +
				"TEMP R0; TEMP R1;\n" +
				"TEMP aR0; TEMP aR1;\n" +
				"TEMP Comb; TEMP aComb;\n" +
				"\n" +
				"PARAM PrimColor = program.env[0];\n" +
				"PARAM EnvColor = program.env[1];\n" +
				"PARAM PrimColorLOD = program.env[2];\n" +
				"ATTRIB Shade = fragment.color;\n" +
				"\n" +
				"OUTPUT Out = result.color;\n" +
				"\n" +
				"TEX Tex0, fragment.texcoord[0], texture[0], 2D;\n" +
				"TEX Tex1, fragment.texcoord[1], texture[1], 2D;\n" +
				"\n";

			for (int i = 0; i < 2; i++)
			{
				switch (Unpacked.cA[i])
				{
					case UnpackedCombinerMux.ComponentsC16.CCMUX_COMBINED:
						pstring += "MOV R0.rgb, Comb;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_TEXEL0:
						if (Textured)
							pstring += "MOV R0.rgb, Tex0;\n";
						else
							pstring += "MOV R0.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_TEXEL1:
						if (Textured)
							pstring += "MOV R0.rgb, Tex1;\n";
						else
							pstring += "MOV R0.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_PRIMITIVE:
						pstring += "MOV R0.rgb, PrimColor;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_SHADE:
						pstring += "MOV R0.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_ENVIRONMENT:
						pstring += "MOV R0.rgb, EnvColor;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_1:
						pstring += "MOV R0.rgb, {1.0, 1.0, 1.0, 1.0};\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_COMBINED_ALPHA:
						pstring += "MOV R0.rgb, Comb.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_TEXEL0_ALPHA:
						if (Textured)
							pstring += "MOV R0.rgb, Tex0.a;\n";
						else
							pstring += "MOV R0.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_TEXEL1_ALPHA:
						if (Textured)
							pstring += "MOV R0.rgb, Tex1.a;\n";
						else
							pstring += "MOV R0.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_PRIMITIVE_ALPHA:
						pstring += "MOV R0.rgb, PrimColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_SHADE_ALPHA:
						pstring += "MOV R0.rgb, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_ENV_ALPHA:
						pstring += "MOV R0.rgb, EnvColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_LOD_FRACTION:
						pstring += "MOV R0.rgb, {0.0, 0.0, 0.0, 0.0};\n";   // unemulated
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_PRIM_LOD_FRAC:
						pstring += "MOV R0.rgb, PrimColorLOD;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_0:
						pstring += "MOV R0.rgb, {0.0, 0.0, 0.0, 0.0};\n";
						break;

					default:
						pstring += "MOV R0.rgb, {0.0, 0.0, 0.0, 0.0};\n";
						break;
				}

				switch (Unpacked.cB[i])
				{
					case UnpackedCombinerMux.ComponentsC16.CCMUX_COMBINED:
						pstring += "MOV R1.rgb, Comb;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_TEXEL0:
						if (Textured)
							pstring += "MOV R1.rgb, Tex0;\n";
						else
							pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_TEXEL1:
						if (Textured)
							pstring += "MOV R1.rgb, Tex1;\n";
						else
							pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_PRIMITIVE:
						pstring += "MOV R1.rgb, PrimColor;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_SHADE:
						pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_ENVIRONMENT:
						pstring += "MOV R1.rgb, EnvColor;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_1:
						pstring += "MOV R1.rgb, {1.0, 1.0, 1.0, 1.0};\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_COMBINED_ALPHA:
						pstring += "MOV R1.rgb, Comb.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_TEXEL0_ALPHA:
						if (Textured)
							pstring += "MOV R1.rgb, Tex0.a;\n";
						else
							pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_TEXEL1_ALPHA:
						if (Textured)
							pstring += "MOV R1.rgb, Tex1.a;\n";
						else
							pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_PRIMITIVE_ALPHA:
						pstring += "MOV R1.rgb, PrimColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_SHADE_ALPHA:
						pstring += "MOV R1.rgb, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_ENV_ALPHA:
						pstring += "MOV R1.rgb, EnvColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_LOD_FRACTION:
						pstring += "MOV R1.rgb, {0.0, 0.0, 0.0, 0.0};\n";   // unemulated
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_PRIM_LOD_FRAC:
						pstring += "MOV R1.rgb, PrimColorLOD;\n";
						break;
					case UnpackedCombinerMux.ComponentsC16.CCMUX_0:
						pstring += "MOV R1.rgb, {0.0, 0.0, 0.0, 0.0};\n";
						break;

					default:
						pstring += "MOV R1.rgb, {0.0, 0.0, 0.0, 0.0};\n";
						break;
				}
				pstring += "SUB R0, R0, R1;\n\n";

				switch (Unpacked.cC[i])
				{
					case UnpackedCombinerMux.ComponentsC32.CCMUX_COMBINED:
						pstring += "MOV R1.rgb, Comb;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_TEXEL0:
						if (Textured)
							pstring += "MOV R1.rgb, Tex0;\n";
						else
							pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_TEXEL1:
						if (Textured)
							pstring += "MOV R1.rgb, Tex1;\n";
						else
							pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_PRIMITIVE:
						pstring += "MOV R1.rgb, PrimColor;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_SHADE:
						pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_ENVIRONMENT:
						pstring += "MOV R1.rgb, EnvColor;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_1:
						pstring += "MOV R1.rgb, {1.0, 1.0, 1.0, 1.0};\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_COMBINED_ALPHA:
						pstring += "MOV R1.rgb, Comb.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_TEXEL0_ALPHA:
						if (Textured)
							pstring += "MOV R1.rgb, Tex0.a;\n";
						else
							pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_TEXEL1_ALPHA:
						if (Textured)
							pstring += "MOV R1.rgb, Tex1.a;\n";
						else
							pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_PRIMITIVE_ALPHA:
						pstring += "MOV R1.rgb, PrimColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_SHADE_ALPHA:
						pstring += "MOV R1.rgb, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_ENV_ALPHA:
						pstring += "MOV R1.rgb, EnvColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_LOD_FRACTION:
						pstring += "MOV R1.rgb, {0.0, 0.0, 0.0, 0.0};\n";   // unemulated
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_PRIM_LOD_FRAC:
						pstring += "MOV R1.rgb, PrimColorLOD;\n";
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_K5:
						pstring += "MOV R1.rgb, {1.0, 1.0, 1.0, 1.0};\n";   // unemulated
						break;
					case UnpackedCombinerMux.ComponentsC32.CCMUX_0:
						pstring += "MOV R1.rgb, {0.0, 0.0, 0.0, 0.0};\n";
						break;

					default:
						pstring += "MOV R1.rgb, {0.0, 0.0, 0.0, 0.0};\n";
						break;
				}
				pstring += "MUL R0, R0, R1;\n\n";

				switch (Unpacked.cD[i])
				{
					case UnpackedCombinerMux.ComponentsC8.CCMUX_COMBINED:
						pstring += "MOV R1.rgb, Comb;\n";
						break;
					case UnpackedCombinerMux.ComponentsC8.CCMUX_TEXEL0:
						if (Textured)
							pstring += "MOV R1.rgb, Tex0;\n";
						else
							pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC8.CCMUX_TEXEL1:
						if (Textured)
							pstring += "MOV R1.rgb, Tex1;\n";
						else
							pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC8.CCMUX_PRIMITIVE:
						pstring += "MOV R1.rgb, PrimColor;\n";
						break;
					case UnpackedCombinerMux.ComponentsC8.CCMUX_SHADE:
						pstring += "MOV R1.rgb, Shade;\n";
						break;
					case UnpackedCombinerMux.ComponentsC8.CCMUX_ENVIRONMENT:
						pstring += "MOV R1.rgb, EnvColor;\n";
						break;
					case UnpackedCombinerMux.ComponentsC8.CCMUX_1:
						pstring += "MOV R1.rgb, {1.0, 1.0, 1.0, 1.0};\n";
						break;
					case UnpackedCombinerMux.ComponentsC8.CCMUX_0:
						pstring += "MOV R1.rgb, {0.0, 0.0, 0.0, 0.0};\n";
						break;

					default:
						pstring += "MOV R1.rgb, {0.0, 0.0, 0.0, 0.0};\n";
						break;
				}
				pstring += "ADD R0, R0, R1;\n\n";

				switch (Unpacked.aA[i])
				{
					case UnpackedCombinerMux.ComponentsA8.ACMUX_COMBINED:
						pstring += "MOV aR0.a, aComb;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_TEXEL0:
						if (Textured)
							pstring += "MOV aR0.a, Tex0.a;\n";
						else
							pstring += "MOV aR0.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_TEXEL1:
						if (Textured)
							pstring += "MOV aR0.a, Tex1.a;\n";
						else
							pstring += "MOV aR0.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_PRIMITIVE:
						pstring += "MOV aR0.a, PrimColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_SHADE:
						pstring += "MOV aR0.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_ENVIRONMENT:
						pstring += "MOV aR0.a, EnvColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_1:
						pstring += "MOV aR0.a, {1.0, 1.0, 1.0, 1.0};\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_0:
						pstring += "MOV aR0.a, {0.0, 0.0, 0.0, 0.0};\n";
						break;

					default:
						pstring += "MOV aR0.a, {0.0, 0.0, 0.0, 0.0};\n";
						break;
				}

				switch (Unpacked.aB[i])
				{
					case UnpackedCombinerMux.ComponentsA8.ACMUX_COMBINED:
						pstring += "MOV aR1.a, aComb;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_TEXEL0:
						if (Textured)
							pstring += "MOV aR1.a, Tex0.a;\n";
						else
							pstring += "MOV aR1.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_TEXEL1:
						if (Textured)
							pstring += "MOV aR1.a, Tex1.a;\n";
						else
							pstring += "MOV aR1.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_PRIMITIVE:
						pstring += "MOV aR1.a, PrimColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_SHADE:
						pstring += "MOV aR1.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_ENVIRONMENT:
						pstring += "MOV aR1.a, EnvColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_1:
						pstring += "MOV aR1.a, {1.0, 1.0, 1.0, 1.0};\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_0:
						pstring += "MOV aR1.a, {0.0, 0.0, 0.0, 0.0};\n";
						break;

					default:
						pstring += "MOV aR1.a, {0.0, 0.0, 0.0, 0.0};\n";
						break;
				}
				pstring += "SUB aR0.a, aR0.a, aR1.a;\n\n";

				switch (Unpacked.aC[i])
				{
					case UnpackedCombinerMux.ComponentsA8.ACMUX_COMBINED:
						pstring += "MOV aR1.a, aComb;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_TEXEL0:
						if (Textured)
							pstring += "MOV aR1.a, Tex0.a;\n";
						else
							pstring += "MOV aR1.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_TEXEL1:
						if (Textured)
							pstring += "MOV aR1.a, Tex1.a;\n";
						else
							pstring += "MOV aR1.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_PRIMITIVE:
						pstring += "MOV aR1.a, PrimColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_SHADE:
						pstring += "MOV aR1.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_ENVIRONMENT:
						pstring += "MOV aR1.a, EnvColor.a;\n";
						break;
					//TODO  check this!
					//case UnpackedCombinerMux.ComponentsA8.ACMUX_PRIM_LOD_FRAC:
					//pstring += "MOV aR1.a, PrimColorLOD.a;\n";
					//break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_1:
						pstring += "MOV aR1.a, {1.0, 1.0, 1.0, 1.0};\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_0:
						pstring += "MOV aR1.a, {0.0, 0.0, 0.0, 0.0};\n";
						break;

					default:
						pstring += "MOV aR1.a, {0.0, 0.0, 0.0, 0.0};\n";
						break;
				}
				pstring += "MUL aR0.a, aR0.a, aR1.a;\n\n";

				switch (Unpacked.aD[i])
				{
					case UnpackedCombinerMux.ComponentsA8.ACMUX_COMBINED:
						pstring += "MOV aR1.a, aComb.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_TEXEL0:
						if (Textured)
							pstring += "MOV aR1.a, Tex0.a;\n";
						else
							pstring += "MOV aR1.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_TEXEL1:
						if (Textured)
							pstring += "MOV aR1.a, Tex1.a;\n";
						else
							pstring += "MOV aR1.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_PRIMITIVE:
						pstring += "MOV aR1.a, PrimColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_SHADE:
						pstring += "MOV aR1.a, Shade.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_ENVIRONMENT:
						pstring += "MOV aR1.a, EnvColor.a;\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_1:
						pstring += "MOV aR1.a, {1.0, 1.0, 1.0, 1.0};\n";
						break;
					case UnpackedCombinerMux.ComponentsA8.ACMUX_0:
						pstring += "MOV aR1.a, {0.0, 0.0, 0.0, 0.0};\n";
						break;

					default:
						pstring += "MOV aR1.a, {0.0, 0.0, 0.0, 0.0};\n";
						break;
				}

				pstring += "ADD aR0.a, aR0.a, aR1.a;\n\n";

				pstring += "MOV Comb.rgb, R0;\n";
				pstring += "MOV aComb.a, aR0;\n\n";
			}

			pstring += "MOV Comb.a, aComb.a;\n" +
				"MOV Out, Comb;\n" +
				"END\n";

			byte[] bytes = Encoding.ASCII.GetBytes(pstring);

			GL.Arb.GenProgram(1, out int gl);
			GL.Arb.BindProgram(AssemblyProgramTargetArb.FragmentProgram, gl);
			GL.Arb.ProgramString(AssemblyProgramTargetArb.FragmentProgram, All.ProgramFormatAsciiArb, bytes.Length, bytes);

			GLID = gl;
		}
	}
}
