using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SceneNavi.OpenGLHelpers;
using SceneNavi.ROMHandler;

namespace SceneNavi.HeaderCommands
{
	public class Waypoints : Generic, IStoreable
	{
		public List<PathHeader> Paths { get; set; }

		public Waypoints(Generic basecmd)
			: base(basecmd)
		{
			Paths = new List<PathHeader>();

			int i = 0;
			while (true)
			{
				PathHeader nph = new PathHeader(ROM, (uint)(GetAddressGeneric() + i * 8), i);
				if (nph.Points == null) break;
				Paths.Add(nph);
				i++;
			}
		}

		public void Store(byte[] databuf, int baseadr)
		{
			foreach (HeaderCommands.Waypoints.PathHeader path in this.Paths)
			{
				foreach (HeaderCommands.Waypoints.Waypoint wp in path.Points)
				{
					byte[] bytes = BitConverter.GetBytes(Endian.SwapInt16((short)wp.X));
					System.Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (wp.Address & 0xFFFFFF)), bytes.Length);
					bytes = BitConverter.GetBytes(Endian.SwapInt16((short)wp.Y));
					System.Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (wp.Address & 0xFFFFFF) + 2), bytes.Length);
					bytes = BitConverter.GetBytes(Endian.SwapInt16((short)wp.Z));
					System.Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (wp.Address & 0xFFFFFF) + 4), bytes.Length);
				}
			}
		}

		public class PathHeader
		{
			public uint Address { get; set; }

			public uint WaypointCount { get; private set; }
			public uint WaypointAddress { get; private set; }

			public List<Waypoint> Points { get; private set; }

			public string Description
			{
				get
				{
					if (ROM == null)
						return "(None)";
					else
						return string.Format("Path #{0}: {1} waypoints", (PathNumber + 1), WaypointCount);
				}
			}

			int PathNumber;

			ROMHandler.ROMHandler ROM;

			public PathHeader() { }

			public PathHeader(ROMHandler.ROMHandler rom, uint adr, int number)
			{
				ROM = rom;
				Address = adr;
				PathNumber = number;

				byte[] segdata = (byte[])ROM.SegmentMapping[(byte)(adr >> 24)];
				if (segdata == null) return;

				WaypointCount = BitConverter.ToUInt32(segdata, (int)(adr & 0xFFFFFF));
				WaypointAddress = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)(adr & 0xFFFFFF) + 4));

				byte[] psegdata = (byte[])ROM.SegmentMapping[(byte)(WaypointAddress >> 24)];
				if (WaypointCount == 0 || WaypointCount > 0xFF || psegdata == null || (WaypointAddress & 0xFFFFFF) >= psegdata.Length) return;

				Points = new List<Waypoint>();
				for (int i = 0; i < WaypointCount; i++)
				{
					Points.Add(new Waypoint(ROM, (uint)(WaypointAddress + (i * 6))));
				}
			}
		}

		public class Waypoint : IPickableObject
		{
			public uint Address { get; set; }

			[DisplayName("X position")]
			public double X { get; set; }
			[DisplayName("Y position")]
			public double Y { get; set; }
			[DisplayName("Z position")]
			public double Z { get; set; }

			[Browsable(false)]
			public Vector3d Position { get { return new Vector3d(X, Y, Z); } set { X = value.X; Y = value.Y; Z = value.Z; } }

			[Browsable(false)]
			public System.Drawing.Color PickColor { get { return System.Drawing.Color.FromArgb(this.GetHashCode() & 0xFFFFFF | (0xFF << 24)); } }

			[Browsable(false)]
			public bool IsMoveable { get { return true; } }

			ROMHandler.ROMHandler ROM;

			public Waypoint() { }

			public Waypoint(ROMHandler.ROMHandler rom, uint adr)
			{
				ROM = rom;
				Address = adr;

				byte[] segdata = (byte[])ROM.SegmentMapping[(byte)(adr >> 24)];
				if (segdata == null) return;

				X = (double)Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)(adr & 0xFFFFFF)));
				Y = (double)Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)(adr & 0xFFFFFF) + 2));
				Z = (double)Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)(adr & 0xFFFFFF) + 4));
			}

			public void Render(PickableObjectRenderType rendertype)
			{
				GL.PushAttrib(AttribMask.AllAttribBits);

				GL.PushMatrix();
				GL.Translate(X, Y, Z);
				GL.Scale(12.0, 12.0, 12.0);

				if (rendertype != PickableObjectRenderType.Picking)
				{
					GL.Color3(0.25, 0.5, 1.0);
					StockObjects.DownArrow.Render();

					if (rendertype == PickableObjectRenderType.Selected)
					{
						StockObjects.SimpleAxisMarker.Render();
						GL.Color3(1.0, 0.5, 0.0);
					}
					else
						GL.Color3(0.0, 0.0, 0.0);

					GL.LineWidth(4.0f);
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
					StockObjects.DownArrow.Render();
				}
				else
				{
					GL.Color3(PickColor);
					StockObjects.DownArrow.Render();
				}

				GL.PopMatrix();
				GL.PopAttrib();
			}
		}
	}
}
