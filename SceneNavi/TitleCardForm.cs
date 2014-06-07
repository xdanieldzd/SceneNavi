using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace SceneNavi
{
    public partial class TitleCardForm : Form
    {
        const int titleCardWidth = 144;
        int titleCardHeight = 0;

        ROMHandler.ROMHandler ROM;
        ROMHandler.SceneTableEntry Scene;

        Bitmap output;
        Rectangle outputRect;

        public TitleCardForm(ROMHandler.ROMHandler rom, ROMHandler.SceneTableEntry ste)
        {
            InitializeComponent();

            ROM = rom;
            Scene = ste;

            ofdImage.SetCommonImageFilter("png");
            sfdImage.SetCommonImageFilter("png");

            ReadImageFromROM();
        }

        private void ReadImageFromROM()
        {
            titleCardHeight = (int)((Scene.LabelEndAddress - Scene.LabelStartAddress) / titleCardWidth);

            byte[] textureBuffer = new byte[titleCardWidth * titleCardHeight * 4];
            SimpleF3DEX2.ImageHelper.IA8(titleCardWidth, titleCardHeight, (titleCardWidth / 8), ROM.Data, (int)Scene.LabelStartAddress, ref textureBuffer);
            textureBuffer.SwapRGBAToBGRA();

            output = new Bitmap(titleCardWidth, titleCardHeight, PixelFormat.Format32bppArgb);
            outputRect = new Rectangle(0, 0, output.Width, output.Height);
            BitmapData bmpData = output.LockBits(outputRect, ImageLockMode.ReadWrite, output.PixelFormat);
            IntPtr ptr = bmpData.Scan0;

            System.Runtime.InteropServices.Marshal.Copy(textureBuffer, 0, ptr, textureBuffer.Length);
            output.UnlockBits(bmpData);

            pbTitleCard.ClientSize = new Size(titleCardWidth * 2, titleCardHeight * 2);
        }

        private void pbTitleCard_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(output, new Rectangle(outputRect.X, outputRect.Y, outputRect.Width * 2, outputRect.Height * 2), outputRect, GraphicsUnit.Pixel);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (sfdImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Bitmap temp = new Bitmap(output);
                temp.Save(sfdImage.FileName);
                temp.Dispose();
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (ofdImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Bitmap import = ImportImage(ofdImage.FileName);
                if (import != null)
                {
                    output = import;
                    pbTitleCard.Invalidate();
                }
            }
        }

        private Bitmap ImportImage(string fn)
        {
            Bitmap import = new Bitmap(fn);

            if (import.Width != titleCardWidth || import.Height != titleCardHeight)
            {
                MessageBox.Show("Selected image has wrong size; image cannot be used.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            uint offset = Scene.LabelStartAddress;

            for (int y = 0; y < import.Height; y++)
            {
                for (int x = 0; x < import.Width; x++)
                {
                    Color pixelColor = import.GetPixel(x, y);
                    int intensity = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    Color newColor = Color.FromArgb(pixelColor.A, intensity, intensity, intensity);
                    import.SetPixel(x, y, newColor);

                    byte packed = (byte)(((byte)intensity).Scale(0, 0xFF, 0, 0xF) << 4);
                    packed |= (byte)(pixelColor.A.Scale(0, 0xFF, 0, 0xF));
                    ROM.Data[offset] = packed;

                    offset++;
                }
            }

            Bitmap temp = new Bitmap(import);
            import.Dispose();

            return temp;
        }
    }
}
