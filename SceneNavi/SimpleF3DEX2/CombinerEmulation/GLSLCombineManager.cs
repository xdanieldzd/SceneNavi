using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.SimpleF3DEX2.CombinerEmulation
{
    internal class GLSLCombineManager
    {
        bool supported;
        F3DEX2Interpreter F3DEX2;
        List<GLSLShaders> shaderCache;

        public GLSLCombineManager(F3DEX2Interpreter f3dex2)
        {
            supported = ((GraphicsContext.CurrentContext as IGraphicsContextInternal).GetAddress("glCreateShader") != IntPtr.Zero);
            F3DEX2 = f3dex2;
            shaderCache = new List<GLSLShaders>();

            foreach (uint[] knownMux in KnownCombinerMuxes.Muxes)
            {
                BindCombiner(knownMux[0], knownMux[1], true);
                BindCombiner(knownMux[0], knownMux[1], false);
            }
        }

        public void BindCombiner(uint m0, uint m1, bool tex)
        {
            if (!supported) return;

            if (m0 == 0 && m1 == 0) return;

            GLSLShaders shader = shaderCache.FirstOrDefault(x => x.Mux0 == m0 && x.Mux1 == m1 &&
                x.HasLightingEnabled == Convert.ToBoolean(F3DEX2.GeometryMode & (uint)General.GeometryMode.LIGHTING) &&
                x.Textured == tex);

            if (shader != null)
                GL.UseProgram(shader.ProgramID);
            else
            {
                shader = new GLSLShaders(m0, m1, F3DEX2, tex);
                shaderCache.Add(shader);
            }

            GL.Uniform1(GL.GetUniformLocation(shader.ProgramID, "tex0"), 0);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramID, "tex1"), 1);
            GL.Uniform4(GL.GetUniformLocation(shader.ProgramID, "primColor"), F3DEX2.PrimColor);
            GL.Uniform4(GL.GetUniformLocation(shader.ProgramID, "envColor"), F3DEX2.EnvColor);
        }
    }
}
