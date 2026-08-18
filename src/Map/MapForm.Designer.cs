namespace GUITools
{
    partial class MapForm
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileImport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSep = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.lblHint = new System.Windows.Forms.ToolStripLabel();
            this.toolSep = new System.Windows.Forms.ToolStripSeparator();
            this.lblControls = new System.Windows.Forms.ToolStripLabel();
            this._status = new System.Windows.Forms.StatusStrip();
            this._statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.grpFileInfo = new System.Windows.Forms.GroupBox();
            this._tree = new System.Windows.Forms.TreeView();
            this.splitRight = new System.Windows.Forms.SplitContainer();
            this._grid = new System.Windows.Forms.PropertyGrid();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this._status.SuspendLayout();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.grpFileInfo.SuspendLayout();
            this.splitRight.Panel1.SuspendLayout();
            this.splitRight.Panel2.SuspendLayout();
            this.splitRight.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip
            //
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFile, this.menuView, this.menuSettings, this.menuHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip.TabIndex = 0;
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFileOpen, this.menuFileImport, this.menuFileSep, this.menuFileClose});
            this.menuFile.Name = "menuFile";
            this.menuFile.Text = "&File";
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.Text = "&Open .trv...";
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
            this.menuFileImport.Name = "menuFileImport";
            this.menuFileImport.Text = "&Import from map*.pkg...";
            this.menuFileImport.Click += new System.EventHandler(this.menuFileImport_Click);
            this.menuFileSep.Name = "menuFileSep";
            this.menuFileClose.Name = "menuFileClose";
            this.menuFileClose.Text = "&Close";
            this.menuFileClose.Click += new System.EventHandler(this.menuFileClose_Click);
            this.menuView.Name = "menuView";
            this.menuView.Text = "&View";
            this.menuSettings.Name = "menuSettings";
            this.menuSettings.Text = "&Settings";
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Text = "&Help";
            //
            // toolStrip
            //
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblHint, this.toolSep, this.lblControls});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1264, 25);
            this.toolStrip.TabIndex = 1;
            this.lblHint.Name = "lblHint";
            this.lblHint.Text = "Full map: baked overview + terrain + props";
            this.toolSep.Name = "toolSep";
            this.lblControls.Name = "lblControls";
            this.lblControls.Text = "Drag orbit · right-drag pan · wheel zoom";
            //
            // _status
            //
            this._status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._statusText });
            this._status.Location = new System.Drawing.Point(0, 739);
            this._status.Name = "_status";
            this._status.Size = new System.Drawing.Size(1264, 22);
            this._status.TabIndex = 2;
            this._statusText.Name = "_statusText";
            this._statusText.Text = "Ready";
            //
            // splitMain
            //
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 49);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Controls.Add(this.grpFileInfo);
            this.splitMain.Panel2.Controls.Add(this.splitRight);
            this.splitMain.Size = new System.Drawing.Size(1264, 690);
            this.splitMain.SplitterDistance = 240;
            this.splitMain.SplitterWidth = 4;
            this.splitMain.TabIndex = 3;
            //
            // grpFileInfo
            //
            this.grpFileInfo.Controls.Add(this._tree);
            this.grpFileInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpFileInfo.Location = new System.Drawing.Point(0, 0);
            this.grpFileInfo.Name = "grpFileInfo";
            this.grpFileInfo.Padding = new System.Windows.Forms.Padding(6);
            this.grpFileInfo.Size = new System.Drawing.Size(240, 690);
            this.grpFileInfo.TabIndex = 0;
            this.grpFileInfo.TabStop = false;
            this.grpFileInfo.Text = "File info.";
            this._tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tree.HideSelection = false;
            this._tree.Location = new System.Drawing.Point(6, 19);
            this._tree.Name = "_tree";
            this._tree.Size = new System.Drawing.Size(228, 665);
            this._tree.TabIndex = 0;
            //
            // splitRight
            //
            this.splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRight.Location = new System.Drawing.Point(0, 0);
            this.splitRight.Name = "splitRight";
            this.splitRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitRight.Panel2.Controls.Add(this._grid);
            this.splitRight.Size = new System.Drawing.Size(1020, 690);
            this.splitRight.SplitterDistance = 510;
            this.splitRight.TabIndex = 0;
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.HelpVisible = false;
            this._grid.Location = new System.Drawing.Point(0, 0);
            this._grid.Name = "_grid";
            this._grid.Size = new System.Drawing.Size(1020, 176);
            this._grid.TabIndex = 0;
            this._grid.ToolbarVisible = false;
            //
            // MapForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this._status);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MapForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GUITools Map";
            this.Shown += new System.EventHandler(this.MapForm_Shown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this._status.ResumeLayout(false);
            this._status.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.grpFileInfo.ResumeLayout(false);
            this.splitRight.Panel1.ResumeLayout(false);
            this.splitRight.Panel2.ResumeLayout(false);
            this.splitRight.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem menuFileImport;
        private System.Windows.Forms.ToolStripSeparator menuFileSep;
        private System.Windows.Forms.ToolStripMenuItem menuFileClose;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuSettings;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel lblHint;
        private System.Windows.Forms.ToolStripSeparator toolSep;
        private System.Windows.Forms.ToolStripLabel lblControls;
        private System.Windows.Forms.StatusStrip _status;
        private System.Windows.Forms.ToolStripStatusLabel _statusText;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.GroupBox grpFileInfo;
        private System.Windows.Forms.TreeView _tree;
        private System.Windows.Forms.SplitContainer splitRight;
        private System.Windows.Forms.PropertyGrid _grid;
    }
}
