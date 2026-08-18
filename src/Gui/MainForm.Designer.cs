namespace GUITools
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpenMap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditHideSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditShowSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditDeleteSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuEditKeepThis = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditHideStack = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditShowStack = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditDeleteStack = new System.Windows.Forms.ToolStripMenuItem();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewHideClosed = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAutomation = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAutomationStartup = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSettingsGameFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSettingsRebuildIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSettingsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.lblCanvas = new System.Windows.Forms.ToolStripLabel();
            this._btnPreview = new System.Windows.Forms.ToolStripButton();
            this._btnEdit = new System.Windows.Forms.ToolStripButton();
            this.toolSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.lblModeHint = new System.Windows.Forms.ToolStripLabel();
            this.toolSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnHideSelected = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteSelected = new System.Windows.Forms.ToolStripButton();
            this.toolSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnKeepThis = new System.Windows.Forms.ToolStripButton();
            this.btnHideStack = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteStack = new System.Windows.Forms.ToolStripButton();
            this._status = new System.Windows.Forms.StatusStrip();
            this._statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitOuter = new System.Windows.Forms.SplitContainer();
            this.splitInner = new System.Windows.Forms.SplitContainer();
            this.grpFileInfo = new System.Windows.Forms.GroupBox();
            this._tree = new System.Windows.Forms.TreeView();
            this.treeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxHideSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxShowSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxDeleteSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxKeepThis = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxHideStack = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxShowStack = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxDeleteThis = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxDeleteStack = new System.Windows.Forms.ToolStripMenuItem();
            this.splitRight = new System.Windows.Forms.SplitContainer();
            this.canvasHost = new System.Windows.Forms.Panel();
            this.grpToolbox = new System.Windows.Forms.GroupBox();
            this._toolbox = new System.Windows.Forms.Panel();
            this._props = new System.Windows.Forms.Panel();
            this.btnTemplate = new System.Windows.Forms.Button();
            this.lblVisibility = new System.Windows.Forms.Label();
            this._visHidden = new System.Windows.Forms.RadioButton();
            this._visVisible = new System.Windows.Forms.RadioButton();
            this._enEnabled = new System.Windows.Forms.RadioButton();
            this._enDisabled = new System.Windows.Forms.RadioButton();
            this.lblDesc = new System.Windows.Forms.Label();
            this._txtDesc = new System.Windows.Forms.TextBox();
            this.lblCmd = new System.Windows.Forms.Label();
            this._txtCmd = new System.Windows.Forms.TextBox();
            this.lblText = new System.Windows.Forms.Label();
            this._txtText = new System.Windows.Forms.TextBox();
            this.lblAspect = new System.Windows.Forms.Label();
            this.radioAspectNone = new System.Windows.Forms.RadioButton();
            this.radioAspectPos = new System.Windows.Forms.RadioButton();
            this.radioAspectSize = new System.Windows.Forms.RadioButton();
            this.radioAspectSizeCenter = new System.Windows.Forms.RadioButton();
            this.radioAspectPosCenter = new System.Windows.Forms.RadioButton();
            this.radioAspectAll = new System.Windows.Forms.RadioButton();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this._status.SuspendLayout();
            this.splitOuter.Panel1.SuspendLayout();
            this.splitOuter.Panel2.SuspendLayout();
            this.splitOuter.SuspendLayout();
            this.splitInner.Panel1.SuspendLayout();
            this.splitInner.Panel2.SuspendLayout();
            this.splitInner.SuspendLayout();
            this.grpFileInfo.SuspendLayout();
            this.treeMenu.SuspendLayout();
            this.splitRight.Panel1.SuspendLayout();
            this.splitRight.Panel2.SuspendLayout();
            this.splitRight.SuspendLayout();
            this.canvasHost.SuspendLayout();
            this.grpToolbox.SuspendLayout();
            this._props.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip
            //
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFile, this.menuEdit, this.menuView, this.menuAutomation, this.menuSettings, this.menuHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1384, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            //
            // menuFile
            //
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFileNew, this.menuFileOpen, this.menuFileSave, this.menuFileSaveAs, this.menuFileSep1,
                this.menuFileExport, this.menuFileOpenMap, this.menuFileSep2, this.menuFileExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Text = "&File";
            //
            this.menuFileNew.Name = "menuFileNew";
            this.menuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuFileNew.Text = "&New";
            this.menuFileNew.Click += new System.EventHandler(this.menuFileNew_Click);
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuFileOpen.Text = "&Open...";
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
            this.menuFileSave.Name = "menuFileSave";
            this.menuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuFileSave.Text = "&Save";
            this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
            this.menuFileSaveAs.Name = "menuFileSaveAs";
            this.menuFileSaveAs.Text = "Save &As...";
            this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
            this.menuFileSep1.Name = "menuFileSep1";
            this.menuFileExport.Name = "menuFileExport";
            this.menuFileExport.Text = "&Export .gui...";
            this.menuFileExport.Click += new System.EventHandler(this.menuFileExport_Click);
            this.menuFileOpenMap.Name = "menuFileOpenMap";
            this.menuFileOpenMap.Text = "Open map (browse .pkg / .trv)...";
            this.menuFileOpenMap.Click += new System.EventHandler(this.menuFileOpenMap_Click);
            this.menuFileSep2.Name = "menuFileSep2";
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Text = "E&xit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            //
            // menuEdit
            //
            this.menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuEditDelete, this.menuEditHideSelected, this.menuEditShowSelected, this.menuEditDeleteSelected,
                this.menuEditSep1, this.menuEditKeepThis, this.menuEditHideStack, this.menuEditShowStack, this.menuEditDeleteStack});
            this.menuEdit.Name = "menuEdit";
            this.menuEdit.Text = "&Edit";
            this.menuEditDelete.Name = "menuEditDelete";
            this.menuEditDelete.Text = "&Delete";
            this.menuEditDelete.Click += new System.EventHandler(this.menuEditDelete_Click);
            this.menuEditHideSelected.Name = "menuEditHideSelected";
            this.menuEditHideSelected.Text = "&Hide selected";
            this.menuEditHideSelected.Click += new System.EventHandler(this.menuEditHideSelected_Click);
            this.menuEditShowSelected.Name = "menuEditShowSelected";
            this.menuEditShowSelected.Text = "&Show selected";
            this.menuEditShowSelected.Click += new System.EventHandler(this.menuEditShowSelected_Click);
            this.menuEditDeleteSelected.Name = "menuEditDeleteSelected";
            this.menuEditDeleteSelected.Text = "Delete selected";
            this.menuEditDeleteSelected.Click += new System.EventHandler(this.menuEditDeleteSelected_Click);
            this.menuEditSep1.Name = "menuEditSep1";
            this.menuEditKeepThis.Name = "menuEditKeepThis";
            this.menuEditKeepThis.Text = "Keep this (hide rest of stack)";
            this.menuEditKeepThis.Click += new System.EventHandler(this.menuEditKeepThis_Click);
            this.menuEditHideStack.Name = "menuEditHideStack";
            this.menuEditHideStack.Text = "Hide entire stack";
            this.menuEditHideStack.Click += new System.EventHandler(this.menuEditHideStack_Click);
            this.menuEditShowStack.Name = "menuEditShowStack";
            this.menuEditShowStack.Text = "Show entire stack";
            this.menuEditShowStack.Click += new System.EventHandler(this.menuEditShowStack_Click);
            this.menuEditDeleteStack.Name = "menuEditDeleteStack";
            this.menuEditDeleteStack.Text = "Delete entire stack";
            this.menuEditDeleteStack.Click += new System.EventHandler(this.menuEditDeleteStack_Click);
            //
            // menuView
            //
            this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuViewRefresh, this.menuViewHideClosed});
            this.menuView.Name = "menuView";
            this.menuView.Text = "&View";
            this.menuViewRefresh.Name = "menuViewRefresh";
            this.menuViewRefresh.Text = "&Refresh";
            this.menuViewRefresh.Click += new System.EventHandler(this.menuViewRefresh_Click);
            this.menuViewHideClosed.Checked = true;
            this.menuViewHideClosed.CheckOnClick = true;
            this.menuViewHideClosed.Name = "menuViewHideClosed";
            this.menuViewHideClosed.Text = "Hide (Closed) controls";
            this.menuViewHideClosed.Click += new System.EventHandler(this.menuViewHideClosed_Click);
            //
            // menuAutomation
            //
            this.menuAutomation.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuAutomationStartup });
            this.menuAutomation.Name = "menuAutomation";
            this.menuAutomation.Text = "&Automation";
            this.menuAutomationStartup.Name = "menuAutomationStartup";
            this.menuAutomationStartup.Text = "Run startup.py (info)";
            this.menuAutomationStartup.Click += new System.EventHandler(this.menuAutomationStartup_Click);
            //
            // menuSettings
            //
            this.menuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuSettingsGameFolder, this.menuSettingsRebuildIndex, this.menuSettingsOptions});
            this.menuSettings.Name = "menuSettings";
            this.menuSettings.Text = "&Settings";
            this.menuSettingsGameFolder.Name = "menuSettingsGameFolder";
            this.menuSettingsGameFolder.Text = "Game data folder...";
            this.menuSettingsGameFolder.Click += new System.EventHandler(this.menuSettingsGameFolder_Click);
            this.menuSettingsRebuildIndex.Name = "menuSettingsRebuildIndex";
            this.menuSettingsRebuildIndex.Text = "Rebuild package index...";
            this.menuSettingsRebuildIndex.Click += new System.EventHandler(this.menuSettingsRebuildIndex_Click);
            this.menuSettingsOptions.Name = "menuSettingsOptions";
            this.menuSettingsOptions.Text = "Options...";
            this.menuSettingsOptions.Click += new System.EventHandler(this.menuSettingsOptions_Click);
            //
            // menuHelp
            //
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuHelpAbout });
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Text = "&Help";
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Text = "&About";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            //
            // toolStrip
            //
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblCanvas, this._btnPreview, this._btnEdit, this.toolSep1, this.lblModeHint, this.toolSep2,
                this.btnHideSelected, this.btnDeleteSelected, this.toolSep3, this.btnKeepThis, this.btnHideStack, this.btnDeleteStack});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1384, 25);
            this.toolStrip.TabIndex = 1;
            this.lblCanvas.Name = "lblCanvas";
            this.lblCanvas.Text = "Canvas:";
            this._btnPreview.Checked = true;
            this._btnPreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this._btnPreview.Name = "_btnPreview";
            this._btnPreview.Text = "Preview";
            this._btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            this._btnEdit.Name = "_btnEdit";
            this._btnEdit.Text = "Edit";
            this._btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            this.toolSep1.Name = "toolSep1";
            this.lblModeHint.Name = "lblModeHint";
            this.lblModeHint.Text = "Ctrl+click buttons to pick  ·  Hide / Delete selected";
            this.toolSep2.Name = "toolSep2";
            this.btnHideSelected.Name = "btnHideSelected";
            this.btnHideSelected.Text = "Hide selected";
            this.btnHideSelected.Click += new System.EventHandler(this.menuEditHideSelected_Click);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Text = "Delete selected";
            this.btnDeleteSelected.Click += new System.EventHandler(this.menuEditDeleteSelected_Click);
            this.toolSep3.Name = "toolSep3";
            this.btnKeepThis.Name = "btnKeepThis";
            this.btnKeepThis.Text = "Keep this";
            this.btnKeepThis.ToolTipText = "Hide the other copies stacked on the same slot";
            this.btnKeepThis.Click += new System.EventHandler(this.menuEditKeepThis_Click);
            this.btnHideStack.Name = "btnHideStack";
            this.btnHideStack.Text = "Hide stack";
            this.btnHideStack.Click += new System.EventHandler(this.menuEditHideStack_Click);
            this.btnDeleteStack.Name = "btnDeleteStack";
            this.btnDeleteStack.Text = "Delete stack";
            this.btnDeleteStack.Click += new System.EventHandler(this.menuEditDeleteStack_Click);
            //
            // _status
            //
            this._status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._statusText });
            this._status.Location = new System.Drawing.Point(0, 799);
            this._status.Name = "_status";
            this._status.Size = new System.Drawing.Size(1384, 22);
            this._status.TabIndex = 2;
            this._statusText.Name = "_statusText";
            this._statusText.Text = "Ready";
            //
            // splitOuter
            //
            this.splitOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitOuter.Location = new System.Drawing.Point(0, 49);
            this.splitOuter.Name = "splitOuter";
            this.splitOuter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitOuter.Panel1.Controls.Add(this.splitInner);
            this.splitOuter.Panel2.Controls.Add(this._props);
            this.splitOuter.Size = new System.Drawing.Size(1384, 750);
            this.splitOuter.SplitterDistance = 590;
            this.splitOuter.TabIndex = 3;
            //
            // splitInner
            //
            this.splitInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitInner.Location = new System.Drawing.Point(0, 0);
            this.splitInner.Name = "splitInner";
            this.splitInner.Panel1.Controls.Add(this.grpFileInfo);
            this.splitInner.Panel2.Controls.Add(this.splitRight);
            this.splitInner.Size = new System.Drawing.Size(1384, 590);
            this.splitInner.SplitterDistance = 240;
            this.splitInner.SplitterWidth = 4;
            this.splitInner.TabIndex = 0;
            //
            // grpFileInfo
            //
            this.grpFileInfo.Controls.Add(this._tree);
            this.grpFileInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpFileInfo.Location = new System.Drawing.Point(0, 0);
            this.grpFileInfo.Name = "grpFileInfo";
            this.grpFileInfo.Padding = new System.Windows.Forms.Padding(6);
            this.grpFileInfo.Size = new System.Drawing.Size(240, 590);
            this.grpFileInfo.TabIndex = 0;
            this.grpFileInfo.TabStop = false;
            this.grpFileInfo.Text = "File info.";
            //
            // _tree
            //
            this._tree.ContextMenuStrip = this.treeMenu;
            this._tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this._tree.FullRowSelect = true;
            this._tree.HideSelection = false;
            this._tree.Location = new System.Drawing.Point(6, 19);
            this._tree.Name = "_tree";
            this._tree.Size = new System.Drawing.Size(228, 565);
            this._tree.TabIndex = 0;
            this._tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
            this._tree.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.TreeDrawNode);
            this._tree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeKeyDown);
            this._tree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeMouseDown);
            //
            // treeMenu
            //
            this.treeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.ctxHideSelected, this.ctxShowSelected, this.ctxDeleteSelected, this.ctxSep1,
                this.ctxKeepThis, this.ctxHideStack, this.ctxShowStack, this.ctxSep2, this.ctxDeleteThis, this.ctxDeleteStack});
            this.treeMenu.Name = "treeMenu";
            this.treeMenu.Size = new System.Drawing.Size(220, 192);
            this.ctxHideSelected.Name = "ctxHideSelected";
            this.ctxHideSelected.Text = "Hide selected";
            this.ctxHideSelected.Click += new System.EventHandler(this.menuEditHideSelected_Click);
            this.ctxShowSelected.Name = "ctxShowSelected";
            this.ctxShowSelected.Text = "Show selected";
            this.ctxShowSelected.Click += new System.EventHandler(this.menuEditShowSelected_Click);
            this.ctxDeleteSelected.Name = "ctxDeleteSelected";
            this.ctxDeleteSelected.Text = "Delete selected";
            this.ctxDeleteSelected.Click += new System.EventHandler(this.menuEditDeleteSelected_Click);
            this.ctxSep1.Name = "ctxSep1";
            this.ctxKeepThis.Name = "ctxKeepThis";
            this.ctxKeepThis.Text = "Keep this (hide other copies)";
            this.ctxKeepThis.Click += new System.EventHandler(this.menuEditKeepThis_Click);
            this.ctxHideStack.Name = "ctxHideStack";
            this.ctxHideStack.Text = "Hide entire stack";
            this.ctxHideStack.Click += new System.EventHandler(this.menuEditHideStack_Click);
            this.ctxShowStack.Name = "ctxShowStack";
            this.ctxShowStack.Text = "Show entire stack";
            this.ctxShowStack.Click += new System.EventHandler(this.menuEditShowStack_Click);
            this.ctxSep2.Name = "ctxSep2";
            this.ctxDeleteThis.Name = "ctxDeleteThis";
            this.ctxDeleteThis.Text = "Delete this";
            this.ctxDeleteThis.Click += new System.EventHandler(this.menuEditDelete_Click);
            this.ctxDeleteStack.Name = "ctxDeleteStack";
            this.ctxDeleteStack.Text = "Delete entire stack";
            this.ctxDeleteStack.Click += new System.EventHandler(this.menuEditDeleteStack_Click);
            //
            // splitRight
            //
            this.splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRight.Location = new System.Drawing.Point(0, 0);
            this.splitRight.Name = "splitRight";
            this.splitRight.Panel1.Controls.Add(this.canvasHost);
            this.splitRight.Panel2.Controls.Add(this.grpToolbox);
            this.splitRight.Size = new System.Drawing.Size(1140, 590);
            this.splitRight.SplitterDistance = 960;
            this.splitRight.SplitterWidth = 4;
            this.splitRight.TabIndex = 0;
            //
            // canvasHost
            //
            this.canvasHost.AutoScroll = true;
            this.canvasHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(96)))), ((int)(((byte)(96)))));
            this.canvasHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasHost.Location = new System.Drawing.Point(0, 0);
            this.canvasHost.Name = "canvasHost";
            this.canvasHost.Size = new System.Drawing.Size(960, 590);
            this.canvasHost.TabIndex = 0;
            //
            // grpToolbox
            //
            this.grpToolbox.Controls.Add(this._toolbox);
            this.grpToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpToolbox.Location = new System.Drawing.Point(0, 0);
            this.grpToolbox.Name = "grpToolbox";
            this.grpToolbox.Size = new System.Drawing.Size(176, 590);
            this.grpToolbox.TabIndex = 0;
            this.grpToolbox.TabStop = false;
            this.grpToolbox.Text = "Controls";
            this._toolbox.AutoScroll = true;
            this._toolbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._toolbox.Location = new System.Drawing.Point(3, 16);
            this._toolbox.Name = "_toolbox";
            this._toolbox.Size = new System.Drawing.Size(170, 571);
            this._toolbox.TabIndex = 0;
            //
            // _props
            //
            this._props.Controls.Add(this.btnTemplate);
            this._props.Controls.Add(this.lblVisibility);
            this._props.Controls.Add(this._visHidden);
            this._props.Controls.Add(this._visVisible);
            this._props.Controls.Add(this._enEnabled);
            this._props.Controls.Add(this._enDisabled);
            this._props.Controls.Add(this.lblDesc);
            this._props.Controls.Add(this._txtDesc);
            this._props.Controls.Add(this.lblCmd);
            this._props.Controls.Add(this._txtCmd);
            this._props.Controls.Add(this.lblText);
            this._props.Controls.Add(this._txtText);
            this._props.Controls.Add(this.lblAspect);
            this._props.Controls.Add(this.radioAspectNone);
            this._props.Controls.Add(this.radioAspectPos);
            this._props.Controls.Add(this.radioAspectSize);
            this._props.Controls.Add(this.radioAspectSizeCenter);
            this._props.Controls.Add(this.radioAspectPosCenter);
            this._props.Controls.Add(this.radioAspectAll);
            this._props.Dock = System.Windows.Forms.DockStyle.Fill;
            this._props.Location = new System.Drawing.Point(0, 0);
            this._props.Name = "_props";
            this._props.Padding = new System.Windows.Forms.Padding(8);
            this._props.Size = new System.Drawing.Size(1384, 156);
            this._props.TabIndex = 0;
            this.btnTemplate.Location = new System.Drawing.Point(8, 8);
            this.btnTemplate.Name = "btnTemplate";
            this.btnTemplate.Size = new System.Drawing.Size(130, 23);
            this.btnTemplate.TabIndex = 0;
            this.btnTemplate.Text = "Select template";
            this.btnTemplate.UseVisualStyleBackColor = true;
            this.btnTemplate.Click += new System.EventHandler(this.btnTemplate_Click);
            this.lblVisibility.AutoSize = true;
            this.lblVisibility.Location = new System.Drawing.Point(8, 43);
            this.lblVisibility.Name = "lblVisibility";
            this.lblVisibility.Size = new System.Drawing.Size(47, 13);
            this.lblVisibility.Text = "Visibility";
            this._visHidden.AutoSize = true;
            this._visHidden.AutoCheck = false;
            this._visHidden.Location = new System.Drawing.Point(80, 41);
            this._visHidden.Name = "_visHidden";
            this._visHidden.Size = new System.Drawing.Size(60, 17);
            this._visHidden.TabIndex = 1;
            this._visHidden.Text = "Hidden";
            this._visHidden.UseVisualStyleBackColor = true;
            this._visHidden.CheckedChanged += new System.EventHandler(this.prop_Changed);
            this._visVisible.AutoSize = true;
            this._visVisible.AutoCheck = false;
            this._visVisible.Location = new System.Drawing.Point(160, 41);
            this._visVisible.Name = "_visVisible";
            this._visVisible.Size = new System.Drawing.Size(56, 17);
            this._visVisible.TabIndex = 2;
            this._visVisible.TabStop = true;
            this._visVisible.Text = "Visible";
            this._visVisible.UseVisualStyleBackColor = true;
            this._visVisible.CheckedChanged += new System.EventHandler(this.prop_Changed);
            this._enEnabled.AutoSize = true;
            this._enEnabled.AutoCheck = false;
            this._enEnabled.Location = new System.Drawing.Point(280, 41);
            this._enEnabled.Name = "_enEnabled";
            this._enEnabled.Size = new System.Drawing.Size(64, 17);
            this._enEnabled.TabIndex = 3;
            this._enEnabled.TabStop = true;
            this._enEnabled.Text = "Enabled";
            this._enEnabled.UseVisualStyleBackColor = true;
            this._enEnabled.CheckedChanged += new System.EventHandler(this.prop_Changed);
            this._enDisabled.AutoSize = true;
            this._enDisabled.AutoCheck = false;
            this._enDisabled.Location = new System.Drawing.Point(360, 41);
            this._enDisabled.Name = "_enDisabled";
            this._enDisabled.Size = new System.Drawing.Size(66, 17);
            this._enDisabled.TabIndex = 4;
            this._enDisabled.Text = "Disabled";
            this._enDisabled.UseVisualStyleBackColor = true;
            this._enDisabled.CheckedChanged += new System.EventHandler(this.prop_Changed);
            this.lblDesc.AutoSize = true;
            this.lblDesc.Location = new System.Drawing.Point(8, 71);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(102, 13);
            this.lblDesc.Text = "Control description";
            this._txtDesc.Location = new System.Drawing.Point(130, 68);
            this._txtDesc.Name = "_txtDesc";
            this._txtDesc.Size = new System.Drawing.Size(360, 20);
            this._txtDesc.TabIndex = 5;
            this._txtDesc.Leave += new System.EventHandler(this.prop_Changed);
            this.lblCmd.AutoSize = true;
            this.lblCmd.Location = new System.Drawing.Point(510, 71);
            this.lblCmd.Name = "lblCmd";
            this.lblCmd.Size = new System.Drawing.Size(54, 13);
            this.lblCmd.Text = "Command";
            this._txtCmd.Location = new System.Drawing.Point(580, 68);
            this._txtCmd.Name = "_txtCmd";
            this._txtCmd.Size = new System.Drawing.Size(220, 20);
            this._txtCmd.TabIndex = 6;
            this._txtCmd.Leave += new System.EventHandler(this.prop_Changed);
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(820, 71);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(28, 13);
            this.lblText.Text = "Text";
            this._txtText.Location = new System.Drawing.Point(860, 68);
            this._txtText.Name = "_txtText";
            this._txtText.Size = new System.Drawing.Size(240, 20);
            this._txtText.TabIndex = 7;
            this._txtText.Leave += new System.EventHandler(this.prop_Changed);
            this.lblAspect.AutoSize = true;
            this.lblAspect.Location = new System.Drawing.Point(8, 99);
            this.lblAspect.Name = "lblAspect";
            this.lblAspect.Size = new System.Drawing.Size(97, 13);
            this.lblAspect.Text = "Auto Aspect Ratio";
            this.radioAspectNone.AutoSize = true;
            this.radioAspectNone.AutoCheck = false;
            this.radioAspectNone.Location = new System.Drawing.Point(130, 97);
            this.radioAspectNone.Name = "radioAspectNone";
            this.radioAspectNone.Size = new System.Drawing.Size(49, 17);
            this.radioAspectNone.TabIndex = 8;
            this.radioAspectNone.TabStop = true;
            this.radioAspectNone.Text = "none";
            this.radioAspectNone.UseVisualStyleBackColor = true;
            this.radioAspectNone.CheckedChanged += new System.EventHandler(this.prop_Changed);
            this.radioAspectPos.AutoSize = true;
            this.radioAspectPos.AutoCheck = false;
            this.radioAspectPos.Location = new System.Drawing.Point(190, 97);
            this.radioAspectPos.Name = "radioAspectPos";
            this.radioAspectPos.Size = new System.Drawing.Size(41, 17);
            this.radioAspectPos.TabIndex = 9;
            this.radioAspectPos.Text = "Pos";
            this.radioAspectPos.UseVisualStyleBackColor = true;
            this.radioAspectPos.CheckedChanged += new System.EventHandler(this.prop_Changed);
            this.radioAspectSize.AutoSize = true;
            this.radioAspectSize.AutoCheck = false;
            this.radioAspectSize.Location = new System.Drawing.Point(240, 97);
            this.radioAspectSize.Name = "radioAspectSize";
            this.radioAspectSize.Size = new System.Drawing.Size(45, 17);
            this.radioAspectSize.TabIndex = 10;
            this.radioAspectSize.Text = "Size";
            this.radioAspectSize.UseVisualStyleBackColor = true;
            this.radioAspectSize.CheckedChanged += new System.EventHandler(this.prop_Changed);
            this.radioAspectSizeCenter.AutoSize = true;
            this.radioAspectSizeCenter.AutoCheck = false;
            this.radioAspectSizeCenter.Location = new System.Drawing.Point(300, 97);
            this.radioAspectSizeCenter.Name = "radioAspectSizeCenter";
            this.radioAspectSizeCenter.Size = new System.Drawing.Size(82, 17);
            this.radioAspectSizeCenter.TabIndex = 11;
            this.radioAspectSizeCenter.Text = "Size.Center";
            this.radioAspectSizeCenter.UseVisualStyleBackColor = true;
            this.radioAspectSizeCenter.CheckedChanged += new System.EventHandler(this.prop_Changed);
            this.radioAspectPosCenter.AutoSize = true;
            this.radioAspectPosCenter.AutoCheck = false;
            this.radioAspectPosCenter.Location = new System.Drawing.Point(400, 97);
            this.radioAspectPosCenter.Name = "radioAspectPosCenter";
            this.radioAspectPosCenter.Size = new System.Drawing.Size(78, 17);
            this.radioAspectPosCenter.TabIndex = 12;
            this.radioAspectPosCenter.Text = "Pos.Center";
            this.radioAspectPosCenter.UseVisualStyleBackColor = true;
            this.radioAspectPosCenter.CheckedChanged += new System.EventHandler(this.prop_Changed);
            this.radioAspectAll.AutoSize = true;
            this.radioAspectAll.AutoCheck = false;
            this.radioAspectAll.Location = new System.Drawing.Point(490, 97);
            this.radioAspectAll.Name = "radioAspectAll";
            this.radioAspectAll.Size = new System.Drawing.Size(36, 17);
            this.radioAspectAll.TabIndex = 13;
            this.radioAspectAll.Text = "All";
            this.radioAspectAll.UseVisualStyleBackColor = true;
            this.radioAspectAll.CheckedChanged += new System.EventHandler(this.prop_Changed);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 821);
            this.Controls.Add(this.splitOuter);
            this.Controls.Add(this._status);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GUITools";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this._status.ResumeLayout(false);
            this._status.PerformLayout();
            this.splitOuter.Panel1.ResumeLayout(false);
            this.splitOuter.Panel2.ResumeLayout(false);
            this.splitOuter.ResumeLayout(false);
            this.splitInner.Panel1.ResumeLayout(false);
            this.splitInner.Panel2.ResumeLayout(false);
            this.splitInner.ResumeLayout(false);
            this.grpFileInfo.ResumeLayout(false);
            this.treeMenu.ResumeLayout(false);
            this.splitRight.Panel1.ResumeLayout(false);
            this.splitRight.Panel2.ResumeLayout(false);
            this.splitRight.ResumeLayout(false);
            this.canvasHost.ResumeLayout(false);
            this.grpToolbox.ResumeLayout(false);
            this._props.ResumeLayout(false);
            this._props.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileNew;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem menuFileSave;
        private System.Windows.Forms.ToolStripMenuItem menuFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator menuFileSep1;
        private System.Windows.Forms.ToolStripMenuItem menuFileExport;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpenMap;
        private System.Windows.Forms.ToolStripSeparator menuFileSep2;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuEdit;
        private System.Windows.Forms.ToolStripMenuItem menuEditDelete;
        private System.Windows.Forms.ToolStripMenuItem menuEditHideSelected;
        private System.Windows.Forms.ToolStripMenuItem menuEditShowSelected;
        private System.Windows.Forms.ToolStripMenuItem menuEditDeleteSelected;
        private System.Windows.Forms.ToolStripSeparator menuEditSep1;
        private System.Windows.Forms.ToolStripMenuItem menuEditKeepThis;
        private System.Windows.Forms.ToolStripMenuItem menuEditHideStack;
        private System.Windows.Forms.ToolStripMenuItem menuEditShowStack;
        private System.Windows.Forms.ToolStripMenuItem menuEditDeleteStack;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuViewRefresh;
        private System.Windows.Forms.ToolStripMenuItem menuViewHideClosed;
        private System.Windows.Forms.ToolStripMenuItem menuAutomation;
        private System.Windows.Forms.ToolStripMenuItem menuAutomationStartup;
        private System.Windows.Forms.ToolStripMenuItem menuSettings;
        private System.Windows.Forms.ToolStripMenuItem menuSettingsGameFolder;
        private System.Windows.Forms.ToolStripMenuItem menuSettingsRebuildIndex;
        private System.Windows.Forms.ToolStripMenuItem menuSettingsOptions;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel lblCanvas;
        private System.Windows.Forms.ToolStripButton _btnPreview;
        private System.Windows.Forms.ToolStripButton _btnEdit;
        private System.Windows.Forms.ToolStripSeparator toolSep1;
        private System.Windows.Forms.ToolStripLabel lblModeHint;
        private System.Windows.Forms.ToolStripSeparator toolSep2;
        private System.Windows.Forms.ToolStripButton btnHideSelected;
        private System.Windows.Forms.ToolStripButton btnDeleteSelected;
        private System.Windows.Forms.ToolStripSeparator toolSep3;
        private System.Windows.Forms.ToolStripButton btnKeepThis;
        private System.Windows.Forms.ToolStripButton btnHideStack;
        private System.Windows.Forms.ToolStripButton btnDeleteStack;
        private System.Windows.Forms.StatusStrip _status;
        private System.Windows.Forms.ToolStripStatusLabel _statusText;
        private System.Windows.Forms.SplitContainer splitOuter;
        private System.Windows.Forms.SplitContainer splitInner;
        private System.Windows.Forms.GroupBox grpFileInfo;
        private System.Windows.Forms.TreeView _tree;
        private System.Windows.Forms.ContextMenuStrip treeMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxHideSelected;
        private System.Windows.Forms.ToolStripMenuItem ctxShowSelected;
        private System.Windows.Forms.ToolStripMenuItem ctxDeleteSelected;
        private System.Windows.Forms.ToolStripSeparator ctxSep1;
        private System.Windows.Forms.ToolStripMenuItem ctxKeepThis;
        private System.Windows.Forms.ToolStripMenuItem ctxHideStack;
        private System.Windows.Forms.ToolStripMenuItem ctxShowStack;
        private System.Windows.Forms.ToolStripSeparator ctxSep2;
        private System.Windows.Forms.ToolStripMenuItem ctxDeleteThis;
        private System.Windows.Forms.ToolStripMenuItem ctxDeleteStack;
        private System.Windows.Forms.SplitContainer splitRight;
        private System.Windows.Forms.Panel canvasHost;
        private System.Windows.Forms.GroupBox grpToolbox;
        private System.Windows.Forms.Panel _toolbox;
        private System.Windows.Forms.Panel _props;
        private System.Windows.Forms.Button btnTemplate;
        private System.Windows.Forms.Label lblVisibility;
        private System.Windows.Forms.RadioButton _visHidden;
        private System.Windows.Forms.RadioButton _visVisible;
        private System.Windows.Forms.RadioButton _enEnabled;
        private System.Windows.Forms.RadioButton _enDisabled;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.TextBox _txtDesc;
        private System.Windows.Forms.Label lblCmd;
        private System.Windows.Forms.TextBox _txtCmd;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.TextBox _txtText;
        private System.Windows.Forms.Label lblAspect;
        private System.Windows.Forms.RadioButton radioAspectNone;
        private System.Windows.Forms.RadioButton radioAspectPos;
        private System.Windows.Forms.RadioButton radioAspectSize;
        private System.Windows.Forms.RadioButton radioAspectSizeCenter;
        private System.Windows.Forms.RadioButton radioAspectPosCenter;
        private System.Windows.Forms.RadioButton radioAspectAll;
    }
}
