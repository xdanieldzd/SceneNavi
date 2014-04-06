using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.OpenGLHelpers
{
    class ScreenWorldConversion
    {
        public static Vector3d WorldToScreen(Vector3d pos)
        {
            int[] viewport = new int[4];
            Matrix4d modelViewMatrix, projectionMatrix;

            GL.GetInteger(GetPName.Viewport, viewport);
            GL.GetDouble(GetPName.ModelviewMatrix, out modelViewMatrix);
            GL.GetDouble(GetPName.ProjectionMatrix, out projectionMatrix);

            return WorldToScreen(pos, modelViewMatrix, projectionMatrix, viewport);
        }

        public static Vector3d WorldToScreen(Vector3d pos, Matrix4d modelviewMatrix, Matrix4d projectionMatrix, int[] viewport)
        {
            Vector3d point;
            Project(ref pos, ref modelviewMatrix, ref projectionMatrix, viewport, out point);
            point.Y = (float)viewport[3] - point.Y;

            return point;
        }

        public static Vector3d ScreenToWorld(Vector2d pos)
        {
            int[] viewport = new int[4];
            Matrix4d modelViewMatrix, projectionMatrix;

            GL.GetInteger(GetPName.Viewport, viewport);
            GL.GetDouble(GetPName.ModelviewMatrix, out modelViewMatrix);
            GL.GetDouble(GetPName.ProjectionMatrix, out projectionMatrix);

            return ScreenToWorld(pos, modelViewMatrix, projectionMatrix, viewport);
        }

        public static Vector3d ScreenToWorld(Vector2d pos, Matrix4d modelviewMatrix, Matrix4d projectionMatrix, int[] viewport)
        {
            Vector3d win = Vector3d.Zero;
            win.X = pos.X;
            win.Y = (viewport[3] - pos.Y);

            double zz = WorldToScreen(Vector3d.Zero, modelviewMatrix, projectionMatrix, viewport).Z;

            float[] boxedZ = new float[1];
            GL.ReadPixels((int)pos.X, viewport[3] - (int)pos.Y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, boxedZ);
            win.Z = boxedZ[0];
            win.Z = zz;

            System.Diagnostics.Debug.Print("{0}, {1}", win.Z, zz);

            return UnProject(win, modelviewMatrix, projectionMatrix, viewport);
        }

        private static Vector3d UnProject(Vector3d screen, Matrix4d modelviewMatrix, Matrix4d projectionMatrix, int[] viewport)
        {
            Vector4d pos = Vector4d.Zero;
            pos.X = ((screen.X - (double)viewport[0]) / (double)viewport[2]) * 2.0 - 1.0;
            pos.Y = ((screen.Y - (double)viewport[1]) / (double)viewport[3]) * 2.0 - 1.0;
            pos.Z = (2.0 * screen.Z) - 1.0;
            pos.W = 1.0;

            Vector4d pos2 = Vector4d.Transform(pos, Matrix4d.Invert(Matrix4d.Mult(modelviewMatrix, projectionMatrix)));
            return new Vector3d(pos2.X, pos2.Y, pos2.Z) / pos2.W;
        }

        private static bool Project(ref Vector3d world, ref Matrix4d modelviewMatrix, ref Matrix4d projectionMatrix, int[] viewport, out Vector3d screen)
        {
            Vector4d _in = new Vector4d(world, 1.0);
            Vector4d _out = new Vector4d();

            Vector4d.Transform(ref _in, ref modelviewMatrix, out _out);
            Vector4d.Transform(ref _out, ref projectionMatrix, out _in);

            if (_in.W == 0.0)
            {
                screen = Vector3d.Zero;
                return false;
            }

            _in.X /= _in.W;
            _in.Y /= _in.W;
            _in.Z /= _in.W;

            /* Map x, y and z to range 0-1 */
            _in.X = _in.X * 0.5 + 0.5;
            _in.Y = _in.Y * 0.5 + 0.5;
            _in.Z = _in.Z * 0.5 + 0.5;

            /* Map x,y to viewport */
            _in.X = _in.X * viewport[2] + viewport[0];
            _in.Y = _in.Y * viewport[3] + viewport[1];

            screen = new Vector3d(_in);
            return true;
        }
    }
}
