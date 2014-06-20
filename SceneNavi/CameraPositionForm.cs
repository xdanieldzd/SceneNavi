using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SceneNavi
{
    public partial class CameraPositionForm : Form
    {
        public CameraPositionForm(OpenGLHelpers.Camera camera) : this(camera, false) { }

        public CameraPositionForm(OpenGLHelpers.Camera camera, bool localize)
        {
            InitializeComponent();
        }
    }
}
