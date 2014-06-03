using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SceneNavi.ROMHandler;
using SceneNavi.OpenGLHelpers;

namespace SceneNavi
{
    /*
     * As usual, my GUI code is a mess! :D
     * There's some useful stuff in here, like the OpenGL picking code, but overall this is probably the least interesting part of the program...
     * ...like, excluding constants and enums or something anyway.
     */

    public enum ToolModes { Camera, MoveableObjs, StaticObjs };
    public enum CombinerTypes { None, ArbCombiner, GLSLCombiner }

    public partial class MainForm : Form
    {
        static readonly Dictionary<ToolModes, string[]> toolModeNametable = new Dictionary<ToolModes, string[]>()
        {
            { ToolModes.Camera, new string[] { "Camera mode", "Mouse can only move around camera" } },
            { ToolModes.MoveableObjs, new string[] { "Moveable objects mode", "Mouse will select and modify moveable objects (ex. actors)" } },
            { ToolModes.StaticObjs, new string[] { "Static objects mode", "Mouse will select and modify static objects (ex. collision)" } },
        };

        static readonly Dictionary<CombinerTypes, string[]> combinerTypeNametable = new Dictionary<CombinerTypes, string[]>()
        {
            { CombinerTypes.None, new string[] { "None", "Does not try to emulate combiner; necessary on older or low-end hardware" } },
            { CombinerTypes.ArbCombiner, new string[] { "ARB Assembly Combiner", "Uses stable, mostly complete ARB combiner emulation; might not work on Intel hardware" } },
            { CombinerTypes.GLSLCombiner, new string[] { "Experimental GLSL Combiner", "Uses experimental GLSL-based combiner emulation; not complete yet" } },
        };

        static readonly string[] requiredOglExtensionsGeneral = new string[] { "GL_ARB_multisample" };
        static readonly string[] requiredOglExtensionsCombinerGeneral = new string[] { "GL_ARB_multitexture" };
        static readonly string[] requiredOglExtensionsARBCombiner = new string[] { "GL_ARB_fragment_program" };
        static readonly string[] requiredOglExtensionsGLSLCombiner = new string[] { "GL_ARB_shading_language_100", "GL_ARB_shader_objects", "GL_ARB_fragment_shader", "GL_ARB_vertex_shader" };

        static string[] allRequiredOglExtensions
        {
            get
            {
                List<string> all = new List<string>();
                all.AddRange(requiredOglExtensionsGeneral);
                all.AddRange(requiredOglExtensionsCombinerGeneral);
                all.AddRange(requiredOglExtensionsARBCombiner);
                all.AddRange(requiredOglExtensionsGLSLCombiner);
                return all.ToArray();
            }
        }

        bool ready, busy;
        bool[] keysDown = new bool[ushort.MaxValue];
        QuickFontWrapper glText;
        Camera camera;
        FPSMonitor fpsMonitor;

        bool supportsCreateShader, supportsGenProgramsARB;

        ToolModes internalToolMode;
        ToolModes currentToolMode
        {
            get { return internalToolMode; }
            set
            {
                Configuration.LastToolMode = internalToolMode = (Enum.IsDefined(typeof(ToolModes), value) ? internalToolMode = value : internalToolMode = ToolModes.Camera);
                bsiToolMode.Text = toolModeNametable[internalToolMode][0];
                if (mouseModeToolStripMenuItem.DropDownItems.Count > 0)
                {
                    (mouseModeToolStripMenuItem.DropDownItems[(int)internalToolMode] as Controls.ToolStripRadioButtonMenuItem).Checked = true;
                }
            }
        }

        CombinerTypes internalCombinerType;
        CombinerTypes currentCombinerType
        {
            get { return internalCombinerType; }
            set
            {
                Configuration.CombinerType = internalCombinerType = (Enum.IsDefined(typeof(CombinerTypes), value) ? internalCombinerType = value : internalCombinerType = CombinerTypes.None);
                if (rom != null) rom.Renderer.InitCombiner();
                displayListsDirty = true;
            }
        }

        ROMHandler.ROMHandler rom;
        bool individualFileMode;
        Dictionary<byte, string> bgms;

        SceneTableEntry currentScene;
        HeaderCommands.Rooms.RoomInfoClass currentRoom;
        List<HeaderCommands.MeshHeader> allMeshHeaders;
        HeaderCommands.Collision.Polygon currentCollisionPolygon;
        HeaderCommands.Collision.PolygonType currentColPolygonType;
        HeaderCommands.Collision.Waterbox currentWaterbox;
        OpenGLHelpers.DisplayListEx.Triangle currentRoomTriangle;
        SimpleF3DEX2.Vertex currentRoomVertex;

        SceneTableEntry tempScene;
        HeaderCommands.Rooms tempRooms;

        // weird but works?
        HeaderCommands.Waypoints.PathHeader activePathHeader
        {
            get { return (cbPathHeaders.SelectedItem as HeaderCommands.Waypoints.PathHeader); }
            set { RefreshPathWaypoints(); }
        }

        BindingSource roomActorComboBinding, transitionComboBinding, spawnPointComboBinding, collisionPolyDataBinding, colPolyTypeDataBinding, waypointPathComboDataBinding, waterboxComboDataBinding;

        bool displayListsDirty, collisionDirty, waterboxesDirty;
        DisplayList collisionDL, waterboxDL;
        TabPage lastTabPage;

        HeaderCommands.IPickableObject pickedObject;
        Vector2d pickObjDisplacement, pickObjLastPosition, pickObjPosition;

        List<XMLActorDefinitionReader.Definition.Item.Option> roomsForWaterboxSelection;

        public MainForm()
        {
            InitializeComponent();

            Application.Idle += new EventHandler(Application_Idle);

            Program.Status.MessageChanged += new StatusMessageHandler.MessageChangedEvent(StatusMsg_OnStatusMessageChanged);

            dgvObjects.DoubleBuffered(true);
            dgvPathWaypoints.DoubleBuffered(true);

            SetFormTitle();
        }

        private void StatusMsg_OnStatusMessageChanged(object sender, StatusMessageHandler.MessageChangedEventArgs e)
        {
            tsslStatus.Text = e.Message;
            statusStrip1.Invoke((MethodInvoker)(() => statusStrip1.Update()));
        }

        private void SetFormTitle()
        {
            string filenamePart = ((rom != null && rom.Loaded) ? string.Format(" - [{0}]", Path.GetFileName(rom.Filename)) : string.Empty);
            string scenePart = (individualFileMode ? string.Format(" ({0})", Path.GetFileName(Configuration.LastSceneFile)) : string.Empty);
            this.Text = string.Concat(Program.AppNameVer, filenamePart, scenePart);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (ready)
            {
                camera.KeyUpdate(keysDown);
                customGLControl.Invalidate();

                bsiCamCoords.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Cam X: {0:00.000}, Y: {1:00.000}, Z: {2:00.000}", camera.Pos.X, camera.Pos.Y, camera.Pos.Z);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ResetCurrentData();
        }

        private void SettingsGUIInit()
        {
            /* Read settings */
            enableTexturesToolStripMenuItem.Checked = Configuration.RenderTextures;
            renderCollisionToolStripMenuItem.Checked = Configuration.RenderCollision;

            whiteToolStripMenuItem.Checked = Configuration.RenderCollisionAsWhite;
            typebasedToolStripMenuItem.Checked = !whiteToolStripMenuItem.Checked;

            renderRoomActorsToolStripMenuItem.Checked = Configuration.RenderRoomActors;
            renderSpawnPointsToolStripMenuItem.Checked = Configuration.RenderSpawnPoints;
            renderTransitionsToolStripMenuItem.Checked = Configuration.RenderTransitions;

            renderPathWaypointsToolStripMenuItem.Checked = Configuration.RenderPathWaypoints;
            linkAllWaypointsInPathToolStripMenuItem.Checked = Configuration.LinkAllWPinPath;

            renderWaterboxesToolStripMenuItem.Checked = Configuration.RenderWaterboxes;

            showWaterboxesPerRoomToolStripMenuItem.Checked = Configuration.ShowWaterboxesPerRoom;

            enableVSyncToolStripMenuItem.Checked = customGLControl.VSync = Configuration.OGLVSync;
            enableAntiAliasingToolStripMenuItem.Checked = Configuration.EnableAntiAliasing;
            enableMipmapsToolStripMenuItem.Checked = Configuration.EnableMipmaps;

            currentToolMode = Configuration.LastToolMode;
            currentCombinerType = Configuration.CombinerType;

            /* Create tool mode menu */
            int i = 0;
            foreach (KeyValuePair<ToolModes, string[]> kvp in toolModeNametable)
            {
                Controls.ToolStripRadioButtonMenuItem tsmi = new Controls.ToolStripRadioButtonMenuItem(kvp.Value[0]) { Tag = kvp.Key, CheckOnClick = true, HelpText = kvp.Value[1] };
                if (currentToolMode == kvp.Key) tsmi.Checked = true;

                tsmi.Click += new EventHandler((s, ex) =>
                {
                    object tag = ((ToolStripMenuItem)s).Tag;
                    if (tag is ToolModes) currentToolMode = ((ToolModes)tag);
                });

                mouseModeToolStripMenuItem.DropDownItems.Add(tsmi);
                i++;
            }

            /* Create combiner type menu */
            i = 0;
            foreach (KeyValuePair<CombinerTypes, string[]> kvp in combinerTypeNametable)
            {
                Controls.ToolStripRadioButtonMenuItem tsmi = new Controls.ToolStripRadioButtonMenuItem(kvp.Value[0]) { Tag = kvp.Key, CheckOnClick = true, HelpText = kvp.Value[1] };
                if (currentCombinerType == kvp.Key) tsmi.Checked = true;

                tsmi.Click += new EventHandler((s, ex) =>
                {
                    object tag = ((ToolStripMenuItem)s).Tag;
                    if (tag is CombinerTypes) currentCombinerType = ((CombinerTypes)tag);
                });

                combinerTypeToolStripMenuItem.DropDownItems.Add(tsmi);
                i++;
            }

            /* Initialize help */
            InitializeMenuHelp();
        }

        private void InitializeMenuHelp()
        {
            /* Kinda buggy in practice (ex. with disabled menu items...) */
            foreach (SceneNavi.Controls.ToolStripHintMenuItem menuItem in menuStrip1.Items.FlattenMenu().ToList())
            {
                if (menuItem.HelpText == null) continue;

                menuItem.Hint += ((s, e) =>
                {
                    if (Program.IsHinting) return;
                    Program.IsHinting = true;
                    Program.Status.Message = (s as SceneNavi.Controls.ToolStripHintMenuItem).HelpText;
                });
                menuItem.Unhint += ((s, e) =>
                {
                    if (!Program.IsHinting) return;
                    Program.IsHinting = false;
                    CreateStatusString();
                });
            }
        }

        private void CreateSceneTree()
        {
            tvScenes.Nodes.Clear();
            TreeNode root = null;

            if (!individualFileMode)
            {
                root = new TreeNode(string.Format("{0} ({1}, v1.{2}; {3} scenes)", rom.Title, rom.GameID, rom.Version, rom.Scenes.Count)) { Tag = rom };
                foreach (SceneTableEntry ste in rom.Scenes)
                {
                    TreeNode scene = new TreeNode(string.Format("{0} (0x{1:X})", ste.Name, ste.SceneStartAddress)) { Tag = ste };

                    if (ste.SceneHeaders.Count != 0)
                    {
                        HeaderCommands.Rooms rooms = ste.SceneHeaders[0].Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms) as HeaderCommands.Rooms;
                        if (rooms == null) continue;

                        foreach (HeaderLoader shead in ste.SceneHeaders)
                        {
                            List<HeaderLoader> rhs = new List<HeaderLoader>();
                            foreach (HeaderCommands.Rooms.RoomInfoClass ric in rooms.RoomInformation)
                                if (ric.Headers.Count != 0) rhs.Add(ric.Headers[shead.Number]);

                            HeaderLoader.HeaderPair hp = new HeaderLoader.HeaderPair(shead, rhs);

                            System.Collections.DictionaryEntry de = new System.Collections.DictionaryEntry();
                            foreach (System.Collections.DictionaryEntry d in rom.XMLStageDescriptions.Names)
                            {
                                HeaderLoader.StageKey sk = d.Key as HeaderLoader.StageKey;
                                if (sk.SceneAddress == ste.SceneStartAddress && sk.HeaderNumber == hp.SceneHeader.Number)
                                {
                                    de = d;
                                    hp.Description = (string)de.Value;
                                    break;
                                }
                            }

                            TreeNode sheadnode = new TreeNode((de.Value == null ? string.Format("Stage #{0}", shead.Number) : (string)de.Value)) { Tag = hp };
                            foreach (HeaderCommands.Rooms.RoomInfoClass ric in rooms.RoomInformation)
                            {
                                TreeNode room = new TreeNode(string.Format("{0} (0x{1:X})", ric.Description, ric.Start)) { Tag = ric };
                                sheadnode.Nodes.Add(room);
                            }

                            scene.Nodes.Add(sheadnode);
                        }
                    }

                    root.Nodes.Add(scene);
                }

                root.Expand();
                tvScenes.Nodes.Add(root);
            }
            else
            {
                root = new TreeNode(tempScene.Name) { Tag = tempScene };
                HeaderCommands.Rooms rooms = tempScene.SceneHeaders[0].Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms) as HeaderCommands.Rooms;

                TreeNode nodeToSelect = null;
                if (rooms != null)
                {
                    foreach (HeaderLoader shead in tempScene.SceneHeaders)
                    {
                        List<HeaderLoader> rhs = new List<HeaderLoader>();
                        foreach (HeaderCommands.Rooms.RoomInfoClass ric in rooms.RoomInformation)
                            if (ric.Headers.Count != 0) rhs.Add(ric.Headers[shead.Number]);

                        HeaderLoader.HeaderPair hp = new HeaderLoader.HeaderPair(shead, rhs);

                        TreeNode sheadnode = new TreeNode(string.Format("Stage #{0}", shead.Number)) { Tag = hp };
                        foreach (HeaderCommands.Rooms.RoomInfoClass ric in rooms.RoomInformation)
                        {
                            TreeNode room = new TreeNode(string.Format("{0} (0x{1:X})", ric.Description, ric.Start)) { Tag = ric };
                            sheadnode.Nodes.Add(room);
                        }
                        sheadnode.Expand();
                        root.Nodes.Add(sheadnode);
                        if (nodeToSelect == null) nodeToSelect = sheadnode.FirstNode;
                    }
                }

                root.Expand();
                tvScenes.Nodes.Add(root);
                tvScenes.SelectedNode = nodeToSelect;
            }
        }

