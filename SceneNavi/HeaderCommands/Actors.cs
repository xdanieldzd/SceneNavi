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
	public class Actors : Generic, IStoreable
	{
		public List<Entry> ActorList { get; set; }

		public Actors(Generic basecmd)
			: base(basecmd)
		{
			ActorList = new List<Entry>();
			for (int i = 0; i < GetCountGeneric(); i++) ActorList.Add(new Entry(ROM, (uint)(GetAddressGeneric() + i * 16), (i + 1),
				(Command == HeaderLoader.CommandTypeIDs.Spawns), (Command == HeaderLoader.CommandTypeIDs.Transitions)));
		}

		public void Store(byte[] databuf, int baseadr)
		{
			foreach (HeaderCommands.Actors.Entry ae in this.ActorList)
			{
				System.Buffer.BlockCopy(ae.RawData, 0, databuf, (int)(baseadr + (ae.Address & 0xFFFFFF)), ae.RawData.Length);
			}
		}

		public class Entry : IPickableObject
		{
			public uint Address { get; set; }
			public byte[] RawData { get; set; }

			public XMLActorDefinitionReader.Definition Definition { get; private set; }
			public string InternalName { get; private set; }

			public bool IsSpawnPoint { get; private set; }
			public bool IsTransitionActor { get; private set; }

			int NumberInList;

			public ushort GetActorNumber
			{
				get
				{
					if (Definition != null)
					{
						object num = XMLActorDefinitionReader.GetValueFromActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.ActorNumber), this);
						if (num != null) return Convert.ToUInt16(num);
					}
					return ushort.MaxValue;
				}
			}

			public string Name
			{
				get
				{
					if (ROM == null) return "(None)";

					string name = (string)ROM.XMLActorNames.Names[GetActorNumber];
					return (name != null ? name : "Unknown actor");
				}
			}

			public string Description
			{
				get
				{
					if (ROM == null)
						return "(None)";
					else
						return string.Format("Actor #{0}; {1}", NumberInList, Name);
				}
			}

			public OpenTK.Vector3d Position
			{
				get
				{
					if (Definition == null) return OpenTK.Vector3d.Zero;
					OpenTK.Vector3d p = new OpenTK.Vector3d();
					object px = XMLActorDefinitionReader.GetValueFromActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.PositionX), this);
					object py = XMLActorDefinitionReader.GetValueFromActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.PositionY), this);
					object pz = XMLActorDefinitionReader.GetValueFromActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.PositionZ), this);
					if (px != null) p.X = Convert.ToDouble(px);
					if (py != null) p.Y = Convert.ToDouble(py);
					if (pz != null) p.Z = Convert.ToDouble(pz);
					return p;
				}

				set
				{
					if (Definition == null) return;
					XMLActorDefinitionReader.SetValueInActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.PositionX), this, Convert.ToInt16(value.X));
					XMLActorDefinitionReader.SetValueInActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.PositionY), this, Convert.ToInt16(value.Y));
					XMLActorDefinitionReader.SetValueInActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.PositionZ), this, Convert.ToInt16(value.Z));
				}
			}

			public OpenTK.Vector3d Rotation
			{
				get
				{
					if (Definition == null) return OpenTK.Vector3d.Zero;

					OpenTK.Vector3d r = new OpenTK.Vector3d();
					object rx = XMLActorDefinitionReader.GetValueFromActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.RotationX), this);
					object ry = XMLActorDefinitionReader.GetValueFromActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.RotationY), this);
					object rz = XMLActorDefinitionReader.GetValueFromActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.RotationZ), this);
					if (rx != null) r.X = Convert.ToDouble(rx);
					if (ry != null) r.Y = Convert.ToDouble(ry);
					if (rz != null) r.Z = Convert.ToDouble(rz);
					return r;
				}

				set
				{
					if (Definition == null) return;
					XMLActorDefinitionReader.SetValueInActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.RotationX), this, (short)value.X);
					XMLActorDefinitionReader.SetValueInActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.RotationY), this, (short)value.Y);
					XMLActorDefinitionReader.SetValueInActor(Definition.Items.Find(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.RotationZ), this, (short)value.Z);
				}
			}

			[Browsable(false)]
			public System.Drawing.Color PickColor { get { return System.Drawing.Color.FromArgb(this.GetHashCode() & 0xFFFFFF | (0xFF << 24)); } }

			[Browsable(false)]
			public bool IsMoveable { get { return true; } }

			ROMHandler.ROMHandler ROM;

			public Entry() { }

			public Entry(ROMHandler.ROMHandler rom, uint adr, int no, bool isspawn, bool istrans)
			{
				ROM = rom;
				Address = adr;
				NumberInList = no;
				IsSpawnPoint = isspawn;
				IsTransitionActor = istrans;

				/* Load raw data */
				RawData = new byte[16];
				byte[] segdata = (byte[])ROM.SegmentMapping[(byte)(adr >> 24)];
				if (segdata == null) return;
				System.Buffer.BlockCopy(segdata, (int)(adr & 0xFFFFFF), RawData, 0, RawData.Length);

				/* Find definition, internal name */
				RefreshVariables();
			}

			public void RefreshVariables()
			{
				int numofs = (IsTransitionActor ? 4 : 0);
				ushort actnum = Endian.SwapUInt16(BitConverter.ToUInt16(RawData, numofs));

				Definition = ROM.XMLActorDefReader.Definitions.Find(x => x.Number == actnum);
				if (Definition == null)
				{
					XMLActorDefinitionReader.Definition.DefaultTypes flagfind = XMLActorDefinitionReader.Definition.DefaultTypes.RoomActor;
					if (IsTransitionActor) flagfind = XMLActorDefinitionReader.Definition.DefaultTypes.TransitionActor;
					else if (IsSpawnPoint) flagfind = XMLActorDefinitionReader.Definition.DefaultTypes.SpawnPoint;
					Definition = ROM.XMLActorDefReader.Definitions.FirstOrDefault(x => x.IsDefault.HasFlag(flagfind));
				}

				if (actnum < ROM.Actors.Count)
					InternalName = ROM.Actors[actnum].Name;
				else
					InternalName = string.Empty;
			}

			public void Render(PickableObjectRenderType rendertype)
			{
				GL.PushAttrib(AttribMask.AllAttribBits);

				GL.PushMatrix();
				GL.Translate(Position);
				GL.Rotate(Rotation.Z / 182.0444444, 0.0, 0.0, 1.0);
				GL.Rotate(Rotation.Y / 182.0444444, 0.0, 1.0, 0.0);
				GL.Rotate(Rotation.X / 182.0444444, 1.0, 0.0, 0.0);

				GL.Scale(12.0, 12.0, 12.0);

				/* Determine render mode */
				if (rendertype == PickableObjectRenderType.Picking)
				{
					/* Picking, so set color to PickColor and render the PickModel */
					GL.Color3(PickColor);
					Definition.PickModel.Render();
				}
				else
				{
					/* Not picking, so first render the DisplayModel */
					Definition.DisplayModel.Render();

					GL.LineWidth(4.0f);

					/* Now determine outline color */
					if (rendertype == PickableObjectRenderType.Normal)
					{
						/* Outline depends on actor type */
						if (IsSpawnPoint) GL.Color4(Color4.Green);
						else if (IsTransitionActor) GL.Color4(Color4.Purple);
						else GL.Color4(Color4.Black);
					}
					else
					{
						/* Orange outline */
						GL.Color3(1.0, 0.5, 0.0);
					}

					/* Set line mode, then render PickModel (as that's more likely to not have colors baked in) */
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
					Definition.PickModel.Render();

					/* When rendering selected actor, make sure to render axis marker too */
					if (rendertype == PickableObjectRenderType.Selected)
					{
						/* And if a FrontOffset is given in definition, rotate before rendering the marker */
						if (Definition.FrontOffset != 0.0) GL.Rotate(Definition.FrontOffset, 0.0, 1.0, 0.0);

						StockObjects.AxisMarker.Render();
					}
				}

				GL.PopMatrix();
				GL.PopAttrib();
			}
		}
	}
}
