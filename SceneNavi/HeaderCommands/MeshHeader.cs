using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.HeaderCommands
{
	public class MeshHeader : Generic, IPickableObject
	{
		public byte Type { get; private set; }
		public byte Count { get; private set; }
		public uint DLTablePointer { get; private set; }
		public uint OtherDataPointer { get; private set; }

		public List<uint> OpaqueDLAddresses { get; private set; }
		public List<uint> TransparentDLAddresses { get; private set; }
		public List<OpenGLHelpers.DisplayListEx> OpaqueDLs { get; private set; }
		public List<OpenGLHelpers.DisplayListEx> TransparentDLs { get; private set; }
		public List<SimpleF3DEX2.SimpleTriangle> TriangleList { get; private set; }

		public bool CachedWithTextures { get; private set; }
		public CombinerTypes CachedWithCombinerType { get; private set; }

		public int PickGLID { get; private set; }
		public List<Vector3d> MaxClipBounds { get; private set; }
		public List<Vector3d> MinClipBounds { get; private set; }

		[Browsable(false)]
		public System.Drawing.Color PickColor { get { return System.Drawing.Color.FromArgb(GetHashCode() & 0xFFFFFF | (0xFF << 24)); } }

		[Browsable(false)]
		public bool IsMoveable { get { return false; } }

		[Browsable(false)]
		public Vector3d Position { get { return Vector3d.Zero; } set { return; } }

		public MeshHeader(Generic basecmd)
			: base(basecmd)
		{
			byte seg = (byte)(GetAddressGeneric() >> 24);
			uint adr = (uint)(GetAddressGeneric() & 0xFFFFFF);

			Type = ((byte[])ROM.SegmentMapping[seg])[adr];
			Count = ((byte[])ROM.SegmentMapping[seg])[adr + 1];
			DLTablePointer = Endian.SwapUInt32(BitConverter.ToUInt32((byte[])ROM.SegmentMapping[seg], (int)(adr + 4)));
			OtherDataPointer = Endian.SwapUInt32(BitConverter.ToUInt32((byte[])ROM.SegmentMapping[seg], (int)(adr + 8)));

			MaxClipBounds = new List<Vector3d>();
			MinClipBounds = new List<Vector3d>();

			OpaqueDLAddresses = new List<uint>();
			TransparentDLAddresses = new List<uint>();

			switch (Type)
			{
				case 0x00:
					{
						for (int i = 0; i < Count; i++)
						{
							OpaqueDLAddresses.Add(Endian.SwapUInt32(BitConverter.ToUInt32((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)], (int)((DLTablePointer & 0xFFFFFF) + (i * 8)))));
							TransparentDLAddresses.Add(Endian.SwapUInt32(BitConverter.ToUInt32((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)], (int)((DLTablePointer & 0xFFFFFF) + (i * 8) + 4))));
						}
						break;
					}
				case 0x01:
					{
						for (int i = 0; i < Count; i++)
							OpaqueDLAddresses.Add(Endian.SwapUInt32(BitConverter.ToUInt32((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)], (int)((DLTablePointer & 0xFFFFFF) + (i * 4)))));
						break;
					}
				case 0x02:
					{
						for (int i = 0; i < Count; i++)
						{
							short s1 = Endian.SwapInt16(BitConverter.ToInt16((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)], (int)((DLTablePointer & 0xFFFFFF) + (i * 16))));
							short s2 = Endian.SwapInt16(BitConverter.ToInt16((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)], (int)((DLTablePointer & 0xFFFFFF) + (i * 16) + 2)));
							short s3 = Endian.SwapInt16(BitConverter.ToInt16((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)], (int)((DLTablePointer & 0xFFFFFF) + (i * 16) + 4)));
							short s4 = Endian.SwapInt16(BitConverter.ToInt16((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)], (int)((DLTablePointer & 0xFFFFFF) + (i * 16) + 6)));

							MaxClipBounds.Add(new Vector3d(s1, 0.0, s2));
							MinClipBounds.Add(new Vector3d(s3, 0.0, s4));

							OpaqueDLAddresses.Add(Endian.SwapUInt32(BitConverter.ToUInt32((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)], (int)((DLTablePointer & 0xFFFFFF) + (i * 16) + 8))));
							TransparentDLAddresses.Add(Endian.SwapUInt32(BitConverter.ToUInt32((byte[])ROM.SegmentMapping[(byte)(DLTablePointer >> 24)], (int)((DLTablePointer & 0xFFFFFF) + (i * 16) + 12))));
						}
						break;
					}

				default: throw new Exception(string.Format("Invalid mesh type 0x{0:X2}", Type));
			}

			OpaqueDLAddresses.RemoveAll(x => x == 0);
			TransparentDLAddresses.RemoveAll(x => x == 0);
		}

		public void CreateDisplayLists(bool texenabled, CombinerTypes combinertype)
		{
			Program.Status.Message = string.Format("Rendering room '{0}'...", (Parent as Rooms.RoomInfoClass).Description);

			CachedWithTextures = texenabled;
			CachedWithCombinerType = combinertype;

			/* Execute DLs once before creating GL lists, to cache textures & fragment programs beforehand */
			foreach (uint dl in OpaqueDLAddresses) ROM.Renderer.Render(dl);
			foreach (uint dl in TransparentDLAddresses) ROM.Renderer.Render(dl);

			/* Copy most recently rendered triangles - this mesh header's DL's - to triangle list */
			TriangleList = new List<SimpleF3DEX2.SimpleTriangle>();
			foreach (SimpleF3DEX2.SimpleTriangle st in ROM.Renderer.LastTriList) TriangleList.Add(st);

			/* Now execute DLs again, with stuff already cached, which speeds everything up! */
			OpaqueDLs = new List<OpenGLHelpers.DisplayListEx>();
			foreach (uint dl in OpaqueDLAddresses)
			{
				OpenGLHelpers.DisplayListEx newdlex = new OpenGLHelpers.DisplayListEx(ListMode.Compile);
				ROM.Renderer.Render(dl, gldl: newdlex);
				newdlex.End();
				OpaqueDLs.Add(newdlex);
			}
			TransparentDLs = new List<OpenGLHelpers.DisplayListEx>();
			foreach (uint dl in TransparentDLAddresses)
			{
				OpenGLHelpers.DisplayListEx newdlex = new OpenGLHelpers.DisplayListEx(ListMode.Compile);
				ROM.Renderer.Render(dl, gldl: newdlex);
				newdlex.End();
				TransparentDLs.Add(newdlex);
			}

			/* Clear the renderer's triangle list */
			ROM.Renderer.LastTriList.Clear();

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
			if (OpaqueDLs != null)
			{
				foreach (OpenGLHelpers.DisplayListEx gldl in OpaqueDLs) gldl.Dispose();
				OpaqueDLs = null;
			}

			if (TransparentDLs != null)
			{
				foreach (OpenGLHelpers.DisplayListEx gldl in TransparentDLs) gldl.Dispose();
				TransparentDLs = null;
			}
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
