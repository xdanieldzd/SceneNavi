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
    public class Collision : Generic, IStoreable
    {
        public Vector3d AbsoluteMinimum { get; private set; }
        public Vector3d AbsoluteMaximum { get; private set; }
        public ushort VertexCount { get; private set; }
        public uint VertexArrayOffset { get; private set; }
        public ushort PolygonCount { get; private set; }
        public uint PolygonArrayOffset { get; private set; }
        public uint PolygonTypeOffset { get; private set; }
        public uint CameraDataOffset { get; private set; }
        public ushort WaterboxCount { get; private set; }
        public uint WaterboxOffset { get; private set; }

        public List<Vector3d> Vertices { get; private set; }
        public List<Polygon> Polygons { get; private set; }
        public List<PolygonType> PolygonTypes { get; private set; }
        public List<Waterbox> Waterboxes { get; private set; }

        public Collision(Generic basecmd)
            : base(basecmd)
        {
            uint adr = (uint)(GetAddressGeneric() & 0xFFFFFF);

            byte[] segdata = (byte[])ROM.SegmentMapping[(byte)(GetAddressGeneric() >> 24)];
            if (segdata == null) return;

            /* Read header */
            AbsoluteMinimum = new Vector3d(
                Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)adr)),
                Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)adr + 0x2)),
                Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)adr + 0x4)));
            AbsoluteMaximum = new Vector3d(
                Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)adr + 0x6)),
                Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)adr + 0x8)),
                Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)adr + 0xA)));

            VertexCount = Endian.SwapUInt16(BitConverter.ToUInt16(segdata, (int)adr + 0xC));
            VertexArrayOffset = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)adr + 0x10));
            PolygonCount = Endian.SwapUInt16(BitConverter.ToUInt16(segdata, (int)adr + 0x14));
            PolygonArrayOffset = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)adr + 0x18));
            PolygonTypeOffset = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)adr + 0x1C));
            CameraDataOffset = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)adr + 0x20));
            WaterboxCount = Endian.SwapUInt16(BitConverter.ToUInt16(segdata, (int)adr + 0x24));
            WaterboxOffset = Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)adr + 0x28));

            /* Read vertices */
            byte[] vertsegdata = (byte[])ROM.SegmentMapping[(byte)(VertexArrayOffset >> 24)];
            if (vertsegdata != null)
            {
                Vertices = new List<Vector3d>();
                for (int i = 0; i < VertexCount; i++)
                {
                    Vertices.Add(new Vector3d(
                        Endian.SwapInt16(BitConverter.ToInt16(vertsegdata, (int)(VertexArrayOffset & 0xFFFFFF) + (i * 6))),
                        Endian.SwapInt16(BitConverter.ToInt16(vertsegdata, (int)(VertexArrayOffset & 0xFFFFFF) + (i * 6) + 2)),
                        Endian.SwapInt16(BitConverter.ToInt16(vertsegdata, (int)(VertexArrayOffset & 0xFFFFFF) + (i * 6) + 4))));
                }
            }

            /* Read polygons */
            Polygons = new List<Polygon>();
            for (int i = 0; i < PolygonCount; i++)
            {
                Polygons.Add(new Polygon(ROM, (uint)(PolygonArrayOffset + (i * 0x10)), i, this));
            }

            /* Read polygon types */
            PolygonTypes = new List<PolygonType>();
            int ptlen = (int)(PolygonArrayOffset - PolygonTypeOffset);                      /* Official maps */
            if (ptlen <= 0) ptlen = (int)(WaterboxOffset - PolygonTypeOffset);              /* SO imports */
            if (ptlen <= 0) ptlen = (int)(this.GetAddressGeneric() - PolygonTypeOffset);    /* HT imports */

            if (ptlen > 0)
            {
                for (uint i = PolygonTypeOffset, j = 0; i < (uint)(PolygonTypeOffset + (ptlen & 0xFFFFFF)); i += 8, j++)
                {
                    PolygonTypes.Add(new PolygonType(ROM, i, (int)j));
                }
            }

            /* Read camera data */
            //

            /* Read waterboxes */
            Waterboxes = new List<Waterbox>();
            for (int i = 0; i < WaterboxCount; i++)
            {
                Waterboxes.Add(new Waterbox(ROM, (uint)(WaterboxOffset + (i * 0x10)), i, this));
            }
        }

        public void Store(byte[] databuf, int baseadr)
        {
            foreach (HeaderCommands.Collision.Polygon poly in this.Polygons)
            {
                /* Polygon type */
                byte[] bytes = BitConverter.GetBytes(Endian.SwapUInt16(poly.PolygonType));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (poly.Address & 0xFFFFFF)), bytes.Length);

                /* Normal stuff etc. */
                bytes = BitConverter.GetBytes(Endian.SwapInt16(poly.NormalXDirection));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (poly.Address & 0xFFFFFF) + 0x8), bytes.Length);
                bytes = BitConverter.GetBytes(Endian.SwapInt16(poly.NormalYDirection));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (poly.Address & 0xFFFFFF) + 0xA), bytes.Length);
                bytes = BitConverter.GetBytes(Endian.SwapInt16(poly.NormalZDirection));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (poly.Address & 0xFFFFFF) + 0xC), bytes.Length);
                bytes = BitConverter.GetBytes(Endian.SwapInt16(poly.CollisionPlaneDistFromOrigin));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (poly.Address & 0xFFFFFF) + 0xE), bytes.Length);

                /* TODO vertex IDs???  even allow editing of those?? */
            }

            foreach (HeaderCommands.Collision.PolygonType ptype in this.PolygonTypes)
            {
                /* Just get & save raw data; should be enough */
                byte[] bytes = BitConverter.GetBytes(Endian.SwapUInt64(ptype.Raw));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (ptype.Address & 0xFFFFFF)), bytes.Length);
            }

            foreach (HeaderCommands.Collision.Waterbox wb in this.Waterboxes)
            {
                /* Position */
                byte[] bytes = BitConverter.GetBytes(Endian.SwapInt16(Convert.ToInt16(wb.Position.X)));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (wb.Address & 0xFFFFFF) + 0x0), bytes.Length);
                bytes = BitConverter.GetBytes(Endian.SwapInt16(Convert.ToInt16(wb.Position.Y)));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (wb.Address & 0xFFFFFF) + 0x2), bytes.Length);
                bytes = BitConverter.GetBytes(Endian.SwapInt16(Convert.ToInt16(wb.Position.Z)));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (wb.Address & 0xFFFFFF) + 0x4), bytes.Length);

                /* Size */
                bytes = BitConverter.GetBytes(Endian.SwapInt16(Convert.ToInt16(wb.SizeXZ.X)));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (wb.Address & 0xFFFFFF) + 0x6), bytes.Length);
                bytes = BitConverter.GetBytes(Endian.SwapInt16(Convert.ToInt16(wb.SizeXZ.Y)));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (wb.Address & 0xFFFFFF) + 0x8), bytes.Length);

                /* Property thingy (room number, whatever else) */
                bytes = BitConverter.GetBytes(Endian.SwapUInt32(wb.RoomPropRaw));
                Buffer.BlockCopy(bytes, 0, databuf, (int)(baseadr + (wb.Address & 0xFFFFFF) + 0xC), bytes.Length);
            }
        }

        public class PolygonType
        {
            public class GroundType
            {
                public byte Value { get; private set; }
                public string Description { get; private set; }
                public System.Drawing.Color RenderColor { get; private set; }

                public GroundType(byte val, string desc, int col)
                {
                    Value = val;
                    Description = desc;
                    RenderColor = System.Drawing.Color.FromArgb(col);
                }
            }

            public static List<GroundType> GroundTypes = new List<GroundType>()
            {
                new GroundType(0, "Dirt", Color4.RosyBrown.ToArgb()),
                new GroundType(1, "Sand", Color4.SandyBrown.ToArgb()),
                new GroundType(2, "Stone", Color4.DarkSlateGray.ToArgb()),
                new GroundType(3, "Wet stone", Color4.SlateBlue.ToArgb()),
                new GroundType(4, "Shallow water", Color4.Blue.ToArgb()),
                new GroundType(5, "Not-as-shallow water", Color4.Blue.ToArgb()),
                new GroundType(6, "Underbrush/grass", Color4.ForestGreen.ToArgb()),
                new GroundType(7, "Lava/goo", Color4.DarkRed.ToArgb()),
                new GroundType(8, "Earth/dirt", Color4.ForestGreen.ToArgb()),
                new GroundType(9, "Wooden plank", Color4.SandyBrown.ToArgb()),
                new GroundType(0xA, "Packed earth/wood", Color4.SandyBrown.ToArgb()),
                new GroundType(0xB, "Earth/dirt",  Color4.Purple.ToArgb()), //fixme?
                new GroundType(0xC, "Ceramic/ice", Color4.SlateBlue.ToArgb()),
                new GroundType(0xD, "Loose earth/carpet", Color4.LightGoldenrodYellow.ToArgb()),
                new GroundType(0xE, "Earth/dirt",  Color4.Purple.ToArgb()), //fixme?
                new GroundType(0xF, "Earth/dirt",  Color4.Purple.ToArgb()), //fixme?
            };

            //TODO
            [Flags]
            public enum Climbing
            {
                NoClimbing = 0x00,
                Ladder = 0x04,
                WholeSurface = 0x08
            }

            public int Number { get; private set; }
            public uint Address { get; private set; }
            public ulong Raw { get; set; }

            public ulong ExitNumber
            {
                get { return (ulong)((Raw & 0x00000F0000000000) >> 40); }
                set { Raw = ((Raw & 0xFFFFF0FFFFFFFFFF) | ((ulong)(value & 0xF) << 40)); }
            }

            public ulong ClimbingCrawlingFlags
            {
                get { return (ulong)((Raw & 0x00F0000000000000) >> 52); }
                set { Raw = ((Raw & 0xFF0FFFFFFFFFFFFF) | ((ulong)(value & 0xF) << 52)); }
            }

            public ulong DamageSurfaceFlags
            {
                get { return (ulong)((Raw & 0x000FF00000000000) >> 44); }
                set { Raw = ((Raw & 0xFFF00FFFFFFFFFFF) | ((ulong)(value & 0xFF) << 44)); }
            }

            public bool IsHookshotable
            {
                get { return ((Raw & 0x0000000000020000) != 0); }
                set { if (value) { Raw |= 0x20000; } else { Raw &= ~((ulong)0x20000); } }
            }

            public uint EchoRange
            {
                get { return (uint)((Raw & 0x000000000000F000) >> 12); }
                set { Raw = ((Raw & 0xFFFFFFFFFFFF0FFF) | ((ulong)(value & 0xF) << 12)); }
            }

            public uint EnvNumber
            {
                get { return (uint)((Raw & 0x0000000000000F00) >> 8); }
                set { Raw = ((Raw & 0xFFFFFFFFFFFFF0FF) | ((ulong)(value & 0xF) << 8)); }
            }

            public bool IsSteep
            {
                get { return ((Raw & 0x0000000000000030) == 0x10); }
                set { if (value) { Raw |= 0x10; } else { Raw &= ~((ulong)0x10); } }
            }

            public uint TerrainType
            {
                get { return (uint)((Raw & 0x00000000000000F0) >> 4); }
                set { Raw = ((Raw & 0xFFFFFFFFFFFFFF0F) | ((ulong)(value & 0xF) << 4)); }
            }

            public uint GroundTypeID
            {
                get { return (uint)(Raw & 0x000000000000000F); }
                set { Raw = ((Raw & 0xFFFFFFFFFFFFFFF0) | (ulong)((value & 0xF))); }
            }

            public System.Drawing.Color RenderColor
            {
                get
                {
                    int rgb = Color4.White.ToArgb();

                    switch (GroundTypeID)
                    {
                        /* Dirt */
                        case 0: rgb = Color4.RosyBrown.ToArgb(); break;
                        /* Sand / wood / earth */
                        case 1:
                        case 9:
                        case 0xA: rgb = Color4.SandyBrown.ToArgb(); break;
                        /* Stone */
                        case 2: rgb = Color4.DarkSlateGray.ToArgb(); break;
                        /* Wet stone */
                        case 3: rgb = Color4.SlateBlue.ToArgb(); break;
                        /* Water */
                        case 4:
                        case 5: rgb = Color4.Blue.ToArgb(); break;
                        /* Grass / other ground */
                        case 6:
                        case 8: rgb = Color4.ForestGreen.ToArgb(); break;
                        /* Lava / goo */
                        case 7: rgb = Color4.DarkRed.ToArgb(); break;
                        /* Ice / ceramic */
                        case 0xC: rgb = Color4.SlateBlue.ToArgb(); break;
                        /* Loose earth / carpet */
                        case 0xD: rgb = Color4.LightGoldenrodYellow.ToArgb(); break;

                        /* ??? unknown */
                        case 0xB:
                        case 0xE:
                        case 0xF: rgb = Color4.Purple.ToArgb(); break;
                    }

                    return System.Drawing.Color.FromArgb((rgb & 0xFFFFFF) | (128 << 24));
                }
            }

            public string Description
            {
                get
                {
                    if (ROM == null)
                        return "(None)";
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("#{0}: {1}", Number, GroundTypes.FirstOrDefault(x => x.Value == GroundTypeID).Description);
                        if (ExitNumber != 0) sb.AppendFormat(", triggers exit #{0}", ExitNumber);
                        //more?
                        return sb.ToString();
                    }
                }
            }

            public bool IsDummy { get { return (ROM == null); } }

            ROMHandler.ROMHandler ROM;

            public PolygonType()
            {
                Number = -1;
            }

            public PolygonType(ROMHandler.ROMHandler rom, uint adr, int number)
            {
                ROM = rom;
                Address = adr;
                Number = number;

                byte[] segdata = (byte[])ROM.SegmentMapping[(byte)(adr >> 24)];
                if (segdata == null) return;

                Raw = Endian.SwapUInt64(BitConverter.ToUInt64(segdata, (int)(adr & 0xFFFFFF)));
            }
        }

        public class Polygon : IPickableObject
        {
            public int Number { get; private set; }
            public uint Address { get; private set; }

            public ushort PolygonType { get; set; }
            public ushort[] VertexIDs { get; set; }
            public Vector3d[] Vertices { get; set; }
            public short NormalXDirection { get; set; }
            public short NormalYDirection { get; set; }
            public short NormalZDirection { get; set; }
            public short CollisionPlaneDistFromOrigin { get; set; }

            [Browsable(false)]
            public Vector3d Position { get { return Vector3d.Zero; } set { } }

            [Browsable(false)]
            public System.Drawing.Color PickColor { get { return System.Drawing.Color.FromArgb(this.GetHashCode() & 0xFFFFFF | (0xFF << 24)); } }

            [Browsable(false)]
            public bool IsMoveable { get { return false; } }

            public string Description
            {
                get
                {
                    if (ROM == null)
                        return "(None)";
                    else
                        return string.Format("#{0}: Vertices {1} / {2} / {3}, type #{4}", Number, VertexIDs[0], VertexIDs[1], VertexIDs[2], PolygonType);
                }
            }

            public bool IsDummy { get { return (ROM == null); } }

            ROMHandler.ROMHandler ROM;
            Collision ParentCollisionHeader;

            public Polygon() { }

            public Polygon(ROMHandler.ROMHandler rom, uint adr, int number, Collision colheader)
            {
                ROM = rom;
                Address = adr;
                Number = number;
                ParentCollisionHeader = colheader;

                byte[] segdata = (byte[])ROM.SegmentMapping[(byte)(adr >> 24)];
                if (segdata == null) return;

                PolygonType = Endian.SwapUInt16(BitConverter.ToUInt16(segdata, (int)(adr & 0xFFFFFF)));
                NormalXDirection = Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)(adr & 0xFFFFFF) + 0x8));
                NormalYDirection = Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)(adr & 0xFFFFFF) + 0xA));
                NormalZDirection = Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)(adr & 0xFFFFFF) + 0xC));
                CollisionPlaneDistFromOrigin = Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)(adr & 0xFFFFFF) + 0xE));

                /* Read vertex IDs & fetch vertices */
                VertexIDs = new ushort[3];
                Vertices = new Vector3d[3];
                for (int i = 0; i < 3; i++)
                {
                    ushort vidx = (ushort)(Endian.SwapUInt16(BitConverter.ToUInt16(segdata, (int)(adr & 0xFFFFFF) + 0x2 + (i * 2))) & 0xFFF);
                    VertexIDs[i] = vidx;
                    Vertices[i] = ParentCollisionHeader.Vertices[vidx];
                }
            }

            public void Render(PickableObjectRenderType rendertype)
            {
                if (rendertype == PickableObjectRenderType.Picking)
                {
                    GL.Color3(PickColor);
                    GL.Begin(BeginMode.Triangles);
                    foreach (Vector3d v in Vertices) GL.Vertex3(v);
                    GL.End();
                }
                else
                {
                    if (rendertype != PickableObjectRenderType.NoColor)
                        GL.Color4(ParentCollisionHeader.PolygonTypes[PolygonType].RenderColor);

                    foreach (Vector3d v in Vertices) GL.Vertex3(v);
                }
            }
        }

        public class Waterbox : IPickableObject
        {
            public int Number { get; private set; }
            public uint Address { get; private set; }

            public Vector3d Position { get; set; }
            public Vector2d SizeXZ { get; set; }

            public uint RoomPropRaw { get; private set; }

            public ushort RoomNumber
            {
                get
                {
                    return (ushort)(RoomPropRaw >> 13);
                }
                set
                {
                    RoomPropRaw = (uint)(Properties | (value << 13));
                }
            }

            public ushort Properties
            {
                get
                {
                    return (ushort)(RoomPropRaw & 0x1FFF);
                }
                set
                {
                    RoomPropRaw = (uint)((RoomNumber << 13) | value);
                }
            }

            [Browsable(false)]
            public System.Drawing.Color PickColor { get { return System.Drawing.Color.FromArgb(this.GetHashCode() & 0xFFFFFF | (0xFF << 24)); } }

            [Browsable(false)]
            public bool IsMoveable { get { return true; } }

            public string Description
            {
                get
                {
                    if (ROM == null)
                        return "(None)";
                    else
                        return string.Format("Waterbox #{0}: X: {1}, Y: {2}, Z: {3}", (Number + 1), Position.X, Position.Y, Position.Z);
                }
            }

            public bool IsDummy { get { return (ROM == null); } }

            ROMHandler.ROMHandler ROM;
            Collision ParentCollisionHeader;

            public Waterbox() { }

            public Waterbox(ROMHandler.ROMHandler rom, uint adr, int number, Collision colheader)
            {
                ROM = rom;
                Address = adr;
                Number = number;
                ParentCollisionHeader = colheader;

                byte[] segdata = (byte[])ROM.SegmentMapping[(byte)(adr >> 24)];
                if (segdata == null) return;

                Position = new Vector3d(
                    (double)Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)(adr & 0xFFFFFF))),
                    (double)Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)((adr & 0xFFFFFF) + 2))),
                    (double)Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)((adr & 0xFFFFFF) + 4))));

                SizeXZ = new Vector2d(
                    (double)Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)((adr & 0xFFFFFF) + 6))),
                    (double)Endian.SwapInt16(BitConverter.ToInt16(segdata, (int)((adr & 0xFFFFFF) + 8))));

                RoomPropRaw = (Endian.SwapUInt32(BitConverter.ToUInt32(segdata, (int)((adr & 0xFFFFFF) + 12))) & 0xFFFFFF);
            }

            public void Render(PickableObjectRenderType rendertype)
            {
                if (rendertype == PickableObjectRenderType.Picking)
                {
                    GL.Color3(PickColor);
                    GL.Begin(BeginMode.Quads);
                    RenderVertices();
                    GL.End();
                }
                else
                    RenderVertices();
            }

            private void RenderVertices()
            {
                GL.Vertex3(Position.X, Position.Y, Position.Z);
                GL.Vertex3(Position.X, Position.Y, Position.Z + SizeXZ.Y);
                GL.Vertex3(Position.X + SizeXZ.X, Position.Y, Position.Z + SizeXZ.Y);
                GL.Vertex3(Position.X + SizeXZ.X, Position.Y, Position.Z);
            }
        }
    }
}
