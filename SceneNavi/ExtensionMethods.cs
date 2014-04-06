using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace SceneNavi
{
    public static class ExtensionMethods
    {
        // http://stackoverflow.com/a/3588137
        public static void UIThread(this Control @this, Action code)
        {
            if (@this.InvokeRequired)
            {
                @this.BeginInvoke(code);
            }
            else
            {
                code.Invoke();
            }
        }

        public static void DoubleBuffered(this Control ctrl, bool setting)
        {
            Type ctrlType = ctrl.GetType();
            PropertyInfo pi = ctrlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (pi != null) pi.SetValue(ctrl, setting, null);
        }

        public static byte Scale(this byte val, byte min, byte max, byte minScale, byte maxScale)
        {
            return (byte)(minScale + (float)(val - min) / (max - min) * (maxScale - minScale));
        }

        public static float Scale(this float val, float min, float max, float minScale, float maxScale)
        {
            return (minScale + (float)(val - min) / (max - min) * (maxScale - minScale));
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static void Init<T>(this T[] array, T defaultValue)
        {
            if (array == null)
                return;

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = defaultValue;
            }
        }

        public static void Fill<T>(this T[] array, T[] data)
        {
            if (array == null)
                return;

            for (int i = 0; i < array.Length; i += data.Length)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    try
                    {
                        array[i + j] = data[j];
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }

        public static IEnumerable<TreeNode> FlattenTree(this TreeView tv)
        {
            return FlattenTree(tv.Nodes);
        }

        public static IEnumerable<TreeNode> FlattenTree(this TreeNodeCollection coll)
        {
            return coll.Cast<TreeNode>().Concat(coll.Cast<TreeNode>().SelectMany(x => FlattenTree(x.Nodes)));
        }

        public static IEnumerable<ToolStripMenuItem> FlattenHintMenu(this MenuStrip menu)
        {
            return FlattenMenu(menu.Items);
        }

        public static IEnumerable<Controls.ToolStripHintMenuItem> FlattenMenu(this ToolStripItemCollection coll)
        {
            return coll.OfType<Controls.ToolStripHintMenuItem>().Concat(coll.OfType<Controls.ToolStripHintMenuItem>().SelectMany(x => FlattenMenu(x.DropDownItems)));
        }
    }
}
