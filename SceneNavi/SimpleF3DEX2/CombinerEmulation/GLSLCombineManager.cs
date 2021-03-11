using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.SimpleF3DEX2.CombinerEmulation
{
	// TODO: fix fragmentshader, general cleanup, etc!

	internal class GLSLCombineManager : ICombinerManager
	{
		readonly bool supported;
		readonly F3DEX2Interpreter F3DEX2;

		readonly int vertexObject, fragmentObject, program;
		readonly int locInputTexel0, locInputTexel1, locInputPrim, locInputPrimLODFrac, locInputEnv;
		readonly int locCombinerIsTwoCycleMode;
		readonly int[] locCombinerColorParams, locCombinerAlphaParams;

		public GLSLCombineManager(F3DEX2Interpreter f3dex2)
		{
			supported = (GraphicsContext.CurrentContext as IGraphicsContextInternal).GetAddress("glCreateShader") != IntPtr.Zero;
			F3DEX2 = f3dex2;

			if (supported)
			{
				var vs = ReadEmbeddedTextResource("VertexShader.glsl");
				var fs = ReadEmbeddedTextResource("FragmentShader.glsl");

				CreateShaders(vs, fs, ref vertexObject, ref fragmentObject, ref program);

				locInputTexel0 = GL.GetUniformLocation(program, "combinerInputTexel[0]");
				locInputTexel1 = GL.GetUniformLocation(program, "combinerInputTexel[1]");
				locInputPrim = GL.GetUniformLocation(program, "combinerInputPrimitive");
				locInputPrimLODFrac = GL.GetUniformLocation(program, "combinerInputPrimitiveLODFrac");
				locInputEnv = GL.GetUniformLocation(program, "combinerInputEnvironment");
				locCombinerIsTwoCycleMode = GL.GetUniformLocation(program, "combinerIsTwoCycleMode");
				locCombinerColorParams = new int[8];
				for (var i = 0; i < locCombinerColorParams.Length; i++) locCombinerColorParams[i] = GL.GetUniformLocation(program, $"combinerColorParam[{i}]");
				locCombinerAlphaParams = new int[8];
				for (var i = 0; i < locCombinerAlphaParams.Length; i++) locCombinerAlphaParams[i] = GL.GetUniformLocation(program, $"combinerAlphaParam[{i}]");

				GL.UseProgram(program);

				GL.Uniform1(locInputTexel0, 0);
				GL.Uniform1(locInputTexel1, 1);
				GL.Uniform4(locInputPrim, new Vector4(1.0f));
				GL.Uniform1(locInputPrimLODFrac, 0.5f);
				GL.Uniform4(locInputEnv, new Vector4(0.5f));
				GL.Uniform1(locCombinerIsTwoCycleMode, 1);
			}
		}

		public void BindCombiner(uint m0, uint m1, bool tex)
		{
			if (!supported || (m0 == 0 && m1 == 0)) return;

			GL.UseProgram(program);

			ConfigureColorCombiner((ulong)m0 << 32 | m1);

			GL.Uniform4(locInputPrim, new Vector4(F3DEX2.PrimColor.R, F3DEX2.PrimColor.G, F3DEX2.PrimColor.B, F3DEX2.PrimColor.A));
			GL.Uniform4(locInputEnv, new Vector4(F3DEX2.EnvColor.R, F3DEX2.EnvColor.G, F3DEX2.EnvColor.B, F3DEX2.EnvColor.A));
		}

		private void ConfigureColorCombiner(ulong mux)
		{
			mux &= 0x00FFFFFFFFFFFFFFFF;

			var a0 = (mux >> 52) & 0b1111;
			var c0 = (mux >> 48) & 0b11111;
			var Aa0 = (mux >> 43) & 0b111;
			var Ac0 = (mux >> 40) & 0b111;
			var a1 = (mux >> 37) & 0b1111;
			var c1 = (mux >> 32) & 0b11111;
			var b0 = (mux >> 28) & 0b1111;
			var b1 = (mux >> 24) & 0b1111;
			var Aa1 = (mux >> 21) & 0b111;
			var Ac1 = (mux >> 18) & 0b111;
			var d0 = (mux >> 15) & 0b111;
			var Ab0 = (mux >> 12) & 0b111;
			var Ad0 = (mux >> 9) & 0b111;
			var d1 = (mux >> 6) & 0b111;
			var Ab1 = (mux >> 3) & 0b111;
			var Ad1 = (mux >> 0) & 0b111;

			var colorParams = new[] { a0, b0, c0, d0, a1, b1, c1, d1 };
			var alphaParams = new[] { Aa0, Ab0, Ac0, Ad0, Aa1, Ab1, Ac1, Ad1 };

			for (var i = 0; i < colorParams.Length; i++) GL.Uniform1(locCombinerColorParams[i], (int)colorParams[i]);
			for (var i = 0; i < alphaParams.Length; i++) GL.Uniform1(locCombinerAlphaParams[i], (int)alphaParams[i]);
		}

		private string ReadEmbeddedTextResource(string name)
		{
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{Application.ProductName}.Resources.{name}"))
			{
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		// Modified from OpenTK examples \Source\Examples\OpenGL\2.x\SimpleGLSL.cs
		private void CreateShaders(string vs, string fs, ref int vertexObject, ref int fragmentObject, ref int program)
		{
			try
			{
				vertexObject = GL.CreateShader(ShaderType.VertexShader);
				fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

				/* Compile vertex shader */
				GL.ShaderSource(vertexObject, vs);
				GL.CompileShader(vertexObject);
				GL.GetShaderInfoLog(vertexObject, out string info);
				GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out int statusCode);

				if (statusCode != 1) throw new ApplicationException(info);

				/* Compile fragment shader */
				GL.ShaderSource(fragmentObject, fs);
				GL.CompileShader(fragmentObject);
				GL.GetShaderInfoLog(fragmentObject, out info);
				GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out statusCode);

				if (statusCode != 1) throw new ApplicationException(info);

				program = GL.CreateProgram();
				GL.AttachShader(program, fragmentObject);
				GL.AttachShader(program, vertexObject);

				GL.LinkProgram(program);
				GL.UseProgram(program);
			}
			catch (ApplicationException ae)
			{
				MessageBox.Show(ae.ToString());
			}
		}
	}
}
