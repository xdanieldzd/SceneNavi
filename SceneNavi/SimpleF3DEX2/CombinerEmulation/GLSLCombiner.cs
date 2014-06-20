using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SceneNavi.OpenGLHelpers;

namespace SceneNavi.SimpleF3DEX2.CombinerEmulation
{
    internal class GLSLCombiner : ICombiner, IDisposable
    {
        F3DEX2Interpreter interpreter;
        List<Shader> shaders;

        public GLSLCombiner()
        {
            Program.Status.Message = "Initializing GLSL combiner...";

            shaders = new List<Shader>();
        }

        public string GetName() { return "Experimental GLSL Combiner"; }
        public string GetDescription() { return "Experimental GLSL-based combiner emulation (incomplete)"; }

        public void Attach(F3DEX2Interpreter interpreter)
        {
            this.interpreter = interpreter;
            Shader tmp = new Shader(this.interpreter, 0);
        }

        public void Reset()
        {
            foreach (Shader shader in shaders) shader.Delete();
        }

        public void Bind(ulong mux)
        {
            if (interpreter == null) throw new CombinerException("F3DEX2 interpreter has not been attached");

            Shader shader = shaders.FirstOrDefault(x => x.Mux == mux);
            if (shader != null) shader.Activate();
            else shaders.Add(new Shader(this.interpreter, mux));
        }

        public void Dispose()
        {
            Reset();
        }

        class Shader
        {
            F3DEX2Interpreter interpreter;

            public ulong Mux { get; private set; }
            UnpackedCombinerMux unpacked;

            string vertexString, fragmentString;
            int vertexObject, fragmentObject, program;

            public Shader(F3DEX2Interpreter interpreter, ulong mux)
            {
                this.interpreter = interpreter;
                this.Mux = mux;
                this.unpacked = new UnpackedCombinerMux(mux);

                this.vertexString =
@"#version 120

varying float fogFactor;

void main(void)
{
 gl_Position     = gl_ModelViewProjectionMatrix * gl_Vertex;
 gl_FrontColor   = gl_Color;
 gl_TexCoord[0]  = gl_MultiTexCoord0;

fogFactor = (gl_Fog.end - gl_FogFragCoord) / (gl_Fog.end - gl_Fog.start);
fogFactor = clamp(fogFactor, 0.0, 1.0);
}";
                this.fragmentString =
@"#version 120

uniform sampler2D Texture0;
varying float fogFactor;

void main(void)
{
vec4 finalColor = vec4(texture2D(Texture0, gl_TexCoord[0])) * gl_Color;
 gl_FragColor  = mix(gl_Fog.color, finalColor, fogFactor );
}";
                GLSL.CreateShaders(this.vertexString, this.fragmentString, ref this.vertexObject, ref this.fragmentObject, ref this.program);
            }

            public void Activate()
            {
                if (GL.IsProgram(this.program)) GL.UseProgram(this.program);
            }

            public void Delete()
            {
                if (GL.IsProgram(this.program)) GL.DeleteProgram(this.program);
            }
        }
    }
}
