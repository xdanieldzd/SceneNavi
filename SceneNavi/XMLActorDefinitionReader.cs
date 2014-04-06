using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace SceneNavi
{
    public class XMLActorDefinitionReader
    {
        public class XMLActorDefinitionReaderException : Exception
        {
            public XMLActorDefinitionReaderException(string errorMessage) : base(errorMessage) { }
            public XMLActorDefinitionReaderException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
        };

        public class Definition
        {
            [Flags]
            public enum DefaultTypes
            {
                None = 0x00,
                RoomActor = 0x01,
                TransitionActor = 0x02,
                SpawnPoint = 0x04
            };

            public class Item
            {
                public enum DisplayStyles
                {
                    Decimal,
                    Hexadecimal
                };

                public enum Usages
                {
                    Generic,
                    ActorNumber,
                    PositionX,
                    PositionY,
                    PositionZ,
                    RotationX,
                    RotationY,
                    RotationZ,
                    NextRoomFront,
                    NextRoomBack
                };

                public class Option
                {
                    public UInt64 Value { get; set; }
                    public string Description { get; set; }

                    public Option()
                    {
                        Value = 0;
                        Description = string.Empty;
                    }
                }

                public int Index { get; set; }
                public Type ValueType { get; set; }
                public DisplayStyles DisplayStyle { get; set; }
                public Usages Usage { get; set; }
                public string Description { get; set; }
                public UInt64 Mask { get; set; }
                public Type ControlType { get; set; }
                public List<Option> Options { get; set; }

                public Item()
                {
                    Index = 0;
                    ValueType = null;
                    DisplayStyle = Definition.Item.DisplayStyles.Decimal;
                    Usage = Definition.Item.Usages.Generic;
                    Description = string.Empty;
                    Mask = UInt64.MaxValue;
                    ControlType = null;
                    Options = new List<Definition.Item.Option>();
                }
            }

            public ushort Number { get; set; }
            public DefaultTypes IsDefault { get; set; }
            public OpenGLHelpers.DisplayList DisplayModel { get; set; }
            public OpenGLHelpers.DisplayList PickModel { get; set; }
            public double FrontOffset { get; set; }
            public List<Item> Items { get; set; }

            public Definition()
            {
                Number = ushort.MaxValue;
                IsDefault = DefaultTypes.None;
                DisplayModel = OpenGLHelpers.StockObjects.ColoredCube;
                PickModel = OpenGLHelpers.StockObjects.Cube;
                FrontOffset = 0.0;
                Items = null;
            }
        }

        public Version ProgramVersion { get; private set; }
        public List<Definition> Definitions { get; private set; }

        public XMLActorDefinitionReader(string defdir)
        {
            Definitions = new List<Definition>();

            string path = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), defdir);
            if (Directory.Exists(path) == false) return;

            ProgramVersion = new Version();

            List<string> XmlFiles = Directory.EnumerateFiles(path, "*.xml").ToList();
            foreach (string fn in XmlFiles)
            {
                Definition ndef = null;
                Definition.Item nitem = null;
                Definition.Item.Option nopt = null;
                OpenGLHelpers.DisplayList displaydl = null;
                OpenGLHelpers.DisplayList pickdl = null;

                XmlTextReader xml = new XmlTextReader(fn);
                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        if (xml.Name == "ActorDatabase")
                        {
                            while (xml.MoveToNextAttribute())
                            {
                                if (xml.Name == "ProgramVersion")
                                {
                                    ProgramVersion = Version.Parse(xml.Value);
                                    if (ProgramVersion != Version.Parse(Application.ProductVersion)) ThrowVersionError();
                                }
                            }
                        }
                        else if (xml.Name == "Definition")
                        {
                            if (ProgramVersion == new Version()) ThrowVersionError();

                            ndef = new Definition();
                            ndef.Items = new List<Definition.Item>();

                            while (xml.MoveToNextAttribute())
                            {
                                switch (xml.Name)
                                {
                                    case "Number":
                                        if (xml.Value.StartsWith("0x") == true)
                                            ndef.Number = ushort.Parse(xml.Value.Substring(2), System.Globalization.NumberStyles.HexNumber);
                                        else
                                            ndef.Number = ushort.Parse(xml.Value);
                                        break;
                                    case "IsDefault":
                                        ndef.IsDefault = (Definition.DefaultTypes)Enum.Parse(typeof(Definition.DefaultTypes), xml.Value);
                                        break;
                                    case "DisplayModel":
                                        displaydl = OpenGLHelpers.StockObjects.GetDisplayList(xml.Value);
                                        if (displaydl != null) ndef.DisplayModel = displaydl;
                                        break;
                                    case "PickModel":
                                        pickdl = OpenGLHelpers.StockObjects.GetDisplayList(xml.Value);
                                        if (pickdl != null) ndef.PickModel = pickdl;
                                        break;
                                    case "FrontOffset":
                                        ndef.FrontOffset = double.Parse(xml.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                }
                            }
                        }
                        else if (xml.Name == "Item")
                        {
                            nitem = new Definition.Item();
                            while (xml.MoveToNextAttribute())
                            {
                                switch (xml.Name)
                                {
                                    case "Index":
                                        nitem.Index = int.Parse(xml.Value);
                                        break;
                                    case "ValueType":
                                        nitem.ValueType = FindTypeInCurrentAssemblies(xml.Value);
                                        break;
                                    case "DisplayStyle":
                                        nitem.DisplayStyle = (Definition.Item.DisplayStyles)Enum.Parse(typeof(Definition.Item.DisplayStyles), xml.Value);
                                        break;
                                    case "Usage":
                                        nitem.Usage = (Definition.Item.Usages)Enum.Parse(typeof(Definition.Item.Usages), xml.Value);
                                        break;
                                    case "Description":
                                        nitem.Description = xml.Value;
                                        break;
                                    case "Mask":
                                        if (xml.Value.StartsWith("0x") == true)
                                            nitem.Mask = UInt64.Parse(xml.Value.Substring(2), System.Globalization.NumberStyles.HexNumber);
                                        else
                                            nitem.Mask = UInt64.Parse(xml.Value);
                                        break;
                                    case "ControlType":
                                        nitem.ControlType = FindTypeInCurrentAssemblies(xml.Value);
                                        break;
                                }
                            }
                            ndef.Items.Add(nitem);
                        }
                        else if (xml.Name == "Option")
                        {
                            nopt = new Definition.Item.Option();
                            while (xml.MoveToNextAttribute())
                            {
                                switch (xml.Name)
                                {
                                    case "Value":
                                        if (xml.Value.StartsWith("0x") == true)
                                            nopt.Value = UInt64.Parse(xml.Value.Substring(2), System.Globalization.NumberStyles.HexNumber);
                                        else
                                            nopt.Value = UInt64.Parse(xml.Value);
                                        break;
                                    case "Description":
                                        nopt.Description = xml.Value;
                                        break;
                                }
                            }
                            nitem.Options.Add(nopt);
                        }
                    }
                    else if (xml.NodeType == XmlNodeType.EndElement)
                    {
                        if (xml.Name == "Definition")
                        {
                            if (displaydl != null && pickdl == null) ndef.PickModel = displaydl;

                            Definitions.Add(ndef);
                        }
                    }
                }
            }
        }

        private void ThrowVersionError()
        {
            throw new XMLActorDefinitionReaderException(string.Format("Program version mismatch; expected {0}, found {1}. Please make sure your XML folder is up-to-date.", Application.ProductVersion, ProgramVersion));
        }

        public static void RefreshActorPositionRotation(HeaderCommands.Actors.Entry ac, Controls.TableLayoutPanelEx tlpex)
        {
            foreach (Control ctrl in tlpex.Controls)
            {
                if (ctrl is TextBox && ctrl.Tag is Definition.Item)
                {
                    Definition.Item item = (ctrl.Tag as Definition.Item);
                    if (item.Usage == Definition.Item.Usages.PositionX || item.Usage == Definition.Item.Usages.PositionY || item.Usage == Definition.Item.Usages.PositionZ ||
                        item.Usage == Definition.Item.Usages.RotationX || item.Usage == Definition.Item.Usages.RotationY || item.Usage == Definition.Item.Usages.RotationZ)
                    {
                        string fstr = "{0}";
                        switch (item.DisplayStyle)
                        {
                            case Definition.Item.DisplayStyles.Hexadecimal: fstr = "0x{0:X}"; break;
                            case Definition.Item.DisplayStyles.Decimal: fstr = "{0:D}"; break;
                        }
                        object val = GetValueFromActor(item, ac);
                        item.ControlType.GetProperty("Text").SetValue(ctrl, string.Format(fstr, val), null);
                    }
                }
            }
        }

        public static void CreateActorEditingControls(HeaderCommands.Actors.Entry ac, Controls.TableLayoutPanelEx tlpex, Action numberchanged, object tag = null, bool individual = false)
        {
            //TODO TODO TODO: more value types, more control types, etc, etc!!!

            /* No proper actor entry given? */
            if (ac == null || ac.Definition == null)
            {
                tlpex.Controls.Clear();
                return;
            }

            /* Get current definition */
            Definition def = ac.Definition;

            /* No definition given? */
            if (def == null) return;

            /* Begin layout creation */
            tlpex.SuspendLayout();
            tlpex.Controls.Clear();

            /* Create description label */
            Label desc = new Label()
            {
                Text = (ac.InternalName == string.Empty ? ac.Name : string.Format("{0} ({1})", ac.Name, ac.InternalName)),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };
            tlpex.Controls.Add(desc, 0, 0);
            tlpex.SetColumnSpan(desc, 2);

            /* Parse items */
            for (int i = 0; i < def.Items.Count; i++)
            {
                /* Get current item */
                Definition.Item item = def.Items[i];

                /* UGLY HACK -> for room number in transition actor with individual file mode... */
                if (item.Usage == Definition.Item.Usages.NextRoomBack || item.Usage == Definition.Item.Usages.NextRoomFront)
                    item.ControlType = (individual ? typeof(TextBox) : typeof(ComboBox));

                /* Get value, create control */
                object val = GetValueFromActor(item, ac);
                object ctrl = Activator.CreateInstance(item.ControlType);

                /* First ControlType check; ex. is label needed? */
                if (item.ControlType == typeof(CheckBox))
                {
                    /* Add control alone */
                    tlpex.Controls.Add(ctrl as Control, 0, (i + 1));
                }
                else
                {
                    /* Add label and control */
                    Label lbl = new Label() { Text = string.Format("{0}:", item.Description), TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
                    tlpex.Controls.Add(lbl, 0, (i + 1));
                    tlpex.Controls.Add(ctrl as Control, 1, (i + 1));
                }

                /* Set control properties */
                item.ControlType.GetProperty("Dock").SetValue(ctrl, DockStyle.Fill, null);
                item.ControlType.GetProperty("Tag").SetValue(ctrl, item, null);
                item.ControlType.GetProperty("Name").SetValue(ctrl, item.Usage.ToString(), null);

                /* ControlType-specific settings */
                if (item.ControlType == typeof(ComboBox))
                {
                    /* Set ComboBox */
                    item.ControlType.GetProperty("DropDownStyle").SetValue(ctrl, ComboBoxStyle.DropDownList, null);
                    item.ControlType.GetProperty("DisplayMember").SetValue(ctrl, "Description", null);

                    if (!individual && (item.Usage == Definition.Item.Usages.NextRoomBack || item.Usage == Definition.Item.Usages.NextRoomFront) && (tag is List<HeaderCommands.Rooms.RoomInfoClass>))
                    {
                        /* Item usage is room number in transition actor; get room listing from function tag */
                        item.Options = new List<Definition.Item.Option>();
                        foreach (HeaderCommands.Rooms.RoomInfoClass ric in (tag as List<HeaderCommands.Rooms.RoomInfoClass>))
                            item.Options.Add(new Definition.Item.Option() { Description = ric.Description, Value = ric.Number });
                    }

                    if (item.Options.Count > 0)
                    {
                        item.ControlType.GetProperty("DataSource").SetValue(ctrl, item.Options, null);
                        item.ControlType.GetProperty("SelectedItem").SetValue(ctrl, item.Options.Find(x => x.Value == (Convert.ToUInt64(val) & item.Mask)), null);
                        (ctrl as ComboBox).SelectedIndexChanged += new EventHandler((s, ex) =>
                        {
                            SetValueInActor(item, ac, ((Definition.Item.Option)((ComboBox)s).SelectedItem).Value);
                        });
                    }
                }
                else if (item.ControlType == typeof(CheckBox))
                {
                    /* Set CheckBox */
                    item.ControlType.GetProperty("Checked").SetValue(ctrl, Convert.ToBoolean(val), null);
                    item.ControlType.GetProperty("Text").SetValue(ctrl, item.Description, null);
                    tlpex.SetColumnSpan(ctrl as Control, 2);
                    (ctrl as CheckBox).CheckedChanged += new EventHandler((s, ex) =>
                    {
                        ChangeBitInActor(item, ac, item.Mask, ((CheckBox)s).Checked);
                    });
                }
                else
                {
                    /* Fallback */
                    if (item.ControlType.GetProperty("Text") != null)
                    {
                        string fstr = "{0}";
                        switch (item.DisplayStyle)
                        {
                            case Definition.Item.DisplayStyles.Hexadecimal: fstr = "0x{0:X}"; break;
                            case Definition.Item.DisplayStyles.Decimal: fstr = "{0:D}"; break;
                        }
                        item.ControlType.GetProperty("Text").SetValue(ctrl, string.Format(fstr, val), null);
                        (ctrl as Control).TextChanged += new EventHandler((s, ex) =>
                        {
                            object newval = Activator.CreateInstance(item.ValueType);
                            System.Reflection.MethodInfo mi = item.ValueType.GetMethod("Parse", new Type[] { typeof(string), typeof(System.Globalization.NumberStyles) });
                            if (mi != null)
                            {
                                /* Determine NumberStyle to use */
                                System.Globalization.NumberStyles ns =
                                    (item.DisplayStyle == Definition.Item.DisplayStyles.Hexadecimal ? System.Globalization.NumberStyles.HexNumber : System.Globalization.NumberStyles.Integer);

                                /* Hex number; is text long enough? */
                                if (ns == System.Globalization.NumberStyles.HexNumber && ((Control)s).Text.Length < 2) return;

                                /* Get value string, depending on NumberStyle */
                                string valstr = (ns == System.Globalization.NumberStyles.HexNumber ? ((Control)s).Text.Substring(2) : ((Control)s).Text);

                                /* Proper value string found? */
                                if (valstr != null && valstr != "")
                                {
                                    try
                                    {
                                        /* Invoke Parse function and get parsed value */
                                        newval = mi.Invoke(newval, new object[] { valstr, ns });

                                        /* Set new value in actor; if usage is ActorNumber, also do callback */
                                        SetValueInActor(item, ac, newval);
                                        if (item.Usage == Definition.Item.Usages.ActorNumber && numberchanged != null) numberchanged();
                                    }
                                    catch (TargetInvocationException tiex)
                                    {
                                        if (tiex.InnerException is FormatException)
                                        {
                                            /* Ignore; happens with ex. malformed hex numbers (i.e. "0xx0") */
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            }

            /* Done */
            tlpex.ResumeLayout();
        }

        private Type FindTypeInCurrentAssemblies(string name)
        {
            Type ntype = null;
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                ntype = asm.GetType(name);
                if (ntype != null) break;
            }
            return ntype;
        }

        public static object GetValueFromActor(Definition.Item item, HeaderCommands.Actors.Entry ac)
        {
            if (item == null || ac == null) return null;

            object val = null;
            if (item.ValueType == typeof(Byte))
                val = (ac.RawData[item.Index] & (Byte)item.Mask);
            if (item.ValueType == typeof(UInt16))
                val = (Endian.SwapUInt16(BitConverter.ToUInt16(ac.RawData, item.Index)) & (UInt16)item.Mask);
            else if (item.ValueType == typeof(Int16))
                val = (Endian.SwapInt16(BitConverter.ToInt16(ac.RawData, item.Index)) & (Int16)item.Mask);

            return Convert.ChangeType(val, item.ValueType);
        }

        public static void SetValueInActor(Definition.Item item, HeaderCommands.Actors.Entry ac, object value)
        {
            if (item == null || ac == null || value == null) return;

            object oldval = null;

            if (item.ValueType == typeof(Byte))
            {
                ac.RawData[item.Index] = (byte)((ac.RawData[item.Index] & ~(Byte)item.Mask) | Convert.ToByte(value));
            }
            else if (item.ValueType == typeof(UInt16))
            {
                oldval = (UInt16)(Endian.SwapUInt16(BitConverter.ToUInt16(ac.RawData, item.Index)) & ~(UInt16)item.Mask);
                UInt16 newval = Endian.SwapUInt16((UInt16)(Convert.ToUInt16(oldval) | Convert.ToUInt16(value)));
                BitConverter.GetBytes(newval).CopyTo(ac.RawData, item.Index);
            }
            else if (item.ValueType == typeof(Int16))
            {
                oldval = (Int16)(Endian.SwapInt16(BitConverter.ToInt16(ac.RawData, item.Index)) & ~(Int16)item.Mask);
                Int16 newval = Endian.SwapInt16((Int16)(Convert.ToInt16(oldval) | Convert.ToInt16(value)));
                BitConverter.GetBytes(newval).CopyTo(ac.RawData, item.Index);
            }

            if (item.Usage == Definition.Item.Usages.ActorNumber) ac.RefreshVariables();
        }

        public static void ChangeBitInActor(Definition.Item item, HeaderCommands.Actors.Entry ac, object value, bool set)
        {
            if (item == null || ac == null || value == null) return;

            //TODO TODO TODO allow bit toggle in non-byte types??
            if (set == true)
            {
                if (item.ValueType == typeof(Byte))
                    ac.RawData[item.Index] |= (byte)(Convert.ToByte(value) & (Byte)item.Mask);
                else
                    throw new Exception("Cannot toggle bits in non-byte value");
            }
            else
            {
                if (item.ValueType == typeof(Byte))
                    ac.RawData[item.Index] &= (byte)~(Convert.ToByte(value) & (Byte)item.Mask);
                else
                    throw new Exception("Cannot toggle bits in non-byte value");
            }
        }
    }
}
