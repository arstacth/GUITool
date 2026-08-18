using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GUITools
{
    public partial class MainForm : Form
    {
        GuiDocument _doc;
        string _path;
        DesignCanvas _canvas;
        readonly Dictionary<string, RadioButton> _aspect = new Dictionary<string, RadioButton>();
        bool _uiLock;
        readonly Dictionary<TreeNode, GuiControl> _nodeMap = new Dictionary<TreeNode, GuiControl>();
        readonly Dictionary<TreeNode, List<GuiControl>> _stackMap = new Dictionary<TreeNode, List<GuiControl>>();
        readonly HashSet<TreeNode> _picked = new HashSet<TreeNode>();
        TreeNode _anchor;

        public MainForm()
        {
            InitializeComponent();
            _canvas = new DesignCanvas();
            _canvas.Dock = DockStyle.Fill;
            _canvas.PreviewMode = true;
            _canvas.SelectionChanged += canvas_SelectionChanged;
            _canvas.DocumentEdited += canvas_DocumentEdited;
            canvasHost.Controls.Add(_canvas);
            FillToolbox();
            _aspect["none"] = radioAspectNone;
            _aspect["Pos"] = radioAspectPos;
            _aspect["Size"] = radioAspectSize;
            _aspect["Size.Center"] = radioAspectSizeCenter;
            _aspect["Pos.Center"] = radioAspectPosCenter;
            _aspect["All"] = radioAspectAll;
            _canvas.HidePickedRequested += (s, e) => HidePicked(true);
            _canvas.DeletePickedRequested += (s, e) => DeletePicked();
            var canvasMenu = new ContextMenuStrip();
            canvasMenu.Items.Add("Hide selected", null, (s, e) => HidePicked(true));
            canvasMenu.Items.Add("Show selected", null, (s, e) => HidePicked(false));
            canvasMenu.Items.Add("Delete selected", null, (s, e) => DeletePicked());
            _canvas.ContextMenuStrip = canvasMenu;
            _uiLock = true;
            _visVisible.Checked = true;
            _enEnabled.Checked = true;
            radioAspectNone.Checked = true;
            _uiLock = false;
            try
            {
                GameData.EnsureInitialized();
                _statusText.Text = GameRuntime.Boot();
            }
            catch (Exception ex)
            {
                _statusText.Text = "Ready — choose a .pkg folder when you need game files";
                try
                {
                    File.WriteAllText(Path.Combine(GameData.CacheRoot ?? ".", "boot_error.txt"), ex.ToString());
                }
                catch { }
            }
            var start = GameData.DefaultGuiPath();
            if (!string.IsNullOrEmpty(start) && File.Exists(start))
            {
                try { LoadDocument(GuiFormat.Load(start), start); }
                catch { LoadDocument(GuiDocument.NewEmpty(), null); }
            }
            else
                LoadDocument(GuiDocument.NewEmpty(), null);
        }

        void FillToolbox()
        {
            int y = 6;
            foreach (var label in ControlCatalog.Toolbox)
            {
                var b = new Button {
                    Text = label,
                    Width = 150,
                    Height = 26,
                    Left = 8,
                    Top = y,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                string captured = label;
                b.Click += (s, e) => AddControl(ControlCatalog.ToolboxToType(captured));
                _toolbox.Controls.Add(b);
                y += 28;
            }
        }

        void MainForm_Shown(object sender, EventArgs e)
        {
            try
            {
                if (splitInner.Width > 400) splitInner.SplitterDistance = 240;
                if (splitRight.Width > 300) splitRight.SplitterDistance = Math.Max(100, splitRight.Width - 180);
                if (splitOuter.Height > 250) splitOuter.SplitterDistance = Math.Max(100, splitOuter.Height - 160);
            }
            catch { }
        }

        void menuFileNew_Click(object sender, EventArgs e) { NewDoc(); }
        void menuFileOpen_Click(object sender, EventArgs e) { OpenDoc(); }
        void menuFileSave_Click(object sender, EventArgs e) { SaveDoc(false); }
        void menuFileSaveAs_Click(object sender, EventArgs e) { SaveDoc(true); }
        void menuFileExport_Click(object sender, EventArgs e) { ExportGui(); }
        void menuFileOpenMap_Click(object sender, EventArgs e) { OpenMap(); }
        void menuFileExit_Click(object sender, EventArgs e) { Close(); }
        void menuEditDelete_Click(object sender, EventArgs e) { DeleteSelected(); }
        void menuEditHideSelected_Click(object sender, EventArgs e) { HidePicked(true); }
        void menuEditShowSelected_Click(object sender, EventArgs e) { HidePicked(false); }
        void menuEditDeleteSelected_Click(object sender, EventArgs e) { DeletePicked(); }
        void menuEditKeepThis_Click(object sender, EventArgs e) { KeepThisInStack(); }
        void menuEditHideStack_Click(object sender, EventArgs e) { HideStack(true); }
        void menuEditShowStack_Click(object sender, EventArgs e) { HideStack(false); }
        void menuEditDeleteStack_Click(object sender, EventArgs e) { DeleteStack(); }
        void menuViewRefresh_Click(object sender, EventArgs e) { RefreshAll(); }
        void menuViewHideClosed_Click(object sender, EventArgs e)
        {
            _canvas.HideClosed = menuViewHideClosed.Checked;
            _canvas.Invalidate();
        }
        void menuAutomationStartup_Click(object sender, EventArgs e)
        {
            string status = GameRuntime.Boot();
            _statusText.Text = status;
            _canvas.Invalidate();
            RefreshTree();
            MessageBox.Show(this,
                "Loaded like the original tool / in-game boot.\n\n"
                + status + "\n\nstartup.py:\n" + (GameRuntime.StartupPath ?? "(not found)")
                + "\n\nThis registers addTextFont / setProperty from scripts.pkg and indexes shaders.pkg.",
                "Game runtime", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        void menuSettingsGameFolder_Click(object sender, EventArgs e) { ChooseGameRoot(); }
        void menuSettingsRebuildIndex_Click(object sender, EventArgs e) { RebuildPkgIndex(); }
        void menuSettingsOptions_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Original options lived in guitool_config.xml (window sizes, copy name, camera).", "Settings");
        }
        void menuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("GUITools\nTalesRunner GUI and map editor.\nLoads .gui / textures from tr*.pkg and maps from map*.pkg.", "About GUITools");
        }
        void btnPreview_Click(object sender, EventArgs e) { SetMode(true); }
        void btnEdit_Click(object sender, EventArgs e) { SetMode(false); }
        void btnTemplate_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Templates recovered from CGUITool*TemplateEntry / ImageCtrlTemplate.", "Template");
        }
        void prop_Changed(object sender, EventArgs e)
        {
            if (_uiLock) return;
            _uiLock = true;
            try
            {
                if (sender == _visVisible || sender == _visHidden)
                {
                    _visVisible.Checked = (sender == _visVisible);
                    _visHidden.Checked = (sender == _visHidden);
                }
                else if (sender == _enEnabled || sender == _enDisabled)
                {
                    _enEnabled.Checked = (sender == _enEnabled);
                    _enDisabled.Checked = (sender == _enDisabled);
                }
                else if (sender is RadioButton)
                {
                    foreach (var kv in _aspect)
                        kv.Value.Checked = (kv.Value == sender);
                }
            }
            finally { _uiLock = false; }
            ApplyProps();
        }
        void tree_AfterSelect(object sender, TreeViewEventArgs e) { ApplyNodeToCanvas(e.Node); }
        void canvas_SelectionChanged(object sender, EventArgs e)
        {
            SelectInTree(_canvas.Selected);
            LoadProps(_canvas.Selected);
            int n = _canvas.PickedCount;
            _statusText.Text = n + " selected on canvas  ·  Ctrl+click to add  ·  Hide selected / Delete selected / Del";
        }
        void canvas_DocumentEdited(object sender, EventArgs e)
        {
            RefreshTree();
            LoadProps(_canvas.Selected);
        }

        void SetMode(bool preview)
        {
            _btnPreview.Checked = preview;
            _btnEdit.Checked = !preview;
            _canvas.PreviewMode = preview;
            _canvas.Invalidate();
            _statusText.Text = preview
                ? "Preview: one sprite per slot (stacked copies hidden)"
                : "Edit: live sprites + dotted copies. Drag the selected control.";
        }

        void TreeDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            bool pick = _picked.Contains(e.Node);
            var fore = pick ? SystemColors.HighlightText : _tree.ForeColor;
            using (var b = new SolidBrush(pick ? SystemColors.Highlight : _tree.BackColor))
                e.Graphics.FillRectangle(b, e.Bounds);
            TextRenderer.DrawText(e.Graphics, e.Node.Text, _tree.Font, e.Bounds, fore,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }

        void TreeMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var node = _tree.GetNodeAt(e.Location);
            if (node == null) return;
            bool ctrl = (ModifierKeys & Keys.Control) != 0;
            bool shift = (ModifierKeys & Keys.Shift) != 0;
            if (ctrl)
            {
                if (!_picked.Remove(node)) _picked.Add(node);
                _anchor = node;
            }
            else if (shift && _anchor != null)
            {
                _picked.Clear();
                foreach (var n in NodesInRange(_anchor, node))
                    _picked.Add(n);
            }
            else
            {
                _picked.Clear();
                _picked.Add(node);
                _anchor = node;
            }
            _tree.Invalidate();
            _statusText.Text = _picked.Count + " selected  ·  Ctrl+click toggle  ·  Shift+click range  ·  Hide/Delete selected";
        }

        void TreeKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeletePicked();
                e.Handled = true;
            }
        }

        void ApplyNodeToCanvas(TreeNode node)
        {
            if (node == null) return;
            List<GuiControl> stack;
            if (_stackMap.TryGetValue(node, out stack) && stack.Count > 0)
            {
                _canvas.Selected = stack[0];
                _canvas.Invalidate();
                LoadProps(stack[0]);
                return;
            }
            GuiControl c;
            if (_nodeMap.TryGetValue(node, out c))
            {
                _canvas.Selected = c;
                var one = new List<GuiControl>();
                one.Add(c);
                _canvas.SetPicked(one);
                _canvas.Invalidate();
                LoadProps(c);
            }
        }

        List<TreeNode> NodesInRange(TreeNode a, TreeNode b)
        {
            var all = new List<TreeNode>();
            Flatten(_tree.Nodes, all);
            int i0 = all.IndexOf(a);
            int i1 = all.IndexOf(b);
            var r = new List<TreeNode>();
            if (i0 < 0 || i1 < 0)
            {
                r.Add(b);
                return r;
            }
            if (i0 > i1) { int t = i0; i0 = i1; i1 = t; }
            for (int i = i0; i <= i1; i++) r.Add(all[i]);
            return r;
        }

        static void Flatten(TreeNodeCollection nodes, List<TreeNode> into)
        {
            foreach (TreeNode n in nodes)
            {
                into.Add(n);
                if (n.IsExpanded) Flatten(n.Nodes, into);
            }
        }

        List<GuiControl> PickedControls()
        {
            var seen = new HashSet<GuiControl>();
            var list = new List<GuiControl>();
            List<GuiControl> canvasPick = _canvas.GetPicked();
            if (canvasPick != null)
            {
                for (int i = 0; i < canvasPick.Count; i++)
                    if (seen.Add(canvasPick[i])) list.Add(canvasPick[i]);
            }
            foreach (var n in _picked)
                AddNodeControls(n, seen, list);
            if (list.Count == 0 && _tree.SelectedNode != null)
                AddNodeControls(_tree.SelectedNode, seen, list);
            return list;
        }

        void AddNodeControls(TreeNode n, HashSet<GuiControl> seen, List<GuiControl> list)
        {
            if (n == null) return;
            List<GuiControl> stack;
            if (_stackMap.TryGetValue(n, out stack))
            {
                foreach (var c in stack)
                    if (seen.Add(c)) list.Add(c);
                return;
            }
            GuiControl one;
            if (_nodeMap.TryGetValue(n, out one) && seen.Add(one))
                list.Add(one);
        }

        void HidePicked(bool hide)
        {
            var list = PickedControls();
            if (list.Count == 0 || _doc == null) return;
            foreach (var c in list)
                if (c != _doc.Root) SetHidden(c, hide);
            _statusText.Text = (hide ? "Hid " : "Showed ") + list.Count + " controls";
            _canvas.SetPicked(null);
            RefreshAll();
        }

        void DeletePicked()
        {
            var list = PickedControls();
            if (list.Count == 0 || _doc == null) return;
            if (list.Count > 1)
            {
                if (MessageBox.Show(this, "Delete " + list.Count + " selected controls?",
                    "Delete selected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }
            foreach (var c in list)
                RemoveControl(c);
            _canvas.Selected = _doc.Root;
            _canvas.SetPicked(null);
            RefreshAll();
        }

        void LoadDocument(GuiDocument doc, string path)
        {
            _doc = doc;
            _path = path;
            GameData.CurrentGuiDir = string.IsNullOrEmpty(path) ? GameData.ToolGui : Path.GetDirectoryName(path);
            _canvas.Document = doc;
            if (doc != null && doc.Root != null)
                _canvas.Size = new Size(Math.Max(400, doc.Root.W + 80), Math.Max(300, doc.Root.H + 60));
            else
                _canvas.Size = new Size(800, 600);
            _canvas.Selected = doc.Root;
            _canvas.Selected = doc.Root;
            RefreshAll();
            Text = "GUITools - " + (path == null ? "untitled" : Path.GetFileName(path));
            string opened = path == null ? "New document" : "Opened " + Path.GetFileName(path);
            if (!string.IsNullOrEmpty(GameRuntime.LastStatus))
                opened += "  ·  " + GameRuntime.LastStatus;
            _statusText.Text = opened;
        }

        void RefreshAll()
        {
            RefreshTree();
            _canvas.Invalidate();
            LoadProps(_canvas.Selected);
        }

        void RefreshTree()
        {
            _tree.BeginUpdate();
            _tree.Nodes.Clear();
            _nodeMap.Clear();
            _stackMap.Clear();
            _picked.Clear();
            _anchor = null;
            var fileNode = _tree.Nodes.Add("File info.");
            var groups = new Dictionary<string, TreeNode>();
            foreach (var g in ControlCatalog.TreeGroups)
                groups[g] = fileNode.Nodes.Add(g);

            if (_doc != null && _doc.Root != null)
            {
                var dlgNode = groups["Dialog settings"].Nodes.Add(_doc.Root.Label);
                _nodeMap[dlgNode] = _doc.Root;
                var stacks = CollectStacks(_doc.Root);
                var stackRoot = fileNode.Nodes.Insert(1, "Overlapping stacks [" + stacks.Count + "]");
                foreach (var stack in stacks)
                {
                    var first = stack[0];
                    string title = first.X + "," + first.Y + "  " + first.W + "x" + first.H
                        + "  x" + stack.Count;
                    var sn = stackRoot.Nodes.Add(title);
                    _stackMap[sn] = stack;
                    foreach (var c in stack)
                    {
                        var n = sn.Nodes.Add(ControlLabel(c));
                        _nodeMap[n] = c;
                    }
                }
                stackRoot.Expand();
                AddTreeControls(_doc.Root.Children, groups, fileNode, null);
                foreach (var t in _doc.Templates)
                {
                    var n = groups["Template"].Nodes.Add(t.Label);
                    _nodeMap[n] = t;
                }
            }
            fileNode.Expand();
            var runtime = _tree.Nodes.Add("Game runtime");
            runtime.Nodes.Add("startup.py  " + (GameRuntime.StartupPath ?? "(missing)"));
            runtime.Nodes.Add("Fonts [" + GameRuntime.FontCount + "]");
            runtime.Nodes.Add("Scripts [" + GameRuntime.ScriptCount + "]");
            TreeNode sh = runtime.Nodes.Add("Shaders [" + GameRuntime.ShaderCount + "]");
            int shown = 0;
            for (int i = 0; i < GameRuntime.ShaderNames.Count && shown < 24; i++)
            {
                string n = Path.GetFileName(GameRuntime.ShaderNames[i]);
                if (n.StartsWith("ui_", StringComparison.OrdinalIgnoreCase)
                    || n.StartsWith("gui_", StringComparison.OrdinalIgnoreCase))
                {
                    sh.Nodes.Add(n);
                    shown++;
                }
            }
            runtime.Expand();
            foreach (TreeNode n in fileNode.Nodes)
                if (n.Nodes.Count > 0) n.Expand();
            _tree.EndUpdate();
            SelectInTree(_canvas.Selected);
        }

        static string ControlLabel(GuiControl c)
        {
            string name = c.Label;
            if (!c.Visible) name = "[hidden] " + name;
            return name;
        }

        List<List<GuiControl>> CollectStacks(GuiControl root)
        {
            var buckets = new Dictionary<string, List<GuiControl>>();
            CollectStackWalk(root, 0, 0, true, buckets);
            var stacks = new List<List<GuiControl>>();
            foreach (var kv in buckets)
                if (kv.Value.Count > 1) stacks.Add(kv.Value);
            stacks.Sort(delegate(List<GuiControl> a, List<GuiControl> b) { return b.Count.CompareTo(a.Count); });
            return stacks;
        }

        void CollectStackWalk(GuiControl c, int absX, int absY, bool isRoot,
            Dictionary<string, List<GuiControl>> buckets)
        {
            if (c == null) return;
            if (!isRoot)
            {
                string key = (absX / 8) + ":" + (absY / 8) + ":" + (c.W / 16) + ":" + (c.H / 16);
                List<GuiControl> list;
                if (!buckets.TryGetValue(key, out list))
                {
                    list = new List<GuiControl>();
                    buckets[key] = list;
                }
                list.Add(c);
            }
            foreach (var ch in c.Children)
                CollectStackWalk(ch, absX + ch.X, absY + ch.Y, false, buckets);
        }

        List<GuiControl> StackOf(GuiControl c)
        {
            if (c == null || _doc == null || _doc.Root == null) return new List<GuiControl>();
            foreach (var stack in CollectStacks(_doc.Root))
            {
                if (stack.Contains(c)) return stack;
            }
            TreeNode node = _tree.SelectedNode;
            List<GuiControl> fromNode;
            if (node != null && _stackMap.TryGetValue(node, out fromNode))
                return fromNode;
            if (node != null && node.Parent != null && _stackMap.TryGetValue(node.Parent, out fromNode))
                return fromNode;
            var one = new List<GuiControl>();
            one.Add(c);
            return one;
        }

        void SetHidden(GuiControl c, bool hidden)
        {
            if (c == null) return;
            c.Visible = !hidden;
            string n = c.Name ?? "";
            if (hidden)
            {
                if (n.IndexOf("(Closed)", StringComparison.OrdinalIgnoreCase) < 0)
                    c.Name = n + "(Closed)";
            }
            else
            {
                c.Name = n.Replace("(Closed)", "").Replace("(closed)", "");
            }
        }

        void KeepThisInStack()
        {
            var c = _canvas.Selected;
            var stack = StackOf(c);
            if (c == null || stack.Count < 2) return;
            foreach (var o in stack)
                SetHidden(o, o != c);
            _statusText.Text = "Kept " + c.Label + ", hid " + (stack.Count - 1) + " copies";
            RefreshAll();
        }

        void HideStack(bool hide)
        {
            var stack = StackOf(_canvas.Selected);
            if (stack.Count == 0) return;
            foreach (var o in stack)
                SetHidden(o, hide);
            _statusText.Text = hide
                ? "Hid stack of " + stack.Count
                : "Showed stack of " + stack.Count;
            RefreshAll();
        }

        void DeleteStack()
        {
            var stack = StackOf(_canvas.Selected);
            if (stack.Count == 0) return;
            if (MessageBox.Show(this, "Delete " + stack.Count + " overlapping copies?",
                "Delete stack", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            foreach (var o in stack)
                RemoveControl(o);
            _canvas.Selected = _doc.Root;
            RefreshAll();
        }

        bool RemoveControl(GuiControl target)
        {
            if (_doc == null || _doc.Root == null || target == null || target == _doc.Root) return false;
            return RemoveFrom(_doc.Root, target);
        }

        static bool RemoveFrom(GuiControl parent, GuiControl target)
        {
            if (parent.Children.Remove(target)) return true;
            foreach (var ch in parent.Children)
                if (RemoveFrom(ch, target)) return true;
            return false;
        }

        void AddTreeControls(IEnumerable<GuiControl> list, Dictionary<string, TreeNode> groups, TreeNode fileNode, HashSet<GuiControl> skip)
        {
            foreach (var c in list)
            {
                if (skip == null || !skip.Contains(c))
                {
                    var gname = ControlCatalog.GroupOf(c.CtrlType);
                    TreeNode parent;
                    if (!groups.TryGetValue(gname, out parent)) parent = fileNode;
                    var n = parent.Nodes.Add(ControlLabel(c));
                    _nodeMap[n] = c;
                }
                if (c.Children.Count > 0)
                    AddTreeControls(c.Children, groups, fileNode, skip);
            }
        }

        void SelectInTree(GuiControl c)
        {
            if (c == null) return;
            foreach (var kv in _nodeMap)
            {
                if (kv.Value == c)
                {
                    _tree.SelectedNode = kv.Key;
                    break;
                }
            }
        }

        void LoadProps(GuiControl c)
        {
            _uiLock = true;
            try
            {
                if (c == null) return;
                _visVisible.Checked = c.Visible;
                _visHidden.Checked = !c.Visible;
                _enEnabled.Checked = c.Enabled;
                _enDisabled.Checked = !c.Enabled;
                _txtDesc.Text = c.Description;
                _txtCmd.Text = c.Command;
                _txtText.Text = c.Text;
                foreach (var kv in _aspect)
                    kv.Value.Checked = kv.Key == (c.AspectRatio ?? "none");
            }
            finally { _uiLock = false; }
        }

        void ApplyProps()
        {
            if (_uiLock || _canvas == null) return;
            var c = _canvas.Selected;
            if (c == null) return;
            c.Visible = _visVisible.Checked;
            c.Enabled = _enEnabled.Checked;
            c.Description = _txtDesc.Text;
            c.Command = _txtCmd.Text;
            c.Text = _txtText.Text;
            foreach (var kv in _aspect)
                if (kv.Value.Checked) c.AspectRatio = kv.Key;
            _canvas.Invalidate();
            RefreshTree();
        }

        void AddControl(string type)
        {
            if (_doc == null || _doc.Root == null) return;
            var c = new GuiControl {
                CtrlType = type,
                Name = type + (_doc.Root.Children.Count + 1),
                Text = type,
                X = 40 + (_doc.Root.Children.Count % 6) * 20,
                Y = 80 + (_doc.Root.Children.Count % 8) * 20,
                W = type.Contains("List") ? 220 : 120,
                H = type.Contains("List") ? 160 : 28
            };
            _doc.Root.Children.Add(c);
            _canvas.Selected = c;
            RefreshAll();
        }

        void DeleteSelected()
        {
            var c = _canvas.Selected;
            if (!RemoveControl(c)) return;
            _canvas.Selected = _doc.Root;
            RefreshAll();
        }

        void NewDoc()
        {
            LoadDocument(GuiDocument.NewEmpty(), null);
        }

        void OpenMap()
        {
            var map = new MapForm();
            map.Show();
            map.BrowseOpen();
        }

        void RebuildPkgIndex()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                int n = GameData.RebuildIndex();
                _statusText.Text = "Indexed " + n + " files from " + GameData.GameRoot;
                MessageBox.Show(this, "Indexed " + n + " files.\nCache: " + GameData.CacheRoot, "Package index");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Index failed");
            }
            finally { Cursor = Cursors.Default; }
        }

        void ChooseGameRoot()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "TalesRunner game folder (contains tr*.pkg)";
                fbd.SelectedPath = GameData.BrowseStartDir();
                if (fbd.ShowDialog(this) == DialogResult.OK)
                {
                    GameData.RememberBrowse(fbd.SelectedPath);
                    GameData.GameRoot = fbd.SelectedPath;
                    _statusText.Text = "Game data: " + GameData.GameRoot;
                }
            }
        }

        void OpenDoc()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "GUI Files (*.gui)|*.gui|JSON (*.json)|*.json|All (*.*)|*.*";
                string[] dirs = {
                    Path.Combine(GameData.CacheRoot ?? "", "gui"),
                    Path.Combine(GameData.CacheRoot ?? "", "gui", "lobby"),
                    GameData.ToolGui,
                    Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "gui"))
                };
                foreach (var d in dirs)
                {
                    if (!string.IsNullOrEmpty(d) && Directory.Exists(d))
                    {
                        ofd.InitialDirectory = d;
                        break;
                    }
                }
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    try { LoadDocument(GuiFormat.Load(ofd.FileName), ofd.FileName); }
                    catch (Exception ex) { MessageBox.Show(this, ex.Message, "Open failed"); }
                }
            }
        }

        void SaveDoc(bool saveAs)
        {
            string path = _path;
            if (saveAs || path == null)
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "JSON (*.json)|*.json|GUI Files (*.gui)|*.gui";
                    sfd.FileName = path == null ? "untitled.json" : Path.GetFileName(path);
                    if (sfd.ShowDialog(this) != DialogResult.OK) return;
                    path = sfd.FileName;
                }
            }
            try
            {
                if (path.EndsWith(".gui", StringComparison.OrdinalIgnoreCase))
                    GuiFormat.SaveGui(_doc, path);
                else
                    GuiFormat.SaveJson(_doc, path);
                _path = path;
                Text = "GUITools - " + Path.GetFileName(path);
                _statusText.Text = "Saved " + path;
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Save failed"); }
        }

        void ExportGui()
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "GUI Files (*.gui)|*.gui";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    GuiFormat.SaveGui(_doc, sfd.FileName);
                    _statusText.Text = "Exported " + sfd.FileName;
                }
            }
        }
    }
}
