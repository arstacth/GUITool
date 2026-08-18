using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GUITools
{
    /// <summary>
    /// In-game style map: opaque overview terrain, collision height, and placed props.
    /// </summary>
    [ToolboxItem(false)]
    [Description("TalesRunner map viewport")]
    public class MapView : UserControl
    {
        MapDocument _doc;
        int[] _ovPix;
        int _ovW, _ovH;
        readonly Dictionary<string, PropTex> _propTex = new Dictionary<string, PropTex>(StringComparer.OrdinalIgnoreCase);
        Bitmap _frame;
        int[] _pixels;
        float[] _zbuf;
        int _fw, _fh;
        float _yaw = 0.42f;
        float _pitch = 1.05f;
        float _zoom = 1.05f;
        float _panX, _panY;
        bool _orbit, _pan;
        Point _last;
        bool _dirty = true;

        public MapView()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(118, 176, 220);
            TabStop = true;
        }

        public void SetDocument(MapDocument doc)
        {
            _doc = doc;
            _yaw = 0.18f;
            _pitch = 1.22f;
            _zoom = 1f;
            _panX = 0;
            _panY = 0;
            _ovPix = null; _ovW = _ovH = 0;
            _propTex.Clear();
            if (doc != null && doc.Overview != null)
            {
                try
                {
                    var bmp = new Bitmap(doc.Overview);
                    DdsImage.ForceOpaque(bmp);
                    _ovW = bmp.Width; _ovH = bmp.Height;
                    _ovPix = new int[_ovW * _ovH];
                    var rect = new Rectangle(0, 0, _ovW, _ovH);
                    var bits = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    Marshal.Copy(bits.Scan0, _ovPix, 0, _ovPix.Length);
                    bmp.UnlockBits(bits);
                    bmp.Dispose();
                }
                catch { _ovPix = null; }
            }
            _dirty = true;
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _dirty = true;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            _last = e.Location;
            if (e.Button == MouseButtons.Left) _orbit = true;
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle) _pan = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _orbit = false;
            _pan = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_orbit)
            {
                _yaw += (e.X - _last.X) * 0.008f;
                _pitch += (e.Y - _last.Y) * 0.006f;
                if (_pitch < 0.35f) _pitch = 0.35f;
                if (_pitch > 1.45f) _pitch = 1.45f;
                _last = e.Location;
                _dirty = true;
                Invalidate();
            }
            else if (_pan)
            {
                _panX += e.X - _last.X;
                _panY += e.Y - _last.Y;
                _last = e.Location;
                _dirty = true;
                Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0) _zoom *= 1.12f; else _zoom /= 1.12f;
            if (_zoom < 0.2f) _zoom = 0.2f;
            if (_zoom > 5f) _zoom = 5f;
            _dirty = true;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_dirty) Render();
            e.Graphics.Clear(BackColor);
            if (_frame != null)
                e.Graphics.DrawImageUnscaled(_frame, 0, 0);
            string hint = "Open a .trv map";
            if (_doc != null)
                hint = Path.GetFileName(_doc.Path) + "   TRV v" + _doc.Version
                    + " / import " + _doc.ImportVersion + "   "
                    + _doc.Terrain.Count + " tris   " + _doc.Props.Count + " props   "
                    + (_ovPix != null ? (_ovW + "x" + _ovH + " map") : "no overview")
                    + "   ·  drag orbit  ·  right-drag pan  ·  wheel zoom";
            using (var bg = new SolidBrush(Color.FromArgb(170, 0, 0, 0)))
                e.Graphics.FillRectangle(bg, 6, 6, Math.Min(Width - 12, 900), 22);
            TextRenderer.DrawText(e.Graphics, hint, Font, new Point(10, 8), Color.White);
        }

        void Render()
        {
            _dirty = false;
            int w = Math.Max(32, Width);
            int h = Math.Max(32, Height);
            if (_frame == null || _fw != w || _fh != h)
            {
                if (_frame != null) _frame.Dispose();
                _frame = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                _pixels = new int[w * h];
                _zbuf = new float[w * h];
                _fw = w; _fh = h;
            }
            int sky = Color.FromArgb(118, 176, 220).ToArgb();
            for (int i = 0; i < _pixels.Length; i++)
            {
                _pixels[i] = sky;
                _zbuf[i] = 1e12f;
            }
            if (_doc == null) { Blit(); return; }

            float minx = _doc.MinX, maxx = _doc.MaxX, miny = _doc.MinY, maxy = _doc.MaxY, minz = _doc.MinZ, maxz = _doc.MaxZ;
            if (maxx <= minx) { minx = -18000; maxx = 18000; miny = -14000; maxy = 12000; minz = 0; maxz = 800; }
            float cx = (minx + maxx) * 0.5f;
            float cy = (miny + maxy) * 0.5f;
            float cz = minz;
            float spanX = Math.Max(1f, maxx - minx);
            float spanY = Math.Max(1f, maxy - miny);
            float spanZ = Math.Max(400f, maxz - minz);
            float cyaw = (float)Math.Cos(_yaw);
            float syaw = (float)Math.Sin(_yaw);
            float cp = (float)Math.Cos(_pitch);
            float sp = (float)Math.Sin(_pitch);
            float vis = Math.Max(spanX, spanY * Math.Abs(cp) + spanZ * Math.Abs(sp));
            float dist = vis * 1.22f / _zoom;
            float focal = Math.Min(w, h) * 0.68f * _zoom;

            DrawQuad(minx, miny, maxx, maxy, minz, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h);
            DrawTerrain(minx, miny, spanX, spanY, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h);
            DrawProps(minz, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h);
            Blit();
        }

        void DrawQuad(float minx, float miny, float maxx, float maxy, float z,
            float cx, float cy, float cz, float cyaw, float syaw, float cp, float sp,
            float dist, float focal, int w, int h)
        {
            float s0, t0, d0, s1, t1, d1, s2, t2, d2, s3, t3, d3;
            if (!Project(minx, miny, z, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s0, out t0, out d0)) return;
            if (!Project(maxx, miny, z, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s1, out t1, out d1)) return;
            if (!Project(maxx, maxy, z, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s2, out t2, out d2)) return;
            if (!Project(minx, maxy, z, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s3, out t3, out d3)) return;
            int[] tex = _ovPix;
            int tw = _ovW, th = _ovH;
            if (tex == null)
            {
                tex = new[] { Color.FromArgb(72, 128, 58).ToArgb() };
                tw = th = 1;
            }
            FillUv(s0, t0, d0, 0, 1, s1, t1, d1, 1, 1, s2, t2, d2, 1, 0, tex, tw, th, false, w, h);
            FillUv(s0, t0, d0, 0, 1, s2, t2, d2, 1, 0, s3, t3, d3, 0, 0, tex, tw, th, false, w, h);
        }

        void DrawTerrain(float minx, float miny, float spanX, float spanY,
            float cx, float cy, float cz, float cyaw, float syaw, float cp, float sp,
            float dist, float focal, int w, int h)
        {
            if (_doc.Terrain == null || _doc.Terrain.Count == 0) return;
            int n = _doc.Terrain.Count;
            int[] tex = _ovPix;
            int tw = _ovW, th = _ovH;
            int[] lit = new int[1];
            for (int i = 0; i < n; i++)
            {
                var t = _doc.Terrain[i];
                float ax = t.X2 - t.X1, ay = t.Y2 - t.Y1, az = t.Z2 - t.Z1;
                float bx = t.X3 - t.X1, by = t.Y3 - t.Y1, bz = t.Z3 - t.Z1;
                float nx = ay * bz - az * by;
                float ny = az * bx - ax * bz;
                float nz = ax * by - ay * bx;
                float nl = (float)Math.Sqrt(nx * nx + ny * ny + nz * nz);
                if (nl < 0.01f) continue;
                float s0, t0, d0, s1, t1, d1, s2, t2, d2;
                if (!Project(t.X1, t.Y1, t.Z1, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s0, out t0, out d0)) continue;
                if (!Project(t.X2, t.Y2, t.Z2, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s1, out t1, out d1)) continue;
                if (!Project(t.X3, t.Y3, t.Z3, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s2, out t2, out d2)) continue;
                float minSX = Math.Min(s0, Math.Min(s1, s2));
                float maxSX = Math.Max(s0, Math.Max(s1, s2));
                float minSY = Math.Min(t0, Math.Min(t1, t2));
                float maxSY = Math.Max(t0, Math.Max(t1, t2));
                if (maxSX < 0 || maxSY < 0 || minSX >= w || minSY >= h) continue;
                if (maxSX - minSX < 1.5f && maxSY - minSY < 1.5f) continue;

                float ndot = Math.Abs((nx * 0.35f + ny * -0.25f + nz * 0.9f) / nl);
                float light = 0.32f + 0.68f * ndot;
                float slope = Math.Abs(nz) / nl;
                float r, g, b;
                if (slope > 0.55f) { r = 88; g = 148; b = 72; }
                else if (slope > 0.25f) { r = 140; g = 128; b = 92; }
                else { r = 168; g = 150; b = 118; }
                int col = (255 << 24) | ((int)(r * light) << 16) | ((int)(g * light) << 8) | (int)(b * light);
                if (tex != null)
                {
                    float u0 = (t.X1 - minx) / spanX, v0 = (t.Y1 - miny) / spanY;
                    float u1 = (t.X2 - minx) / spanX, v1 = (t.Y2 - miny) / spanY;
                    float u2 = (t.X3 - minx) / spanX, v2 = (t.Y3 - miny) / spanY;
                    FillUv(s0, t0, d0, u0, 1 - v0, s1, t1, d1, u1, 1 - v1, s2, t2, d2, u2, 1 - v2, tex, tw, th, false, w, h);
                }
                else
                {
                    lit[0] = col;
                    FillUv(s0, t0, d0, 0, 0, s1, t1, d1, 0, 0, s2, t2, d2, 0, 0, lit, 1, 1, false, w, h);
                }
            }
        }

        void DrawProps(float groundZ, float cx, float cy, float cz, float cyaw, float syaw, float cp, float sp,
            float dist, float focal, int w, int h)
        {
            if (_doc.Props == null || _doc.Props.Count == 0) return;
            EnsurePropTextures();
            var list = new List<MapProp>();
            for (int i = 0; i < _doc.Props.Count; i++)
            {
                var p = _doc.Props[i];
                string n = p.Texture ?? "";
                if (n.IndexOf("풀") >= 0 && p.Height < 280) continue;
                list.Add(p);
            }
            list.Sort(delegate(MapProp a, MapProp b)
            {
                float da = a.X * syaw + a.Y * cyaw;
                float db = b.X * syaw + b.Y * cyaw;
                return da.CompareTo(db);
            });
            int cap = Math.Min(list.Count, 420);
            for (int i = 0; i < cap; i++)
            {
                var p = list[i];
                PropTex tex;
                if (!_propTex.TryGetValue(p.Texture, out tex) || tex == null) continue;
                float fx = p.Fx, fy = p.Fy;
                float fl = (float)Math.Sqrt(fx * fx + fy * fy);
                if (fl < 0.001f) { fx = 1; fy = 0; fl = 1; }
                fx /= fl; fy /= fl;
                float hw = Math.Min(p.Width > 10 ? p.Width : 600f, 2400f) * 0.45f;
                float hh = Math.Min(p.Height > 10 ? p.Height : 600f, 2000f) * 0.72f;
                float rx = -fy * hw, ry = fx * hw;
                float z = p.Z;
                if (z < groundZ - 200 || z > groundZ + 2500) z = groundZ;
                float x0 = p.X - rx, y0 = p.Y - ry;
                float x1 = p.X + rx, y1 = p.Y + ry;
                float s0, t0, d0, s1, t1, d1, s2, t2, d2, s3, t3, d3;
                if (!Project(x0, y0, z, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s0, out t0, out d0)) continue;
                if (!Project(x1, y1, z, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s1, out t1, out d1)) continue;
                if (!Project(x1, y1, z + hh, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s2, out t2, out d2)) continue;
                if (!Project(x0, y0, z + hh, cx, cy, cz, cyaw, syaw, cp, sp, dist, focal, w, h, out s3, out t3, out d3)) continue;
                FillUv(s0, t0, d0, 0, 1, s1, t1, d1, 1, 1, s2, t2, d2, 1, 0, tex.Pix, tex.W, tex.H, true, w, h);
                FillUv(s0, t0, d0, 0, 1, s2, t2, d2, 1, 0, s3, t3, d3, 0, 0, tex.Pix, tex.W, tex.H, true, w, h);
            }
        }

        void EnsurePropTextures()
        {
            if (_propTex.Count > 0 || _doc == null) return;
            for (int i = 0; i < _doc.Props.Count; i++)
            {
                string name = _doc.Props[i].Texture;
                if (string.IsNullOrEmpty(name) || _propTex.ContainsKey(name)) continue;
                string path = Path.Combine(_doc.Folder ?? "", name);
                if (!File.Exists(path)) { _propTex[name] = null; continue; }
                try
                {
                    using (var img = DdsImage.LoadFile(path))
                    {
                        var bmp = new Bitmap(img);
                        DdsImage.PunchDarkTransparent(bmp, 8);
                        var pt = new PropTex();
                        pt.W = bmp.Width; pt.H = bmp.Height;
                        pt.Pix = new int[pt.W * pt.H];
                        var rect = new Rectangle(0, 0, pt.W, pt.H);
                        var bits = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                        Marshal.Copy(bits.Scan0, pt.Pix, 0, pt.Pix.Length);
                        bmp.UnlockBits(bits);
                        bmp.Dispose();
                        _propTex[name] = pt;
                    }
                }
                catch { _propTex[name] = null; }
            }
        }

        bool Project(float x, float y, float z, float cx, float cy, float cz,
            float cyaw, float syaw, float cp, float sp, float dist, float focal, int w, int h,
            out float sx, out float sy, out float depth)
        {
            float dx = x - cx, dy = y - cy, dz = z - cz;
            float rx = dx * cyaw - dy * syaw;
            float ry = dx * syaw + dy * cyaw;
            float ry2 = ry * cp - dz * sp;
            float rz2 = ry * sp + dz * cp;
            depth = ry2 + dist;
            sx = sy = 0;
            if (depth < 8f) return false;
            float f = focal / depth;
            sx = w * 0.5f + rx * f + _panX;
            sy = h * 0.56f - rz2 * f + _panY;
            return true;
        }

        void FillUv(float x0, float y0, float z0, float u0, float v0,
            float x1, float y1, float z1, float u1, float v1,
            float x2, float y2, float z2, float u2, float v2,
            int[] tex, int tw, int th, bool alpha, int w, int h)
        {
            int minX = (int)Math.Min(x0, Math.Min(x1, x2));
            int maxX = (int)Math.Max(x0, Math.Max(x1, x2));
            int minY = (int)Math.Min(y0, Math.Min(y1, y2));
            int maxY = (int)Math.Max(y0, Math.Max(y1, y2));
            if (maxX < 0 || maxY < 0 || minX >= w || minY >= h) return;
            if (minX < 0) minX = 0; if (minY < 0) minY = 0;
            if (maxX >= w) maxX = w - 1; if (maxY >= h) maxY = h - 1;
            float area = (x1 - x0) * (y2 - y0) - (x2 - x0) * (y1 - y0);
            if (area > -0.5f && area < 0.5f) return;
            float inv = 1f / area;
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float w0 = ((x1 - x) * (y2 - y) - (x2 - x) * (y1 - y)) * inv;
                    float w1 = ((x2 - x) * (y0 - y) - (x0 - x) * (y2 - y)) * inv;
                    float w2 = 1f - w0 - w1;
                    if (w0 < 0 || w1 < 0 || w2 < 0)
                    {
                        if (w0 > 0 || w1 > 0 || w2 > 0) continue;
                        w0 = -w0; w1 = -w1; w2 = -w2;
                    }
                    float d = z0 * w0 + z1 * w1 + z2 * w2;
                    int i = y * w + x;
                    if (d >= _zbuf[i]) continue;
                    float u = u0 * w0 + u1 * w1 + u2 * w2;
                    float v = v0 * w0 + v1 * w1 + v2 * w2;
                    if (u < 0) u = 0; if (u > 1) u = 1;
                    if (v < 0) v = 0; if (v > 1) v = 1;
                    int px = (int)(u * (tw - 1));
                    int py = (int)(v * (th - 1));
                    int col = tex[py * tw + px];
                    if (alpha && ((col >> 24) & 255) < 20) continue;
                    _zbuf[i] = d;
                    _pixels[i] = col | unchecked((int)0xFF000000);
                }
            }
        }

        void Blit()
        {
            var rect = new Rectangle(0, 0, _fw, _fh);
            var bits = _frame.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(_pixels, 0, bits.Scan0, _pixels.Length);
            _frame.UnlockBits(bits);
        }

        class PropTex
        {
            public int[] Pix;
            public int W, H;
        }
    }
}
