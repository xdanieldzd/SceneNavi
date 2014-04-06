using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SceneNavi
{
    public partial class ColorPickerDialog : Form
    {
        Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; UpdateForm(); }
        }

        static Color red = Color.FromArgb(255, 0, 0);
        static Color green = Color.FromArgb(0, 255, 0);
        static Color blue = Color.FromArgb(0, 0, 255);

        enum ColorComponents { Red, Green, Blue, Alpha };

        public ColorPickerDialog() : this(Color.White, false) { }

        public ColorPickerDialog(Color defaultColor) : this(defaultColor, false) { }

        public ColorPickerDialog(Color defaultColor, bool localize)
        {
            InitializeComponent();

            Color = defaultColor;

            if (localize)
            {
                btnOK.Text = GUIHelpers.LoadLocalizedString("user32.dll", 800, "OK");
                btnCancel.Text = GUIHelpers.LoadLocalizedString("user32.dll", 801, "Cancel");

                lblRed.Text = GUIHelpers.LoadLocalizedString("comdlg32.dll", 1049, "Red");
                lblGreen.Text = GUIHelpers.LoadLocalizedString("comdlg32.dll", 1042, "Green");
                lblBlue.Text = GUIHelpers.LoadLocalizedString("comdlg32.dll", 1052, "Blue");
            }
        }

        private void DrawGradient(PaintEventArgs e, Color gradientColor, ColorComponents colorComponent)
        {
            Color startColor = Color.Black;
            if (colorComponent == ColorComponents.Red)
                startColor = Color.FromArgb(gradientColor.A, 0, gradientColor.G, gradientColor.B);
            else if (colorComponent == ColorComponents.Green)
                startColor = Color.FromArgb(gradientColor.A, gradientColor.R, 0, gradientColor.B);
            else if (colorComponent == ColorComponents.Blue)
                startColor = Color.FromArgb(gradientColor.A, gradientColor.R, gradientColor.G, 0);
            else if (colorComponent == ColorComponents.Alpha)
                startColor = Color.FromArgb(0, gradientColor.R, gradientColor.G, gradientColor.B);

            using (LinearGradientBrush lgb = new LinearGradientBrush(e.ClipRectangle, startColor, gradientColor, LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(lgb, e.ClipRectangle);
            }
        }

        private void DrawMarker(object sender, PaintEventArgs e, ColorComponents colorComponent)
        {
            Control control = (sender as Control);

            byte orgValue = 0;
            if (colorComponent == ColorComponents.Red)
                orgValue = color.R;
            else if (colorComponent == ColorComponents.Green)
                orgValue = color.G;
            else if (colorComponent == ColorComponents.Blue)
                orgValue = color.B;
            else if (colorComponent == ColorComponents.Alpha)
                orgValue = color.A;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            float xpos = ScaleRange(orgValue, 0, 255, 0, control.ClientRectangle.Width);
            using (Pen pen = new Pen((colorComponent == ColorComponents.Alpha ? Color.Black : Color.FromArgb((-orgValue + 255), (-orgValue + 255), (-orgValue + 255))), 4.0f))
            {
                pen.EndCap = LineCap.Custom;
                pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4.0f, 5.5f, true);
                e.Graphics.DrawLine(pen, xpos, 0, xpos, control.ClientRectangle.Height);
            }
        }

        private float ScaleRange(int value, int minValue, int maxValue, int minScaleValue, int maxScaleValue)
        {
            return (float)(minScaleValue + (float)(value - minValue) / (maxValue - minValue) * (maxScaleValue - minScaleValue));
        }

        private void ColorSlide(object sender, MouseEventArgs e, ColorComponents colorComponent)
        {
            Control control = (sender as Control);

            int xLocation = e.X.Clamp(0, control.ClientRectangle.Width);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                byte newValue = (byte)ScaleRange(xLocation, 0, control.ClientRectangle.Width, 0, 255);

                switch (colorComponent)
                {
                    case ColorComponents.Red:
                        Color = Color.FromArgb(color.A, newValue, color.G, color.B);
                        break;

                    case ColorComponents.Green:
                        Color = Color.FromArgb(color.A, color.R, newValue, color.B);
                        break;

                    case ColorComponents.Blue:
                        Color = Color.FromArgb(color.A, color.R, color.G, newValue);
                        break;

                    case ColorComponents.Alpha:
                        Color = Color.FromArgb(newValue, color.R, color.G, color.B);
                        break;
                }
            }
        }

        private void UpdateForm()
        {
            nudRed.Value = color.R;
            nudGreen.Value = color.G;
            nudBlue.Value = color.B;
            nudAlpha.Value = color.A;

            pbColorGradientRed.Invalidate();
            pbColorGradientGreen.Invalidate();
            pbColorGradientBlue.Invalidate();
            pbColorGradientAlpha.Invalidate();

            pbPreview.Invalidate();
        }

        private void pbColorGradientRed_Paint(object sender, PaintEventArgs e)
        {
            DrawGradient(e, Color.FromArgb(red.R, color.G, color.B), ColorComponents.Red);
            DrawMarker(sender, e, ColorComponents.Red);
        }

        private void pbColorGradientGreen_Paint(object sender, PaintEventArgs e)
        {
            DrawGradient(e, Color.FromArgb(color.R, green.G, color.B), ColorComponents.Green);
            DrawMarker(sender, e, ColorComponents.Green);
        }

        private void pbColorGradientBlue_Paint(object sender, PaintEventArgs e)
        {
            DrawGradient(e, Color.FromArgb(color.R, color.G, blue.B), ColorComponents.Blue);
            DrawMarker(sender, e, ColorComponents.Blue);
        }

        private void pbColorGradientAlpha_Paint(object sender, PaintEventArgs e)
        {
            DrawGradient(e, Color.White, ColorComponents.Alpha);
            DrawMarker(sender, e, ColorComponents.Alpha);
        }

        private void pbColorGradientRed_MouseDown(object sender, MouseEventArgs e)
        {
            ColorSlide(sender, e, ColorComponents.Red);
        }

        private void pbColorGradientRed_MouseMove(object sender, MouseEventArgs e)
        {
            ColorSlide(sender, e, ColorComponents.Red);
        }

        private void pbColorGradientGreen_MouseDown(object sender, MouseEventArgs e)
        {
            ColorSlide(sender, e, ColorComponents.Green);
        }

        private void pbColorGradientGreen_MouseMove(object sender, MouseEventArgs e)
        {
            ColorSlide(sender, e, ColorComponents.Green);
        }

        private void pbColorGradientBlue_MouseDown(object sender, MouseEventArgs e)
        {
            ColorSlide(sender, e, ColorComponents.Blue);
        }

        private void pbColorGradientBlue_MouseMove(object sender, MouseEventArgs e)
        {
            ColorSlide(sender, e, ColorComponents.Blue);
        }

        private void pbColorGradientAlpha_MouseDown(object sender, MouseEventArgs e)
        {
            ColorSlide(sender, e, ColorComponents.Alpha);
        }

        private void pbColorGradientAlpha_MouseMove(object sender, MouseEventArgs e)
        {
            ColorSlide(sender, e, ColorComponents.Alpha);
        }

        private void nudRed_ValueChanged(object sender, EventArgs e)
        {
            Color = Color.FromArgb(color.A, (byte)((sender as NumericUpDown).Value), color.G, color.B);
        }

        private void nudGreen_ValueChanged(object sender, EventArgs e)
        {
            Color = Color.FromArgb(color.A, color.R, (byte)((sender as NumericUpDown).Value), color.B);
        }

        private void nudBlue_ValueChanged(object sender, EventArgs e)
        {
            Color = Color.FromArgb(color.A, color.R, color.G, (byte)((sender as NumericUpDown).Value));
        }

        private void nudAlpha_ValueChanged(object sender, EventArgs e)
        {
            Color = Color.FromArgb((byte)((sender as NumericUpDown).Value), color.R, color.G, color.B);
        }

        private void pbPreview_Paint(object sender, PaintEventArgs e)
        {
            using (Brush brush = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(brush, ((sender as PictureBox).ClientRectangle));
            }
        }

        private void pbPreview_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog cdlg = new ColorDialog();
            cdlg.Color = color;
            cdlg.FullOpen = true;

            if (cdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color = Color.FromArgb(color.A, cdlg.Color);
            }
        }
    }
}