        private void PopulateMiscControls()
        {
            if (rom == null) return;

            bgms = new Dictionary<byte, string>();
            foreach (System.Collections.DictionaryEntry de in rom.XMLSongNames.Names) bgms.Add((byte)de.Key, (string)de.Value);
        }

        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* Get last ROM */
            if (Configuration.LastROM != string.Empty)
            {
                ofdOpenROM.InitialDirectory = Path.GetDirectoryName(Configuration.LastROM);
                ofdOpenROM.FileName = Path.GetFileName(Configuration.LastROM);
            }

            if (ofdOpenROM.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Program.Status.Message = "Loading; please wait...";
                Cursor.Current = Cursors.WaitCursor;

                individualFileMode = false;
                displayListsDirty = collisionDirty = waterboxesDirty = true;

                Configuration.LastROM = ofdOpenROM.FileName;
                rom = new ROMHandler.ROMHandler(ofdOpenROM.FileName);

                if (rom.Loaded)
                {
                    ResetCurrentData();

                    PopulateMiscControls();

                    CreateSceneTree();
                    SetFormTitle();
#if BLAHBLUB
                    //header dumper
                    System.IO.TextWriter tw = System.IO.File.CreateText("D:\\roms\\n64\\headers.txt");
                    tw.WriteLine("ROM: {0} ({1}, v1.{2}; {3})", ROM.Title, ROM.GameID, ROM.Version, ROM.BuildDateString);
                    foreach (SceneTableEntry ste in ROM.Scenes)
                    {
                        HeaderCommands.Rooms rooms = null;
                        tw.WriteLine(" SCENE: " + ste.Name);
                        foreach (HeaderLoader hl in ste.SceneHeaders)
                        {
                            tw.WriteLine("  HEADER: " + hl.Description);
                            foreach (HeaderCommands.Generic cmd in hl.Commands/*.OrderBy(x => (x.Data >> 56))*/)
                            {
                                if (cmd is HeaderCommands.Rooms) rooms = (cmd as HeaderCommands.Rooms);

                                //if (!((cmd.Data >> 56) == (byte)HeaderLoader.CommandTypeIDs.SubHeaders) && !(cmd is HeaderCommands.Actors) && !(cmd is HeaderCommands.Collision) &&
                                //    !(cmd is HeaderCommands.MeshHeader) && !(cmd is HeaderCommands.Objects) && !(cmd is HeaderCommands.Rooms) && !(cmd is HeaderCommands.SpecialObjects) &&
                                //    !(cmd is HeaderCommands.Waypoints))
                                tw.WriteLine("   COMMAND: " + cmd.ByteString + "; " + cmd.Description);
                            }
                        }

                        if (rooms != null)
                        {
                            foreach (HeaderCommands.Rooms.RoomInfoClass ric in rooms.RoomInformation)
                            {
                                tw.WriteLine("  ROOM: " + ric.Description);
                                foreach (HeaderLoader hl in ric.Headers)
                                {
                                    tw.WriteLine("   HEADER: " + hl.Description);
                                    foreach (HeaderCommands.Generic cmd in hl.Commands/*.OrderBy(x => (x.Data >> 56))*/)
                                    {
                                        //if (!((cmd.Data >> 56) == (byte)HeaderLoader.CommandTypeIDs.SubHeaders) && !(cmd is HeaderCommands.Actors) && !(cmd is HeaderCommands.Collision) &&
                                        //    !(cmd is HeaderCommands.MeshHeader) && !(cmd is HeaderCommands.Objects) && !(cmd is HeaderCommands.Rooms) && !(cmd is HeaderCommands.SpecialObjects) &&
                                        //    !(cmd is HeaderCommands.Waypoints))
                                        tw.WriteLine("    COMMAND: " + cmd.ByteString + "; " + cmd.Description);
                                    }
                                }
                            }
                        }

                        tw.WriteLine();
                    }
                    tw.Close();
#endif
                }
                else
                {
                    Program.Status.Message = "Error loading ROM";
                }

                Cursor.Current = DefaultCursor;

                editDataTablesToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled = openSceneToolStripMenuItem.Enabled = rOMInformationToolStripMenuItem.Enabled = customGLControl.Enabled = rom.Loaded;
            }
        }

        private void openSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* Get last scene and room */
            if (Configuration.LastSceneFile != string.Empty)
            {
                ofdOpenScene.InitialDirectory = Path.GetDirectoryName(Configuration.LastSceneFile);
                ofdOpenScene.FileName = Path.GetFileName(Configuration.LastSceneFile);
            }

            if (Configuration.LastRoomFile != string.Empty)
            {
                ofdOpenRoom.InitialDirectory = Path.GetDirectoryName(Configuration.LastRoomFile);
                ofdOpenRoom.FileName = Path.GetFileName(Configuration.LastRoomFile);
            }

            if (ofdOpenScene.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Configuration.LastSceneFile = ofdOpenScene.FileName;

                if ((tempScene = new SceneTableEntry(rom, ofdOpenScene.FileName)) != null)
                {
                    if (ofdOpenRoom.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                    Configuration.LastRoomFile = ofdOpenRoom.FileName;

                    individualFileMode = true;
                    displayListsDirty = collisionDirty = waterboxesDirty = true;

                    ResetCurrentData(true);
                    tempScene.ReadScene((tempRooms = new HeaderCommands.Rooms(rom, tempScene, ofdOpenRoom.FileName)));
                    CreateSceneTree();

                    SetFormTitle();

                    openSceneToolStripMenuItem.Enabled = false;
                    closeSceneToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Status.Message = "Saving; please wait...";

            Cursor.Current = Cursors.WaitCursor;
            SaveAllData();
            Cursor.Current = DefaultCursor;

            RefreshCurrentData();
        }

        private void closeSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            individualFileMode = false;
            displayListsDirty = collisionDirty = waterboxesDirty = true;

            ResetCurrentData();
            CreateSceneTree();
            SetFormTitle();

            closeSceneToolStripMenuItem.Enabled = false;
            openSceneToolStripMenuItem.Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SaveAllData()
        {
            if (individualFileMode)
            {
                if (tempRooms.RoomInformation.Count != 1) throw new Exception("Zero or more than one individual room file loaded; this should not happen!");

                ParseStoreHeaders(tempScene.SceneHeaders, tempScene.Data, 0);
                ParseStoreHeaders(tempRooms.RoomInformation[0].Headers, tempRooms.RoomInformation[0].Data, 0);

                BinaryWriter bwScene = new BinaryWriter(File.Open(Configuration.LastSceneFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
                bwScene.Write(tempScene.Data);
                bwScene.Close();

                BinaryWriter bwRoom = new BinaryWriter(File.Open(Configuration.LastRoomFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
                bwRoom.Write(tempRooms.RoomInformation[0].Data);
                bwRoom.Close();
            }
            else
            {
                /* Store scene table entries & scenes */
                foreach (SceneTableEntry ste in rom.Scenes)
                {
                    ste.SaveTableEntry();
                    ParseStoreHeaders(ste.SceneHeaders, rom.Data, (int)ste.SceneStartAddress);
                }

                /* Store entrance table entries */
                foreach (EntranceTableEntry ete in rom.Entrances) ete.SaveTableEntry();

                /* Copy code data */
                Buffer.BlockCopy(rom.CodeData, 0, rom.Data, (int)rom.Code.PStart, rom.CodeData.Length);

                /* Write to file */
                BinaryWriter bw = new BinaryWriter(File.Open(rom.Filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
                bw.Write(rom.Data);
                bw.Close();
            }
        }

        private void ParseStoreHeaders(List<HeaderLoader> headers, byte[] databuf, int baseadr)
        {
            foreach (HeaderLoader hl in headers)
            {
                /* Fetch and parse room headers first */
                if (!individualFileMode)
                {
                    HeaderCommands.Rooms rooms = (hl.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms) as HeaderCommands.Rooms);
                    if (rooms != null)
                    {
                        foreach (HeaderCommands.Rooms.RoomInfoClass ric in rooms.RoomInformation) ParseStoreHeaders(ric.Headers, databuf, (int)ric.Start);
                    }
                }

                /* Now store all storeable commands */
                foreach (HeaderCommands.IStoreable hc in hl.Commands.Where(x => x is HeaderCommands.IStoreable))
                    hc.Store(databuf, baseadr);
            }
        }

        private void ResetCurrentData(bool norefresh = false)
        {
            currentScene = null;
            currentRoom = null;
            currentRoomTriangle = null;
            currentRoomVertex = null;

            if (!norefresh) RefreshCurrentData();
        }

        private void CreateStatusString()
        {
            List<string> infostrs = new List<string>();

            if (currentScene != null)
            {
                if (currentRoom == null)
                {
                    infostrs.Add(string.Format("{0}", currentScene.Name));

                    HeaderCommands.Rooms rooms = (currentScene.CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms) as HeaderCommands.Rooms);
                    if (rooms != null) infostrs.Add(string.Format("{0} room{1}", rooms.RoomInformation.Count, (rooms.RoomInformation.Count != 1 ? "s" : "")));
                }
                else if (currentRoom != null)
                {
                    infostrs.Add(string.Format("{0}, {1}", currentScene.Name, currentRoom.Description));
                }
            }
            else
            {
                infostrs.Add(string.Format("Ready{0}", ((Configuration.ShownIntelWarning || Configuration.ShownExtensionWarning) ? " (limited combiner)" : string.Empty)));
                if (rom != null) infostrs.Add(string.Format("{0} ({1}, v1.{2}; {3} scenes)", rom.Title, rom.GameID, rom.Version, rom.Scenes.Count));
            }

            if (currentRoom != null && currentRoom.ActiveRoomActorData != null)
            {
                infostrs.Add(string.Format("{0} room actor{1}", currentRoom.ActiveRoomActorData.ActorList.Count, (currentRoom.ActiveRoomActorData.ActorList.Count != 1 ? "s" : "")));
            }

            if (currentScene != null && currentScene.ActiveTransitionData != null && currentRoom == null)
            {
                infostrs.Add(string.Format("{0} transition actor{1}", currentScene.ActiveTransitionData.ActorList.Count, (currentScene.ActiveTransitionData.ActorList.Count != 1 ? "s" : "")));
            }

            if (currentScene != null && currentScene.ActiveSpawnPointData != null && currentRoom == null)
            {
                infostrs.Add(string.Format("{0} spawn point{1}", currentScene.ActiveSpawnPointData.ActorList.Count, (currentScene.ActiveSpawnPointData.ActorList.Count != 1 ? "s" : "")));
            }

            if (currentRoom != null && currentRoom.ActiveObjects != null)
            {
                infostrs.Add(string.Format("{0} object{1}", currentRoom.ActiveObjects.ObjectList.Count, (currentRoom.ActiveObjects.ObjectList.Count != 1 ? "s" : "")));
            }

            if (currentScene != null && currentScene.ActiveWaypoints != null && currentRoom == null)
            {
                infostrs.Add(string.Format("{0} path{1}", currentScene.ActiveWaypoints.Paths.Count, (currentScene.ActiveWaypoints.Paths.Count != 1 ? "s" : "")));
            }

            Program.Status.Message = string.Join("; ", infostrs);
        }

        private void RefreshCurrentData()
        {
            CreateStatusString();

            if (currentScene != null)
            {
                editAreaTitleCardToolStripMenuItem.Enabled = (currentScene.LabelStartAddress != 0 && currentScene.LabelEndAddress != 0);

                HeaderCommands.Rooms rooms = (currentScene.CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms) as HeaderCommands.Rooms);
                if (rooms != null)
                {
                    roomsForWaterboxSelection = new List<XMLActorDefinitionReader.Definition.Item.Option>();
                    roomsForWaterboxSelection.Add(new XMLActorDefinitionReader.Definition.Item.Option() { Description = "(All Rooms)", Value = 0x3F });
                    foreach (HeaderCommands.Rooms.RoomInfoClass ric in rooms.RoomInformation)
                        roomsForWaterboxSelection.Add(new XMLActorDefinitionReader.Definition.Item.Option() { Description = ric.Description, Value = ric.Number });
                }

                if (currentRoom == null)
                {
                    rom.SegmentMapping.Remove((byte)0x02);
                    rom.SegmentMapping.Add((byte)0x02, currentScene.Data);

                    allMeshHeaders = new List<HeaderCommands.MeshHeader>();

                    if (rooms != null)
                    {
                        foreach (HeaderLoader hl in rooms.RoomInformation.SelectMany(x => x.Headers))
                            allMeshHeaders.Add(hl.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.MeshHeader) as HeaderCommands.MeshHeader);
                    }
                    allMeshHeaders = allMeshHeaders.Distinct().ToList();
                }
                else if (currentRoom != null)
                {
                    rom.SegmentMapping.Remove((byte)0x02);
                    rom.SegmentMapping.Remove((byte)0x03);
                    rom.SegmentMapping.Add((byte)0x02, currentScene.Data);
                    rom.SegmentMapping.Add((byte)0x03, currentRoom.Data);
                }
            }
            else
            {
                editAreaTitleCardToolStripMenuItem.Enabled = false;
            }

            if (currentRoom != null && currentRoom.ActiveRoomActorData != null)
            {
                RefreshRoomActorList();
            }
            else
            {
                cbActors.Enabled = false;
                cbActors.DataSource = null;
            }

            if (currentScene != null && currentScene.ActiveTransitionData != null)
            {
                RefreshTransitionList();
            }
            else
            {
                cbTransitions.Enabled = false;
                cbTransitions.DataSource = null;
            }

            if (currentScene != null && currentScene.ActiveSpawnPointData != null)
            {
                RefreshSpawnPointList();
            }
            else
            {
                cbSpawnPoints.Enabled = false;
                cbSpawnPoints.DataSource = null;
            }

            if (currentScene != null && currentScene.ActiveSpecialObjs != null)
            {
                cbSpecialObjs.Enabled = true;
                cbSpecialObjs.DisplayMember = "Name";
                cbSpecialObjs.ValueMember = "ObjectNumber";
                cbSpecialObjs.DataSource = new BindingSource() { DataSource = HeaderCommands.SpecialObjects.Types };
                cbSpecialObjs.DataBindings.Clear();
                cbSpecialObjs.DataBindings.Add("SelectedValue", currentScene.ActiveSpecialObjs, "SelectedSpecialObjects");
            }
            else
            {
                cbSpecialObjs.Enabled = false;
                cbSpecialObjs.DataSource = null;
                cbSpecialObjs.DataBindings.Clear();
            }

            if (currentRoom != null && currentRoom.ActiveObjects != null)
            {
                dgvObjects.Enabled = true;
                dgvObjects.DataSource = new BindingSource() { DataSource = currentRoom.ActiveObjects.ObjectList };
                dgvObjects.Columns["Address"].Visible = false;
                dgvObjects.Columns["Number"].DefaultCellStyle.Format = "X4";
                //dgvObjects.Columns["Name"].ReadOnly = !ROM.HasFileNameTable;
                dgvObjects.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                dgvObjects.Enabled = false;
                dgvObjects.DataSource = null;
            }

            if (currentScene != null && currentScene.ActiveWaypoints != null)
            {
                RefreshWaypointPathList(currentScene.ActiveWaypoints);
            }
            else
            {
                cbPathHeaders.Enabled = false;
                cbPathHeaders.DataSource = null;
            }

            if (currentScene != null && currentScene.ActiveCollision != null)
            {
                RefreshCollisionPolyAndTypeLists();
            }
            else
            {
                cbCollisionPolys.Enabled = cbCollisionPolyTypes.Enabled = txtColPolyRawData.Enabled = nudColPolyType.Enabled = cbColPolyGroundTypes.Enabled = false;
                cbCollisionPolys.DataSource = cbCollisionPolyTypes.DataSource = cbColPolyGroundTypes.DataSource = null;
                txtColPolyRawData.Text = string.Empty;
            }

            if (currentScene != null && currentScene.ActiveCollision != null && currentScene.ActiveCollision.Waterboxes.Count > 0)
            {
                List<HeaderCommands.Collision.Waterbox> wblist = new List<HeaderCommands.Collision.Waterbox>();
                wblist.Add(new HeaderCommands.Collision.Waterbox());
                wblist.AddRange(currentScene.ActiveCollision.Waterboxes);

                waterboxComboDataBinding = new BindingSource();
                waterboxComboDataBinding.DataSource = wblist;
                cbWaterboxes.DataSource = waterboxComboDataBinding;
                cbWaterboxes.DisplayMember = "Description";
                cbWaterboxes.Enabled = true;
            }
            else
            {
                cbWaterboxes.Enabled = tlpExWaterboxes.Visible = false;
                cbWaterboxes.DataSource = null;
            }

            RefreshPathWaypoints();

            if (currentScene != null && currentScene.ActiveSettingsSoundScene != null)
            {
                cbSceneMetaBGM.Enabled = true;
                cbSceneMetaBGM.ValueMember = "Key";
                cbSceneMetaBGM.DisplayMember = "Value";
                cbSceneMetaBGM.DataSource = new BindingSource() { DataSource = bgms.OrderBy(x => x.Key).ToList() };
                cbSceneMetaBGM.DataBindings.Clear();
                cbSceneMetaBGM.DataBindings.Add("SelectedValue", currentScene.ActiveSettingsSoundScene, "TrackID");
                nudSceneMetaReverb.Value = currentScene.ActiveSettingsSoundScene.Reverb;
                nudSceneMetaNightSFX.Value = currentScene.ActiveSettingsSoundScene.NightSfxID;
                nudSceneMetaReverb.Enabled = nudSceneMetaNightSFX.Enabled = true;
            }
            else
            {
                cbSceneMetaBGM.Enabled = false;
                cbSceneMetaBGM.DataBindings.Clear();
                cbSceneMetaBGM.SelectedItem = null;
                nudSceneMetaReverb.Value = nudSceneMetaNightSFX.Value = 0;
                nudSceneMetaReverb.Enabled = nudSceneMetaNightSFX.Enabled = false;
            }

            collisionDirty = true;
            waterboxesDirty = true;
        }

        private void RefreshWaypointPathList(HeaderCommands.Waypoints wp)
        {
            if (wp == null) return;

            List<HeaderCommands.Waypoints.PathHeader> pathlist = new List<HeaderCommands.Waypoints.PathHeader>();
            pathlist.Add(new HeaderCommands.Waypoints.PathHeader());
            pathlist.AddRange(wp.Paths);

            waypointPathComboDataBinding = new BindingSource();
            waypointPathComboDataBinding.DataSource = pathlist;
            cbPathHeaders.DataSource = waypointPathComboDataBinding;
            cbPathHeaders.DisplayMember = "Description";
            cbPathHeaders.Enabled = true;
        }

        private void RefreshPathWaypoints()
        {
            if (activePathHeader != null && activePathHeader.Points != null)
            {
                dgvPathWaypoints.Enabled = true;
                dgvPathWaypoints.DataSource = new BindingSource() { DataSource = activePathHeader.Points };
                dgvPathWaypoints.ClearSelection();
                dgvPathWaypoints.Columns["Address"].Visible = false;
                dgvPathWaypoints.Columns["X"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvPathWaypoints.Columns["Y"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvPathWaypoints.Columns["Z"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            else
            {
                dgvPathWaypoints.Enabled = false;
                dgvPathWaypoints.DataSource = null;
            }
        }

        private void dgvObjects_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            if (dgv.Columns[e.ColumnIndex].Name == "Number")
            {
                if (e != null && e.Value != null && e.DesiredType.Equals(typeof(string)))
                {
                    try
                    {
                        e.Value = string.Format("0x{0:X4}", e.Value);
                        e.FormattingApplied = true;
                    }
                    catch
                    {
                        /* Not hexadecimal */
                    }
                }
            }
        }

        private void dgvObjects_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            if (dgv.Columns[e.ColumnIndex].Name == "Number")
            {
                if (e != null && e.Value != null && e.DesiredType.Equals(typeof(ushort)))
                {
                    string str = (e.Value as string);
                    bool ishex = str.StartsWith("0x");

                    ushort val = 0;
                    if (ushort.TryParse((ishex ? str.Substring(2) : str), (ishex ? System.Globalization.NumberStyles.AllowHexSpecifier : System.Globalization.NumberStyles.None),
                        System.Globalization.CultureInfo.InvariantCulture, out val))
                    {
                        e.Value = val;
                        e.ParsingApplied = true;
                    }
                }
            }
        }

        private void dgvObjects_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            int column = dgv.CurrentCell.ColumnIndex;
            string name = dgv.Columns[column].DataPropertyName;

            if (name.Equals("Name") && e.Control is TextBox)
            {
                TextBox tb = e.Control as TextBox;
                tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tb.AutoCompleteCustomSource = rom.ObjectNameACStrings;
                tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
        }

        private void dgvObjects_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is FormatException) System.Media.SystemSounds.Hand.Play();
        }

        private void tvScenes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is ROMHandler.ROMHandler)
                ResetCurrentData();
            if (e.Node.Tag is SceneTableEntry)
            {
                if (currentScene != (e.Node.Tag as SceneTableEntry))
                {
                    currentScene = (e.Node.Tag as SceneTableEntry);
                    currentScene.CurrentSceneHeader = currentScene.SceneHeaders[0];
                }
                currentRoom = null;
                currentRoomTriangle = null;
                currentRoomVertex = null;
            }
            else if (e.Node.Tag is HeaderLoader.HeaderPair)
            {
                HeaderLoader.HeaderPair hp = (e.Node.Tag as HeaderLoader.HeaderPair);

                if (hp.SceneHeader.Parent != currentScene) currentScene = (hp.SceneHeader.Parent as SceneTableEntry);
                currentScene.CurrentSceneHeader = hp.SceneHeader;

                currentRoom = null;
                currentRoomTriangle = null;
                currentRoomVertex = null;
            }
            else if (e.Node.Tag is HeaderCommands.Rooms.RoomInfoClass)
            {
                HeaderLoader.HeaderPair hp = (e.Node.Parent.Tag as HeaderLoader.HeaderPair);

                if (hp.SceneHeader.Parent != currentScene) currentScene = (hp.SceneHeader.Parent as SceneTableEntry);
                currentScene.CurrentSceneHeader = hp.SceneHeader;

                currentRoom = (e.Node.Tag as HeaderCommands.Rooms.RoomInfoClass);
                if (hp.SceneHeader.Number < currentRoom.Headers.Count)
                    currentRoom.CurrentRoomHeader = currentRoom.Headers[hp.SceneHeader.Number];

                currentRoomTriangle = null;
                currentRoomVertex = null;
            }

            RefreshCurrentData();
        }

        private void tvScenes_MouseUp(object sender, MouseEventArgs e)
        {
            SceneNavi.Controls.TreeViewEx tree = (sender as SceneNavi.Controls.TreeViewEx);

            if (e.Button == MouseButtons.Right)
            {
                Point pt = new Point(e.X, e.Y);
                tree.PointToClient(pt);

                TreeNode Node = tree.GetNodeAt(pt);
                if (Node != null)
                {
                    if (Node.Bounds.Contains(pt))
                    {
                        tree.SelectedNode = Node;
                        cmsSceneTree.Show(tree, pt);
                    }
                }
            }
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO  make more useful! print statistics about the object in question in the msgbox? like actor counts, etc?
            object tag = tvScenes.SelectedNode.Tag;

            if (tag is ROMHandler.ROMHandler)
            {
                //meh
                rOMInformationToolStripMenuItem_Click(rOMInformationToolStripMenuItem, EventArgs.Empty);
            }
            else if (tag is SceneTableEntry)
            {
                SceneTableEntry ste = (tag as SceneTableEntry);

                MessageBox.Show(
                    string.Format("Filename: {0}\nROM location: 0x{1:X} - 0x{2:X}\nScene headers: {3} headers", ste.DMAFilename, ste.SceneStartAddress, ste.SceneEndAddress, ste.SceneHeaders.Count),
                    "Scene Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (tag is HeaderLoader.HeaderPair)
            {
                HeaderLoader.HeaderPair hp = (tag as HeaderLoader.HeaderPair);
                MessageBox.Show(
                    string.Format("Stage: {0}\nScene header: #{1} (0x{2:X})\n", hp.Description, hp.SceneHeader.Number, hp.SceneHeader.Offset),
                    "Stage Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (tag is HeaderCommands.Rooms.RoomInfoClass)
            {
                HeaderCommands.Rooms.RoomInfoClass ric = (tag as HeaderCommands.Rooms.RoomInfoClass);
                MessageBox.Show(
                    string.Format("Filename: {0}\nROM location: 0x{1:X} - 0x{2:X}\nRoom headers: {3} headers", ric.DMAFilename, ric.Start, ric.End, ric.Headers.Count),
                    "Room Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void StartupExtensionChecks()
        {
            // !!
            // !!TODO!! clean up mess below <.<
            // !!


            // TODO  check for actual function addresses instead of just extension support
            // ex.    bool hasActiveTexture = ((GraphicsContext.CurrentContext as IGraphicsContextInternal).GetAddress("glActiveTexture") != IntPtr.Zero);
            // might help with intel support? at least on more modern intel chipsets? dunno, whatever, might be something to do for the future

            // TEMP TEMP  removed intel check until next public version, want feedback

            /* Check for those damn Intel chips and their shitty drivers(?), then disable combiner emulation if found. I'm sick of bug reports I can't fix because Intel is dumb. */
            /*if (Initialization.VendorIsIntel)
            {
                if (!Configuration.ShownIntelWarning)
                {
                    DisableCombiner(true, false);

                    Configuration.ShownIntelWarning = true;

                    MessageBox.Show(
                        "Your graphics hardware has been detected as being Intel-based. Because of known problems with Intel hardware and proper OpenGL support, " +
                        "combiner emulation has been disabled and correct graphics rendering cannot be guaranteed.", "Intel Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            */
            /* With Intel out of the way, check if all necessary GL extensions etc. are supported */
            supportsCreateShader = Initialization.SupportsFunction("glCreateShader");
            supportsGenProgramsARB = Initialization.SupportsFunction("glGenProgramsARB");

            StringBuilder extErrorMessages = new StringBuilder();
            List<string> extMissAll = new List<string>();

            List<string> extMissGeneral = Initialization.CheckForExtensions(requiredOglExtensionsGeneral);
            extMissAll.AddRange(extMissGeneral);
            if (extMissGeneral.Contains("GL_ARB_multisample"))
            {
                enableAntiAliasingToolStripMenuItem.Checked = Configuration.EnableAntiAliasing = false;
                enableAntiAliasingToolStripMenuItem.Enabled = false;
                extErrorMessages.AppendLine("Multisampling is not supported. Anti-aliasing support has been disabled.");
            }

            List<string> extMissCombinerGeneral = Initialization.CheckForExtensions(requiredOglExtensionsCombinerGeneral);
            extMissAll.AddRange(extMissCombinerGeneral);
            if (extMissCombinerGeneral.Contains("GL_ARB_multitexture"))
            {
                DisableCombiner(true, true);
                extErrorMessages.AppendLine("Multitexturing is not supported. Combiner emulation has been disabled and correct graphics rendering cannot be guaranteed.");
            }
            else
            {
                List<string> extMissARBCombiner = Initialization.CheckForExtensions(requiredOglExtensionsARBCombiner);
                extMissAll.AddRange(extMissARBCombiner);
                if (extMissARBCombiner.Count > 0 || !supportsGenProgramsARB)
                {
                    extErrorMessages.AppendLine("ARB Fragment Programs are not supported. ARB Assembly Combiner has been disabled.");
                }

                List<string> extMissGLSLCombiner = Initialization.CheckForExtensions(requiredOglExtensionsGLSLCombiner);
                extMissAll.AddRange(extMissGLSLCombiner);
                if (extMissGLSLCombiner.Count > 0)
                {
                    extErrorMessages.AppendLine("OpenGL Shading Language is not supported. GLSL Combiner has been disabled.");
                }

                DisableCombiner((extMissARBCombiner.Count > 0 || !supportsGenProgramsARB), (extMissGLSLCombiner.Count > 0));
            }

            if (extMissAll.Count > 0 || !supportsGenProgramsARB)
            {
                if (!Configuration.ShownExtensionWarning)
                {
                    Configuration.ShownExtensionWarning = true;

                    StringBuilder sb = new StringBuilder();

                    if (extMissAll.Count > 0)
                    {
                        sb.AppendFormat("The following OpenGL Extension{0} not supported by your hardware:\n", ((extMissAll.Count - 1) > 0 ? "s are" : " is"));
                        sb.AppendLine();
                        foreach (string str in extMissAll) sb.AppendFormat("* {0}\n", str);
                        sb.AppendLine();
                    }

                    if (!supportsGenProgramsARB)
                    {
                        //TODO make nicer, like exts above, not just bools?
                        sb.AppendFormat("The OpenGL function call glGenProgramARB is not supported by your hardware.");
                        sb.AppendLine();
                        sb.AppendLine();
                    }

                    sb.Append(extErrorMessages);

                    MessageBox.Show(sb.ToString(), "Extension Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void DisableCombiner(bool arb, bool glsl)
        {
            if ((arb && currentCombinerType == CombinerTypes.ArbCombiner) || (glsl && currentCombinerType == CombinerTypes.GLSLCombiner))
                currentCombinerType = CombinerTypes.None;

            foreach (ToolStripMenuItem tsmi in combinerTypeToolStripMenuItem.DropDownItems)
            {
                if (tsmi.Tag is CombinerTypes &&
                    ((((CombinerTypes)tsmi.Tag) == CombinerTypes.ArbCombiner && arb) ||
                    (((CombinerTypes)tsmi.Tag) == CombinerTypes.GLSLCombiner && glsl)))
                {
                    tsmi.Enabled = false;
                    tsmi.Checked = false;
                }
            }
        }

        private void customGLControl_Load(object sender, EventArgs e)
        {
            SettingsGUIInit();

            StartupExtensionChecks();

            Initialization.SetDefaults();

            glText = new QuickFontWrapper(new Font("Verdana", 10.0f, FontStyle.Bold));
            camera = new Camera();
            fpsMonitor = new FPSMonitor();

            ready = true;
        }

        private void customGLControl_Paint(object sender, PaintEventArgs e)
        {
            if (!ready) return;

            try
            {
                fpsMonitor.Update();

                RenderInit(((GLControl)sender).Width, ((GLControl)sender).Height);

                if (rom != null && rom.Loaded)
                {
                    /* Scene/rooms */
                    RenderScene();

                    /* Prepare for actors */
                    GL.PushAttrib(AttribMask.AllAttribBits);
                    GL.Disable(EnableCap.Texture2D);
                    GL.Disable(EnableCap.Lighting);
                    if (supportsGenProgramsARB) GL.Disable((EnableCap)All.FragmentProgram);
                    if (supportsCreateShader) GL.UseProgram(0);
                    {
                        /* Room actors */
                        if (Configuration.RenderRoomActors && currentRoom != null && currentRoom.ActiveRoomActorData != null)
                            foreach (HeaderCommands.Actors.Entry ac in currentRoom.ActiveRoomActorData.ActorList)
                                ac.Render(ac == (cbActors.SelectedItem as HeaderCommands.Actors.Entry) &&
                                    cbActors.Visible ? HeaderCommands.PickableObjectRenderType.Selected : HeaderCommands.PickableObjectRenderType.Normal);

                        /* Spawn points */
                        if (Configuration.RenderSpawnPoints && currentScene != null && currentScene.ActiveSpawnPointData != null)
                            foreach (HeaderCommands.Actors.Entry ac in currentScene.ActiveSpawnPointData.ActorList)
                                ac.Render(ac == (cbSpawnPoints.SelectedItem as HeaderCommands.Actors.Entry) &&
                                    cbSpawnPoints.Visible ? HeaderCommands.PickableObjectRenderType.Selected : HeaderCommands.PickableObjectRenderType.Normal);

                        /* Transitions */
                        if (Configuration.RenderTransitions && currentScene != null && currentScene.ActiveTransitionData != null)
                            foreach (HeaderCommands.Actors.Entry ac in currentScene.ActiveTransitionData.ActorList)
                                ac.Render(ac == (cbTransitions.SelectedItem as HeaderCommands.Actors.Entry) &&
                                    cbTransitions.Visible ? HeaderCommands.PickableObjectRenderType.Selected : HeaderCommands.PickableObjectRenderType.Normal);

                        /* Path waypoints */
                        if (Configuration.RenderPathWaypoints && activePathHeader != null && activePathHeader.Points != null)
                        {
                            /* Link waypoints? */
                            if (Configuration.LinkAllWPinPath)
                            {
                                GL.LineWidth(4.0f);
                                GL.Color3(0.25, 0.5, 1.0);

                                GL.Begin(BeginMode.LineStrip);
                                foreach (HeaderCommands.Waypoints.Waypoint wp in activePathHeader.Points) GL.Vertex3(wp.X, wp.Y, wp.Z);
                                GL.End();
                            }

                            HeaderCommands.Waypoints.Waypoint selwp = (dgvPathWaypoints.SelectedCells.Count != 0 ? dgvPathWaypoints.SelectedCells[0].OwningRow.DataBoundItem as HeaderCommands.Waypoints.Waypoint : null);
                            foreach (HeaderCommands.Waypoints.Waypoint wp in activePathHeader.Points)
                                wp.Render(wp == selwp && cbPathHeaders.Visible ? HeaderCommands.PickableObjectRenderType.Selected : HeaderCommands.PickableObjectRenderType.Normal);
                        }
                    }
                    GL.PopAttrib();

                    /* Collision */
                    if (Configuration.RenderCollision && currentScene != null && currentScene.ActiveCollision != null)
                    {
                        if (!collisionDirty && collisionDL != null)
                        {
                            collisionDL.Render();
                        }
                        else
                        {
                            collisionDirty = false;

                            if (collisionDL != null) collisionDL.Dispose();
                            collisionDL = new DisplayList(ListMode.CompileAndExecute);

                            GL.PushAttrib(AttribMask.AllAttribBits);
                            GL.Disable(EnableCap.Texture2D);
                            GL.Disable(EnableCap.Lighting);
                            if (supportsGenProgramsARB) GL.Disable((EnableCap)All.FragmentProgram);
                            if (supportsCreateShader) GL.UseProgram(0);
                            GL.DepthRange(0.0, 0.99999);

                            if (Configuration.RenderCollisionAsWhite) GL.Color4(1.0, 1.0, 1.0, 0.5);

                            GL.Begin(BeginMode.Triangles);
                            foreach (HeaderCommands.Collision.Polygon poly in currentScene.ActiveCollision.Polygons)
                            {
                                if (poly == currentCollisionPolygon && cbCollisionPolys.Visible)
                                {
                                    GL.Color4(0.5, 0.5, 1.0, 0.5);
                                    poly.Render(HeaderCommands.PickableObjectRenderType.NoColor);
                                    if (Configuration.RenderCollisionAsWhite) GL.Color4(1.0, 1.0, 1.0, 0.5);
                                }
                                else
                                {
                                    if (Configuration.RenderCollisionAsWhite)
                                        poly.Render(HeaderCommands.PickableObjectRenderType.NoColor);
                                    else
                                        poly.Render(HeaderCommands.PickableObjectRenderType.Normal);
                                }
                            }
                            GL.End();

                            GL.DepthRange(0.0, 0.99998);
                            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                            GL.LineWidth(2.0f);
                            GL.Color3(Color.Black);
                            GL.Begin(BeginMode.Triangles);
                            foreach (HeaderCommands.Collision.Polygon poly in currentScene.ActiveCollision.Polygons) poly.Render(HeaderCommands.PickableObjectRenderType.NoColor);
                            GL.End();

                            GL.PopAttrib();

                            collisionDL.End();
                        }
                    }

                    /* Waterboxes */
                    if (Configuration.RenderWaterboxes && currentScene != null && currentScene.ActiveCollision != null)
                    {
                        if (!waterboxesDirty && waterboxDL != null)
                        {
                            waterboxDL.Render();
                        }
                        else
                        {
                            waterboxesDirty = false;

                            if (waterboxDL != null) waterboxDL.Dispose();
                            waterboxDL = new DisplayList(ListMode.CompileAndExecute);

                            GL.PushAttrib(AttribMask.AllAttribBits);
                            GL.Disable(EnableCap.Texture2D);
                            GL.Disable(EnableCap.Lighting);
                            if (supportsGenProgramsARB) GL.Disable((EnableCap)All.FragmentProgram);
                            if (supportsCreateShader) GL.UseProgram(0);
                            GL.Disable(EnableCap.CullFace);

                            GL.Begin(BeginMode.Quads);
                            foreach (HeaderCommands.Collision.Waterbox wb in currentScene.ActiveCollision.Waterboxes)
                            {
                                double alpha = ((Configuration.ShowWaterboxesPerRoom && currentRoom != null && (wb.RoomNumber != currentRoom.Number && wb.RoomNumber != 0x3F)) ? 0.1 : 0.5);

                                if (wb == currentWaterbox && cbWaterboxes.Visible)
                                    GL.Color4(0.5, 1.0, 0.5, alpha);
                                else
                                    GL.Color4(0.0, 0.5, 1.0, alpha);

                                wb.Render(HeaderCommands.PickableObjectRenderType.Normal);
                            }
                            GL.End();

                            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                            GL.LineWidth(2.0f);
                            GL.Begin(BeginMode.Quads);
                            foreach (HeaderCommands.Collision.Waterbox wb in currentScene.ActiveCollision.Waterboxes)
                            {
                                double alpha = ((Configuration.ShowWaterboxesPerRoom && currentRoom != null && (wb.RoomNumber != currentRoom.Number && wb.RoomNumber != 0x3F)) ? 0.1 : 0.5);
                                GL.Color4(0.0, 0.0, 0.0, alpha);
                                wb.Render(HeaderCommands.PickableObjectRenderType.Normal);
                            }
                            GL.End();

                            GL.Enable(EnableCap.CullFace);
                            GL.PopAttrib();

                            GL.Color4(Color.White);

                            waterboxDL.End();
                        }
                    }

                    /* Render selected room triangle overlay */
                    if (currentRoomTriangle != null && !Configuration.RenderCollision)
                    {
                        currentRoomTriangle.Render(HeaderCommands.PickableObjectRenderType.Normal);
                    }

                    /* 2D text overlay */
                    RenderTextOverlay();
                }

                ((GLControl)sender).SwapBuffers();
            }
            catch (EntryPointNotFoundException)
            {
                //
            }
        }

        private void RenderInit(int width, int height)
        {
            GL.ClearColor(System.Drawing.Color.LightBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Initialization.SetViewport(width, height);
            camera.Position();
            GL.Scale(0.02, 0.02, 0.02);
        }

        private void RenderScene()
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            //if (CurrentScene != null && CurrentScene.CurrentSceneHeader != null) CurrentScene.ActiveEnvSettings.EnvSettingList[0].CreateLighting();

            if (currentRoom != null && currentRoom.ActiveMeshHeader != null)
            {
                /* Render single room */
                RenderMeshHeader(currentRoom.ActiveMeshHeader);
                displayListsDirty = false;
            }
            else if (currentScene != null && currentScene.CurrentSceneHeader != null)
            {
                /* Render all rooms */
                foreach (HeaderCommands.Rooms.RoomInfoClass ric in
                    (currentScene.CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms) as HeaderCommands.Rooms).RoomInformation)
                {
                    rom.SegmentMapping.Remove((byte)0x02);
                    rom.SegmentMapping.Remove((byte)0x03);
                    rom.SegmentMapping.Add((byte)0x02, (ric.Parent as SceneTableEntry).Data);
                    rom.SegmentMapping.Add((byte)0x03, ric.Data);

                    if (ric.Headers.Count == 0) continue;

                    HeaderCommands.MeshHeader mh = (ric.Headers[0].Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.MeshHeader) as HeaderCommands.MeshHeader);
                    if (mh == null) continue;

                    RenderMeshHeader(mh);
                }
                displayListsDirty = false;
            }

            GL.PopAttrib();
        }

        private void RenderMeshHeader(HeaderCommands.MeshHeader mh)
        {
            if (mh.DLs == null || displayListsDirty || mh.CachedWithTextures != Configuration.RenderTextures || mh.CachedWithCombinerType != Configuration.CombinerType)
            {
                /* Display lists aren't yet cached OR cached DLs are wrong */
                if (mh.DLs != null)
                {
                    foreach (DisplayList gldl in mh.DLs) gldl.Dispose();
                    mh.DLs.Clear();
                }

                mh.CreateDisplayLists(Configuration.RenderTextures, Configuration.CombinerType);
                RefreshCurrentData();
            }

            /* Render DLs */
            foreach (DisplayList gldl in mh.DLs) gldl.Render();
        }

        private void RenderTextOverlay()
        {
            glText.Begin();
            if (!Configuration.OGLVSync) glText.Print(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00} FPS", fpsMonitor.Value), new Vector2d(10.0, 10.0));
            glText.End();
        }

        private HeaderCommands.IPickableObject TryPickObject(int x, int y, bool moveable)
        {
            if (currentScene == null) return null;

            HeaderCommands.IPickableObject picked = null;
            List<HeaderCommands.IPickableObject> pickobjs = new List<HeaderCommands.IPickableObject>();

            /* Room model triangle vertices */
            if (!Configuration.RenderCollision && currentRoomTriangle != null)
                pickobjs.AddRange(currentRoomTriangle.Vertices);

            /* Room model triangles */
            if (currentRoom != null && currentRoom.ActiveMeshHeader != null && !Configuration.RenderCollision &&
                currentRoom.ActiveMeshHeader.DLs.Count > 0 && currentRoom.ActiveMeshHeader.DLs[0].Triangles.Count > 0)
            {
                if (currentRoom.ActiveMeshHeader.DLs[0].Triangles[0].IsMoveable == moveable)
                {
                    foreach (DisplayListEx dlex in currentRoom.ActiveMeshHeader.DLs)
                        pickobjs.AddRange(dlex.Triangles);
                }
            }

            /* Rooms */
            if (allMeshHeaders != null && currentRoom == null && !Configuration.RenderCollision && allMeshHeaders.Count > 0 && allMeshHeaders[0].IsMoveable == moveable)
                pickobjs.AddRange(allMeshHeaders);

            /* Room actors */
            if (currentRoom != null && currentRoom.ActiveRoomActorData != null && Configuration.RenderRoomActors && currentRoom.ActiveRoomActorData.ActorList.Count > 0 &&
                currentRoom.ActiveRoomActorData.ActorList[0].IsMoveable == moveable)
                pickobjs.AddRange(currentRoom.ActiveRoomActorData.ActorList);

            /* Spawn points */
            if (currentScene.ActiveSpawnPointData != null && Configuration.RenderSpawnPoints && currentScene.ActiveSpawnPointData.ActorList.Count > 0 &&
                currentScene.ActiveSpawnPointData.ActorList[0].IsMoveable == moveable)
                pickobjs.AddRange(currentScene.ActiveSpawnPointData.ActorList);

            /* Transition actors */
            if (currentScene.ActiveTransitionData != null && Configuration.RenderTransitions && currentScene.ActiveTransitionData.ActorList.Count > 0 &&
                currentScene.ActiveTransitionData.ActorList[0].IsMoveable == moveable)
                pickobjs.AddRange(currentScene.ActiveTransitionData.ActorList);

            /* Waypoints */
            if (activePathHeader != null && activePathHeader.Points != null && Configuration.RenderPathWaypoints && activePathHeader.Points.Count > 0 &&
                activePathHeader.Points[0].IsMoveable == moveable)
                pickobjs.AddRange(activePathHeader.Points);

            /* Waterboxes */
            if (currentScene.ActiveCollision != null && Configuration.RenderWaterboxes && currentScene.ActiveCollision.Waterboxes.Count > 0 &&
                currentScene.ActiveCollision.Waterboxes[0].IsMoveable == moveable)
                pickobjs.AddRange(currentScene.ActiveCollision.Waterboxes);

            /* Collision polygons */
            if (currentScene.ActiveCollision != null && Configuration.RenderCollision && currentScene.ActiveCollision.Polygons.Count > 0 &&
                currentScene.ActiveCollision.Polygons[0].IsMoveable == moveable)
                pickobjs.AddRange(currentScene.ActiveCollision.Polygons);

            if ((picked = DoPicking(x, y, pickobjs)) != null)
            {
                /* Wrong mode? */
                if (picked.IsMoveable != moveable) return null;

                /* What's been picked...? */
                if (picked is HeaderCommands.Waypoints.Waypoint)
                {
                    dgvPathWaypoints.ClearSelection();
                    DataGridViewRow row = dgvPathWaypoints.Rows.OfType<DataGridViewRow>().FirstOrDefault(xx => xx.DataBoundItem == picked as HeaderCommands.Waypoints.Waypoint);
                    if (row == null) return null;
                    row.Cells["X"].Selected = true;
                    tabControl1.SelectTab(tpWaypoints);
                }
                else if (picked is HeaderCommands.Actors.Entry)
                {
                    HeaderCommands.Actors.Entry actor = (picked as HeaderCommands.Actors.Entry);

                    if (actor.IsSpawnPoint)
                    {
                        cbSpawnPoints.SelectedItem = actor;
                        tabControl1.SelectTab(tpSpawnPoints);
                    }
                    else if (actor.IsTransitionActor)
                    {
                        cbTransitions.SelectedItem = actor;
                        tabControl1.SelectTab(tpTransitions);
                    }
                    else
                    {
                        cbActors.SelectedItem = actor;
                        tabControl1.SelectTab(tpRoomActors);
                    }
                }
                else if (picked is HeaderCommands.Collision.Polygon)
                {
                    currentCollisionPolygon = (picked as HeaderCommands.Collision.Polygon);

                    cbCollisionPolys.SelectedItem = currentCollisionPolygon;
                    tabControl1.SelectTab(tpCollision);
                }
                else if (picked is HeaderCommands.Collision.Waterbox)
                {
                    currentWaterbox = (picked as HeaderCommands.Collision.Waterbox);

                    cbWaterboxes.SelectedItem = currentWaterbox;
                    tabControl1.SelectTab(tpWaterboxes);
                }
                else if (picked is HeaderCommands.MeshHeader)
                {
                    tvScenes.SelectedNode = tvScenes.FlattenTree().FirstOrDefault(xx =>
                        xx.Tag == ((picked as HeaderCommands.MeshHeader).Parent as HeaderCommands.Rooms.RoomInfoClass) &&
                        (xx.Parent.Tag as HeaderLoader.HeaderPair).SceneHeader.Number == currentScene.CurrentSceneHeader.Number);
                }
                else if (picked is OpenGLHelpers.DisplayListEx.Triangle)
                {
                    if (currentRoomTriangle != picked)
                    {
                        if (currentRoomTriangle != null) currentRoomTriangle.SelectedVertex = null;

                        currentRoomTriangle = (picked as OpenGLHelpers.DisplayListEx.Triangle);
                        currentRoomVertex = null;
                    }
                }
                else if (picked is SimpleF3DEX2.Vertex)
                {
                    currentRoomTriangle.SelectedVertex = currentRoomVertex = (picked as SimpleF3DEX2.Vertex);
                }
            }

            return picked;
        }

        private HeaderCommands.IPickableObject DoPicking(int x, int y, List<HeaderCommands.IPickableObject> objlist)
        {
            /* It's MAGIC! I fucking hate picking and shit. */
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Blend);
            if (supportsGenProgramsARB) GL.Disable((EnableCap)All.FragmentProgram);
            if (supportsCreateShader) GL.UseProgram(0);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (HeaderCommands.IPickableObject obj in objlist)
            {
                if (obj is HeaderCommands.Collision.Polygon || obj is OpenGLHelpers.DisplayListEx.Triangle)
                    GL.Enable(EnableCap.CullFace);
                else
                    GL.Disable(EnableCap.CullFace);

                obj.Render(HeaderCommands.PickableObjectRenderType.Picking);
            }

            GL.PopAttrib();

            byte[] pixel = new byte[3];
            int[] viewport = new int[4];

            GL.GetInteger(GetPName.Viewport, viewport);
            GL.ReadPixels(x, viewport[3] - y, 1, 1, PixelFormat.Bgr, PixelType.UnsignedByte, pixel);
            int argb = (int)pixel[0] + (((int)pixel[1]) << 8) + (((int)pixel[2]) << 16);

            return objlist.FirstOrDefault(xx => (xx.PickColor.ToArgb() & 0xFFFFFF) == argb);
        }

        private void customGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            camera.ButtonsDown |= e.Button;

            switch (currentToolMode)
            {
                case ToolModes.Camera:
                    {
                        /* Camera only */
                        if (Convert.ToBoolean(camera.ButtonsDown & MouseButtons.Left))
                            camera.MouseCenter(new Vector2d(e.X, e.Y));
                        break;
                    }

                case ToolModes.MoveableObjs:
                case ToolModes.StaticObjs:
                    {
                        /* Object picking */
                        if (Convert.ToBoolean(camera.ButtonsDown & MouseButtons.Left) || Convert.ToBoolean(camera.ButtonsDown & MouseButtons.Middle))
                        {
                            pickedObject = TryPickObject(e.X, e.Y, (currentToolMode == ToolModes.MoveableObjs));
                            if (pickedObject == null)
                            {
                                /* No pick? Camera */
                                camera.MouseCenter(new Vector2d(e.X, e.Y));
                            }
                            else
                            {
                                /* Object found */
                                pickObjLastPosition = pickObjPosition = new Vector2d(e.X, e.Y);
                                pickObjDisplacement = Vector2d.Zero;
                                ((Control)sender).Focus();

                                /* Mark GLDLs as dirty? */
                                collisionDirty = (pickedObject is HeaderCommands.Collision.Polygon);
                                waterboxesDirty = (pickedObject is HeaderCommands.Collision.Waterbox);

                                /* Static object? Camera */
                                if (currentToolMode == ToolModes.StaticObjs)
                                {
                                    camera.MouseCenter(new Vector2d(e.X, e.Y));
                                    /*if (e.Clicks == 2 && currentRoomVertex != null)
                                    {
                                        EditVertexColor(currentRoomVertex);
                                    }*/
                                }
                            }
                        }
                        else if (Convert.ToBoolean(camera.ButtonsDown & MouseButtons.Right))
                        {
                            pickedObject = TryPickObject(e.X, e.Y, (currentToolMode == ToolModes.MoveableObjs));
                            if (pickedObject != null)
                            {
                                if (currentToolMode == ToolModes.MoveableObjs)
                                {
                                    if (pickedObject is HeaderCommands.Actors.Entry)
                                    {
                                        HeaderCommands.Actors.Entry ac = (pickedObject as HeaderCommands.Actors.Entry);
                                        /* Determine what menu entries should be enabled */
                                        xAxisToolStripMenuItem.Enabled = !(ac.Definition.Items.FirstOrDefault(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.RotationX) == null);
                                        yAxisToolStripMenuItem.Enabled = !(ac.Definition.Items.FirstOrDefault(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.RotationY) == null);
                                        zAxisToolStripMenuItem.Enabled = !(ac.Definition.Items.FirstOrDefault(x => x.Usage == XMLActorDefinitionReader.Definition.Item.Usages.RotationZ) == null);
                                        rotateToolStripMenuItem.Enabled = (xAxisToolStripMenuItem.Enabled || yAxisToolStripMenuItem.Enabled || zAxisToolStripMenuItem.Enabled);
                                    }
                                    else
                                        rotateToolStripMenuItem.Enabled = false;

                                    cmsMoveableObjectEdit.Show(((Control)sender).PointToScreen(e.Location));
                                }
                                else if (currentToolMode == ToolModes.StaticObjs)
                                {
                                    if (pickedObject is SimpleF3DEX2.Vertex)
                                    {
                                        cmsVertexEdit.Show(((Control)sender).PointToScreen(e.Location));
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }

        private void customGLControl_MouseUp(object sender, MouseEventArgs e)
        {
            camera.ButtonsDown &= ~e.Button;
        }

        private void customGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            switch (currentToolMode)
            {
                case ToolModes.Camera:
                    {
                        if (Convert.ToBoolean(e.Button & MouseButtons.Left))
                            camera.MouseMove(new Vector2d(e.X, e.Y));
                        break;
                    }

                case ToolModes.MoveableObjs:
                    {
                        if (!Convert.ToBoolean(e.Button & MouseButtons.Left) && !Convert.ToBoolean(e.Button & MouseButtons.Middle)) break;

                        if (pickedObject == null)
                            camera.MouseMove(new Vector2d(e.X, e.Y));
                        else
                        {
                            // TODO  make this not shitty; try to get the "new method" to work with anything that's not at (0,0,0)

                            /* Determine mouse position and displacement */
                            pickObjPosition = new Vector2d(e.X, e.Y);
                            pickObjDisplacement = (pickObjPosition - pickObjLastPosition);

                            /* No displacement? Exit */
                            if (pickObjDisplacement == Vector2d.Zero) return;

                            /* Calculate camera rotation */
                            double CamXRotd = camera.Rot.X * (double)(Math.PI / 180);
                            double CamYRotd = camera.Rot.Y * (double)(Math.PI / 180);

                            /* Speed modifiers */
                            double movemod = 3.0;
                            if (keysDown[(ushort)Keys.Space]) movemod = 8.0;
                            else if (keysDown[(ushort)Keys.ShiftKey]) movemod = 0.75;

                            /* WARNING: Cam position stuff below is "I dunno why it works, but it does!" */
                            Vector3d objpos = pickedObject.Position;

                            if (Convert.ToBoolean(e.Button & MouseButtons.Middle) || (Convert.ToBoolean(e.Button & MouseButtons.Left) && keysDown[(ushort)Keys.ControlKey]))
                            {
                                /* Middle mouse button OR left button + Ctrl -> move forward/backward */
                                objpos.X += ((Math.Sin(CamYRotd) * -pickObjDisplacement.Y) * movemod);
                                objpos.Z -= ((Math.Cos(CamYRotd) * -pickObjDisplacement.Y) * movemod);

                                camera.Pos.X -= ((Math.Sin(CamYRotd) * (-pickObjDisplacement.Y * camera.CameraCoeff * camera.Sensitivity) / 1.25) * movemod);
                                camera.Pos.Z += ((Math.Cos(CamYRotd) * (-pickObjDisplacement.Y * camera.CameraCoeff * camera.Sensitivity) / 1.25) * movemod);
                            }
                            else if (Convert.ToBoolean(e.Button & MouseButtons.Left))
                            {
                                /* Left mouse button -> move up/down/left/right */
                                objpos.X += ((Math.Cos(CamYRotd) * pickObjDisplacement.X) * movemod);
                                objpos.Y -= pickObjDisplacement.Y * movemod;
                                objpos.Z += ((Math.Sin(CamYRotd) * pickObjDisplacement.X) * movemod);

                                camera.Pos.X -= ((Math.Cos(CamYRotd) * (pickObjDisplacement.X * camera.CameraCoeff * camera.Sensitivity) / 1.25) * movemod);
                                camera.Pos.Y += ((pickObjDisplacement.Y * camera.CameraCoeff * camera.Sensitivity) / 1.25) * movemod;
                                camera.Pos.Z -= ((Math.Sin(CamYRotd) * (pickObjDisplacement.X * camera.CameraCoeff * camera.Sensitivity) / 1.25) * movemod);
                            }

                            /* Round away decimal places (mainly for waypoints) */
                            objpos.X = Math.Round(objpos.X, 0);
                            objpos.Y = Math.Round(objpos.Y, 0);
                            objpos.Z = Math.Round(objpos.Z, 0);
                            pickedObject.Position = objpos;

                            /* Refresh GUI according to type of picked object */
                            if (pickedObject is HeaderCommands.Waypoints.Waypoint)
                            {
                                foreach (DataGridViewCell cell in dgvPathWaypoints.SelectedCells)
                                {
                                    for (int i = 0; i < dgvPathWaypoints.ColumnCount; i++) dgvPathWaypoints.UpdateCellValue(i, cell.RowIndex);
                                }
                            }
                            else if (pickedObject is HeaderCommands.Actors.Entry)
                            {
                                HeaderCommands.Actors.Entry actor = (pickedObject as HeaderCommands.Actors.Entry);

                                if (actor.IsSpawnPoint)
                                    XMLActorDefinitionReader.RefreshActorPositionRotation(actor, tlpExSpawnPoints);
                                else if (actor.IsTransitionActor)
                                    XMLActorDefinitionReader.RefreshActorPositionRotation(actor, tlpExTransitions);
                                else
                                    XMLActorDefinitionReader.RefreshActorPositionRotation(actor, tlpExRoomActors);
                            }
                            else if (pickedObject is HeaderCommands.Collision.Waterbox)
                            {
                                waterboxesDirty = true;
                                RefreshWaterboxControls();
                            }

                            pickObjLastPosition = pickObjPosition;

                            ((Control)sender).Focus();
                        }
                        break;
                    }

                case ToolModes.StaticObjs:
                    {
                        if (Convert.ToBoolean(e.Button & MouseButtons.Left)/* && PickedObject == null*/)
                            camera.MouseMove(new Vector2d(e.X, e.Y));
                        break;
                    }
            }
        }

        private void customGLControl_KeyDown(object sender, KeyEventArgs e)
        {
            keysDown[(ushort)e.KeyValue] = true;
        }

        private void customGLControl_KeyUp(object sender, KeyEventArgs e)
        {
            keysDown[(ushort)e.KeyValue] = false;
        }

        private void customGLControl_Leave(object sender, EventArgs e)
        {
            keysDown.Fill(new bool[] { false });
        }

        private void EditVertexColor(SimpleF3DEX2.Vertex vertex)
        {
            ColorPickerDialog cdlg = new ColorPickerDialog(Color.FromArgb(vertex.Colors[3], vertex.Colors[0], vertex.Colors[1], vertex.Colors[2]));

            if (cdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vertex.Colors[0] = cdlg.Color.R;
                vertex.Colors[1] = cdlg.Color.G;
                vertex.Colors[2] = cdlg.Color.B;
                vertex.Colors[3] = cdlg.Color.A;

                // KLUDGE! Write to local room data HERE for rendering, write to ROM in SimpleF3DEX2.Vertex, the vertex.Store(...) below
                currentRoom.Data[(vertex.Address & 0xFFFFFF) + 12] = vertex.Colors[0];
                currentRoom.Data[(vertex.Address & 0xFFFFFF) + 13] = vertex.Colors[1];
                currentRoom.Data[(vertex.Address & 0xFFFFFF) + 14] = vertex.Colors[2];
                currentRoom.Data[(vertex.Address & 0xFFFFFF) + 15] = vertex.Colors[3];

                vertex.Store(individualFileMode ? null : rom.Data, (int)currentRoom.Start);

                displayListsDirty = true;
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            collisionDirty = (e.Action == TabControlAction.Selecting && e.TabPage == tpCollision || lastTabPage == tpCollision);
            waterboxesDirty = (e.Action == TabControlAction.Selecting && e.TabPage == tpWaterboxes || lastTabPage == tpWaterboxes);

            lastTabPage = e.TabPage;
        }

        private void nudSceneMetaReverb_ValueChanged(object sender, EventArgs e)
        {
            if (currentScene != null && currentScene.ActiveSettingsSoundScene != null) currentScene.ActiveSettingsSoundScene.Reverb = (byte)((NumericUpDown)sender).Value;
        }

        private void nudSceneMetaNightSFX_ValueChanged(object sender, EventArgs e)
        {
            if (currentScene != null && currentScene.ActiveSettingsSoundScene != null) currentScene.ActiveSettingsSoundScene.NightSfxID = (byte)((NumericUpDown)sender).Value;
        }

        private void RefreshRoomActorList()
        {
            List<HeaderCommands.Actors.Entry> actorlist = new List<HeaderCommands.Actors.Entry>();
            actorlist.Add(new HeaderCommands.Actors.Entry());
            actorlist.AddRange(currentRoom.ActiveRoomActorData.ActorList);

            roomActorComboBinding = new BindingSource();
            roomActorComboBinding.DataSource = actorlist;
            cbActors.DataSource = roomActorComboBinding;
            cbActors.DisplayMember = "Description";
            cbActors.Enabled = true;
        }

        private void RefreshTransitionList()
        {
            List<HeaderCommands.Actors.Entry> actorlist = new List<HeaderCommands.Actors.Entry>();
            actorlist.Add(new HeaderCommands.Actors.Entry());
            actorlist.AddRange(currentScene.ActiveTransitionData.ActorList);

            transitionComboBinding = new BindingSource();
            transitionComboBinding.DataSource = actorlist;
            cbTransitions.DataSource = transitionComboBinding;
            cbTransitions.DisplayMember = "Description";
            cbTransitions.Enabled = true;
        }

        private void RefreshSpawnPointList()
        {
            List<HeaderCommands.Actors.Entry> actorlist = new List<HeaderCommands.Actors.Entry>();
            actorlist.Add(new HeaderCommands.Actors.Entry());
            actorlist.AddRange(currentScene.ActiveSpawnPointData.ActorList);

            spawnPointComboBinding = new BindingSource();
            spawnPointComboBinding.DataSource = actorlist;
            cbSpawnPoints.DataSource = spawnPointComboBinding;
            cbSpawnPoints.DisplayMember = "Description";
            cbSpawnPoints.Enabled = true;
        }

        private void cbActors_SelectedIndexChanged(object sender, EventArgs e)
        {
            HeaderCommands.Actors.Entry ac = ((ComboBox)sender).SelectedItem as HeaderCommands.Actors.Entry;
            pickedObject = (ac as HeaderCommands.IPickableObject);

            XMLActorDefinitionReader.CreateActorEditingControls(ac, tlpExRoomActors, new Action(() =>
            {
                int idx = ((ComboBox)sender).SelectedIndex;
                RefreshRoomActorList();
                ((ComboBox)sender).SelectedIndex = idx;
                SelectActorNumberControl(tlpExRoomActors);
            }), individual: individualFileMode);
        }

        private void cbTransitions_SelectedIndexChanged(object sender, EventArgs e)
        {
            HeaderCommands.Actors.Entry ac = ((ComboBox)sender).SelectedItem as HeaderCommands.Actors.Entry;
            pickedObject = (ac as HeaderCommands.IPickableObject);

            HeaderCommands.Rooms rooms = null;
            if (currentScene != null && currentScene.CurrentSceneHeader != null)
                rooms = currentScene.CurrentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms) as HeaderCommands.Rooms;

            XMLActorDefinitionReader.CreateActorEditingControls(ac, tlpExTransitions, new Action(() =>
            {
                int idx = ((ComboBox)sender).SelectedIndex;
                RefreshTransitionList();
                ((ComboBox)sender).SelectedIndex = idx;
                SelectActorNumberControl(tlpExTransitions);
            }), (rooms != null ? rooms.RoomInformation : null), individualFileMode);
        }

        private void cbSpawnPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            HeaderCommands.Actors.Entry ac = ((ComboBox)sender).SelectedItem as HeaderCommands.Actors.Entry;
            pickedObject = (ac as HeaderCommands.IPickableObject);

            XMLActorDefinitionReader.CreateActorEditingControls(ac, tlpExSpawnPoints, new Action(() =>
            {
                int idx = ((ComboBox)sender).SelectedIndex;
                RefreshSpawnPointList();
                ((ComboBox)sender).SelectedIndex = idx;
                SelectActorNumberControl(tlpExSpawnPoints);
            }), individual: individualFileMode);
        }

        private void SelectActorNumberControl(TableLayoutPanel tlp)
        {
            Control ctrl = tlp.Controls.Find("ActorNumber", false).FirstOrDefault();
            if (ctrl != null && ctrl is TextBox)
            {
                TextBox txt = (ctrl as TextBox);
                txt.SelectionStart = txt.Text.Length;
                txt.Select();
            }
        }

        private void cbPathHeaders_SelectionChangeCommitted(object sender, EventArgs e)
        {
            activePathHeader = (((ComboBox)sender).SelectedItem as HeaderCommands.Waypoints.PathHeader);
        }

        private void dgvPathWaypoints_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(((DataGridView)sender).RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 18, e.RowBounds.Location.Y + 4);
            }
        }

        private void dgvPathWaypoints_SelectionChanged(object sender, EventArgs e)
        {
            HeaderCommands.Waypoints.Waypoint selwp = (dgvPathWaypoints.SelectedCells.Count != 0 ? dgvPathWaypoints.SelectedCells[0].OwningRow.DataBoundItem as HeaderCommands.Waypoints.Waypoint : null);
            if (selwp == null) return;
            pickedObject = (selwp as HeaderCommands.IPickableObject);
            collisionDirty = true;
        }

        private void RefreshCollisionPolyAndTypeLists()
        {
            /* Type list */
            List<HeaderCommands.Collision.PolygonType> typelist = new List<HeaderCommands.Collision.PolygonType>();
            typelist.Add(new HeaderCommands.Collision.PolygonType());
            typelist.AddRange(currentScene.ActiveCollision.PolygonTypes);

            colPolyTypeDataBinding = new BindingSource();
            colPolyTypeDataBinding.DataSource = typelist;
            cbCollisionPolyTypes.DataSource = colPolyTypeDataBinding;
            cbCollisionPolyTypes.DisplayMember = "Description";
            cbCollisionPolyTypes.Enabled = true;

            txtColPolyRawData.Enabled = true;
            cbColPolyGroundTypes.DataSource = HeaderCommands.Collision.PolygonType.GroundTypes;
            cbColPolyGroundTypes.DisplayMember = "Description";
            cbColPolyGroundTypes.Enabled = true;
            //TODO more editing stuff

            /* Poly list */
            List<HeaderCommands.Collision.Polygon> polylist = new List<HeaderCommands.Collision.Polygon>();
            polylist.Add(new HeaderCommands.Collision.Polygon());
            polylist.AddRange(currentScene.ActiveCollision.Polygons);

            collisionPolyDataBinding = new BindingSource();
            collisionPolyDataBinding.DataSource = polylist;
            cbCollisionPolys.SelectedIndex = -1;
            cbCollisionPolys.DataSource = collisionPolyDataBinding;
            cbCollisionPolys.DisplayMember = "Description";
            cbCollisionPolys.Enabled = true;

            nudColPolyType.Minimum = 0;
            nudColPolyType.Maximum = (currentScene.ActiveCollision.PolygonTypes.Count - 1);
            nudColPolyType.Enabled = true;
        }

        private void cbCollisionPolys_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentCollisionPolygon = (((ComboBox)sender).SelectedItem as HeaderCommands.Collision.Polygon);
            if (currentCollisionPolygon == null) return;

            pickedObject = (currentCollisionPolygon as HeaderCommands.IPickableObject);
            collisionDirty = true;

            lblColPolyType.Visible = nudColPolyType.Visible = btnJumpToPolyType.Visible = !currentCollisionPolygon.IsDummy;
            if (!currentCollisionPolygon.IsDummy)
            {
                nudColPolyType.Value = currentCollisionPolygon.PolygonType;
                //TODO more here
            }
        }

        private void nudColPolyType_ValueChanged(object sender, EventArgs e)
        {
            currentCollisionPolygon.PolygonType = (ushort)((NumericUpDown)sender).Value;
            collisionPolyDataBinding.ResetCurrentItem();
        }

        private void btnJumpToPolyType_Click(object sender, EventArgs e)
        {
            if (cbCollisionPolyTypes.Items.Count > 0)
                cbCollisionPolyTypes.SelectedItem = (colPolyTypeDataBinding.List as List<HeaderCommands.Collision.PolygonType>).FirstOrDefault(x => x.Number == currentCollisionPolygon.PolygonType);
        }

        private void cbCollisionPolyTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedItem == null) return;

            currentColPolygonType = (((ComboBox)sender).SelectedItem as HeaderCommands.Collision.PolygonType);

            busy = true;
            RefreshColPolyTypeControls();
            busy = false;
        }

        private void RefreshColPolyTypeControls()
        {
            txtColPolyRawData.Text = string.Format("0x{0:X16}", currentColPolygonType.Raw);
            lblColPolyRawData.Visible = txtColPolyRawData.Visible = !currentColPolygonType.IsDummy;
            cbColPolyGroundTypes.SelectedItem = HeaderCommands.Collision.PolygonType.GroundTypes.FirstOrDefault(x => x.Value == currentColPolygonType.GroundTypeID);
            lblColPolyGroundType.Visible = cbColPolyGroundTypes.Visible = !currentColPolygonType.IsDummy;

            if (!busy) colPolyTypeDataBinding.ResetCurrentItem();

            collisionDirty = true;
        }

        private void txtColPolyRawData_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (sender as TextBox);
            if (!txt.ContainsFocus) return;

            System.Globalization.NumberStyles ns = (txt.Text.StartsWith("0x") ? System.Globalization.NumberStyles.HexNumber : System.Globalization.NumberStyles.Integer);
            string valstr = (ns == System.Globalization.NumberStyles.HexNumber ? txt.Text.Substring(2) : txt.Text);
            ulong newval = ulong.Parse(valstr, ns);

            currentColPolygonType.Raw = newval;
            RefreshColPolyTypeControls();
        }

        private void cbColPolyGroundTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(sender as ComboBox).ContainsFocus) return;

            currentColPolygonType.GroundTypeID = (((ComboBox)sender).SelectedItem as HeaderCommands.Collision.PolygonType.GroundType).Value;
            RefreshColPolyTypeControls();
        }

        #region Waterboxes

        private void RefreshWaterboxControls()
        {
            if (tlpExWaterboxes.Visible = (currentWaterbox != null && !currentWaterbox.IsDummy))
            {
                tlpExWaterboxes.SuspendLayout();

                busy = true;

                txtWaterboxPositionX.Text = string.Format("{0}", currentWaterbox.Position.X);
                txtWaterboxPositionY.Text = string.Format("{0}", currentWaterbox.Position.Y);
                txtWaterboxPositionZ.Text = string.Format("{0}", currentWaterbox.Position.Z);
                txtWaterboxSizeX.Text = string.Format("{0}", currentWaterbox.SizeXZ.X);
                txtWaterboxSizeZ.Text = string.Format("{0}", currentWaterbox.SizeXZ.Y);
                txtWaterboxProperties.Text = string.Format("0x{0:X}", currentWaterbox.Properties);

                if (roomsForWaterboxSelection != null && roomsForWaterboxSelection.Count > 0)
                {
                    cbWaterboxRoom.DataSource = roomsForWaterboxSelection;
                    cbWaterboxRoom.DisplayMember = "Description";
                    cbWaterboxRoom.SelectedItem = roomsForWaterboxSelection.FirstOrDefault(x => x.Value == currentWaterbox.RoomNumber);
                }

                busy = false;

                tlpExWaterboxes.ResumeLayout();
            }

            waterboxesDirty = true;
        }

        private void cbWaterboxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentWaterbox = (((ComboBox)sender).SelectedItem as HeaderCommands.Collision.Waterbox);
            if (currentWaterbox != null)
            {
                pickedObject = (currentWaterbox as HeaderCommands.IPickableObject);
                waterboxesDirty = true;

                txtWaterboxPositionX.Enabled = txtWaterboxPositionY.Enabled = txtWaterboxPositionZ.Enabled = txtWaterboxSizeX.Enabled = txtWaterboxSizeZ.Enabled = txtWaterboxProperties.Enabled
                    = !currentWaterbox.IsDummy;
            }
            RefreshWaterboxControls();
        }

        private void ModifyCurrentWaterbox()
        {
            if (busy) return;

            try
            {
                currentWaterbox.Position = new Vector3d(double.Parse(txtWaterboxPositionX.Text), double.Parse(txtWaterboxPositionY.Text), double.Parse(txtWaterboxPositionZ.Text));
                currentWaterbox.SizeXZ = new Vector2d(double.Parse(txtWaterboxSizeX.Text), double.Parse(txtWaterboxSizeZ.Text));
                currentWaterbox.RoomNumber = (ushort)(cbWaterboxRoom.SelectedItem as XMLActorDefinitionReader.Definition.Item.Option).Value;

                if (txtWaterboxProperties.Text.StartsWith("0x"))
                    currentWaterbox.Properties = ushort.Parse(txtWaterboxProperties.Text.Substring(2), System.Globalization.NumberStyles.HexNumber);
                else
                    currentWaterbox.Properties = ushort.Parse(txtWaterboxProperties.Text);

                waterboxesDirty = true;
            }
            catch (FormatException)
            {
                System.Media.SystemSounds.Hand.Play();
            }
        }

        private void txtWaterboxPositionX_TextChanged(object sender, EventArgs e)
        {
            ModifyCurrentWaterbox();
        }

        private void txtWaterboxPositionY_TextChanged(object sender, EventArgs e)
        {
            ModifyCurrentWaterbox();
        }

        private void txtWaterboxPositionZ_TextChanged(object sender, EventArgs e)
        {
            ModifyCurrentWaterbox();
        }

        private void txtWaterboxSizeX_TextChanged(object sender, EventArgs e)
        {
            ModifyCurrentWaterbox();
        }

        private void txtWaterboxSizeZ_TextChanged(object sender, EventArgs e)
        {
            ModifyCurrentWaterbox();
        }

        private void cbWaterboxRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModifyCurrentWaterbox();
        }

        private void txtWaterboxProperties_TextChanged(object sender, EventArgs e)
        {
            ModifyCurrentWaterbox();
        }

        #endregion

        private void bsiToolMode_Click(object sender, EventArgs e)
        {
            currentToolMode++;
        }

        private void deselectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pickedObject == null) return;

            if (pickedObject is HeaderCommands.Actors.Entry)
            {
                HeaderCommands.Actors.Entry ac = (pickedObject as HeaderCommands.Actors.Entry);
                if (ac.IsTransitionActor)
                    cbTransitions.SelectedIndex = 0;
                else if (ac.IsSpawnPoint)
                    cbSpawnPoints.SelectedIndex = 0;
                else
                    cbActors.SelectedIndex = 0;
            }
            else if (pickedObject is HeaderCommands.Waypoints.Waypoint)
            {
                dgvPathWaypoints.ClearSelection();
            }
            else if (pickedObject is HeaderCommands.Collision.Waterbox)
            {
                cbWaterboxes.SelectedIndex = 0;
            }

            pickedObject = null;
        }

        private void RotatePickedObject(Vector3d rot)
        {
            if (pickedObject == null) return;

            if (pickedObject is HeaderCommands.Actors.Entry)
            {
                HeaderCommands.Actors.Entry actor = (pickedObject as HeaderCommands.Actors.Entry);
                actor.Rotation = Vector3d.Add(actor.Rotation, rot);

                if (actor.IsSpawnPoint)
                    XMLActorDefinitionReader.RefreshActorPositionRotation(actor, tlpExSpawnPoints);
                else if (actor.IsTransitionActor)
                    XMLActorDefinitionReader.RefreshActorPositionRotation(actor, tlpExTransitions);
                else
                    XMLActorDefinitionReader.RefreshActorPositionRotation(actor, tlpExRoomActors);
            }
        }

        private void xPlus45DegreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotatePickedObject(new Vector3d(8192.0, 0.0, 0.0));
        }

        private void xMinus45DegreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotatePickedObject(new Vector3d(-8192.0, 0.0, 0.0));
        }

        private void yPlus45DegreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotatePickedObject(new Vector3d(0.0, 8192.0, 0.0));
        }

        private void yMinus45DegreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotatePickedObject(new Vector3d(0.0, -8192.0, 0.0));
        }

        private void zPlus45DegreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotatePickedObject(new Vector3d(0.0, 0.0, 8192.0));
        }

        private void zMinus45DegreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotatePickedObject(new Vector3d(0.0, 0.0, -8192.0));
        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentRoomVertex != null) EditVertexColor(currentRoomVertex);
        }

        private void propertiesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (currentRoomVertex == null) return;

            StringBuilder vertexInfo = new StringBuilder();
            vertexInfo.AppendFormat("Vertex at address 0x{0:X8}:\n\n", currentRoomVertex.Address);
            vertexInfo.AppendFormat("Position: {0}\n", currentRoomVertex.Position);
            vertexInfo.AppendFormat("Texture Coordinates: {0}\n", currentRoomVertex.TexCoord);
            vertexInfo.AppendFormat("Colors: ({0}, {1}, {2}, {3})\n", currentRoomVertex.Colors[0], currentRoomVertex.Colors[1], currentRoomVertex.Colors[2], currentRoomVertex.Colors[3]);
            vertexInfo.AppendFormat("Normals: ({0}, {1}, {2})\n", currentRoomVertex.Normals[0], currentRoomVertex.Normals[1], currentRoomVertex.Normals[2]);

            MessageBox.Show(vertexInfo.ToString(), "Vertex Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void bsiCamCoords_Click(object sender, EventArgs e)
        {
            camera.Reset();
        }

        #region Menu events

        private void resetCameraPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            camera.Reset();
        }

        private void enableTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.RenderTextures = ((ToolStripMenuItem)sender).Checked;
            displayListsDirty = true;
        }

        private void enableVSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customGLControl.VSync = Configuration.OGLVSync = ((ToolStripMenuItem)sender).Checked;
        }

        private void enableAntiAliasingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* Determine anti-aliasing status */
            if (Configuration.EnableAntiAliasing = ((ToolStripMenuItem)sender).Checked)
            {
                int samples = 0;
                GL.GetInteger(GetPName.MaxSamples, out samples);
                Configuration.AntiAliasingSamples = samples;
            }
            else
                Configuration.AntiAliasingSamples = 0;

            if (MessageBox.Show(string.Format("{0}abling anti-aliasing requires restarting SceneNavi.\n\nDo you want to restart the program now?", (Configuration.EnableAntiAliasing ? "En" : "Dis")),
                "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Configuration.IsRestarting = true;
                Application.Restart();
            }
        }

        private void enableMipmapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.EnableMipmaps = ((ToolStripMenuItem)sender).Checked;

            if (rom == null || rom.Scenes == null) return;

            /* Destroy, destroy! Kill all the display lists! ...or should I say "Exterminate!"? Then again, I'm not a Doctor Who fan... */
            foreach (HeaderLoader sh in rom.Scenes.SelectMany(x => x.SceneHeaders))
            {
                HeaderCommands.Rooms rooms = (sh.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms)) as HeaderCommands.Rooms;
                if (rooms == null) continue;

                foreach (HeaderLoader rh in rooms.RoomInformation.SelectMany(x => x.Headers))
                {
                    HeaderCommands.MeshHeader mh = (rh.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.MeshHeader)) as HeaderCommands.MeshHeader;
                    if (mh != null) mh.DestroyDisplayLists();
                }
            }

            rom.Renderer.ResetTextureCache();

            displayListsDirty = true;
        }

        private void renderCollisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.RenderCollision = ((ToolStripMenuItem)sender).Checked;
            if (Configuration.RenderCollision) collisionDirty = true;
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.RenderCollisionAsWhite = ((Controls.ToolStripRadioButtonMenuItem)sender).Checked;
            collisionDirty = true;
        }

        private void typebasedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.RenderCollisionAsWhite = !((Controls.ToolStripRadioButtonMenuItem)sender).Checked;
            collisionDirty = true;
        }

        private void renderRoomActorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.RenderRoomActors = ((ToolStripMenuItem)sender).Checked;
        }

        private void renderSpawnPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.RenderSpawnPoints = ((ToolStripMenuItem)sender).Checked;
        }

        private void renderTransitionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.RenderTransitions = ((ToolStripMenuItem)sender).Checked;
        }

        private void renderPathWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.RenderPathWaypoints = ((ToolStripMenuItem)sender).Checked;
        }

        private void renderWaterboxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.RenderWaterboxes = ((ToolStripMenuItem)sender).Checked;
        }

        private void linkAllWaypointsInPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.LinkAllWPinPath = ((ToolStripMenuItem)sender).Checked;
        }

        private void showWaterboxesPerRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.ShowWaterboxesPerRoom = ((ToolStripMenuItem)sender).Checked;
        }

        private void rOMInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string info = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{0} ({1}, v1.{2}), {3} MB ({4} Mbit)\n{5}\nCreated by {6}, built on {7:F}\n\nCode file at 0x{8:X} - 0x{9:X} ({10})\n- DMA table address: 0x{11:X}\n- File name table address: {12}\n" +
                "- Scene table address: {13}\n- Actor table address: {14}\n- Object table address: {15}\n- Entrance table address: {16}",
                rom.Title, rom.GameID, rom.Version, (rom.Size / 0x100000), (rom.Size / 0x20000), (rom.HasZ64TablesHack ? "(uses 'z64tables' extended tables)\n" : ""),
                rom.Creator, rom.BuildDate, rom.Code.PStart, (rom.Code.IsCompressed ? rom.Code.PEnd : rom.Code.VEnd),
                (rom.Code.IsCompressed ? "compressed" : "uncompressed"), rom.DMATableAddress, (rom.HasFileNameTable ? ("0x" + rom.FileNameTableAddress.ToString("X")) : "none"),
                (rom.HasZ64TablesHack ? ("0x" + rom.SceneTableAddress.ToString("X") + " (in ROM)") : ("0x" + rom.SceneTableAddress.ToString("X"))),
                (rom.HasZ64TablesHack ? ("0x" + rom.ActorTableAddress.ToString("X") + " (in ROM)") : ("0x" + rom.ActorTableAddress.ToString("X"))),
                (rom.HasZ64TablesHack ? ("0x" + rom.ObjectTableAddress.ToString("X") + " (in ROM)") : ("0x" + rom.ObjectTableAddress.ToString("X"))),
                (rom.HasZ64TablesHack ? ("0x" + rom.EntranceTableAddress.ToString("X") + " (in ROM)") : ("0x" + rom.EntranceTableAddress.ToString("X"))));

            MessageBox.Show(info, "ROM Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void editDataTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new TableEditorForm(rom).ShowDialog();
        }

        private void editAreaTitleCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new TitleCardForm(rom, currentScene).ShowDialog();
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UpdateCheckDialog().ShowDialog();
        }

        private void openGLInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder oglInfoString = new StringBuilder();

            oglInfoString.AppendFormat("Vendor: {0}\n", Initialization.VendorString);
            oglInfoString.AppendFormat("Renderer: {0}\n", Initialization.RendererString);
            oglInfoString.AppendFormat("Version: {0}\n", Initialization.VersionString);
            oglInfoString.AppendFormat("Shading Language Version: {0}\n", Initialization.ShadingLanguageVersionString);
            oglInfoString.AppendLine();

            oglInfoString.AppendFormat("Max Texture Units: {0}\n", Initialization.GetInteger(GetPName.MaxTextureUnits));
            oglInfoString.AppendFormat("Max Texture Size: {0}\n", Initialization.GetInteger(GetPName.MaxTextureSize));
            oglInfoString.AppendLine();

            oglInfoString.AppendFormat("{0} OpenGL extension(s) supported.\n", Initialization.SupportedExtensions.Length);
            oglInfoString.AppendLine();

            oglInfoString.AppendLine("Status of requested extensions:");

            foreach (string extension in allRequiredOglExtensions) oglInfoString.AppendFormat("* {0}\t{1}\n", extension.PadRight(40), Initialization.CheckForExtension(extension) ? "supported" : "not supported");

            MessageBox.Show(oglInfoString.ToString(), "OpenGL Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime linkerTimestamp = AssemblyHelpers.RetrieveLinkerTimestamp();

            string buildString = string.Format("(Build: {0})", linkerTimestamp.ToString("MM/dd/yyyy HH:mm:ss UTCzzz", System.Globalization.CultureInfo.InvariantCulture));
            string yearString = (linkerTimestamp.Year == 2013 ? "2013" : string.Format("2013-{0}", linkerTimestamp.ToString("yyyy")));

            MessageBox.Show(string.Format("{0} {1}\n\nScene/room actor editor for The Legend of Zelda: Ocarina of Time\n\nWritten {2} by xdaniel / http://magicstone.de/dzd/", Program.AppNameVer, buildString, yearString),
                string.Format("About {0}", Application.ProductName), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}
