using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GUITools
{
    /// <summary>
    /// Focus editor: one live sprite per overlapping slot, plus the selection.
    /// Extra stacked copies are not painted (tree still lists all of them).
    /// </summary>
    [ToolboxItem(false)]
    [Description("TalesRunner GUI canvas")]
    public class DesignCanvas : UserControl
    {
        public GuiDocument Document { get; set; }
        public GuiControl Selected { get; set; }
        public bool PreviewMode { get; set; }
        public bool HideClosed { get; set; }
        public event EventHandler SelectionChanged;
        public event EventHandler DocumentEdited;
        public event EventHandler HidePickedRequested;
        public event EventHandler DeletePickedRequested;

        readonly List<GuiControl> _picked = new List<GuiControl>();

        Point _origin = new Point(40, 16);
        Point _dragStart;
        Rectangle _startBounds;
        HandleKind _handle = HandleKind.None;
        bool _dragging;
        readonly HashSet<GuiControl> _scene = new HashSet<GuiControl>();

        enum HandleKind { None, Move, N, S, E, W, NE, NW, SE, SW }

        public DesignCanvas()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(48, 50, 56);
            TabStop = true;
            PreviewMode = true;
            HideClosed = true;
            SetStyle(ControlStyles.Selectable, true);
        }

        Rectangle DialogScreen()
        {
            if (Document == null || Document.Root == null)
                return new Rectangle(_origin.X, _origin.Y, 640, 400);
            return new Rectangle(_origin.X, _origin.Y, Math.Max(8, Document.Root.W), Math.Max(8, Document.Root.H));
        }

        Rectangle ToScreen(GuiControl target)
        {
            if (Document == null || Document.Root == null || target == null)
                return Rectangle.Empty;
            if (target == Document.Root) return DialogScreen();
            var stack = new List<GuiControl>();
            if (!FindPath(Document.Root, target, stack))
                return new Rectangle(DialogScreen().X + target.X, DialogScreen().Y + target.Y, target.W, target.H);
            var r = DialogScreen();
            for (int i = 1; i < stack.Count; i++)
            {
                var c = stack[i];
                r = new Rectangle(r.X + c.X, r.Y + c.Y, Math.Max(1, c.W), Math.Max(1, c.H));
            }
            return r;
        }

        static bool FindPath(GuiControl node, GuiControl target, List<GuiControl> stack)
        {
            stack.Add(node);
            if (node == target) return true;
            foreach (var ch in node.Children)
                if (FindPath(ch, target, stack)) return true;
            stack.RemoveAt(stack.Count - 1);
            return false;
        }

        bool IsClosed(GuiControl c)
        {
            return HideClosed && c != null && c.Name != null
                && c.Name.IndexOf("(Closed)", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        int SlotScore(GuiControl c)
        {
            if (c == null) return -2000;
            if (!c.Visible) return -1500;
            if (IsClosed(c)) return -1000;
            if (c == Selected) return 20000;
            string n = (c.Name ?? "");
            string nl = n.ToLowerInvariant();
            string img = (c.ResolveImageFile() ?? "").Replace('/', '\\').ToLowerInvariant();
            int s = 10;
            if (c.CtrlType == "Frame" || c.CtrlType == "Dialog")
            {
                s += 90;
                if (c.W > 400) s += 50;
                if (nl.IndexOf("base") >= 0) s += 40;
            }
            if (nl.IndexOf("channel") >= 0 || nl.IndexOf("match") >= 0 || nl.IndexOf("room") >= 0) s += 70;
            if (nl.IndexOf("lobby") >= 0 || nl == "btnbase") s += 50;
            if (n.Length > 0 && !string.Equals(n, c.CtrlType, StringComparison.Ordinal)) s += 35;
            else s -= 20;
            if (nl.IndexOf("copy") >= 0 || nl.StartsWith("popup") || nl.StartsWith("newimg")
                || nl.StartsWith("gotopark") || nl.StartsWith("deco") || nl.IndexOf("template") >= 0
                || nl.IndexOf("autofishing") >= 0 || nl.EndsWith("dis")) s -= 90;
            if (img.IndexOf("\\park\\") >= 0 || img.IndexOf("\\gui2\\") >= 0
                || img.IndexOf("\\gui3\\") >= 0 || img.IndexOf("\\shop\\") >= 0) s -= 50;
            return s;
        }

        void RebuildScene()
        {
            _scene.Clear();
            if (Document == null || Document.Root == null) return;
            var best = new Dictionary<string, GuiControl>();
            var bestScore = new Dictionary<string, int>();
            CollectScene(Document.Root, 0, 0, best, bestScore);
            foreach (var kv in best)
                if (kv.Value != null) _scene.Add(kv.Value);
            if (Selected != null && Selected != Document.Root)
                _scene.Add(Selected);
            for (int i = 0; i < _picked.Count; i++)
                if (_picked[i] != null) _scene.Add(_picked[i]);
        }

        void CollectScene(GuiControl c, int absX, int absY,
            Dictionary<string, GuiControl> best, Dictionary<string, int> bestScore)
        {
            if (c == null) return;
            if (c != Document.Root)
            {
                if (c.CtrlType == "Frame" && c.W >= 800 && c.H >= 40)
                    _scene.Add(c);
                string key = (absX / 8) + ":" + (absY / 8);
                int score = SlotScore(c);
                int prev;
                if (!bestScore.TryGetValue(key, out prev) || score >= prev)
                {
                    best[key] = c;
                    bestScore[key] = score;
                }
            }
            foreach (var ch in c.Children)
                CollectScene(ch, absX + ch.X, absY + ch.Y, best, bestScore);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.SmoothingMode = SmoothingMode.None;
            if (Document == null || Document.Root == null)
            {
                TextRenderer.DrawText(g, "Open a .gui file", Font, new Point(16, 16), Color.Silver);
                return;
            }
            RebuildScene();
            var dlg = DialogScreen();
            using (var shadow = new SolidBrush(Color.FromArgb(28, 28, 32)))
                g.FillRectangle(shadow, dlg.X + 4, dlg.Y + 4, dlg.Width, dlg.Height);
            using (var b = new SolidBrush(Color.FromArgb(32, 36, 48)))
                g.FillRectangle(b, dlg);
            g.DrawRectangle(Pens.Black, dlg);

            var state = g.Save();
            g.SetClip(dlg);
            DrawTree(g, Document.Root, dlg, true);
            g.Restore(state);

            if (Selected != null && Selected != Document.Root)
                DrawSelection(g, ToScreen(Selected), true);
            for (int i = 0; i < _picked.Count; i++)
            {
                if (_picked[i] == null || _picked[i] == Selected || _picked[i] == Document.Root) continue;
                DrawSelection(g, ToScreen(_picked[i]), false);
            }

            string hint = PreviewMode
                ? "Ctrl+click 2–3 buttons to pick them  ·  Hide selected / Delete selected  ·  Del key"
                : "Ctrl+click to pick several  ·  drag gold box  ·  Hide / Delete selected";
            TextRenderer.DrawText(g, hint, Font, new Point(8, Height - 22), Color.FromArgb(190, 190, 196));
        }

        void DrawTree(Graphics g, GuiControl c, Rectangle screen, bool isRoot)
        {
            var r = isRoot ? screen : new Rectangle(screen.X + c.X, screen.Y + c.Y, Math.Max(1, c.W), Math.Max(1, c.H));
            if (isRoot || _scene.Contains(c))
            {
                if (c.Visible || c == Selected || isRoot)
                    DrawSprite(g, c, r);
            }

            foreach (var ch in c.Children)
                DrawTree(g, ch, r, false);
        }

        void DrawSprite(Graphics g, GuiControl c, Rectangle r)
        {
            if (!DrawGameImage(g, c, r))
            {
                if (c.CtrlType == "Dialog")
                {
                    using (var b = new SolidBrush(Color.FromArgb(48, 56, 80)))
                        g.FillRectangle(b, r);
                }
                else if (c.CtrlType == "EditCtrl")
                {
                    g.FillRectangle(Brushes.White, r);
                    g.DrawRectangle(Pens.Gray, r);
                }
            }
            bool sprite = c.CtrlType == "Image" || c.CtrlType == "ImageCtrl" || c.CtrlType == "ItemImageCtrl"
                || c.CtrlType == "ImageButton" || c.CtrlType == "FrameButton" || c.CtrlType == "Frame";
            if (!sprite && !string.IsNullOrEmpty(c.Text) && c.CtrlType != "Dialog")
            {
                var fg = (c.CtrlType == "EditCtrl") ? Color.Black : Color.White;
                Font font = GameRuntime.FontFor(c);
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;
                TextRenderer.DrawText(g, c.Text, font, r, fg, flags);
            }
        }

        bool DrawGameImage(Graphics g, GuiControl c, Rectangle dest)
        {
            string imgPath = c.ResolveImageFile();
            Image img = GameData.GetImage(imgPath);
            if (img == null) return false;
            var srcVals = c.ResolveSourceRect();
            Rectangle src = srcVals != null
                ? GuiFormat.AtlasRect(srcVals, img.Width, img.Height)
                : new Rectangle(0, 0, img.Width, img.Height);
            if (src.Width <= 0 || src.Height <= 0) return false;
            try
            {
                bool frame = c.CtrlType == "Frame" || c.CtrlType == "Dialog";
                int border = frame ? GuiFormat.AtlasBorder(srcVals) : 0;
                DrawNineSlice(g, img, src, dest, border);
                return true;
            }
            catch { return false; }
        }

        static void DrawNineSlice(Graphics g, Image img, Rectangle src, Rectangle dest, int border)
        {
            int b = border;
            bool sameSize = Math.Abs(src.Width - dest.Width) <= 1 && Math.Abs(src.Height - dest.Height) <= 1;
            if (sameSize || b < 1 || src.Width <= b * 2 || src.Height <= b * 2
                || dest.Width <= b * 2 || dest.Height <= b * 2)
            {
                g.DrawImage(img, dest, src, GraphicsUnit.Pixel);
                return;
            }
            int[] sw = { b, src.Width - 2 * b, b };
            int[] sh = { b, src.Height - 2 * b, b };
            int[] dw = { b, dest.Width - 2 * b, b };
            int[] dh = { b, dest.Height - 2 * b, b };
            int[] sx = { src.X, src.X + b, src.Right - b };
            int[] sy = { src.Y, src.Y + b, src.Bottom - b };
            int[] dx = { dest.X, dest.X + b, dest.Right - b };
            int[] dy = { dest.Y, dest.Y + b, dest.Bottom - b };
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                {
                    if (sw[col] <= 0 || sh[row] <= 0 || dw[col] <= 0 || dh[row] <= 0) continue;
                    g.DrawImage(img,
                        new Rectangle(dx[col], dy[row], dw[col], dh[row]),
                        new Rectangle(sx[col], sy[row], sw[col], sh[row]),
                        GraphicsUnit.Pixel);
                }
        }

        void DrawSelection(Graphics g, Rectangle r, bool primary)
        {
            Color col = primary ? Color.Gold : Color.FromArgb(255, 180, 80);
            using (var p = new Pen(col, primary ? 2 : 1))
                g.DrawRectangle(p, Rectangle.Inflate(r, 1, 1));
            if (PreviewMode || !primary) return;
            foreach (var h in HandleRects(r))
                g.FillRectangle(Brushes.OrangeRed, h);
        }

        Rectangle[] HandleRects(Rectangle r)
        {
            int s = 6;
            return new[] {
                new Rectangle(r.Left - s/2, r.Top - s/2, s, s),
                new Rectangle(r.Right - s/2, r.Top - s/2, s, s),
                new Rectangle(r.Left - s/2, r.Bottom - s/2, s, s),
                new Rectangle(r.Right - s/2, r.Bottom - s/2, s, s),
                new Rectangle(r.Left + r.Width/2 - s/2, r.Top - s/2, s, s),
                new Rectangle(r.Left + r.Width/2 - s/2, r.Bottom - s/2, s, s),
                new Rectangle(r.Left - s/2, r.Top + r.Height/2 - s/2, s, s),
                new Rectangle(r.Right - s/2, r.Top + r.Height/2 - s/2, s, s),
            };
        }

        HandleKind HitHandle(Rectangle r, Point p)
        {
            var hs = HandleRects(r);
            HandleKind[] kinds = { HandleKind.NW, HandleKind.NE, HandleKind.SW, HandleKind.SE, HandleKind.N, HandleKind.S, HandleKind.W, HandleKind.E };
            for (int i = 0; i < hs.Length; i++)
                if (hs[i].Contains(p)) return kinds[i];
            if (r.Contains(p)) return HandleKind.Move;
            return HandleKind.None;
        }

        GuiControl HitTest(GuiControl node, Rectangle screen, Point p, bool isRoot)
        {
            var r = isRoot ? screen : new Rectangle(screen.X + node.X, screen.Y + node.Y, Math.Max(1, node.W), Math.Max(1, node.H));
            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                var h = HitTest(node.Children[i], r, p, false);
                if (h != null) return h;
            }
            if (!isRoot && r.Contains(p))
            {
                if (!_scene.Contains(node) && node != Selected && !IsPicked(node)) return null;
                return node;
            }
            return null;
        }

        public List<GuiControl> GetPicked()
        {
            var list = new List<GuiControl>();
            for (int i = 0; i < _picked.Count; i++)
                if (_picked[i] != null && _picked[i] != Document.Root) list.Add(_picked[i]);
            if (list.Count == 0 && Selected != null && Selected != Document.Root)
                list.Add(Selected);
            return list;
        }

        public int PickedCount { get { return GetPicked().Count; } }

        bool IsPicked(GuiControl c)
        {
            return c != null && _picked.Contains(c);
        }

        public void SetPicked(IEnumerable<GuiControl> items)
        {
            _picked.Clear();
            if (items == null) return;
            foreach (var c in items)
                if (c != null && !_picked.Contains(c)) _picked.Add(c);
            Invalidate();
        }

        void TogglePicked(GuiControl c)
        {
            if (c == null || c == Document.Root) return;
            if (_picked.Contains(c)) _picked.Remove(c);
            else _picked.Add(c);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            if (Document == null || Document.Root == null) return;
            GuiControl hit = HitTest(Document.Root, DialogScreen(), e.Location, true);
            if (hit == null && DialogScreen().Contains(e.Location))
                hit = Document.Root;
            bool ctrl = (ModifierKeys & Keys.Control) != 0;
            if (ctrl && hit != null && hit != Document.Root)
            {
                TogglePicked(hit);
                Selected = hit;
            }
            else if (e.Button == MouseButtons.Right && hit != null && IsPicked(hit))
            {
                Selected = hit;
            }
            else
            {
                _picked.Clear();
                if (hit != null && hit != Document.Root) _picked.Add(hit);
                Selected = hit;
            }
            if (SelectionChanged != null) SelectionChanged(this, EventArgs.Empty);
            if (e.Button == MouseButtons.Left && hit != null && hit != Document.Root && !PreviewMode && !ctrl)
            {
                _handle = HitHandle(ToScreen(hit), e.Location);
                _dragging = _handle != HandleKind.None;
                _dragStart = e.Location;
                _startBounds = hit.Bounds;
            }
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Delete)
            {
                if (DeletePickedRequested != null) DeletePickedRequested(this, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.H && (ModifierKeys & Keys.Control) != 0)
            {
                if (HidePickedRequested != null) HidePickedRequested(this, EventArgs.Empty);
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_dragging || Selected == null || Selected == Document.Root) return;
            int dx = e.X - _dragStart.X;
            int dy = e.Y - _dragStart.Y;
            var b = _startBounds;
            switch (_handle)
            {
                case HandleKind.Move: b.Offset(dx, dy); break;
                case HandleKind.E: b.Width = Math.Max(8, b.Width + dx); break;
                case HandleKind.W: b.X += dx; b.Width = Math.Max(8, b.Width - dx); break;
                case HandleKind.S: b.Height = Math.Max(8, b.Height + dy); break;
                case HandleKind.N: b.Y += dy; b.Height = Math.Max(8, b.Height - dy); break;
                case HandleKind.SE: b.Width = Math.Max(8, b.Width + dx); b.Height = Math.Max(8, b.Height + dy); break;
                case HandleKind.NW: b.X += dx; b.Y += dy; b.Width = Math.Max(8, b.Width - dx); b.Height = Math.Max(8, b.Height - dy); break;
                case HandleKind.NE: b.Y += dy; b.Width = Math.Max(8, b.Width + dx); b.Height = Math.Max(8, b.Height - dy); break;
                case HandleKind.SW: b.X += dx; b.Width = Math.Max(8, b.Width - dx); b.Height = Math.Max(8, b.Height + dy); break;
            }
            Selected.Bounds = b;
            Invalidate();
            if (SelectionChanged != null) SelectionChanged(this, EventArgs.Empty);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_dragging)
            {
                _dragging = false;
                if (DocumentEdited != null) DocumentEdited(this, EventArgs.Empty);
            }
        }
    }
}
