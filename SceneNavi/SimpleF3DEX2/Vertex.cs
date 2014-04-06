using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.SimpleF3DEX2
{
    internal class Vertex : HeaderCommands.IPickableObject, HeaderCommands.IStoreable
    {
        ROMHandler.ROMHandler ROM;

        [Browsable(false)]
        public System.Drawing.Color PickColor { get { return System.Drawing.Color.FromArgb(this.GetHashCode() & 0xFFFFFF | (0xFF << 24)); } }

        [Browsable(false)]
        public bool IsMoveable { get { return false; } }

        public Vector3d Position { get; set; }
        public Vector2d TexCoord { get; set; }
        public byte[] Colors { get; set; }
        public sbyte[] Normals { get; set; }

        public uint Address { get; set; }

        public Vertex(ROMHandler.ROMHandler rom, byte[] raw, uint adr, Matrix4d mtx)
        {
            ROM = rom;

            Address = adr;
            adr &= 0xFFFFFF;

            Position = new Vector3d(
                (double)Endian.SwapInt16(BitConverter.ToInt16(raw, (int)adr)),
                (double)Endian.SwapInt16(BitConverter.ToInt16(raw, (int)(adr + 2))),
                (double)Endian.SwapInt16(BitConverter.ToInt16(raw, (int)(adr + 4))));

            Position = Vector3d.Transform(Position, mtx);

            TexCoord = new Vector2d(
                (float)(Endian.SwapInt16(BitConverter.ToInt16(raw, (int)(adr + 8)))) * General.Fixed2Float[5],
                (float)(Endian.SwapInt16(BitConverter.ToInt16(raw, (int)(adr + 10)))) * General.Fixed2Float[5]);

            TexCoord.Normalize();

            Colors = new byte[] { raw[adr + 12], raw[adr + 13], raw[adr + 14], raw[adr + 15] };
            Normals = new sbyte[] { (sbyte)raw[adr + 12], (sbyte)raw[adr + 13], (sbyte)raw[adr + 14] };
        }

        public void Store(byte[] databuf, int baseadr)
        {
            // KLUDGE! Write to ROM HERE, write to local room data for rendering in MainForm

            // (Colors only!)
            databuf[(int)(baseadr + (Address & 0xFFFFFF)) + 12] = Colors[0];
            databuf[(int)(baseadr + (Address & 0xFFFFFF)) + 13] = Colors[1];
            databuf[(int)(baseadr + (Address & 0xFFFFFF)) + 14] = Colors[2];
            databuf[(int)(baseadr + (Address & 0xFFFFFF)) + 15] = Colors[3];
        }

        public void Render(HeaderCommands.PickableObjectRenderType rendertype)
        {
            if (rendertype == HeaderCommands.PickableObjectRenderType.Picking)
            {
                GL.PushAttrib(AttribMask.AllAttribBits);
                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.Lighting);
                GL.Disable((EnableCap)All.FragmentProgram);
                GL.Disable(EnableCap.CullFace);

                GL.DepthRange(0.0, 0.999);
                GL.PointSize(50.0f);
                GL.Color3(PickColor);
                GL.Begin(BeginMode.Points);
                GL.Vertex3(Position);
                GL.End();

                GL.PopAttrib();
            }
        }
    }
}
