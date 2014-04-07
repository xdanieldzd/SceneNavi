using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.OpenGLHelpers
{
    public class DisplayListEx : DisplayList
    {
        internal class Triangle : HeaderCommands.IPickableObject
        {
            [Browsable(false)]
            public Vector3d Position { get { return Vector3d.Zero; } set { } }

            [Browsable(false)]
            public System.Drawing.Color PickColor { get { return System.Drawing.Color.FromArgb(this.GetHashCode() & 0xFFFFFF | (0xFF << 24)); } }

            [Browsable(false)]
            public bool IsMoveable { get { return false; } }

            public List<SimpleF3DEX2.Vertex> Vertices { get; private set; }
            public SimpleF3DEX2.Vertex SelectedVertex { get; set; }

            public Triangle(params SimpleF3DEX2.Vertex[] verts)
            {
                Vertices = new List<SimpleF3DEX2.Vertex>();
                Vertices.AddRange(verts);
            }

            public void Render(HeaderCommands.PickableObjectRenderType rendertype)
            {
                if (rendertype == HeaderCommands.PickableObjectRenderType.Picking)
                {
                    GL.Color3(PickColor);
                    GL.Begin(BeginMode.Triangles);
                    foreach (SimpleF3DEX2.Vertex v in Vertices) GL.Vertex3(v.Position);
                    GL.End();
                }
                else if (rendertype == HeaderCommands.PickableObjectRenderType.Normal)
                {
                    GL.PushAttrib(AttribMask.AllAttribBits);
                    GL.Disable(EnableCap.Texture2D);
                    GL.Disable(EnableCap.Lighting);
                    if (Initialization.SupportsFunction("glGenProgramsARB")) GL.Disable((EnableCap)All.FragmentProgram);
                    if (Initialization.SupportsFunction("glCreateShader")) GL.UseProgram(0);
                    GL.Disable(EnableCap.CullFace);
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    GL.DepthRange(0.0, 0.99995);

                    /* White poly */
                    GL.Color4(1.0, 1.0, 1.0, 0.25);
                    GL.Begin(BeginMode.Triangles);
                    foreach (SimpleF3DEX2.Vertex v in Vertices) GL.Vertex3(v.Position);
                    GL.End();
                    /* Black outlines */
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    GL.LineWidth(2.0f);
                    GL.Color4(Color4.Black);
                    GL.Begin(BeginMode.Triangles);
                    foreach (SimpleF3DEX2.Vertex v in Vertices) GL.Vertex3(v.Position);
                    GL.End();
                    /* Vertex points */
                    GL.DepthRange(0.0, 0.999);
                    GL.PointSize(25.0f);
                    GL.Begin(BeginMode.Points);
                    foreach (SimpleF3DEX2.Vertex v in Vertices)
                    {
                        if (SelectedVertex == v) GL.Color4(Color4.LimeGreen);
                        else GL.Color4(Color4.Red);

                        GL.Vertex3(v.Position);
                    }
                    GL.End();

                    GL.PopAttrib();
                }
            }
        }

        internal List<Triangle> Triangles { get; private set; }

        public DisplayListEx()
            : base()
        {
            Triangles = new List<Triangle>();
        }

        public DisplayListEx(ListMode listmode)
            : base(listmode)
        {
            Triangles = new List<Triangle>();
        }
    }
}
