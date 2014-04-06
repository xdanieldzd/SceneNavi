using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.SimpleF3DEX2
{
    public class SimpleTriangle
    {
        public Vector3d[] Vertices { get; private set; }

        public SimpleTriangle(Vector3d v1, Vector3d v2, Vector3d v3)
        {
            Vertices = new Vector3d[3];
            Vertices[0] = v1;
            Vertices[1] = v2;
            Vertices[2] = v3;
        }
    }
}
