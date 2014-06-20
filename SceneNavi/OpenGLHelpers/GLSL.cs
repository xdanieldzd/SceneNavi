using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.OpenGLHelpers
{
    public class GLSL
    {
        // Modified from OpenTK examples \Source\Examples\OpenGL\2.x\SimpleGLSL.cs
        public static void CreateShaders(string vs, string fs, ref int vertexObject, ref int fragmentObject, ref int program)
        {
            try
            {
                int statusCode;
                string info;

                vertexObject = GL.CreateShader(ShaderType.VertexShader);
                fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

                /* Compile vertex shader */
                GL.ShaderSource(vertexObject, vs);
                GL.CompileShader(vertexObject);
                GL.GetShaderInfoLog(vertexObject, out info);
                GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out statusCode);

                if (statusCode != 1) throw new GLSLException(info);

                /* Compile vertex shader */
                GL.ShaderSource(fragmentObject, fs);
                GL.CompileShader(fragmentObject);
                GL.GetShaderInfoLog(fragmentObject, out info);
                GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out statusCode);

                if (statusCode != 1) throw new GLSLException(info);

                program = GL.CreateProgram();
                GL.AttachShader(program, fragmentObject);
                GL.AttachShader(program, vertexObject);

                GL.LinkProgram(program);
                GL.UseProgram(program);
            }
            catch (ApplicationException ae)
            {
                System.Windows.Forms.MessageBox.Show(ae.ToString());
            }
        }

        public class GLSLException : Exception
        {
            public GLSLException(string message) : base(message) { }
            public GLSLException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
