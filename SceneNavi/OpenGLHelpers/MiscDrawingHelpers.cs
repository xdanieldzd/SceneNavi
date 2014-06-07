using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.OpenGLHelpers
{
    class MiscDrawingHelpers
    {
        public static int DummyTextureID { get; private set; }

        static MiscDrawingHelpers()
        {
            byte[] texbuf = new byte[16];
            texbuf.Fill(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

            DummyTextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, DummyTextureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 2, 2, 0, PixelFormat.Rgba, PixelType.UnsignedByte, texbuf);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)All.Linear);
        }

        public static void RenderCube(bool color = false)
        {
            RenderCube(new Vector3d(1.0, 1.0, 1.0), Vector3d.Zero, color);
        }

        public static void RenderCube(Vector3d dimensions, bool color = false)
        {
            RenderCube(dimensions, Vector3d.Zero, color);
        }

        public static void RenderCube(Vector3d dimensions, Vector3d offset, bool color = false)
        {
            Vector2d xvec = new Vector2d(dimensions.X + offset.X, -dimensions.X + offset.X);
            Vector2d yvec = new Vector2d(dimensions.Y + offset.Y, -dimensions.Y + offset.Y);
            Vector2d zvec = new Vector2d(dimensions.Z + offset.Z, -dimensions.Z + offset.Z);

            GL.Begin(PrimitiveType.Quads);
            // Top
            if (color) GL.Color3(1.0, 1.0, 1.0);
            GL.Vertex3(xvec.X, yvec.X, zvec.Y);
            GL.Vertex3(xvec.Y, yvec.X, zvec.Y);
            GL.Vertex3(xvec.Y, yvec.X, zvec.X);
            GL.Vertex3(xvec.X, yvec.X, zvec.X);
            // Bottom
            if (color) GL.Color3(0.5, 0.5, 0.5);
            GL.Vertex3(xvec.X, yvec.Y, zvec.X);
            GL.Vertex3(xvec.Y, yvec.Y, zvec.X);
            GL.Vertex3(xvec.Y, yvec.Y, zvec.Y);
            GL.Vertex3(xvec.X, yvec.Y, zvec.Y);
            // Front
            if (color) GL.Color3(1.0, 0.0, 0.0);
            GL.Vertex3(xvec.X, yvec.X, zvec.X);
            GL.Vertex3(xvec.Y, yvec.X, zvec.X);
            GL.Vertex3(xvec.Y, yvec.Y, zvec.X);
            GL.Vertex3(xvec.X, yvec.Y, zvec.X);
            // Back
            if (color) GL.Color3(0.0, 1.0, 0.0);
            GL.Vertex3(xvec.X, yvec.Y, zvec.Y);
            GL.Vertex3(xvec.Y, yvec.Y, zvec.Y);
            GL.Vertex3(xvec.Y, yvec.X, zvec.Y);
            GL.Vertex3(xvec.X, yvec.X, zvec.Y);
            // Left
            if (color) GL.Color3(1.0, 1.0, 0.0);
            GL.Vertex3(xvec.Y, yvec.X, zvec.X);
            GL.Vertex3(xvec.Y, yvec.X, zvec.Y);
            GL.Vertex3(xvec.Y, yvec.Y, zvec.Y);
            GL.Vertex3(xvec.Y, yvec.Y, zvec.X);
            // Right
            if (color) GL.Color3(0.0, 0.0, 1.0);
            GL.Vertex3(xvec.X, yvec.X, zvec.Y);
            GL.Vertex3(xvec.X, yvec.X, zvec.X);
            GL.Vertex3(xvec.X, yvec.Y, zvec.X);
            GL.Vertex3(xvec.X, yvec.Y, zvec.Y);
            GL.End();
        }

        // Based on http://slabode.exofire.net/circle_draw.shtml
        public static void RenderNotchCircle(double r, int segments)
        {
            double theta = (double)(2 * Math.PI / (double)segments);
            double c = Math.Cos(theta);
            double s = Math.Sin(theta);
            double t;

            double x = r;
            double y = 0;

            GL.Begin(PrimitiveType.LineLoop);
            for (int ii = 0; ii < segments; ii++)
            {
                if (ii == segments / 4)
                    GL.Vertex2(x, (y + 1.5));
                else
                    GL.Vertex2(x, y);
                t = x;
                x = c * x - s * y;
                y = s * t + c * y;
            }
            GL.End();
        }

        public static void RenderSphere(Vector3d c, double r, int n)
        {
            int i, j;
            double theta1, theta2, theta3;
            Vector3d e, p;

            if (r < 0) r = -r;
            if (n < 0) n = -n;

            if (n < 4 || r <= 0)
            {
                GL.Begin(PrimitiveType.Points);
                GL.Vertex3(c);
                GL.End();
                return;
            }

            const double TWOPI = (Math.PI * 2);
            const double PID2 = (Math.PI / 2);

            for (j = 0; j < n / 2; j++)
            {
                theta1 = j * TWOPI / n - PID2;
                theta2 = (j + 1) * TWOPI / n - PID2;

                GL.Begin(PrimitiveType.QuadStrip);
                for (i = n; i >= 0; i--)
                {
                    theta3 = i * TWOPI / n;

                    e.X = Math.Cos(theta2) * Math.Cos(theta3);
                    e.Y = Math.Sin(theta2);
                    e.Z = Math.Cos(theta2) * Math.Sin(theta3);
                    p.X = c.X + r * e.X;
                    p.Y = c.Y + r * e.Y;
                    p.Z = c.Z + r * e.Z;
                    GL.Vertex3(p);

                    e.X = Math.Cos(theta1) * Math.Cos(theta3);
                    e.Y = Math.Sin(theta1);
                    e.Z = Math.Cos(theta1) * Math.Sin(theta3);
                    p.X = c.X + r * e.X;
                    p.Y = c.Y + r * e.Y;
                    p.Z = c.Z + r * e.Z;
                    GL.Vertex3(p);
                }
                GL.End();
            }
        }

        public static void DrawBox(Vector3d mvMin, Vector3d mvMax)
        {
            Vector3d[] v = new Vector3d[8];
            v[0] = new Vector3d(mvMin.X, mvMin.Y, mvMin.Z);
            v[1] = new Vector3d(mvMax.X, mvMin.Y, mvMin.Z);
            v[2] = new Vector3d(mvMax.X, mvMax.Y, mvMin.Z);
            v[3] = new Vector3d(mvMin.X, mvMax.Y, mvMin.Z);
            v[4] = new Vector3d(mvMin.X, mvMin.Y, mvMax.Z);
            v[5] = new Vector3d(mvMax.X, mvMin.Y, mvMax.Z);
            v[6] = new Vector3d(mvMax.X, mvMax.Y, mvMax.Z);
            v[7] = new Vector3d(mvMin.X, mvMax.Y, mvMax.Z);

            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Disable(EnableCap.Lighting);
            GL.Color4(Color4.Red);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(v[0]); GL.Vertex3(v[1]);
            GL.Vertex3(v[1]); GL.Vertex3(v[2]);
            GL.Vertex3(v[2]); GL.Vertex3(v[3]);
            GL.Vertex3(v[3]); GL.Vertex3(v[0]);
            GL.Vertex3(v[4]); GL.Vertex3(v[5]);
            GL.Vertex3(v[5]); GL.Vertex3(v[6]);
            GL.Vertex3(v[6]); GL.Vertex3(v[7]);
            GL.Vertex3(v[7]); GL.Vertex3(v[4]);
            GL.Vertex3(v[0]); GL.Vertex3(v[4]);
            GL.Vertex3(v[1]); GL.Vertex3(v[5]);
            GL.Vertex3(v[2]); GL.Vertex3(v[6]);
            GL.Vertex3(v[3]); GL.Vertex3(v[7]);
            GL.End();
            GL.PopAttrib();
        }

        public static void DrawBox(Vector3d Min, Vector3d Max, Color4 Color, bool Outline = false)
        {
            Vector3d Distance = Max - Min;
            Vector3d[] Verts = new Vector3d[8];

            Verts[0] = Min;
            Verts[1] = Min + new Vector3d(Distance.X, 0, 0);
            Verts[2] = Min + new Vector3d(Distance.X, Distance.Y, 0);
            Verts[3] = Min + new Vector3d(0, Distance.Y, 0);

            Verts[4] = Min + new Vector3d(0, 0, Distance.Z);
            Verts[5] = Min + new Vector3d(Distance.X, 0, Distance.Z);
            Verts[6] = Min + new Vector3d(Distance.X, Distance.Y, Distance.Z);
            Verts[7] = Min + new Vector3d(0, Distance.Y, Distance.Z);

            for (int it = 0; it < 2; it++)
            {
                // TODO  fix loop thingy; some quads don't render!
                PrimitiveType loop = (it == 1 ? PrimitiveType.LineLoop : PrimitiveType.Quads);
                PrimitiveType norm = (it == 1 ? PrimitiveType.Lines : PrimitiveType.QuadStrip);
                Color4 col = (it == 1 ? Color4.Black : Color);

                GL.Color4(col);

                GL.Begin(loop);
                for (int i = 0; i < 4; i++)
                    GL.Vertex3(Verts[i]);
                GL.End();

                GL.Begin(norm);
                for (int i = 0; i < 4; i++)
                {
                    GL.Vertex3(Verts[i]);
                    GL.Vertex3(Verts[i + 4]);
                }
                GL.End();

                GL.Begin(loop);
                for (int i = 0; i < 4; i++)
                    GL.Vertex3(Verts[i + 4]);
                GL.End();
            }
        }
    }
}
