using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using QuickFont;

namespace SceneNavi.OpenGLHelpers
{
    class QuickFontWrapper
    {
        static Color4 defcol = Color4.White;
        QFont qf;

        public QuickFontWrapper(Font font)
        {
            qf = new QuickFont.QFont(font, new QuickFont.QFontBuilderConfiguration(true) { TextGenerationRenderHint = QuickFont.TextGenerationRenderHint.ClearTypeGridFit });
            qf.Options.CharacterSpacing = -0.1f;
            qf.Options.WordSpacing = 0.5f;
            qf.Options.LineSpacing = 1.2f;
        }

        public void Begin()
        {
            QuickFont.QFont.InvalidateViewport();
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);
            if (Initialization.SupportsFunction("glGenProgramsARB")) GL.Disable((EnableCap)All.FragmentProgram);
            QuickFont.QFont.Begin();
        }

        public void Print(string str, Vector2d pos, Color4? col = null)
        {
            qf.Options.Colour = (col.HasValue ? col.Value : Color4.White);
            GL.PushMatrix();
            GL.Translate(new Vector3d(pos.X, pos.Y, 0.0));
            qf.Print(str);
            GL.PopMatrix();
        }

        public void End()
        {
            QuickFont.QFont.End();
            GL.PopAttrib();
        }
    }
}
