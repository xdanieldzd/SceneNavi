using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SceneNavi.HeaderCommands
{
    public enum PickableObjectRenderType
    {
        Normal,     /* Standard */
        Selected,   /* When selected */
        Picking,    /* During picking */
        NoColor     /* Color already specified */
    }

    public interface IPickableObject
    {
        Color PickColor { get; }
        bool IsMoveable { get; }
        OpenTK.Vector3d Position { get; set; }
        void Render(PickableObjectRenderType rendertype);
    }
}
