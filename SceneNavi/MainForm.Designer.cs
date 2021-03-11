namespace SceneNavi
{
	partial class MainForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.openROMToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.openSceneToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.saveToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
			this.closeSceneToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.rOMInformationToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.editToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.editDataTablesToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.editAreaTitleCardToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.toolsToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.mouseModeToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.collisionHighlightToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.whiteToolStripMenuItem = new SceneNavi.Controls.ToolStripRadioButtonMenuItem();
			this.typebasedToolStripMenuItem = new SceneNavi.Controls.ToolStripRadioButtonMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
			this.resetCameraPositionToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.optionsToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.renderElementsToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.renderCollisionToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.renderRoomActorsToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.renderSpawnPointsToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.renderTransitionsToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.renderPathWaypointsToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.renderWaterboxesToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.linkAllWaypointsInPathToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.showWaterboxesPerRoomToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.enableTexturesToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.emulateDrawDistanceToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.combinerTypeToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
			this.openGLToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.enableVSyncToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.enableAntiAliasingToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.helpToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.checkForUpdateToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
			this.openGLInformationToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.aboutToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.bsiToolMode = new SceneNavi.GUIHelpers.ButtonStripItem();
			this.separatorStripItem1 = new SceneNavi.GUIHelpers.SeparatorStripItem();
			this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.separatorStripItem2 = new SceneNavi.GUIHelpers.SeparatorStripItem();
			this.bsiCamCoords = new SceneNavi.GUIHelpers.ButtonStripItem();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tpScenes = new System.Windows.Forms.TabPage();
			this.tvScenes = new SceneNavi.Controls.TreeViewEx();
			this.tpSceneMetadata = new System.Windows.Forms.TabPage();
			this.tlpExSceneMeta = new SceneNavi.Controls.TableLayoutPanelEx();
			this.nudSceneMetaNightSFX = new System.Windows.Forms.NumericUpDown();
			this.cbSceneMetaBGM = new System.Windows.Forms.ComboBox();
			this.lblSceneMetaBGM = new System.Windows.Forms.Label();
			this.lblSceneMetaReverb = new System.Windows.Forms.Label();
			this.nudSceneMetaReverb = new System.Windows.Forms.NumericUpDown();
			this.lblSceneMetaNightSFX = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tpRoomActors = new System.Windows.Forms.TabPage();
			this.tlpExRoomActors = new SceneNavi.Controls.TableLayoutPanelEx();
			this.cbActors = new System.Windows.Forms.ComboBox();
			this.tpObjects = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.lblSpecialObjs = new System.Windows.Forms.Label();
			this.dgvObjects = new System.Windows.Forms.DataGridView();
			this.cbSpecialObjs = new System.Windows.Forms.ComboBox();
			this.tpSpawnPoints = new System.Windows.Forms.TabPage();
			this.tlpExSpawnPoints = new SceneNavi.Controls.TableLayoutPanelEx();
			this.cbSpawnPoints = new System.Windows.Forms.ComboBox();
			this.tpTransitions = new System.Windows.Forms.TabPage();
			this.tlpExTransitions = new SceneNavi.Controls.TableLayoutPanelEx();
			this.cbTransitions = new System.Windows.Forms.ComboBox();
			this.tpWaypoints = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.dgvPathWaypoints = new System.Windows.Forms.DataGridView();
			this.cbPathHeaders = new System.Windows.Forms.ComboBox();
			this.tpCollision = new System.Windows.Forms.TabPage();
			this.tlpExCollision = new SceneNavi.Controls.TableLayoutPanelEx();
			this.label2 = new System.Windows.Forms.Label();
			this.cbCollisionPolyTypes = new System.Windows.Forms.ComboBox();
			this.cbCollisionPolys = new System.Windows.Forms.ComboBox();
			this.lblColPolyType = new System.Windows.Forms.Label();
			this.nudColPolyType = new System.Windows.Forms.NumericUpDown();
			this.lblColPolyGroundType = new System.Windows.Forms.Label();
			this.cbColPolyGroundTypes = new System.Windows.Forms.ComboBox();
			this.lblColPolyRawData = new System.Windows.Forms.Label();
			this.txtColPolyRawData = new System.Windows.Forms.TextBox();
			this.btnJumpToPolyType = new System.Windows.Forms.Button();
			this.lblCollisionPolys = new System.Windows.Forms.Label();
			this.lblCollisionPolyTypes = new System.Windows.Forms.Label();
			this.tpWaterboxes = new System.Windows.Forms.TabPage();
			this.tlpExWaterboxes = new SceneNavi.Controls.TableLayoutPanelEx();
			this.lblWaterboxProperties = new System.Windows.Forms.Label();
			this.txtWaterboxProperties = new System.Windows.Forms.TextBox();
			this.lblWaterboxPositionX = new System.Windows.Forms.Label();
			this.txtWaterboxPositionX = new System.Windows.Forms.TextBox();
			this.lblWaterboxPositionY = new System.Windows.Forms.Label();
			this.txtWaterboxPositionY = new System.Windows.Forms.TextBox();
			this.lblWaterboxPositionZ = new System.Windows.Forms.Label();
			this.txtWaterboxPositionZ = new System.Windows.Forms.TextBox();
			this.lblWaterboxSizeX = new System.Windows.Forms.Label();
			this.txtWaterboxSizeX = new System.Windows.Forms.TextBox();
			this.lblWaterboxSizeZ = new System.Windows.Forms.Label();
			this.txtWaterboxSizeZ = new System.Windows.Forms.TextBox();
			this.lblWaterboxRoom = new System.Windows.Forms.Label();
			this.cbWaterboxRoom = new System.Windows.Forms.ComboBox();
			this.cbWaterboxes = new System.Windows.Forms.ComboBox();
			this.ofdOpenROM = new System.Windows.Forms.OpenFileDialog();
			this.cmsMoveableObjectEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deselectToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
			this.rotateToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.xAxisToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.xPlus45DegreesToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.xMinus45DegreesToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.yAxisToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.yPlus45DegreesToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.yMinus45DegreesToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.zAxisToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.zPlus45DegreesToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.zMinus45DegreesToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.ofdOpenScene = new System.Windows.Forms.OpenFileDialog();
			this.ofdOpenRoom = new System.Windows.Forms.OpenFileDialog();
			this.cmsSceneTree = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.propertiesToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.customGLControl = new SceneNavi.Controls.CustomGLControl();
			this.cmsVertexEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.changeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
			this.propertiesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.emulateFogToolStripMenuItem = new SceneNavi.Controls.ToolStripHintMenuItem();
			this.menuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tpScenes.SuspendLayout();
			this.tpSceneMetadata.SuspendLayout();
			this.tlpExSceneMeta.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSceneMetaNightSFX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSceneMetaReverb)).BeginInit();
			this.tpRoomActors.SuspendLayout();
			this.tpObjects.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvObjects)).BeginInit();
			this.tpSpawnPoints.SuspendLayout();
			this.tpTransitions.SuspendLayout();
			this.tpWaypoints.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvPathWaypoints)).BeginInit();
			this.tpCollision.SuspendLayout();
			this.tlpExCollision.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudColPolyType)).BeginInit();
			this.tpWaterboxes.SuspendLayout();
			this.tlpExWaterboxes.SuspendLayout();
			this.cmsMoveableObjectEdit.SuspendLayout();
			this.cmsSceneTree.SuspendLayout();
			this.cmsVertexEdit.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(970, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openROMToolStripMenuItem,
            this.openSceneToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem9,
            this.closeSceneToolStripMenuItem,
            this.toolStripMenuItem1,
            this.rOMInformationToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.HelpText = null;
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openROMToolStripMenuItem
			// 
			this.openROMToolStripMenuItem.HelpText = "Select ROM to open";
			this.openROMToolStripMenuItem.Name = "openROMToolStripMenuItem";
			this.openROMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openROMToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.openROMToolStripMenuItem.Text = "&Open ROM";
			this.openROMToolStripMenuItem.Click += new System.EventHandler(this.openROMToolStripMenuItem_Click);
			// 
			// openSceneToolStripMenuItem
			// 
			this.openSceneToolStripMenuItem.Enabled = false;
			this.openSceneToolStripMenuItem.HelpText = "Select a pair of Scene and Room files to open";
			this.openSceneToolStripMenuItem.Name = "openSceneToolStripMenuItem";
			this.openSceneToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.openSceneToolStripMenuItem.Text = "Open S&cene...";
			this.openSceneToolStripMenuItem.Click += new System.EventHandler(this.openSceneToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Enabled = false;
			this.saveToolStripMenuItem.HelpText = "Save changes to the current file";
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.saveToolStripMenuItem.Text = "&Save...";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// toolStripMenuItem9
			// 
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.Size = new System.Drawing.Size(203, 6);
			// 
			// closeSceneToolStripMenuItem
			// 
			this.closeSceneToolStripMenuItem.Enabled = false;
			this.closeSceneToolStripMenuItem.HelpText = "Close the currently opened Scene and Room files";
			this.closeSceneToolStripMenuItem.Name = "closeSceneToolStripMenuItem";
			this.closeSceneToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.closeSceneToolStripMenuItem.Text = "C&lose Scene";
			this.closeSceneToolStripMenuItem.Click += new System.EventHandler(this.closeSceneToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(203, 6);
			// 
			// rOMInformationToolStripMenuItem
			// 
			this.rOMInformationToolStripMenuItem.Enabled = false;
			this.rOMInformationToolStripMenuItem.HelpText = "Show information about the loaded ROM";
			this.rOMInformationToolStripMenuItem.Name = "rOMInformationToolStripMenuItem";
			this.rOMInformationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
			this.rOMInformationToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.rOMInformationToolStripMenuItem.Text = "ROM &Information";
			this.rOMInformationToolStripMenuItem.Click += new System.EventHandler(this.rOMInformationToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(203, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.HelpText = "Quit the application";
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editDataTablesToolStripMenuItem,
            this.editAreaTitleCardToolStripMenuItem});
			this.editToolStripMenuItem.HelpText = null;
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// editDataTablesToolStripMenuItem
			// 
			this.editDataTablesToolStripMenuItem.Enabled = false;
			this.editDataTablesToolStripMenuItem.HelpText = "Edit various data tables in the ROM";
			this.editDataTablesToolStripMenuItem.Name = "editDataTablesToolStripMenuItem";
			this.editDataTablesToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.editDataTablesToolStripMenuItem.Text = "Edit &Data Tables";
			this.editDataTablesToolStripMenuItem.Click += new System.EventHandler(this.editDataTablesToolStripMenuItem_Click);
			// 
			// editAreaTitleCardToolStripMenuItem
			// 
			this.editAreaTitleCardToolStripMenuItem.Enabled = false;
			this.editAreaTitleCardToolStripMenuItem.HelpText = "Export and import the current area\'s title card";
			this.editAreaTitleCardToolStripMenuItem.Name = "editAreaTitleCardToolStripMenuItem";
			this.editAreaTitleCardToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.editAreaTitleCardToolStripMenuItem.Text = "Edit &Area Title Card";
			this.editAreaTitleCardToolStripMenuItem.Click += new System.EventHandler(this.editAreaTitleCardToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mouseModeToolStripMenuItem,
            this.collisionHighlightToolStripMenuItem,
            this.toolStripMenuItem7,
            this.resetCameraPositionToolStripMenuItem});
			this.toolsToolStripMenuItem.HelpText = null;
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.toolsToolStripMenuItem.Text = "&Tools";
			// 
			// mouseModeToolStripMenuItem
			// 
			this.mouseModeToolStripMenuItem.HelpText = "Select the current mouse mode";
			this.mouseModeToolStripMenuItem.Name = "mouseModeToolStripMenuItem";
			this.mouseModeToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.mouseModeToolStripMenuItem.Text = "&Mouse Mode";
			// 
			// collisionHighlightToolStripMenuItem
			// 
			this.collisionHighlightToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whiteToolStripMenuItem,
            this.typebasedToolStripMenuItem});
			this.collisionHighlightToolStripMenuItem.HelpText = "Change collision highlighting behavior";
			this.collisionHighlightToolStripMenuItem.Name = "collisionHighlightToolStripMenuItem";
			this.collisionHighlightToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.collisionHighlightToolStripMenuItem.Text = "Collision &Highlight";
			// 
			// whiteToolStripMenuItem
			// 
			this.whiteToolStripMenuItem.CheckOnClick = true;
			this.whiteToolStripMenuItem.HelpText = null;
			this.whiteToolStripMenuItem.Name = "whiteToolStripMenuItem";
			this.whiteToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.whiteToolStripMenuItem.Text = "All &White";
			this.whiteToolStripMenuItem.Click += new System.EventHandler(this.whiteToolStripMenuItem_Click);
			// 
			// typebasedToolStripMenuItem
			// 
			this.typebasedToolStripMenuItem.CheckOnClick = true;
			this.typebasedToolStripMenuItem.HelpText = null;
			this.typebasedToolStripMenuItem.Name = "typebasedToolStripMenuItem";
			this.typebasedToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.typebasedToolStripMenuItem.Text = "Ground &Type-based";
			this.typebasedToolStripMenuItem.Click += new System.EventHandler(this.typebasedToolStripMenuItem_Click);
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(189, 6);
			// 
			// resetCameraPositionToolStripMenuItem
			// 
			this.resetCameraPositionToolStripMenuItem.HelpText = "Reset the camera\'s position";
			this.resetCameraPositionToolStripMenuItem.Name = "resetCameraPositionToolStripMenuItem";
			this.resetCameraPositionToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.resetCameraPositionToolStripMenuItem.Text = "&Reset Camera Position";
			this.resetCameraPositionToolStripMenuItem.Click += new System.EventHandler(this.resetCameraPositionToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderElementsToolStripMenuItem,
            this.toolStripMenuItem4,
            this.enableTexturesToolStripMenuItem,
            this.emulateDrawDistanceToolStripMenuItem,
            this.emulateFogToolStripMenuItem,
            this.combinerTypeToolStripMenuItem,
            this.toolStripMenuItem10,
            this.openGLToolStripMenuItem});
			this.optionsToolStripMenuItem.HelpText = null;
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionsToolStripMenuItem.Text = "&Options";
			// 
			// renderElementsToolStripMenuItem
			// 
			this.renderElementsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderCollisionToolStripMenuItem,
            this.toolStripMenuItem3,
            this.renderRoomActorsToolStripMenuItem,
            this.renderSpawnPointsToolStripMenuItem,
            this.renderTransitionsToolStripMenuItem,
            this.renderPathWaypointsToolStripMenuItem,
            this.renderWaterboxesToolStripMenuItem,
            this.toolStripMenuItem5,
            this.linkAllWaypointsInPathToolStripMenuItem,
            this.showWaterboxesPerRoomToolStripMenuItem});
			this.renderElementsToolStripMenuItem.HelpText = "Select scene elements to render";
			this.renderElementsToolStripMenuItem.Name = "renderElementsToolStripMenuItem";
			this.renderElementsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.renderElementsToolStripMenuItem.Text = "&Render Elements";
			// 
			// renderCollisionToolStripMenuItem
			// 
			this.renderCollisionToolStripMenuItem.CheckOnClick = true;
			this.renderCollisionToolStripMenuItem.HelpText = "Render the collision overlay";
			this.renderCollisionToolStripMenuItem.Name = "renderCollisionToolStripMenuItem";
			this.renderCollisionToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.renderCollisionToolStripMenuItem.Text = "Render &Collision";
			this.renderCollisionToolStripMenuItem.Click += new System.EventHandler(this.renderCollisionToolStripMenuItem_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(219, 6);
			// 
			// renderRoomActorsToolStripMenuItem
			// 
			this.renderRoomActorsToolStripMenuItem.CheckOnClick = true;
			this.renderRoomActorsToolStripMenuItem.HelpText = "Render the current room\'s actors";
			this.renderRoomActorsToolStripMenuItem.Name = "renderRoomActorsToolStripMenuItem";
			this.renderRoomActorsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.renderRoomActorsToolStripMenuItem.Text = "Render &Room Actors";
			this.renderRoomActorsToolStripMenuItem.Click += new System.EventHandler(this.renderRoomActorsToolStripMenuItem_Click);
			// 
			// renderSpawnPointsToolStripMenuItem
			// 
			this.renderSpawnPointsToolStripMenuItem.CheckOnClick = true;
			this.renderSpawnPointsToolStripMenuItem.HelpText = "Render all spawn points in the scene";
			this.renderSpawnPointsToolStripMenuItem.Name = "renderSpawnPointsToolStripMenuItem";
			this.renderSpawnPointsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.renderSpawnPointsToolStripMenuItem.Text = "Render &Spawn Points";
			this.renderSpawnPointsToolStripMenuItem.Click += new System.EventHandler(this.renderSpawnPointsToolStripMenuItem_Click);
			// 
			// renderTransitionsToolStripMenuItem
			// 
			this.renderTransitionsToolStripMenuItem.CheckOnClick = true;
			this.renderTransitionsToolStripMenuItem.HelpText = "Render all transition actors in the scene";
			this.renderTransitionsToolStripMenuItem.Name = "renderTransitionsToolStripMenuItem";
			this.renderTransitionsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.renderTransitionsToolStripMenuItem.Text = "Render Tr&ansitions";
			this.renderTransitionsToolStripMenuItem.Click += new System.EventHandler(this.renderTransitionsToolStripMenuItem_Click);
			// 
			// renderPathWaypointsToolStripMenuItem
			// 
			this.renderPathWaypointsToolStripMenuItem.CheckOnClick = true;
			this.renderPathWaypointsToolStripMenuItem.HelpText = "Render all path waypoints in the scene";
			this.renderPathWaypointsToolStripMenuItem.Name = "renderPathWaypointsToolStripMenuItem";
			this.renderPathWaypointsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.renderPathWaypointsToolStripMenuItem.Text = "Render &Path Waypoints";
			this.renderPathWaypointsToolStripMenuItem.Click += new System.EventHandler(this.renderPathWaypointsToolStripMenuItem_Click);
			// 
			// renderWaterboxesToolStripMenuItem
			// 
			this.renderWaterboxesToolStripMenuItem.CheckOnClick = true;
			this.renderWaterboxesToolStripMenuItem.HelpText = "Render all waterboxes in the scene";
			this.renderWaterboxesToolStripMenuItem.Name = "renderWaterboxesToolStripMenuItem";
			this.renderWaterboxesToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.renderWaterboxesToolStripMenuItem.Text = "Render &Waterboxes";
			this.renderWaterboxesToolStripMenuItem.Click += new System.EventHandler(this.renderWaterboxesToolStripMenuItem_Click);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(219, 6);
			// 
			// linkAllWaypointsInPathToolStripMenuItem
			// 
			this.linkAllWaypointsInPathToolStripMenuItem.CheckOnClick = true;
			this.linkAllWaypointsInPathToolStripMenuItem.HelpText = "Link all waypoints in each path together";
			this.linkAllWaypointsInPathToolStripMenuItem.Name = "linkAllWaypointsInPathToolStripMenuItem";
			this.linkAllWaypointsInPathToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.linkAllWaypointsInPathToolStripMenuItem.Text = "&Link Waypoints in Path";
			this.linkAllWaypointsInPathToolStripMenuItem.Click += new System.EventHandler(this.linkAllWaypointsInPathToolStripMenuItem_Click);
			// 
			// showWaterboxesPerRoomToolStripMenuItem
			// 
			this.showWaterboxesPerRoomToolStripMenuItem.CheckOnClick = true;
			this.showWaterboxesPerRoomToolStripMenuItem.HelpText = "Fade out all waterboxes not loaded in the current room";
			this.showWaterboxesPerRoomToolStripMenuItem.Name = "showWaterboxesPerRoomToolStripMenuItem";
			this.showWaterboxesPerRoomToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.showWaterboxesPerRoomToolStripMenuItem.Text = "Show Waterboxes per Room";
			this.showWaterboxesPerRoomToolStripMenuItem.Click += new System.EventHandler(this.showWaterboxesPerRoomToolStripMenuItem_Click);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(192, 6);
			// 
			// enableTexturesToolStripMenuItem
			// 
			this.enableTexturesToolStripMenuItem.CheckOnClick = true;
			this.enableTexturesToolStripMenuItem.HelpText = "Enable rendering of textures";
			this.enableTexturesToolStripMenuItem.Name = "enableTexturesToolStripMenuItem";
			this.enableTexturesToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.enableTexturesToolStripMenuItem.Text = "Enable &Textures";
			this.enableTexturesToolStripMenuItem.Click += new System.EventHandler(this.enableTexturesToolStripMenuItem_Click);
			// 
			// emulateDrawDistanceToolStripMenuItem
			// 
			this.emulateDrawDistanceToolStripMenuItem.CheckOnClick = true;
			this.emulateDrawDistanceToolStripMenuItem.HelpText = "Limit draw distance according to environment settings";
			this.emulateDrawDistanceToolStripMenuItem.Name = "emulateDrawDistanceToolStripMenuItem";
			this.emulateDrawDistanceToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.emulateDrawDistanceToolStripMenuItem.Text = "Emulate &Draw Distance";
			this.emulateDrawDistanceToolStripMenuItem.Click += new System.EventHandler(this.emulateDrawDistanceToolStripMenuItem_Click);
			// 
			// combinerTypeToolStripMenuItem
			// 
			this.combinerTypeToolStripMenuItem.HelpText = "Select color combiner emulator to use";
			this.combinerTypeToolStripMenuItem.Name = "combinerTypeToolStripMenuItem";
			this.combinerTypeToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.combinerTypeToolStripMenuItem.Text = "&Combiner Type";
			// 
			// toolStripMenuItem10
			// 
			this.toolStripMenuItem10.Name = "toolStripMenuItem10";
			this.toolStripMenuItem10.Size = new System.Drawing.Size(192, 6);
			// 
			// openGLToolStripMenuItem
			// 
			this.openGLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableVSyncToolStripMenuItem,
            this.enableAntiAliasingToolStripMenuItem});
			this.openGLToolStripMenuItem.HelpText = "Toggle various OpenGL properties";
			this.openGLToolStripMenuItem.Name = "openGLToolStripMenuItem";
			this.openGLToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.openGLToolStripMenuItem.Text = "&OpenGL";
			// 
			// enableVSyncToolStripMenuItem
			// 
			this.enableVSyncToolStripMenuItem.CheckOnClick = true;
			this.enableVSyncToolStripMenuItem.HelpText = "Toggle vertical sync; limit frames-per-second to monitor refresh rate";
			this.enableVSyncToolStripMenuItem.Name = "enableVSyncToolStripMenuItem";
			this.enableVSyncToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.enableVSyncToolStripMenuItem.Text = "Enable &VSync";
			this.enableVSyncToolStripMenuItem.Click += new System.EventHandler(this.enableVSyncToolStripMenuItem_Click);
			// 
			// enableAntiAliasingToolStripMenuItem
			// 
			this.enableAntiAliasingToolStripMenuItem.CheckOnClick = true;
			this.enableAntiAliasingToolStripMenuItem.HelpText = "Toggle anti-aliasing; requires application restart";
			this.enableAntiAliasingToolStripMenuItem.Name = "enableAntiAliasingToolStripMenuItem";
			this.enableAntiAliasingToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.enableAntiAliasingToolStripMenuItem.Text = "Enable &Anti-aliasing";
			this.enableAntiAliasingToolStripMenuItem.Click += new System.EventHandler(this.enableAntiAliasingToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdateToolStripMenuItem,
            this.toolStripMenuItem8,
            this.openGLInformationToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.HelpText = null;
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// checkForUpdateToolStripMenuItem
			// 
			this.checkForUpdateToolStripMenuItem.HelpText = "Check online for application updates";
			this.checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
			this.checkForUpdateToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.checkForUpdateToolStripMenuItem.Text = "&Check for Update...";
			this.checkForUpdateToolStripMenuItem.Visible = false;
			this.checkForUpdateToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdateToolStripMenuItem_Click);
			// 
			// toolStripMenuItem8
			// 
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Size = new System.Drawing.Size(189, 6);
			this.toolStripMenuItem8.Visible = false;
			// 
			// openGLInformationToolStripMenuItem
			// 
			this.openGLInformationToolStripMenuItem.HelpText = "Show information about the system\'s OpenGL support";
			this.openGLInformationToolStripMenuItem.Name = "openGLInformationToolStripMenuItem";
			this.openGLInformationToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.openGLInformationToolStripMenuItem.Text = "&OpenGL Information...";
			this.openGLInformationToolStripMenuItem.Click += new System.EventHandler(this.openGLInformationToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.HelpText = "About the application";
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.aboutToolStripMenuItem.Text = "&About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bsiToolMode,
            this.separatorStripItem1,
            this.tsslStatus,
            this.separatorStripItem2,
            this.bsiCamCoords});
			this.statusStrip1.Location = new System.Drawing.Point(0, 504);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.ShowItemToolTips = true;
			this.statusStrip1.Size = new System.Drawing.Size(970, 24);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// bsiToolMode
			// 
			this.bsiToolMode.AutoSize = false;
			this.bsiToolMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.bsiToolMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bsiToolMode.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
			this.bsiToolMode.Name = "bsiToolMode";
			this.bsiToolMode.Size = new System.Drawing.Size(160, 19);
			this.bsiToolMode.Text = "---";
			this.bsiToolMode.ToolTipText = "Click to cycle through mouse modes";
			this.bsiToolMode.Click += new System.EventHandler(this.bsiToolMode_Click);
			// 
			// separatorStripItem1
			// 
			this.separatorStripItem1.Name = "separatorStripItem1";
			this.separatorStripItem1.Size = new System.Drawing.Size(6, 24);
			// 
			// tsslStatus
			// 
			this.tsslStatus.Name = "tsslStatus";
			this.tsslStatus.Size = new System.Drawing.Size(501, 19);
			this.tsslStatus.Spring = true;
			this.tsslStatus.Text = "---";
			this.tsslStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// separatorStripItem2
			// 
			this.separatorStripItem2.Name = "separatorStripItem2";
			this.separatorStripItem2.Size = new System.Drawing.Size(6, 24);
			// 
			// bsiCamCoords
			// 
			this.bsiCamCoords.AutoSize = false;
			this.bsiCamCoords.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.bsiCamCoords.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bsiCamCoords.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
			this.bsiCamCoords.Name = "bsiCamCoords";
			this.bsiCamCoords.Size = new System.Drawing.Size(270, 19);
			this.bsiCamCoords.Text = "---";
			this.bsiCamCoords.ToolTipText = "Click to reset the camera\'s position";
			this.bsiCamCoords.Click += new System.EventHandler(this.bsiCamCoords_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tpScenes);
			this.tabControl1.Controls.Add(this.tpSceneMetadata);
			this.tabControl1.Controls.Add(this.tpRoomActors);
			this.tabControl1.Controls.Add(this.tpObjects);
			this.tabControl1.Controls.Add(this.tpSpawnPoints);
			this.tabControl1.Controls.Add(this.tpTransitions);
			this.tabControl1.Controls.Add(this.tpWaypoints);
			this.tabControl1.Controls.Add(this.tpCollision);
			this.tabControl1.Controls.Add(this.tpWaterboxes);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Right;
			this.tabControl1.Location = new System.Drawing.Point(640, 24);
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(330, 480);
			this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
			this.tabControl1.TabIndex = 3;
			this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
			// 
			// tpScenes
			// 
			this.tpScenes.Controls.Add(this.tvScenes);
			this.tpScenes.Location = new System.Drawing.Point(4, 40);
			this.tpScenes.Name = "tpScenes";
			this.tpScenes.Padding = new System.Windows.Forms.Padding(3);
			this.tpScenes.Size = new System.Drawing.Size(322, 436);
			this.tpScenes.TabIndex = 0;
			this.tpScenes.Text = "Scenes";
			this.tpScenes.UseVisualStyleBackColor = true;
			// 
			// tvScenes
			// 
			this.tvScenes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvScenes.HideSelection = false;
			this.tvScenes.HotTracking = true;
			this.tvScenes.Location = new System.Drawing.Point(3, 3);
			this.tvScenes.Name = "tvScenes";
			this.tvScenes.ShowLines = false;
			this.tvScenes.Size = new System.Drawing.Size(316, 430);
			this.tvScenes.TabIndex = 0;
			this.tvScenes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvScenes_AfterSelect);
			this.tvScenes.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvScenes_MouseUp);
			// 
			// tpSceneMetadata
			// 
			this.tpSceneMetadata.Controls.Add(this.tlpExSceneMeta);
			this.tpSceneMetadata.Location = new System.Drawing.Point(4, 40);
			this.tpSceneMetadata.Name = "tpSceneMetadata";
			this.tpSceneMetadata.Size = new System.Drawing.Size(322, 436);
			this.tpSceneMetadata.TabIndex = 6;
			this.tpSceneMetadata.Text = "Scene Metadata";
			this.tpSceneMetadata.UseVisualStyleBackColor = true;
			// 
			// tlpExSceneMeta
			// 
			this.tlpExSceneMeta.ColumnCount = 4;
			this.tlpExSceneMeta.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
			this.tlpExSceneMeta.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tlpExSceneMeta.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
			this.tlpExSceneMeta.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tlpExSceneMeta.Controls.Add(this.nudSceneMetaNightSFX, 1, 1);
			this.tlpExSceneMeta.Controls.Add(this.cbSceneMetaBGM, 1, 0);
			this.tlpExSceneMeta.Controls.Add(this.lblSceneMetaBGM, 0, 0);
			this.tlpExSceneMeta.Controls.Add(this.lblSceneMetaReverb, 0, 2);
			this.tlpExSceneMeta.Controls.Add(this.nudSceneMetaReverb, 1, 2);
			this.tlpExSceneMeta.Controls.Add(this.lblSceneMetaNightSFX, 0, 1);
			this.tlpExSceneMeta.Controls.Add(this.label1, 0, 3);
			this.tlpExSceneMeta.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpExSceneMeta.Location = new System.Drawing.Point(0, 0);
			this.tlpExSceneMeta.Name = "tlpExSceneMeta";
			this.tlpExSceneMeta.RowCount = 4;
			this.tlpExSceneMeta.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExSceneMeta.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExSceneMeta.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExSceneMeta.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpExSceneMeta.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpExSceneMeta.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpExSceneMeta.Size = new System.Drawing.Size(322, 436);
			this.tlpExSceneMeta.TabIndex = 0;
			// 
			// nudSceneMetaNightSFX
			// 
			this.nudSceneMetaNightSFX.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nudSceneMetaNightSFX.Location = new System.Drawing.Point(99, 30);
			this.nudSceneMetaNightSFX.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudSceneMetaNightSFX.Name = "nudSceneMetaNightSFX";
			this.nudSceneMetaNightSFX.Size = new System.Drawing.Size(58, 20);
			this.nudSceneMetaNightSFX.TabIndex = 4;
			this.nudSceneMetaNightSFX.ValueChanged += new System.EventHandler(this.nudSceneMetaNightSFX_ValueChanged);
			// 
			// cbSceneMetaBGM
			// 
			this.tlpExSceneMeta.SetColumnSpan(this.cbSceneMetaBGM, 3);
			this.cbSceneMetaBGM.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbSceneMetaBGM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSceneMetaBGM.FormattingEnabled = true;
			this.cbSceneMetaBGM.Location = new System.Drawing.Point(99, 3);
			this.cbSceneMetaBGM.Name = "cbSceneMetaBGM";
			this.cbSceneMetaBGM.Size = new System.Drawing.Size(220, 21);
			this.cbSceneMetaBGM.TabIndex = 0;
			// 
			// lblSceneMetaBGM
			// 
			this.lblSceneMetaBGM.AutoSize = true;
			this.lblSceneMetaBGM.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblSceneMetaBGM.Location = new System.Drawing.Point(3, 0);
			this.lblSceneMetaBGM.Name = "lblSceneMetaBGM";
			this.lblSceneMetaBGM.Size = new System.Drawing.Size(90, 27);
			this.lblSceneMetaBGM.TabIndex = 1;
			this.lblSceneMetaBGM.Text = "Music Track:";
			this.lblSceneMetaBGM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblSceneMetaReverb
			// 
			this.lblSceneMetaReverb.AutoSize = true;
			this.lblSceneMetaReverb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblSceneMetaReverb.Location = new System.Drawing.Point(3, 53);
			this.lblSceneMetaReverb.Name = "lblSceneMetaReverb";
			this.lblSceneMetaReverb.Size = new System.Drawing.Size(90, 26);
			this.lblSceneMetaReverb.TabIndex = 2;
			this.lblSceneMetaReverb.Text = "Reverb:";
			this.lblSceneMetaReverb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// nudSceneMetaReverb
			// 
			this.nudSceneMetaReverb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nudSceneMetaReverb.Location = new System.Drawing.Point(99, 56);
			this.nudSceneMetaReverb.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudSceneMetaReverb.Name = "nudSceneMetaReverb";
			this.nudSceneMetaReverb.Size = new System.Drawing.Size(58, 20);
			this.nudSceneMetaReverb.TabIndex = 3;
			this.nudSceneMetaReverb.ValueChanged += new System.EventHandler(this.nudSceneMetaReverb_ValueChanged);
			// 
			// lblSceneMetaNightSFX
			// 
			this.lblSceneMetaNightSFX.AutoSize = true;
			this.lblSceneMetaNightSFX.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblSceneMetaNightSFX.Location = new System.Drawing.Point(3, 27);
			this.lblSceneMetaNightSFX.Name = "lblSceneMetaNightSFX";
			this.lblSceneMetaNightSFX.Size = new System.Drawing.Size(90, 26);
			this.lblSceneMetaNightSFX.TabIndex = 5;
			this.lblSceneMetaNightSFX.Text = "Nighttime SFX:";
			this.lblSceneMetaNightSFX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.tlpExSceneMeta.SetColumnSpan(this.label1, 4);
			this.label1.Location = new System.Drawing.Point(3, 79);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "(More options in progress)";
			// 
			// tpRoomActors
			// 
			this.tpRoomActors.AutoScroll = true;
			this.tpRoomActors.Controls.Add(this.tlpExRoomActors);
			this.tpRoomActors.Controls.Add(this.cbActors);
			this.tpRoomActors.Location = new System.Drawing.Point(4, 40);
			this.tpRoomActors.Name = "tpRoomActors";
			this.tpRoomActors.Padding = new System.Windows.Forms.Padding(3);
			this.tpRoomActors.Size = new System.Drawing.Size(322, 436);
			this.tpRoomActors.TabIndex = 1;
			this.tpRoomActors.Text = "Room Actors";
			this.tpRoomActors.UseVisualStyleBackColor = true;
			// 
			// tlpExRoomActors
			// 
			this.tlpExRoomActors.AutoSize = true;
			this.tlpExRoomActors.ColumnCount = 2;
			this.tlpExRoomActors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tlpExRoomActors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tlpExRoomActors.Dock = System.Windows.Forms.DockStyle.Top;
			this.tlpExRoomActors.Location = new System.Drawing.Point(3, 24);
			this.tlpExRoomActors.Name = "tlpExRoomActors";
			this.tlpExRoomActors.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this.tlpExRoomActors.RowCount = 1;
			this.tlpExRoomActors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpExRoomActors.Size = new System.Drawing.Size(316, 6);
			this.tlpExRoomActors.TabIndex = 0;
			// 
			// cbActors
			// 
			this.cbActors.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbActors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbActors.Enabled = false;
			this.cbActors.FormattingEnabled = true;
			this.cbActors.Location = new System.Drawing.Point(3, 3);
			this.cbActors.Name = "cbActors";
			this.cbActors.Size = new System.Drawing.Size(316, 21);
			this.cbActors.TabIndex = 2;
			this.cbActors.SelectedIndexChanged += new System.EventHandler(this.cbActors_SelectedIndexChanged);
			// 
			// tpObjects
			// 
			this.tpObjects.Controls.Add(this.tableLayoutPanel1);
			this.tpObjects.Location = new System.Drawing.Point(4, 40);
			this.tpObjects.Name = "tpObjects";
			this.tpObjects.Padding = new System.Windows.Forms.Padding(3);
			this.tpObjects.Size = new System.Drawing.Size(322, 436);
			this.tpObjects.TabIndex = 4;
			this.tpObjects.Text = "Objects";
			this.tpObjects.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tableLayoutPanel1.Controls.Add(this.lblSpecialObjs, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.dgvObjects, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.cbSpecialObjs, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(316, 430);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// lblSpecialObjs
			// 
			this.lblSpecialObjs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblSpecialObjs.Location = new System.Drawing.Point(0, 0);
			this.lblSpecialObjs.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.lblSpecialObjs.Name = "lblSpecialObjs";
			this.lblSpecialObjs.Size = new System.Drawing.Size(126, 21);
			this.lblSpecialObjs.TabIndex = 1;
			this.lblSpecialObjs.Text = "Special Scene Objects:";
			this.lblSpecialObjs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// dgvObjects
			// 
			this.dgvObjects.AllowUserToAddRows = false;
			this.dgvObjects.AllowUserToDeleteRows = false;
			this.dgvObjects.AllowUserToResizeColumns = false;
			this.dgvObjects.AllowUserToResizeRows = false;
			this.dgvObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.tableLayoutPanel1.SetColumnSpan(this.dgvObjects, 2);
			this.dgvObjects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvObjects.Enabled = false;
			this.dgvObjects.Location = new System.Drawing.Point(0, 27);
			this.dgvObjects.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.dgvObjects.MultiSelect = false;
			this.dgvObjects.Name = "dgvObjects";
			this.dgvObjects.RowHeadersVisible = false;
			this.dgvObjects.Size = new System.Drawing.Size(316, 403);
			this.dgvObjects.TabIndex = 4;
			this.dgvObjects.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvObjects_CellFormatting);
			this.dgvObjects.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.dgvObjects_CellParsing);
			this.dgvObjects.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvObjects_DataError);
			this.dgvObjects.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvObjects_EditingControlShowing);
			// 
			// cbSpecialObjs
			// 
			this.cbSpecialObjs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbSpecialObjs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSpecialObjs.Enabled = false;
			this.cbSpecialObjs.FormattingEnabled = true;
			this.cbSpecialObjs.Location = new System.Drawing.Point(126, 0);
			this.cbSpecialObjs.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.cbSpecialObjs.Name = "cbSpecialObjs";
			this.cbSpecialObjs.Size = new System.Drawing.Size(190, 21);
			this.cbSpecialObjs.TabIndex = 2;
			// 
			// tpSpawnPoints
			// 
			this.tpSpawnPoints.AutoScroll = true;
			this.tpSpawnPoints.Controls.Add(this.tlpExSpawnPoints);
			this.tpSpawnPoints.Controls.Add(this.cbSpawnPoints);
			this.tpSpawnPoints.Location = new System.Drawing.Point(4, 40);
			this.tpSpawnPoints.Name = "tpSpawnPoints";
			this.tpSpawnPoints.Padding = new System.Windows.Forms.Padding(3);
			this.tpSpawnPoints.Size = new System.Drawing.Size(322, 436);
			this.tpSpawnPoints.TabIndex = 3;
			this.tpSpawnPoints.Text = "Spawn Points";
			this.tpSpawnPoints.UseVisualStyleBackColor = true;
			// 
			// tlpExSpawnPoints
			// 
			this.tlpExSpawnPoints.AutoSize = true;
			this.tlpExSpawnPoints.ColumnCount = 2;
			this.tlpExSpawnPoints.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tlpExSpawnPoints.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tlpExSpawnPoints.Dock = System.Windows.Forms.DockStyle.Top;
			this.tlpExSpawnPoints.Location = new System.Drawing.Point(3, 24);
			this.tlpExSpawnPoints.Name = "tlpExSpawnPoints";
			this.tlpExSpawnPoints.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this.tlpExSpawnPoints.RowCount = 1;
			this.tlpExSpawnPoints.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpExSpawnPoints.Size = new System.Drawing.Size(316, 6);
			this.tlpExSpawnPoints.TabIndex = 3;
			// 
			// cbSpawnPoints
			// 
			this.cbSpawnPoints.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbSpawnPoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSpawnPoints.Enabled = false;
			this.cbSpawnPoints.FormattingEnabled = true;
			this.cbSpawnPoints.Location = new System.Drawing.Point(3, 3);
			this.cbSpawnPoints.Name = "cbSpawnPoints";
			this.cbSpawnPoints.Size = new System.Drawing.Size(316, 21);
			this.cbSpawnPoints.TabIndex = 4;
			this.cbSpawnPoints.SelectedIndexChanged += new System.EventHandler(this.cbSpawnPoints_SelectedIndexChanged);
			// 
			// tpTransitions
			// 
			this.tpTransitions.AutoScroll = true;
			this.tpTransitions.Controls.Add(this.tlpExTransitions);
			this.tpTransitions.Controls.Add(this.cbTransitions);
			this.tpTransitions.Location = new System.Drawing.Point(4, 40);
			this.tpTransitions.Name = "tpTransitions";
			this.tpTransitions.Padding = new System.Windows.Forms.Padding(3);
			this.tpTransitions.Size = new System.Drawing.Size(322, 436);
			this.tpTransitions.TabIndex = 2;
			this.tpTransitions.Text = "Transitions";
			this.tpTransitions.UseVisualStyleBackColor = true;
			// 
			// tlpExTransitions
			// 
			this.tlpExTransitions.AutoSize = true;
			this.tlpExTransitions.ColumnCount = 2;
			this.tlpExTransitions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tlpExTransitions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tlpExTransitions.Dock = System.Windows.Forms.DockStyle.Top;
			this.tlpExTransitions.Location = new System.Drawing.Point(3, 24);
			this.tlpExTransitions.Name = "tlpExTransitions";
			this.tlpExTransitions.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this.tlpExTransitions.RowCount = 1;
			this.tlpExTransitions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpExTransitions.Size = new System.Drawing.Size(316, 6);
			this.tlpExTransitions.TabIndex = 3;
			// 
			// cbTransitions
			// 
			this.cbTransitions.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbTransitions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTransitions.Enabled = false;
			this.cbTransitions.FormattingEnabled = true;
			this.cbTransitions.Location = new System.Drawing.Point(3, 3);
			this.cbTransitions.Name = "cbTransitions";
			this.cbTransitions.Size = new System.Drawing.Size(316, 21);
			this.cbTransitions.TabIndex = 4;
			this.cbTransitions.SelectedIndexChanged += new System.EventHandler(this.cbTransitions_SelectedIndexChanged);
			// 
			// tpWaypoints
			// 
			this.tpWaypoints.AutoScroll = true;
			this.tpWaypoints.Controls.Add(this.tableLayoutPanel2);
			this.tpWaypoints.Location = new System.Drawing.Point(4, 40);
			this.tpWaypoints.Name = "tpWaypoints";
			this.tpWaypoints.Padding = new System.Windows.Forms.Padding(3);
			this.tpWaypoints.Size = new System.Drawing.Size(322, 436);
			this.tpWaypoints.TabIndex = 5;
			this.tpWaypoints.Text = "Waypoints";
			this.tpWaypoints.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.Controls.Add(this.dgvPathWaypoints, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.cbPathHeaders, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(316, 430);
			this.tableLayoutPanel2.TabIndex = 2;
			// 
			// dgvPathWaypoints
			// 
			this.dgvPathWaypoints.AllowUserToAddRows = false;
			this.dgvPathWaypoints.AllowUserToDeleteRows = false;
			this.dgvPathWaypoints.AllowUserToResizeColumns = false;
			this.dgvPathWaypoints.AllowUserToResizeRows = false;
			this.dgvPathWaypoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPathWaypoints.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPathWaypoints.Enabled = false;
			this.dgvPathWaypoints.Location = new System.Drawing.Point(0, 27);
			this.dgvPathWaypoints.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.dgvPathWaypoints.MultiSelect = false;
			this.dgvPathWaypoints.Name = "dgvPathWaypoints";
			this.dgvPathWaypoints.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
			this.dgvPathWaypoints.Size = new System.Drawing.Size(316, 403);
			this.dgvPathWaypoints.TabIndex = 4;
			this.dgvPathWaypoints.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvPathWaypoints_RowPostPaint);
			this.dgvPathWaypoints.SelectionChanged += new System.EventHandler(this.dgvPathWaypoints_SelectionChanged);
			// 
			// cbPathHeaders
			// 
			this.cbPathHeaders.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbPathHeaders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPathHeaders.Enabled = false;
			this.cbPathHeaders.FormattingEnabled = true;
			this.cbPathHeaders.Location = new System.Drawing.Point(0, 0);
			this.cbPathHeaders.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.cbPathHeaders.Name = "cbPathHeaders";
			this.cbPathHeaders.Size = new System.Drawing.Size(316, 21);
			this.cbPathHeaders.TabIndex = 2;
			this.cbPathHeaders.SelectionChangeCommitted += new System.EventHandler(this.cbPathHeaders_SelectionChangeCommitted);
			// 
			// tpCollision
			// 
			this.tpCollision.AutoScroll = true;
			this.tpCollision.Controls.Add(this.tlpExCollision);
			this.tpCollision.Location = new System.Drawing.Point(4, 40);
			this.tpCollision.Name = "tpCollision";
			this.tpCollision.Size = new System.Drawing.Size(322, 436);
			this.tpCollision.TabIndex = 7;
			this.tpCollision.Text = "Collision";
			this.tpCollision.UseVisualStyleBackColor = true;
			// 
			// tlpExCollision
			// 
			this.tlpExCollision.ColumnCount = 2;
			this.tlpExCollision.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tlpExCollision.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tlpExCollision.Controls.Add(this.label2, 0, 10);
			this.tlpExCollision.Controls.Add(this.cbCollisionPolyTypes, 0, 7);
			this.tlpExCollision.Controls.Add(this.cbCollisionPolys, 0, 2);
			this.tlpExCollision.Controls.Add(this.lblColPolyType, 0, 3);
			this.tlpExCollision.Controls.Add(this.nudColPolyType, 1, 3);
			this.tlpExCollision.Controls.Add(this.lblColPolyGroundType, 0, 9);
			this.tlpExCollision.Controls.Add(this.cbColPolyGroundTypes, 1, 9);
			this.tlpExCollision.Controls.Add(this.lblColPolyRawData, 0, 8);
			this.tlpExCollision.Controls.Add(this.txtColPolyRawData, 1, 8);
			this.tlpExCollision.Controls.Add(this.btnJumpToPolyType, 1, 4);
			this.tlpExCollision.Controls.Add(this.lblCollisionPolys, 0, 1);
			this.tlpExCollision.Controls.Add(this.lblCollisionPolyTypes, 0, 6);
			this.tlpExCollision.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpExCollision.Location = new System.Drawing.Point(0, 0);
			this.tlpExCollision.Name = "tlpExCollision";
			this.tlpExCollision.RowCount = 11;
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 7F));
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 7F));
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExCollision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tlpExCollision.Size = new System.Drawing.Size(322, 436);
			this.tlpExCollision.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.tlpExCollision.SetColumnSpan(this.label2, 4);
			this.label2.Location = new System.Drawing.Point(3, 203);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "(More options in progress)";
			// 
			// cbCollisionPolyTypes
			// 
			this.tlpExCollision.SetColumnSpan(this.cbCollisionPolyTypes, 2);
			this.cbCollisionPolyTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbCollisionPolyTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbCollisionPolyTypes.Enabled = false;
			this.cbCollisionPolyTypes.FormattingEnabled = true;
			this.cbCollisionPolyTypes.Location = new System.Drawing.Point(3, 125);
			this.cbCollisionPolyTypes.Name = "cbCollisionPolyTypes";
			this.cbCollisionPolyTypes.Size = new System.Drawing.Size(316, 21);
			this.cbCollisionPolyTypes.TabIndex = 5;
			this.cbCollisionPolyTypes.SelectedIndexChanged += new System.EventHandler(this.cbCollisionPolyTypes_SelectedIndexChanged);
			// 
			// cbCollisionPolys
			// 
			this.tlpExCollision.SetColumnSpan(this.cbCollisionPolys, 2);
			this.cbCollisionPolys.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbCollisionPolys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbCollisionPolys.Enabled = false;
			this.cbCollisionPolys.FormattingEnabled = true;
			this.cbCollisionPolys.Location = new System.Drawing.Point(3, 23);
			this.cbCollisionPolys.Name = "cbCollisionPolys";
			this.cbCollisionPolys.Size = new System.Drawing.Size(316, 21);
			this.cbCollisionPolys.TabIndex = 0;
			this.cbCollisionPolys.SelectedIndexChanged += new System.EventHandler(this.cbCollisionPolys_SelectedIndexChanged);
			// 
			// lblColPolyType
			// 
			this.lblColPolyType.AutoSize = true;
			this.lblColPolyType.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblColPolyType.Location = new System.Drawing.Point(3, 47);
			this.lblColPolyType.Name = "lblColPolyType";
			this.lblColPolyType.Size = new System.Drawing.Size(122, 26);
			this.lblColPolyType.TabIndex = 1;
			this.lblColPolyType.Text = "Polygon Type ID:";
			this.lblColPolyType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblColPolyType.Visible = false;
			// 
			// nudColPolyType
			// 
			this.nudColPolyType.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nudColPolyType.Enabled = false;
			this.nudColPolyType.Location = new System.Drawing.Point(131, 50);
			this.nudColPolyType.Name = "nudColPolyType";
			this.nudColPolyType.Size = new System.Drawing.Size(188, 20);
			this.nudColPolyType.TabIndex = 2;
			this.nudColPolyType.Visible = false;
			this.nudColPolyType.ValueChanged += new System.EventHandler(this.nudColPolyType_ValueChanged);
			// 
			// lblColPolyGroundType
			// 
			this.lblColPolyGroundType.AutoSize = true;
			this.lblColPolyGroundType.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblColPolyGroundType.Location = new System.Drawing.Point(3, 176);
			this.lblColPolyGroundType.Name = "lblColPolyGroundType";
			this.lblColPolyGroundType.Size = new System.Drawing.Size(122, 27);
			this.lblColPolyGroundType.TabIndex = 3;
			this.lblColPolyGroundType.Text = "Ground Type:";
			this.lblColPolyGroundType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblColPolyGroundType.Visible = false;
			// 
			// cbColPolyGroundTypes
			// 
			this.cbColPolyGroundTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbColPolyGroundTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbColPolyGroundTypes.Enabled = false;
			this.cbColPolyGroundTypes.FormattingEnabled = true;
			this.cbColPolyGroundTypes.Location = new System.Drawing.Point(131, 179);
			this.cbColPolyGroundTypes.Name = "cbColPolyGroundTypes";
			this.cbColPolyGroundTypes.Size = new System.Drawing.Size(188, 21);
			this.cbColPolyGroundTypes.TabIndex = 4;
			this.cbColPolyGroundTypes.Visible = false;
			this.cbColPolyGroundTypes.SelectedIndexChanged += new System.EventHandler(this.cbColPolyGroundTypes_SelectedIndexChanged);
			// 
			// lblColPolyRawData
			// 
			this.lblColPolyRawData.AutoSize = true;
			this.lblColPolyRawData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblColPolyRawData.Location = new System.Drawing.Point(3, 149);
			this.lblColPolyRawData.Name = "lblColPolyRawData";
			this.lblColPolyRawData.Size = new System.Drawing.Size(122, 27);
			this.lblColPolyRawData.TabIndex = 6;
			this.lblColPolyRawData.Text = "Raw:";
			this.lblColPolyRawData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblColPolyRawData.Visible = false;
			// 
			// txtColPolyRawData
			// 
			this.txtColPolyRawData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtColPolyRawData.Enabled = false;
			this.txtColPolyRawData.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtColPolyRawData.Location = new System.Drawing.Point(131, 152);
			this.txtColPolyRawData.Name = "txtColPolyRawData";
			this.txtColPolyRawData.Size = new System.Drawing.Size(188, 21);
			this.txtColPolyRawData.TabIndex = 7;
			this.txtColPolyRawData.Visible = false;
			this.txtColPolyRawData.TextChanged += new System.EventHandler(this.txtColPolyRawData_TextChanged);
			// 
			// btnJumpToPolyType
			// 
			this.btnJumpToPolyType.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnJumpToPolyType.Location = new System.Drawing.Point(131, 76);
			this.btnJumpToPolyType.Name = "btnJumpToPolyType";
			this.btnJumpToPolyType.Size = new System.Drawing.Size(188, 23);
			this.btnJumpToPolyType.TabIndex = 8;
			this.btnJumpToPolyType.Text = "&Jump to Polygon Type";
			this.btnJumpToPolyType.UseVisualStyleBackColor = true;
			this.btnJumpToPolyType.Visible = false;
			this.btnJumpToPolyType.Click += new System.EventHandler(this.btnJumpToPolyType_Click);
			// 
			// lblCollisionPolys
			// 
			this.lblCollisionPolys.AutoSize = true;
			this.lblCollisionPolys.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblCollisionPolys.Location = new System.Drawing.Point(3, 7);
			this.lblCollisionPolys.Name = "lblCollisionPolys";
			this.lblCollisionPolys.Size = new System.Drawing.Size(122, 13);
			this.lblCollisionPolys.TabIndex = 9;
			this.lblCollisionPolys.Text = "Collision Polygons:";
			// 
			// lblCollisionPolyTypes
			// 
			this.lblCollisionPolyTypes.AutoSize = true;
			this.lblCollisionPolyTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblCollisionPolyTypes.Location = new System.Drawing.Point(3, 109);
			this.lblCollisionPolyTypes.Name = "lblCollisionPolyTypes";
			this.lblCollisionPolyTypes.Size = new System.Drawing.Size(122, 13);
			this.lblCollisionPolyTypes.TabIndex = 10;
			this.lblCollisionPolyTypes.Text = "Polygon Types:";
			// 
			// tpWaterboxes
			// 
			this.tpWaterboxes.Controls.Add(this.tlpExWaterboxes);
			this.tpWaterboxes.Controls.Add(this.cbWaterboxes);
			this.tpWaterboxes.Location = new System.Drawing.Point(4, 40);
			this.tpWaterboxes.Name = "tpWaterboxes";
			this.tpWaterboxes.Padding = new System.Windows.Forms.Padding(3);
			this.tpWaterboxes.Size = new System.Drawing.Size(322, 436);
			this.tpWaterboxes.TabIndex = 8;
			this.tpWaterboxes.Text = "Waterboxes";
			this.tpWaterboxes.UseVisualStyleBackColor = true;
			// 
			// tlpExWaterboxes
			// 
			this.tlpExWaterboxes.AutoSize = true;
			this.tlpExWaterboxes.ColumnCount = 2;
			this.tlpExWaterboxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tlpExWaterboxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tlpExWaterboxes.Controls.Add(this.lblWaterboxProperties, 0, 7);
			this.tlpExWaterboxes.Controls.Add(this.txtWaterboxProperties, 0, 7);
			this.tlpExWaterboxes.Controls.Add(this.lblWaterboxPositionX, 0, 1);
			this.tlpExWaterboxes.Controls.Add(this.txtWaterboxPositionX, 1, 1);
			this.tlpExWaterboxes.Controls.Add(this.lblWaterboxPositionY, 0, 2);
			this.tlpExWaterboxes.Controls.Add(this.txtWaterboxPositionY, 1, 2);
			this.tlpExWaterboxes.Controls.Add(this.lblWaterboxPositionZ, 0, 3);
			this.tlpExWaterboxes.Controls.Add(this.txtWaterboxPositionZ, 1, 3);
			this.tlpExWaterboxes.Controls.Add(this.lblWaterboxSizeX, 0, 4);
			this.tlpExWaterboxes.Controls.Add(this.txtWaterboxSizeX, 1, 4);
			this.tlpExWaterboxes.Controls.Add(this.lblWaterboxSizeZ, 0, 5);
			this.tlpExWaterboxes.Controls.Add(this.txtWaterboxSizeZ, 1, 5);
			this.tlpExWaterboxes.Controls.Add(this.lblWaterboxRoom, 0, 6);
			this.tlpExWaterboxes.Controls.Add(this.cbWaterboxRoom, 1, 6);
			this.tlpExWaterboxes.Dock = System.Windows.Forms.DockStyle.Top;
			this.tlpExWaterboxes.Location = new System.Drawing.Point(3, 24);
			this.tlpExWaterboxes.Name = "tlpExWaterboxes";
			this.tlpExWaterboxes.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this.tlpExWaterboxes.RowCount = 8;
			this.tlpExWaterboxes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpExWaterboxes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExWaterboxes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExWaterboxes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExWaterboxes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExWaterboxes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExWaterboxes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExWaterboxes.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpExWaterboxes.Size = new System.Drawing.Size(316, 189);
			this.tlpExWaterboxes.TabIndex = 4;
			// 
			// lblWaterboxProperties
			// 
			this.lblWaterboxProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblWaterboxProperties.Location = new System.Drawing.Point(3, 163);
			this.lblWaterboxProperties.Name = "lblWaterboxProperties";
			this.lblWaterboxProperties.Size = new System.Drawing.Size(120, 26);
			this.lblWaterboxProperties.TabIndex = 13;
			this.lblWaterboxProperties.Text = "Properties:";
			this.lblWaterboxProperties.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtWaterboxProperties
			// 
			this.txtWaterboxProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtWaterboxProperties.Location = new System.Drawing.Point(129, 166);
			this.txtWaterboxProperties.Name = "txtWaterboxProperties";
			this.txtWaterboxProperties.Size = new System.Drawing.Size(184, 20);
			this.txtWaterboxProperties.TabIndex = 14;
			this.txtWaterboxProperties.TextChanged += new System.EventHandler(this.txtWaterboxProperties_TextChanged);
			// 
			// lblWaterboxPositionX
			// 
			this.lblWaterboxPositionX.AutoSize = true;
			this.lblWaterboxPositionX.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblWaterboxPositionX.Location = new System.Drawing.Point(3, 6);
			this.lblWaterboxPositionX.Name = "lblWaterboxPositionX";
			this.lblWaterboxPositionX.Size = new System.Drawing.Size(120, 26);
			this.lblWaterboxPositionX.TabIndex = 5;
			this.lblWaterboxPositionX.Text = "Position (X):";
			this.lblWaterboxPositionX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtWaterboxPositionX
			// 
			this.txtWaterboxPositionX.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtWaterboxPositionX.Location = new System.Drawing.Point(129, 9);
			this.txtWaterboxPositionX.Name = "txtWaterboxPositionX";
			this.txtWaterboxPositionX.Size = new System.Drawing.Size(184, 20);
			this.txtWaterboxPositionX.TabIndex = 6;
			this.txtWaterboxPositionX.TextChanged += new System.EventHandler(this.txtWaterboxPositionX_TextChanged);
			// 
			// lblWaterboxPositionY
			// 
			this.lblWaterboxPositionY.AutoSize = true;
			this.lblWaterboxPositionY.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblWaterboxPositionY.Location = new System.Drawing.Point(3, 32);
			this.lblWaterboxPositionY.Name = "lblWaterboxPositionY";
			this.lblWaterboxPositionY.Size = new System.Drawing.Size(120, 26);
			this.lblWaterboxPositionY.TabIndex = 5;
			this.lblWaterboxPositionY.Text = "Position (Y):";
			this.lblWaterboxPositionY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtWaterboxPositionY
			// 
			this.txtWaterboxPositionY.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtWaterboxPositionY.Location = new System.Drawing.Point(129, 35);
			this.txtWaterboxPositionY.Name = "txtWaterboxPositionY";
			this.txtWaterboxPositionY.Size = new System.Drawing.Size(184, 20);
			this.txtWaterboxPositionY.TabIndex = 6;
			this.txtWaterboxPositionY.TextChanged += new System.EventHandler(this.txtWaterboxPositionY_TextChanged);
			// 
			// lblWaterboxPositionZ
			// 
			this.lblWaterboxPositionZ.AutoSize = true;
			this.lblWaterboxPositionZ.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblWaterboxPositionZ.Location = new System.Drawing.Point(3, 58);
			this.lblWaterboxPositionZ.Name = "lblWaterboxPositionZ";
			this.lblWaterboxPositionZ.Size = new System.Drawing.Size(120, 26);
			this.lblWaterboxPositionZ.TabIndex = 5;
			this.lblWaterboxPositionZ.Text = "Position (Z):";
			this.lblWaterboxPositionZ.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtWaterboxPositionZ
			// 
			this.txtWaterboxPositionZ.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtWaterboxPositionZ.Location = new System.Drawing.Point(129, 61);
			this.txtWaterboxPositionZ.Name = "txtWaterboxPositionZ";
			this.txtWaterboxPositionZ.Size = new System.Drawing.Size(184, 20);
			this.txtWaterboxPositionZ.TabIndex = 6;
			this.txtWaterboxPositionZ.TextChanged += new System.EventHandler(this.txtWaterboxPositionZ_TextChanged);
			// 
			// lblWaterboxSizeX
			// 
			this.lblWaterboxSizeX.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblWaterboxSizeX.Location = new System.Drawing.Point(3, 84);
			this.lblWaterboxSizeX.Name = "lblWaterboxSizeX";
			this.lblWaterboxSizeX.Size = new System.Drawing.Size(120, 26);
			this.lblWaterboxSizeX.TabIndex = 7;
			this.lblWaterboxSizeX.Text = "Size (X):";
			this.lblWaterboxSizeX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtWaterboxSizeX
			// 
			this.txtWaterboxSizeX.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtWaterboxSizeX.Location = new System.Drawing.Point(129, 87);
			this.txtWaterboxSizeX.Name = "txtWaterboxSizeX";
			this.txtWaterboxSizeX.Size = new System.Drawing.Size(184, 20);
			this.txtWaterboxSizeX.TabIndex = 8;
			this.txtWaterboxSizeX.TextChanged += new System.EventHandler(this.txtWaterboxSizeX_TextChanged);
			// 
			// lblWaterboxSizeZ
			// 
			this.lblWaterboxSizeZ.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblWaterboxSizeZ.Location = new System.Drawing.Point(3, 110);
			this.lblWaterboxSizeZ.Name = "lblWaterboxSizeZ";
			this.lblWaterboxSizeZ.Size = new System.Drawing.Size(120, 26);
			this.lblWaterboxSizeZ.TabIndex = 9;
			this.lblWaterboxSizeZ.Text = "Size (Z):";
			this.lblWaterboxSizeZ.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtWaterboxSizeZ
			// 
			this.txtWaterboxSizeZ.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtWaterboxSizeZ.Location = new System.Drawing.Point(129, 113);
			this.txtWaterboxSizeZ.Name = "txtWaterboxSizeZ";
			this.txtWaterboxSizeZ.Size = new System.Drawing.Size(184, 20);
			this.txtWaterboxSizeZ.TabIndex = 10;
			this.txtWaterboxSizeZ.TextChanged += new System.EventHandler(this.txtWaterboxSizeZ_TextChanged);
			// 
			// lblWaterboxRoom
			// 
			this.lblWaterboxRoom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblWaterboxRoom.Location = new System.Drawing.Point(3, 136);
			this.lblWaterboxRoom.Name = "lblWaterboxRoom";
			this.lblWaterboxRoom.Size = new System.Drawing.Size(120, 27);
			this.lblWaterboxRoom.TabIndex = 11;
			this.lblWaterboxRoom.Text = "Exists in Room:";
			this.lblWaterboxRoom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbWaterboxRoom
			// 
			this.cbWaterboxRoom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbWaterboxRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbWaterboxRoom.Location = new System.Drawing.Point(129, 139);
			this.cbWaterboxRoom.Name = "cbWaterboxRoom";
			this.cbWaterboxRoom.Size = new System.Drawing.Size(184, 21);
			this.cbWaterboxRoom.TabIndex = 12;
			this.cbWaterboxRoom.SelectedIndexChanged += new System.EventHandler(this.cbWaterboxRoom_SelectedIndexChanged);
			// 
			// cbWaterboxes
			// 
			this.cbWaterboxes.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbWaterboxes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbWaterboxes.Enabled = false;
			this.cbWaterboxes.FormattingEnabled = true;
			this.cbWaterboxes.Location = new System.Drawing.Point(3, 3);
			this.cbWaterboxes.Name = "cbWaterboxes";
			this.cbWaterboxes.Size = new System.Drawing.Size(316, 21);
			this.cbWaterboxes.TabIndex = 3;
			this.cbWaterboxes.SelectedIndexChanged += new System.EventHandler(this.cbWaterboxes_SelectedIndexChanged);
			// 
			// ofdOpenROM
			// 
			this.ofdOpenROM.Filter = "Nintendo 64 ROMs (*.z64;*.v64;*.n64;*.bin)|*.z64;*.v64;*.n64;*.bin|All Files (*.*" +
    ")|*.*";
			this.ofdOpenROM.Title = "Open ROM File";
			// 
			// cmsMoveableObjectEdit
			// 
			this.cmsMoveableObjectEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deselectToolStripMenuItem,
            this.toolStripMenuItem6,
            this.rotateToolStripMenuItem});
			this.cmsMoveableObjectEdit.Name = "cmsPickableObjectEdit";
			this.cmsMoveableObjectEdit.Size = new System.Drawing.Size(119, 54);
			// 
			// deselectToolStripMenuItem
			// 
			this.deselectToolStripMenuItem.HelpText = null;
			this.deselectToolStripMenuItem.Name = "deselectToolStripMenuItem";
			this.deselectToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.deselectToolStripMenuItem.Text = "&Deselect";
			this.deselectToolStripMenuItem.Click += new System.EventHandler(this.deselectToolStripMenuItem_Click);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(115, 6);
			// 
			// rotateToolStripMenuItem
			// 
			this.rotateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xAxisToolStripMenuItem,
            this.yAxisToolStripMenuItem,
            this.zAxisToolStripMenuItem});
			this.rotateToolStripMenuItem.HelpText = null;
			this.rotateToolStripMenuItem.Name = "rotateToolStripMenuItem";
			this.rotateToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.rotateToolStripMenuItem.Text = "&Rotate";
			// 
			// xAxisToolStripMenuItem
			// 
			this.xAxisToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xPlus45DegreesToolStripMenuItem,
            this.xMinus45DegreesToolStripMenuItem});
			this.xAxisToolStripMenuItem.HelpText = null;
			this.xAxisToolStripMenuItem.Name = "xAxisToolStripMenuItem";
			this.xAxisToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			this.xAxisToolStripMenuItem.Text = "&X Axis";
			// 
			// xPlus45DegreesToolStripMenuItem
			// 
			this.xPlus45DegreesToolStripMenuItem.HelpText = null;
			this.xPlus45DegreesToolStripMenuItem.Name = "xPlus45DegreesToolStripMenuItem";
			this.xPlus45DegreesToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.xPlus45DegreesToolStripMenuItem.Text = "+45 Degrees";
			this.xPlus45DegreesToolStripMenuItem.Click += new System.EventHandler(this.xPlus45DegreesToolStripMenuItem_Click);
			// 
			// xMinus45DegreesToolStripMenuItem
			// 
			this.xMinus45DegreesToolStripMenuItem.HelpText = null;
			this.xMinus45DegreesToolStripMenuItem.Name = "xMinus45DegreesToolStripMenuItem";
			this.xMinus45DegreesToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.xMinus45DegreesToolStripMenuItem.Text = "-45 Degrees";
			this.xMinus45DegreesToolStripMenuItem.Click += new System.EventHandler(this.xMinus45DegreesToolStripMenuItem_Click);
			// 
			// yAxisToolStripMenuItem
			// 
			this.yAxisToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.yPlus45DegreesToolStripMenuItem,
            this.yMinus45DegreesToolStripMenuItem});
			this.yAxisToolStripMenuItem.HelpText = null;
			this.yAxisToolStripMenuItem.Name = "yAxisToolStripMenuItem";
			this.yAxisToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			this.yAxisToolStripMenuItem.Text = "&Y Axis";
			// 
			// yPlus45DegreesToolStripMenuItem
			// 
			this.yPlus45DegreesToolStripMenuItem.HelpText = null;
			this.yPlus45DegreesToolStripMenuItem.Name = "yPlus45DegreesToolStripMenuItem";
			this.yPlus45DegreesToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.yPlus45DegreesToolStripMenuItem.Text = "+45 Degrees";
			this.yPlus45DegreesToolStripMenuItem.Click += new System.EventHandler(this.yPlus45DegreesToolStripMenuItem_Click);
			// 
			// yMinus45DegreesToolStripMenuItem
			// 
			this.yMinus45DegreesToolStripMenuItem.HelpText = null;
			this.yMinus45DegreesToolStripMenuItem.Name = "yMinus45DegreesToolStripMenuItem";
			this.yMinus45DegreesToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.yMinus45DegreesToolStripMenuItem.Text = "-45 Degrees";
			this.yMinus45DegreesToolStripMenuItem.Click += new System.EventHandler(this.yMinus45DegreesToolStripMenuItem_Click);
			// 
			// zAxisToolStripMenuItem
			// 
			this.zAxisToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zPlus45DegreesToolStripMenuItem,
            this.zMinus45DegreesToolStripMenuItem});
			this.zAxisToolStripMenuItem.HelpText = null;
			this.zAxisToolStripMenuItem.Name = "zAxisToolStripMenuItem";
			this.zAxisToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			this.zAxisToolStripMenuItem.Text = "&Z Axis";
			// 
			// zPlus45DegreesToolStripMenuItem
			// 
			this.zPlus45DegreesToolStripMenuItem.HelpText = null;
			this.zPlus45DegreesToolStripMenuItem.Name = "zPlus45DegreesToolStripMenuItem";
			this.zPlus45DegreesToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.zPlus45DegreesToolStripMenuItem.Text = "+45 Degrees";
			this.zPlus45DegreesToolStripMenuItem.Click += new System.EventHandler(this.zPlus45DegreesToolStripMenuItem_Click);
			// 
			// zMinus45DegreesToolStripMenuItem
			// 
			this.zMinus45DegreesToolStripMenuItem.HelpText = null;
			this.zMinus45DegreesToolStripMenuItem.Name = "zMinus45DegreesToolStripMenuItem";
			this.zMinus45DegreesToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.zMinus45DegreesToolStripMenuItem.Text = "-45 Degrees";
			this.zMinus45DegreesToolStripMenuItem.Click += new System.EventHandler(this.zMinus45DegreesToolStripMenuItem_Click);
			// 
			// ofdOpenScene
			// 
			this.ofdOpenScene.Filter = "Scene Files (*.zscene)|*.zscene|All Files (*.*)|*.*";
			this.ofdOpenScene.Title = "Open Scene File";
			// 
			// ofdOpenRoom
			// 
			this.ofdOpenRoom.Filter = "Room Files (*.zmap;*.zroom)|*.zmap;*.zroom|All Files (*.*)|*.*";
			this.ofdOpenRoom.Title = "Open Room File";
			// 
			// cmsSceneTree
			// 
			this.cmsSceneTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertiesToolStripMenuItem});
			this.cmsSceneTree.Name = "cmsSceneTree";
			this.cmsSceneTree.Size = new System.Drawing.Size(128, 26);
			// 
			// propertiesToolStripMenuItem
			// 
			this.propertiesToolStripMenuItem.HelpText = null;
			this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
			this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
			this.propertiesToolStripMenuItem.Text = "&Properties";
			this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
			// 
			// customGLControl
			// 
			this.customGLControl.BackColor = System.Drawing.Color.Black;
			this.customGLControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.customGLControl.Enabled = false;
			this.customGLControl.Location = new System.Drawing.Point(0, 24);
			this.customGLControl.Name = "customGLControl";
			this.customGLControl.Size = new System.Drawing.Size(640, 480);
			this.customGLControl.TabIndex = 0;
			this.customGLControl.VSync = false;
			this.customGLControl.Load += new System.EventHandler(this.customGLControl_Load);
			this.customGLControl.Paint += new System.Windows.Forms.PaintEventHandler(this.customGLControl_Paint);
			this.customGLControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.customGLControl_KeyDown);
			this.customGLControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.customGLControl_KeyUp);
			this.customGLControl.Leave += new System.EventHandler(this.customGLControl_Leave);
			this.customGLControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.customGLControl_MouseDown);
			this.customGLControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.customGLControl_MouseMove);
			this.customGLControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.customGLControl_MouseUp);
			// 
			// cmsVertexEdit
			// 
			this.cmsVertexEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeColorToolStripMenuItem,
            this.toolStripMenuItem11,
            this.propertiesToolStripMenuItem1});
			this.cmsVertexEdit.Name = "cmsVertexEdit";
			this.cmsVertexEdit.Size = new System.Drawing.Size(157, 54);
			// 
			// changeColorToolStripMenuItem
			// 
			this.changeColorToolStripMenuItem.Name = "changeColorToolStripMenuItem";
			this.changeColorToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.changeColorToolStripMenuItem.Text = "Change &Color...";
			this.changeColorToolStripMenuItem.Click += new System.EventHandler(this.changeColorToolStripMenuItem_Click);
			// 
			// toolStripMenuItem11
			// 
			this.toolStripMenuItem11.Name = "toolStripMenuItem11";
			this.toolStripMenuItem11.Size = new System.Drawing.Size(153, 6);
			// 
			// propertiesToolStripMenuItem1
			// 
			this.propertiesToolStripMenuItem1.Name = "propertiesToolStripMenuItem1";
			this.propertiesToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
			this.propertiesToolStripMenuItem1.Text = "&Properties";
			this.propertiesToolStripMenuItem1.Click += new System.EventHandler(this.propertiesToolStripMenuItem1_Click);
			// 
			// emulateFogToolStripMenuItem
			// 
			this.emulateFogToolStripMenuItem.CheckOnClick = true;
			this.emulateFogToolStripMenuItem.HelpText = "Emulate fog according to environment settings";
			this.emulateFogToolStripMenuItem.Name = "emulateFogToolStripMenuItem";
			this.emulateFogToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.emulateFogToolStripMenuItem.Text = "Emulate &Fog";
			this.emulateFogToolStripMenuItem.Click += new System.EventHandler(this.emulateFogToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(970, 528);
			this.Controls.Add(this.customGLControl);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tpScenes.ResumeLayout(false);
			this.tpSceneMetadata.ResumeLayout(false);
			this.tlpExSceneMeta.ResumeLayout(false);
			this.tlpExSceneMeta.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSceneMetaNightSFX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSceneMetaReverb)).EndInit();
			this.tpRoomActors.ResumeLayout(false);
			this.tpRoomActors.PerformLayout();
			this.tpObjects.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvObjects)).EndInit();
			this.tpSpawnPoints.ResumeLayout(false);
			this.tpSpawnPoints.PerformLayout();
			this.tpTransitions.ResumeLayout(false);
			this.tpTransitions.PerformLayout();
			this.tpWaypoints.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvPathWaypoints)).EndInit();
			this.tpCollision.ResumeLayout(false);
			this.tlpExCollision.ResumeLayout(false);
			this.tlpExCollision.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudColPolyType)).EndInit();
			this.tpWaterboxes.ResumeLayout(false);
			this.tpWaterboxes.PerformLayout();
			this.tlpExWaterboxes.ResumeLayout(false);
			this.tlpExWaterboxes.PerformLayout();
			this.cmsMoveableObjectEdit.ResumeLayout(false);
			this.cmsSceneTree.ResumeLayout(false);
			this.cmsVertexEdit.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Controls.CustomGLControl customGLControl;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private Controls.ToolStripHintMenuItem fileToolStripMenuItem;
		private Controls.ToolStripHintMenuItem openROMToolStripMenuItem;
		private Controls.ToolStripHintMenuItem saveToolStripMenuItem;
		private Controls.ToolStripHintMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tpScenes;
		private System.Windows.Forms.TabPage tpRoomActors;
		private SceneNavi.Controls.TreeViewEx tvScenes;
		private Controls.TableLayoutPanelEx tlpExRoomActors;
		private System.Windows.Forms.ComboBox cbActors;
		private System.Windows.Forms.TabPage tpTransitions;
		private Controls.TableLayoutPanelEx tlpExTransitions;
		private System.Windows.Forms.ComboBox cbTransitions;
		private System.Windows.Forms.TabPage tpSpawnPoints;
		private Controls.TableLayoutPanelEx tlpExSpawnPoints;
		private System.Windows.Forms.ComboBox cbSpawnPoints;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.OpenFileDialog ofdOpenROM;
		private Controls.ToolStripHintMenuItem optionsToolStripMenuItem;
		private Controls.ToolStripHintMenuItem renderRoomActorsToolStripMenuItem;
		private Controls.ToolStripHintMenuItem renderSpawnPointsToolStripMenuItem;
		private Controls.ToolStripHintMenuItem renderTransitionsToolStripMenuItem;
		private Controls.ToolStripHintMenuItem helpToolStripMenuItem;
		private Controls.ToolStripHintMenuItem aboutToolStripMenuItem;
		private Controls.ToolStripHintMenuItem rOMInformationToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.TabPage tpObjects;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label lblSpecialObjs;
		private System.Windows.Forms.ComboBox cbSpecialObjs;
		private System.Windows.Forms.DataGridView dgvObjects;
		private Controls.ToolStripHintMenuItem editToolStripMenuItem;
		private Controls.ToolStripHintMenuItem editDataTablesToolStripMenuItem;
		private System.Windows.Forms.TabPage tpWaypoints;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.DataGridView dgvPathWaypoints;
		private System.Windows.Forms.ComboBox cbPathHeaders;
		private Controls.ToolStripHintMenuItem renderPathWaypointsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private Controls.ToolStripHintMenuItem linkAllWaypointsInPathToolStripMenuItem;
		private System.Windows.Forms.TabPage tpSceneMetadata;
		private Controls.ToolStripHintMenuItem enableTexturesToolStripMenuItem;
		private Controls.ToolStripHintMenuItem renderCollisionToolStripMenuItem;
		private System.Windows.Forms.TabPage tpCollision;
		private Controls.TableLayoutPanelEx tlpExCollision;
		private Controls.ToolStripHintMenuItem collisionHighlightToolStripMenuItem;
		private SceneNavi.Controls.ToolStripRadioButtonMenuItem whiteToolStripMenuItem;
		private SceneNavi.Controls.ToolStripRadioButtonMenuItem typebasedToolStripMenuItem;
		private System.Windows.Forms.ComboBox cbCollisionPolys;
		private System.Windows.Forms.Label lblColPolyType;
		private System.Windows.Forms.NumericUpDown nudColPolyType;
		private System.Windows.Forms.Label lblColPolyGroundType;
		private System.Windows.Forms.ComboBox cbColPolyGroundTypes;
		private System.Windows.Forms.ComboBox cbCollisionPolyTypes;
		private System.Windows.Forms.Label lblColPolyRawData;
		private System.Windows.Forms.TextBox txtColPolyRawData;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
		private Controls.ToolStripHintMenuItem enableVSyncToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip cmsMoveableObjectEdit;
		private Controls.ToolStripHintMenuItem deselectToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
		private Controls.ToolStripHintMenuItem rotateToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
		private Controls.ToolStripHintMenuItem xAxisToolStripMenuItem;
		private Controls.ToolStripHintMenuItem xPlus45DegreesToolStripMenuItem;
		private Controls.ToolStripHintMenuItem xMinus45DegreesToolStripMenuItem;
		private Controls.ToolStripHintMenuItem yAxisToolStripMenuItem;
		private Controls.ToolStripHintMenuItem yPlus45DegreesToolStripMenuItem;
		private Controls.ToolStripHintMenuItem yMinus45DegreesToolStripMenuItem;
		private Controls.ToolStripHintMenuItem zAxisToolStripMenuItem;
		private Controls.ToolStripHintMenuItem zPlus45DegreesToolStripMenuItem;
		private Controls.ToolStripHintMenuItem zMinus45DegreesToolStripMenuItem;
		private Controls.ToolStripHintMenuItem toolsToolStripMenuItem;
		private Controls.ToolStripHintMenuItem mouseModeToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
		private Controls.ToolStripHintMenuItem resetCameraPositionToolStripMenuItem;
		private System.Windows.Forms.Button btnJumpToPolyType;
		private System.Windows.Forms.Label lblCollisionPolys;
		private System.Windows.Forms.Label lblCollisionPolyTypes;
		private Controls.ToolStripHintMenuItem checkForUpdateToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
		private Controls.ToolStripHintMenuItem openSceneToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
		private Controls.ToolStripHintMenuItem closeSceneToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog ofdOpenScene;
		private System.Windows.Forms.OpenFileDialog ofdOpenRoom;
		private Controls.ToolStripHintMenuItem combinerTypeToolStripMenuItem;
		private Controls.ToolStripHintMenuItem renderElementsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem10;
		private Controls.TableLayoutPanelEx tlpExSceneMeta;
		private System.Windows.Forms.ComboBox cbSceneMetaBGM;
		private System.Windows.Forms.Label lblSceneMetaBGM;
		private System.Windows.Forms.Label lblSceneMetaReverb;
		private System.Windows.Forms.NumericUpDown nudSceneMetaReverb;
		private System.Windows.Forms.NumericUpDown nudSceneMetaNightSFX;
		private System.Windows.Forms.Label lblSceneMetaNightSFX;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ContextMenuStrip cmsSceneTree;
		private Controls.ToolStripHintMenuItem propertiesToolStripMenuItem;
		private Controls.ToolStripHintMenuItem editAreaTitleCardToolStripMenuItem;
		private Controls.ToolStripHintMenuItem renderWaterboxesToolStripMenuItem;
		private System.Windows.Forms.TabPage tpWaterboxes;
		private Controls.TableLayoutPanelEx tlpExWaterboxes;
		private System.Windows.Forms.ComboBox cbWaterboxes;
		private Controls.ToolStripHintMenuItem showWaterboxesPerRoomToolStripMenuItem;
		private System.Windows.Forms.Label lblWaterboxPositionX;
		private System.Windows.Forms.TextBox txtWaterboxPositionX;
		private System.Windows.Forms.Label lblWaterboxPositionY;
		private System.Windows.Forms.TextBox txtWaterboxPositionY;
		private System.Windows.Forms.Label lblWaterboxPositionZ;
		private System.Windows.Forms.TextBox txtWaterboxPositionZ;
		private System.Windows.Forms.Label lblWaterboxSizeX;
		private System.Windows.Forms.TextBox txtWaterboxSizeX;
		private System.Windows.Forms.Label lblWaterboxSizeZ;
		private System.Windows.Forms.TextBox txtWaterboxSizeZ;
		private System.Windows.Forms.Label lblWaterboxRoom;
		private System.Windows.Forms.ComboBox cbWaterboxRoom;
		private System.Windows.Forms.Label lblWaterboxProperties;
		private System.Windows.Forms.TextBox txtWaterboxProperties;
		private Controls.ToolStripHintMenuItem enableAntiAliasingToolStripMenuItem;
		private Controls.ToolStripHintMenuItem openGLToolStripMenuItem;
		private Controls.ToolStripHintMenuItem emulateDrawDistanceToolStripMenuItem;
		private GUIHelpers.ButtonStripItem bsiCamCoords;
		private GUIHelpers.ButtonStripItem bsiToolMode;
		private GUIHelpers.SeparatorStripItem separatorStripItem1;
		private GUIHelpers.SeparatorStripItem separatorStripItem2;
		private Controls.ToolStripHintMenuItem openGLInformationToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip cmsVertexEdit;
		private System.Windows.Forms.ToolStripMenuItem changeColorToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem11;
		private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem1;
		private Controls.ToolStripHintMenuItem emulateFogToolStripMenuItem;
	}
}
