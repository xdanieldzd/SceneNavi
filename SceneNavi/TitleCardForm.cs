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
        const int TitleCardWidth = 144;
        int TitleCardHeight = 0;

        ROMHandler.ROMHandler ROM;
        ROMHandler.SceneTableEntry Scene;

        Bitmap output;
        Rectangle outputRect;

        public TitleCardForm(ROMHandler.ROMHandler rom, ROMHandler.SceneTableEntry ste)
        {
            InitializeComponent();

            ROM = rom;
            Scene = ste;

            PrepareFileFilters();
            ReadImageFromROM();

            //this.Text = string.Format("{0:X} -> {1:X}", ste.LabelStartAddress, ste.LabelEndAddress);
        }

        private void PrepareFileFilters()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;
            string filter = string.Empty;
            int pngidx = 0;

            int filtercnt = 1;
            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                if (codecName.Contains("PNG")) pngidx = filtercnt;
                filter = String.Format("{0}{1}{2} ({3})|{3}", filter, sep, codecName, c.FilenameExtension);
                sep = "|";
                filtercnt++;
            }

            filter = String.Format("{0}{1}{2} ({3})|{3}", filter, sep, "All Files", "*.*");

            ofdImage.Filter = sfdImage.Filter = filter;
            ofdImage.FilterIndex = sfdImage.FilterIndex = pngidx;
        }

        private void ReadImageFromROM()
        {
            TitleCardHeight = (int)((Scene.LabelEndAddress - Scene.LabelStartAddress) / TitleCardWidth);

            byte[] texbuf = new byte[TitleCardWidth * TitleCardHeight * 4];
            NImage.NImageUtil.IA8(TitleCardWidth, TitleCardHeight, (TitleCardWidth / 8), ROM.Data, (int)Scene.LabelStartAddress, ref texbuf);

            for (int i = 0; i < texbuf.Length; i += 4)
            {
                byte R = texbuf[i];
                byte G = texbuf[i + 1];
                byte B = texbuf[i + 2];
                byte A = texbuf[i + 3];

                texbuf[i] = B;
                texbuf[i + 1] = G;
                texbuf[i + 2] = R;
                texbuf[i + 3] = A;
            }

            output = new Bitmap(TitleCardWidth, TitleCardHeight, PixelFormat.Format32bppArgb);
            outputRect = new Rectangle(0, 0, output.Width, output.Height);
            BitmapData bmpData = output.LockBits(outputRect, ImageLockMode.ReadWrite, output.PixelFormat);
            IntPtr ptr = bmpData.Scan0;

            System.Runtime.InteropServices.Marshal.Copy(texbuf, 0, ptr, texbuf.Length);
            output.UnlockBits(bmpData);

            pbTitleCard.ClientSize = new Size(TitleCardWidth * 2, TitleCardHeight * 2);
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
                output.Save(sfdImage.FileName);
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

            if (import.Width != TitleCardWidth || import.Height != TitleCardHeight)
            {
                MessageBox.Show("Selected image has wrong size; image cannot be used.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            uint ofs = Scene.LabelStartAddress;

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
                    ROM.Data[ofs] = packed;

                    ofs++;
                }
            }

            return import;
        }
    }
}
