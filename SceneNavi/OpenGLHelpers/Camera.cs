using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.OpenGLHelpers
{
    public class Camera
    {
        public double Sensitivity = 0.5;
        public double CameraCoeff = 0.05;
        public Vector3d Pos, Rot;
        public Vector2d MouseCoord, MouseCoordOld;
        public MouseButtons ButtonsDown;

        public Camera()
        {
            Reset();
        }

        public void Reset()
        {
            Pos = new Vector3d(0.0, 0.0, -15.0);
            Rot = new Vector3d(0.0, 0.0, 0.0);
        }

        public void MouseCenter(Vector2d NewMouseCoord)
        {
            MouseCoordOld = MouseCoord;
            MouseCoord = NewMouseCoord;
        }

        public void MouseMove(Vector2d NewMouseCoord)
        {
            bool Changed = false;
            double Dx = 0.0, Dy = 0.0;

            if (NewMouseCoord.X != MouseCoord.X)
            {
                Dx = (NewMouseCoord.X - MouseCoord.X) * Sensitivity;
                Changed = true;
            }
            if (NewMouseCoord.Y != MouseCoord.Y)
            {
                Dy = (NewMouseCoord.Y - MouseCoord.Y) * Sensitivity;
                Changed = true;
            }

            if (Changed)
            {
                if (MouseCoord.X < NewMouseCoord.X)
                {
                    Rot.Y += (NewMouseCoord.X - MouseCoord.X) * 0.225;
                    if (Rot.Y > 360.0) Rot.Y = 0.0;
                }
                else
                {
                    Rot.Y -= (MouseCoord.X - NewMouseCoord.X) * 0.225;
                    if (Rot.Y < -360.0) Rot.Y = 0.0;
                }

                if (MouseCoord.Y < NewMouseCoord.Y)
                {
                    if (Rot.X >= 90.0)
                        Rot.X = 90.0;
                    else
                        Rot.X += (Dy / Sensitivity) * 0.225;
                }
                else
                {
                    if (Rot.X <= -90.0)
                        Rot.X = -90.0;
                    else
                        Rot.X += (Dy / Sensitivity) * 0.225;
                }

                MouseCoordOld = MouseCoord;
                MouseCoord = NewMouseCoord;
            }
        }

        public void KeyUpdate(bool[] KeysDown)
        {
            double RotYRad = (Rot.Y / 180.0 * Math.PI);
            double RotXRad = (Rot.X / 180.0 * Math.PI);

            double Modifier = 2.0;
            if (KeysDown[(char)Keys.Space]) Modifier = 10.0;
            else if (KeysDown[(char)Keys.ShiftKey]) Modifier = 0.5;

            if (KeysDown[(char)Keys.W])
            {
                if (Rot.X >= 90.0 || Rot.X <= -90.0)
                {
                    Pos.Y += Math.Sin(RotXRad) * CameraCoeff * 2.0 * Modifier;
                }
                else
                {
                    Pos.X -= Math.Sin(RotYRad) * CameraCoeff * 2.0 * Modifier;
                    Pos.Z += Math.Cos(RotYRad) * CameraCoeff * 2.0 * Modifier;
                    Pos.Y += Math.Sin(RotXRad) * CameraCoeff * 2.0 * Modifier;
                }
            }

            if (KeysDown[(char)Keys.S])
            {
                if (Rot.X >= 90.0 || Rot.X <= -90.0)
                {
                    Pos.Y -= Math.Sin(RotXRad) * CameraCoeff * 2.0 * Modifier;
                }
                else
                {
                    Pos.X += Math.Sin(RotYRad) * CameraCoeff * 2.0 * Modifier;
                    Pos.Z -= Math.Cos(RotYRad) * CameraCoeff * 2.0 * Modifier;
                    Pos.Y -= Math.Sin(RotXRad) * CameraCoeff * 2.0 * Modifier;
                }
            }

            if (KeysDown[(char)Keys.A])
            {
                Pos.X += Math.Cos(RotYRad) * CameraCoeff * 2.0 * Modifier;
                Pos.Z += Math.Sin(RotYRad) * CameraCoeff * 2.0 * Modifier;
            }

            if (KeysDown[(char)Keys.D])
            {
                Pos.X -= Math.Cos(RotYRad) * CameraCoeff * 2.0 * Modifier;
                Pos.Z -= Math.Sin(RotYRad) * CameraCoeff * 2.0 * Modifier;
            }
        }

        public void Position()
        {
            GL.Rotate(Rot.X, 1.0, 0.0, 0.0);
            GL.Rotate(Rot.Y, 0.0, 1.0, 0.0);
            GL.Rotate(Rot.Z, 0.0, 0.0, 1.0);
            GL.Translate(Pos);
        }
    }
}
