using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.OpenGLHelpers
{
    public class DisplayList : IDisposable
    {
        public class DisplayListException : Exception
        {
            public DisplayListException(string errorMessage) : base(errorMessage) { }
            public DisplayListException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
        }

        int glid;

        public DisplayList()
        {
            Generate();
        }

        public DisplayList(ListMode listmode)
        {
            Generate();
            New(listmode);
        }

        public void Generate()
        {
            glid = GL.GenLists(1);
        }

        public void New(ListMode listmode)
        {
            if (!GL.IsList(glid)) Generate();

            GL.NewList(glid, listmode);
        }

        public void End()
        {
            GL.EndList();
        }

        public void Render()
        {
            if (!GL.IsList(glid)) throw new DisplayListException("Cannot render display list; no list generated");

            GL.CallList(glid);
        }

        public void Dispose()
        {
            if (GL.IsList(glid)) GL.DeleteLists(glid, 1);
        }
    }
}
