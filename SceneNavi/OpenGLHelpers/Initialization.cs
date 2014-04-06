using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.OpenGLHelpers
{
    static class Initialization
    {
        public static string RendererString
        {
            get { return GL.GetString(StringName.Renderer) ?? "[null]"; }
        }

        public static string VendorString
        {
            get { return GL.GetString(StringName.Vendor) ?? "[null]"; }
        }

        public static string VersionString
        {
            get { return GL.GetString(StringName.Version) ?? "[null]"; }
        }

        public static string ShadingLanguageVersionString
        {
            get { return GL.GetString(StringName.ShadingLanguageVersion) ?? "[null]"; }
        }

        public static string[] SupportedExtensions
        {
            get { return GL.GetString(StringName.Extensions).Split(new char[] { ' ' }) ?? new string[] { "[null]" }; }
        }

        public static bool VendorIsIntel
        {
            get { return VendorString.Contains("Intel"); }
        }

        public static bool SupportsFunction(string function)
        {
            return ((GraphicsContext.CurrentContext as IGraphicsContextInternal).GetAddress(function) != IntPtr.Zero);
        }

        public static void ActiveTextureChecked(TextureUnit unit)
        {
            if (SupportsFunction("glActiveTextureARB")) GL.Arb.ActiveTexture(unit);
        }

        public static void MultiTexCoord2Checked(TextureUnit unit, double s, double t)
        {
            if (SupportsFunction("glMultiTexCoord2dARB"))
                GL.Arb.MultiTexCoord2(unit, s, t);
            else
                GL.TexCoord2(s, t);
        }

        public static int GetInteger(GetPName name)
        {
            int outval;
            GL.GetInteger(name, out outval);
            return outval;
        }

        public static int MaxTextureUnits { get { int outval; GL.GetInteger(GetPName.MaxTextureUnits, out outval); return outval; } }

        public static List<string> CheckForExtensions(string[] NeededExts)
        {
            List<string> missing = new List<string>();
            if (NeededExts != null) missing.AddRange(NeededExts.Where(x => !CheckForExtension(x)));
            return missing;
        }

        public static bool CheckForExtension(string NeededExt)
        {
            return SupportedExtensions.Contains(NeededExt);
        }

        public static void SetDefaults()
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.PointSmooth);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.ClearColor(System.Drawing.Color.LightBlue);
            GL.ClearDepth(5.0);

            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Enable(EnableCap.Light0);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Normalize);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        public static void SetViewport(int Width, int Height)
        {
            if (Width == 0 || Height == 0) return;

            GL.Viewport(0, 0, Width, Height);

            double aspect = Width / (double)Height;
            Matrix4 PerspMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect, 0.1f, 15000);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.MultMatrix(ref PerspMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }
    }
}
