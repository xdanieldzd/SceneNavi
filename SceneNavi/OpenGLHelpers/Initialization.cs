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
    static class Initialization
    {
        public enum ProjectionTypes { Perspective, Orthographic };

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
            get
            {
                string str = GL.GetString(StringName.ShadingLanguageVersion);
                if (str == null || str == string.Empty) return "[unsupported]";
                else return str;
            }
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

        public static void CreateViewportAndProjection(ProjectionTypes projectionType, Rectangle clientRectangle, float near, float far)
        {
            if (clientRectangle.Width <= 0 || clientRectangle.Height <= 0) return;

            GL.Viewport(0, 0, clientRectangle.Width, clientRectangle.Height);

            Matrix4 projectionMatrix = Matrix4.Identity;

            switch (projectionType)
            {
                case ProjectionTypes.Perspective:
                    double aspect = clientRectangle.Width / (double)clientRectangle.Height;
                    projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver3, (float)aspect, near, far);
                    break;

                case ProjectionTypes.Orthographic:
                    projectionMatrix = Matrix4.CreateOrthographicOffCenter(clientRectangle.Left, clientRectangle.Right, clientRectangle.Bottom, clientRectangle.Top, near, far);
                    break;
            }

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.MultMatrix(ref projectionMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }
    }
}
