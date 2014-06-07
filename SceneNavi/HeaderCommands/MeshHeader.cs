using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.HeaderCommands
{
    public class MeshHeader : Generic, IPickableObject
    {
        public byte Type { get; private set; }
        public byte Count { get; private set; }
        public uint DLTablePointer { get; private set; }
        public uint OtherDataPointer { get; private set; }

        public List<uint> DLAddresses { get; private set; }
        public List<OpenGLHelpers.DisplayListEx> DLs { get; private set; }
        public List<SimpleF3DEX2.SimpleTriangle> TriangleList { get; private set; }

        public bool CachedWithTextures { get; private set; }
        public CombinerTypes CachedWithCombinerType { get; private set; }

        public int PickGLID { get; private set; }
        public List<Vector3d> MaxClipBounds { get; private set; }
        public List<Vector3d> MinClipBounds { get; private set; }

        [Browsable(false)]
        public System.Drawing.Color PickColor { get { return System.Drawing.Color.FromArgb(this.GetHashCode() & 0xFFFFFF | (0xFF << 24)); } }

        [Browsable(false)]
        public bool IsMoveable { get { return false; } }

        [Browsable(false)]
        public OpenTK.Vector3d Position { get { return Vector3d.Zero; } set { return; } }

        public MeshHeader(Generic basecmd)
            : base(basecmd)
        {
            byte seg = (byte)(GetAddressGeneric() >> 24);
            uint adr = (uint)(GetAddressGeneric() & 0xFFFFFF);

            Type = ((byte[])ROM.SegmentMapping[seg])[adr];
            Count = ((byte[])ROM.SegmentMapping[seg])[adr + 1];
            DLTablePointer = Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[seg]), (int)(adr + 4)));
            OtherDataPointer = Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[seg]), (int)(adr + 8)));

            DLAddresses = new List<uint>();

            MaxClipBounds = new List<Vector3d>();
            MinClipBounds = new List<Vector3d>();

            List<uint> opaqueDLs = new List<uint>();
            List<uint> transparentDLs = new List<uint>();

            switch (Type)
            {
                case 0x00:
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            opaqueDLs.Add(Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)]), (int)((DLTablePointer & 0xFFFFFF) + (i * 8)))));
                            transparentDLs.Add(Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)]), (int)((DLTablePointer & 0xFFFFFF) + (i * 8) + 4))));
                        }
                        break;
                    }
                case 0x01:
                    {
                        for (int i = 0; i < Count; i++)
                            opaqueDLs.Add(Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)]), (int)((DLTablePointer & 0xFFFFFF) + (i * 4)))));
                        break;
                    }
                case 0x02:
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            MaxClipBounds.Add(new Vector3d(
                                Endian.SwapInt16(BitConverter.ToInt16(((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)]), (int)((DLTablePointer & 0xFFFFFF) + (i * 16)))),
                                0.0,
                                Endian.SwapInt16(BitConverter.ToInt16(((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)]), (int)((DLTablePointer & 0xFFFFFF) + (i * 16) + 2)))));
                            MinClipBounds.Add(new Vector3d(
                                Endian.SwapInt16(BitConverter.ToInt16(((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)]), (int)((DLTablePointer & 0xFFFFFF) + (i * 16)) + 4)),
                                0.0,
                                Endian.SwapInt16(BitConverter.ToInt16(((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)]), (int)((DLTablePointer & 0xFFFFFF) + (i * 16) + 6)))));

                            opaqueDLs.Add(Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)]), (int)((DLTablePointer & 0xFFFFFF) + (i * 16) + 8))));
                            transparentDLs.Add(Endian.SwapUInt32(BitConverter.ToUInt32(((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)]), (int)((DLTablePointer & 0xFFFFFF) + (i * 16) + 12))));
                        }
                        break;
                    }

                default: throw new Exception(string.Format("Invalid mesh type 0x{0:X2}", Type));
            }

            DLAddresses.AddRange(opaqueDLs);
            DLAddresses.AddRange(transparentDLs);
            DLAddresses.RemoveAll(x => x == 0);
        }

        public void CreateDisplayLists(bool texenabled, CombinerTypes combinertype)
        {
            Program.Status.Message = string.Format("Rendering room '{0}'...", (this.Parent as HeaderCommands.Rooms.RoomInfoClass).Description);

            CachedWithTextures = texenabled;
            CachedWithCombinerType = combinertype;

            /* Execute DLs once before creating GL lists, to cache textures & fragment programs beforehand */
            foreach (uint dl in DLAddresses) this.ROM.Renderer.Render(dl);

            /* Copy most recently rendered triangles - this mesh header's DL's - to triangle list */
            TriangleList = new List<SimpleF3DEX2.SimpleTriangle>();
            foreach (SimpleF3DEX2.SimpleTriangle st in this.ROM.Renderer.LastTriList) TriangleList.Add(st);

            /* Now execute DLs again, with stuff already cached, which speeds everything up! */
            DLs = new List<OpenGLHelpers.DisplayListEx>();
            foreach (uint dl in DLAddresses)
            {
                OpenGLHelpers.DisplayListEx newdlex = new OpenGLHelpers.DisplayListEx(ListMode.Compile);
                this.ROM.Renderer.Render(dl, gldl: newdlex);
                newdlex.End();
                DLs.Add(newdlex);
            }

            /* Clear the renderer's triangle list */
            this.ROM.Renderer.LastTriList.Clear();

            /* Finally, from the triangle list compiled before, create a simple display list for picking purposes */
            PickGLID = GL.GenLists(1);
            GL.NewList(PickGLID, ListMode.Compile);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);
            if (OpenGLHelpers.Initialization.SupportsFunction("glGenProgramsARB")) GL.Disable((EnableCap)All.FragmentProgram);
            GL.Begin(PrimitiveType.Triangles);
            foreach (SimpleF3DEX2.SimpleTriangle st in TriangleList)
            {
                GL.Vertex3(st.Vertices[0]);
                GL.Vertex3(st.Vertices[1]);
                GL.Vertex3(st.Vertices[2]);
            }
            GL.End();
            GL.EndList();
        }

        public void DestroyDisplayLists()
        {
            if (DLs == null) return;

            foreach (OpenGLHelpers.DisplayListEx gldl in DLs) gldl.Dispose();
            DLs = null;
        }

        public void Render(PickableObjectRenderType rendertype)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);
            if (rendertype == PickableObjectRenderType.Picking)
            {
                GL.Color3(PickColor);
                GL.CallList(PickGLID);
            }
            GL.PopAttrib();
        }
    }
}
