using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.OpenGLHelpers
{
    static class StockObjects
    {
        public static DisplayList AxisMarker { get; private set; }
        public static DisplayList SimpleAxisMarker { get; private set; }

        public static DisplayList Cube { get; private set; }
        public static DisplayList ColoredCube { get; private set; }
        public static DisplayList Sphere { get; private set; }
        public static DisplayList DownArrow { get; private set; }

        public static DisplayList Slab { get; private set; }
        public static DisplayList ColoredSlab { get; private set; }

        public static DisplayList Door { get; private set; }
        public static DisplayList ColoredDoor { get; private set; }

        static StockObjects()
        {
            /* Axis marker */
            AxisMarker = new DisplayList(ListMode.Compile);
            GL.LineWidth(4.0f);
            GL.DepthRange(0.0, 0.99999);

            double radius = 12.0;
            GL.LineWidth(4.0f);
            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0, 0.0, 0.0);
            GL.Vertex3(radius, 0.0, 0.0);
            GL.Vertex3(-radius, 0.0, 0.0);
            GL.Color3(0.0, 1.0, 0.0);
            GL.Vertex3(0.0, radius, 0.0);
            GL.Vertex3(0.0, -radius, 0.0);
            GL.End();

            GL.PushMatrix();
            GL.Rotate(90.0, 1.0, 0.0, 0.0);
            GL.Color3(0.0, 0.0, 1.0);
            MiscDrawingHelpers.RenderNotchCircle(radius / 2.0, 48);
            GL.PopMatrix();

            GL.DepthRange(0.0, 1.0);
            GL.LineWidth(1.0f);
            AxisMarker.End();

            /* Simple axis marker */
            SimpleAxisMarker = new DisplayList(ListMode.Compile);
            GL.LineWidth(4.0f);
            GL.DepthRange(0.0, 0.99999);

            GL.LineWidth(4.0f);
            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0, 0.0, 0.0);
            GL.Vertex3(radius, 0.0, 0.0);
            GL.Vertex3(-radius, 0.0, 0.0);
            GL.Color3(0.0, 1.0, 0.0);
            GL.Vertex3(0.0, radius, 0.0);
            GL.Vertex3(0.0, -radius, 0.0);
            GL.Color3(0.0, 0.0, 1.0);
            GL.Vertex3(0.0, 0.0, radius);
            GL.Vertex3(0.0, 0.0, -radius);
            GL.End();

            GL.DepthRange(0.0, 1.0);
            GL.LineWidth(1.0f);
            SimpleAxisMarker.End();

            /* Simple cube */
            Cube = new DisplayList(ListMode.Compile);
            MiscDrawingHelpers.RenderCube(false);
            Cube.End();

            /* Colored cube */
            ColoredCube = new DisplayList(ListMode.Compile);
            MiscDrawingHelpers.RenderCube(true);
            ColoredCube.End();

            /* Sphere */
            Sphere = new DisplayList(ListMode.Compile);
            MiscDrawingHelpers.RenderSphere(Vector3d.Zero, 3.0, 12);
            Sphere.End();

            /* Down arrow */
            DownArrow = new DisplayList(ListMode.Compile);
            // Top
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(1.0, 2.0, -1.0);
            GL.Vertex3(-1.0, 2.0, -1.0);
            GL.Vertex3(-1.0, 2.0, 1.0);
            GL.Vertex3(1.0, 2.0, 1.0);
            GL.End();

            GL.Begin(BeginMode.Triangles);
            // Left
            GL.Vertex3(-1.0, 2.0, 1.0);
            GL.Vertex3(-1.0, 2.0, -1.0);
            GL.Vertex3(0.0, 0.0, 0.0);
            // Right
            GL.Vertex3(1.0, 2.0, -1.0);
            GL.Vertex3(1.0, 2.0, 1.0);
            GL.Vertex3(0.0, 0.0, 0.0);
            // Front
            GL.Vertex3(1.0, 2.0, 1.0);
            GL.Vertex3(-1.0, 2.0, 1.0);
            GL.Vertex3(0.0, 0.0, 0.0);
            // Back
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(-1.0, 2.0, -1.0);
            GL.Vertex3(1.0, 2.0, -1.0);
            GL.End();
            DownArrow.End();

            /* Simple slab (ex. boxes, chests) */
            Slab = new DisplayList(ListMode.Compile);
            MiscDrawingHelpers.RenderCube(new Vector3d(3.0, 2.0, 2.0), false);
            Slab.End();

            /* Colored slab */
            ColoredSlab = new DisplayList(ListMode.Compile);
            MiscDrawingHelpers.RenderCube(new Vector3d(3.0, 2.0, 2.0), true);
            ColoredSlab.End();

            /* Door */
            Door = new DisplayList(ListMode.Compile);
            MiscDrawingHelpers.RenderCube(new Vector3d(2.5, 4.5, 0.5), new Vector3d(0.0, 4.0, 0.0), false);
            Door.End();

            /* Colored door */
            ColoredDoor = new DisplayList(ListMode.Compile);
            MiscDrawingHelpers.RenderCube(new Vector3d(2.5, 4.5, 0.5), new Vector3d(0.0, 4.0, 0.0), true);
            ColoredDoor.End();
        }

        public static DisplayList GetDisplayList(string prop)
        {
            Type tp = typeof(StockObjects);
            System.Reflection.PropertyInfo propinfo = tp.GetProperty(prop, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (propinfo == null) return null;
            return (propinfo.GetValue(null, null) as DisplayList);
        }
    }
}
