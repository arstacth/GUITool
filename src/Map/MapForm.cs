using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GUITools
{
    public partial class MapForm : Form
    {
        MapDocument _doc;
        MapView _view;

        public MapForm()
        {
            InitializeComponent();
            _view = new MapView();
            _view.Dock = DockStyle.Fill;
            splitRight.Panel1.Controls.Add(_view);
        }

        void MapForm_Shown(object sender, EventArgs e)
        {
            try
            {
                if (splitMain.Width > 300) splitMain.SplitterDistance = 240;
                if (splitRight.Height > 200) splitRight.SplitterDistance = Math.Max(120, splitRight.Height - 180);
            }
            catch { }
        }

        void menuFileOpen_Click(object sender, EventArgs e) { OpenTrv(); }
        void menuFileImport_Click(object sender, EventArgs e) { BrowseOpen(); }
        void menuFileClose_Click(object sender, EventArgs e) { Close(); }

        public void BrowseOpen()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Open map package or .trv";
                ofd.Filter = "Map packages (*.pkg)|*.pkg|TalesRunner maps (*.trv)|*.trv|All|*.*";
                ofd.InitialDirectory = GameData.BrowseStartDir();
                if (ofd.ShowDialog(this) != DialogResult.OK) return;
                GameData.RememberBrowse(ofd.FileName);
                string ext = Path.GetExtension(ofd.FileName).ToLowerInvariant();
                if (ext == ".pkg")
                    ImportPkgFile(ofd.FileName);
                else
                    OpenPath(ofd.FileName);
            }
        }

        public void OpenPath(string trvPath)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                _doc = MapDocument.LoadTrv(trvPath);
                Text = "GUITools Map — " + Path.GetFileName(trvPath);
                _view.SetDocument(_doc);
                _grid.SelectedObject = new MapInfo {
                    File = _doc.Path,
                    FileVersion = _doc.Version,
                    ImportVersion = _doc.ImportVersion,
                    TrackObjects = _doc.TrackObjects.Count,
                    TerrainTris = _doc.Terrain.Count,
                    Assets = _doc.Textures.Count
                };
                FillTree();
                _statusText.Text = Path.GetFileName(trvPath) + " — TRV v" + _doc.Version
                    + " / import " + _doc.ImportVersion + ", triangles " + _doc.Terrain.Count
                    + ", props " + _doc.Props.Count
                    + ", objects " + _doc.TrackObjects.Count
                    + ", textures " + _doc.Textures.Count;
            }
            finally { Cursor = Cursors.Default; }
        }

        void FillTree()
        {
            _tree.BeginUpdate();
            _tree.Nodes.Clear();
            if (_doc == null) { _tree.EndUpdate(); return; }
            var track = _tree.Nodes.Add("TrackList [" + _doc.TrackObjects.Count + "]");
            foreach (var n in _doc.TrackObjects) track.Nodes.Add(n);
            var terrain = _tree.Nodes.Add("Terrain [" + _doc.Terrain.Count + " tris]");
            terrain.Nodes.Add("AABB X " + (int)_doc.MinX + " .. " + (int)_doc.MaxX);
            terrain.Nodes.Add("AABB Y " + (int)_doc.MinY + " .. " + (int)_doc.MaxY);
            terrain.Nodes.Add("AABB Z " + (int)_doc.MinZ + " .. " + (int)_doc.MaxZ);
            var mats = _tree.Nodes.Add("Terrain Materials [" + _doc.Textures.Count + "]");
            foreach (var t in _doc.Textures) mats.Nodes.Add(t);
            var props = _tree.Nodes.Add("Component [" + _doc.Props.Count + "]");
            int shown = 0;
            foreach (var p in _doc.Props)
            {
                if (shown++ > 80) { props.Nodes.Add("..."); break; }
                props.Nodes.Add(p.Texture);
            }
            _tree.Nodes.Add("Map settings");
            track.Expand();
            terrain.Expand();
            _tree.EndUpdate();
        }

        void OpenTrv()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "TalesRunner maps (*.trv)|*.trv|All|*.*";
                ofd.InitialDirectory = GameData.BrowseStartDir();
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    GameData.RememberBrowse(ofd.FileName);
                    OpenPath(ofd.FileName);
                }
            }
        }

        void ImportPkg()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Map packages (*.pkg)|*.pkg|All|*.*";
                ofd.InitialDirectory = GameData.BrowseStartDir();
                if (ofd.ShowDialog(this) != DialogResult.OK) return;
                GameData.RememberBrowse(ofd.FileName);
                ImportPkgFile(ofd.FileName);
            }
        }

        void ImportPkgFile(string pkgPath)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var entries = PkgArchive.ListEntries(pkgPath);
                var trvs = new System.Collections.Generic.List<PkgFileEntry>();
                foreach (var e in entries)
                    if (e.Path.EndsWith(".trv", StringComparison.OrdinalIgnoreCase))
                        trvs.Add(e);
                if (trvs.Count == 0)
                {
                    MessageBox.Show(this, "No .trv files in this pkg.");
                    return;
                }
                string pick = trvs[0].Path;
                using (var pickDlg = new Form())
                {
                    pickDlg.Text = "Choose map";
                    pickDlg.Width = 520; pickDlg.Height = 400;
                    pickDlg.StartPosition = FormStartPosition.CenterParent;
                    var lb = new ListBox { Dock = DockStyle.Fill };
                    foreach (var e in trvs) lb.Items.Add(e.Path);
                    lb.SelectedIndex = 0;
                    var ok = new Button { Text = "Import", Dock = DockStyle.Bottom, Height = 32 };
                    ok.Click += (s, ev) => { pickDlg.DialogResult = DialogResult.OK; pickDlg.Close(); };
                    pickDlg.Controls.Add(lb);
                    pickDlg.Controls.Add(ok);
                    if (pickDlg.ShowDialog(this) != DialogResult.OK) return;
                    pick = lb.SelectedItem.ToString();
                }
                PkgFileEntry chosen = null;
                foreach (var e in trvs) if (e.Path == pick) chosen = e;
                if (chosen == null) return;
                string folder = Path.GetDirectoryName(pick.Replace('/', '\\'));
                Directory.CreateDirectory(Path.Combine(GameData.CacheRoot, folder ?? "maps"));
                string prefix = folder + "\\";
                foreach (var e in entries)
                {
                    string p = e.Path.Replace('/', '\\');
                    if (!p.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && p != pick.Replace('/', '\\'))
                        continue;
                    string ext = Path.GetExtension(p).ToLowerInvariant();
                    bool want = ext == ".trv" || ext == ".trv_sm" || ext == ".trv_rd"
                        || ext == ".png" || ext == ".jpg" || ext == ".txt" || ext == ".tga"
                        || ext == ".dds" || ext == ".ca3" || ext == ".m1" || ext == ".pt1";
                    if (!want && p != pick.Replace('/', '\\')) continue;
                    string dest = Path.Combine(GameData.CacheRoot, p);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                    File.WriteAllBytes(dest, PkgArchive.Extract(pkgPath, e.Offset, e.Parts));
                }
                string trvDest = Path.Combine(GameData.CacheRoot, pick.Replace('/', '\\'));
                OpenPath(trvDest);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Import failed");
            }
            finally { Cursor = Cursors.Default; }
        }

        public class MapInfo
        {
            public string File { get; set; }
            public int FileVersion { get; set; }
            public int ImportVersion { get; set; }
            public int TrackObjects { get; set; }
            public int TerrainTris { get; set; }
            public int Assets { get; set; }
        }
    }
}
