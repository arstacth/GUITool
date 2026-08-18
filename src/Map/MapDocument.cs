using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GUITools
{
    public class MapTri
    {
        public float X1, Y1, Z1, X2, Y2, Z2, X3, Y3, Z3;
        public int Flag;
    }

    public class MapProp
    {
        public string Texture;
        public float X, Y, Z;
        public float Width, Height;
        public float Fx, Fy;
    }

    public class MapDocument
    {
        public string Path;
        public int Version;
        public int ImportVersion;
        public string Magic = "";
        public List<string> TrackObjects = new List<string>();
        public List<string> Textures = new List<string>();
        public List<MapTri> Terrain = new List<MapTri>();
        public List<MapProp> Props = new List<MapProp>();
        public Image Overview;
        public string Folder;
        public float MinX, MaxX, MinY, MaxY, MinZ, MaxZ;

        public static MapDocument LoadTrv(string trvPath)
        {
            var doc = new MapDocument();
            doc.Path = trvPath;
            var data = File.ReadAllBytes(trvPath);
            if (data.Length < 0x40)
                throw new InvalidDataException("TRV too small");
            doc.Magic = Encoding.ASCII.GetString(data, 0, 16).TrimEnd('\0');
            if (doc.Magic.IndexOf("TRGameLevelFile", StringComparison.Ordinal) < 0)
                throw new InvalidDataException("Not a TRGameLevelFile");
            doc.Version = BitConverter.ToInt32(data, 0x34);
            doc.ImportVersion = BitConverter.ToInt32(data, 0x38);
            doc.Folder = System.IO.Path.GetDirectoryName(trvPath);
            string stem = System.IO.Path.GetFileNameWithoutExtension(trvPath);
            string sm = System.IO.Path.Combine(doc.Folder, stem + ".trv_sm");
            if (File.Exists(sm))
                LoadSm(doc, File.ReadAllBytes(sm));
            LoadProps(doc, data);
            ExpandBoundsFromProps(doc);
            PadBounds(doc);
            if (Directory.Exists(doc.Folder))
            {
                foreach (var f in Directory.GetFiles(doc.Folder))
                {
                    string ext = System.IO.Path.GetExtension(f).ToLowerInvariant();
                    if (ext == ".png" || ext == ".jpg" || ext == ".dds" || ext == ".tga")
                        doc.Textures.Add(System.IO.Path.GetFileName(f));
                }
                doc.Textures.Sort(StringComparer.OrdinalIgnoreCase);
            }
            string[] overviewNames = { "map.png", "map.dds", stem + ".png" };
            foreach (var n in overviewNames)
            {
                string p = System.IO.Path.Combine(doc.Folder, n);
                if (!File.Exists(p)) continue;
                try
                {
                    Image img = DdsImage.LoadFile(p);
                    var bmp = img as Bitmap;
                    if (bmp != null) DdsImage.ForceOpaque(bmp);
                    doc.Overview = img;
                    break;
                }
                catch { }
            }
            if (doc.Overview == null)
                BakeOverview(doc);
            return doc;
        }

        static void LoadSm(MapDocument doc, byte[] sm)
        {
            if (sm.Length < 28) return;
            int nObj = BitConverter.ToInt32(sm, 20);
            if (nObj < 0 || nObj > 4000) nObj = 0;
            int off = 24;
            for (int i = 0; i < nObj && off + 2 < sm.Length; i++)
            {
                int n = BitConverter.ToUInt16(sm, off);
                if (n < 1 || n > 64 || off + 2 + n + 36 > sm.Length) break;
                bool ok = true;
                for (int k = 0; k < n; k++)
                    if (sm[off + 2 + k] < 32 && sm[off + 2 + k] != 0) { ok = false; break; }
                if (!ok) break;
                try
                {
                    string name = Encoding.GetEncoding(949).GetString(sm, off + 2, n).TrimEnd('\0');
                    if (!string.IsNullOrEmpty(name) && !doc.TrackObjects.Contains(name))
                        doc.TrackObjects.Add(name);
                }
                catch { }
                off += 2 + n + 36;
            }
            if (off + 4 > sm.Length) return;
            int nTri = BitConverter.ToInt32(sm, off);
            off += 4;
            if (nTri < 1 || nTri > 400000) return;
            float minx = float.MaxValue, miny = float.MaxValue, minz = float.MaxValue;
            float maxx = float.MinValue, maxy = float.MinValue, maxz = float.MinValue;
            for (int i = 0; i < nTri; i++)
            {
                if (off + 148 > sm.Length) break;
                var t = new MapTri();
                t.Flag = BitConverter.ToInt32(sm, off);
                t.X1 = BitConverter.ToSingle(sm, off + 4);
                t.Y1 = BitConverter.ToSingle(sm, off + 8);
                t.Z1 = BitConverter.ToSingle(sm, off + 12);
                t.X2 = BitConverter.ToSingle(sm, off + 16);
                t.Y2 = BitConverter.ToSingle(sm, off + 20);
                t.Z2 = BitConverter.ToSingle(sm, off + 24);
                t.X3 = BitConverter.ToSingle(sm, off + 28);
                t.Y3 = BitConverter.ToSingle(sm, off + 32);
                t.Z3 = BitConverter.ToSingle(sm, off + 36);
                off += 148;
                if (!Finite(t.X1) || !Finite(t.Y1) || !Finite(t.Z1)) continue;
                if (!Finite(t.X2) || !Finite(t.Y2) || !Finite(t.Z2)) continue;
                if (!Finite(t.X3) || !Finite(t.Y3) || !Finite(t.Z3)) continue;
                if (TooFar(t.X1, t.Y1, t.Z1) || TooFar(t.X2, t.Y2, t.Z2) || TooFar(t.X3, t.Y3, t.Z3)) continue;
                doc.Terrain.Add(t);
                Acc(t.X1, t.Y1, t.Z1, ref minx, ref maxx, ref miny, ref maxy, ref minz, ref maxz);
                Acc(t.X2, t.Y2, t.Z2, ref minx, ref maxx, ref miny, ref maxy, ref minz, ref maxz);
                Acc(t.X3, t.Y3, t.Z3, ref minx, ref maxx, ref miny, ref maxy, ref minz, ref maxz);
            }
            if (doc.Terrain.Count == 0) return;
            doc.MinX = minx; doc.MaxX = maxx;
            doc.MinY = miny; doc.MaxY = maxy;
            doc.MinZ = minz; doc.MaxZ = maxz;
        }

        static bool Finite(float v)
        {
            return !(float.IsNaN(v) || float.IsInfinity(v));
        }

        static bool TooFar(float x, float y, float z)
        {
            return Math.Abs(x) > 200000f || Math.Abs(y) > 200000f || Math.Abs(z) > 80000f;
        }

        static void PadBounds(MapDocument doc)
        {
            if (doc.MaxX <= doc.MinX) return;
            float padX = Math.Max(400f, (doc.MaxX - doc.MinX) * 0.03f);
            float padY = Math.Max(400f, (doc.MaxY - doc.MinY) * 0.03f);
            doc.MinX -= padX; doc.MaxX += padX;
            doc.MinY -= padY; doc.MaxY += padY;
        }

        static void BakeOverview(MapDocument doc)
        {
            if (doc.Terrain == null || doc.Terrain.Count < 8) return;
            if (doc.MaxX <= doc.MinX || doc.MaxY <= doc.MinY) return;
            float minx = doc.MinX, maxx = doc.MaxX, miny = doc.MinY, maxy = doc.MaxY;
            float spanX = Math.Max(1f, maxx - minx);
            float spanY = Math.Max(1f, maxy - miny);
            int w = 768, h = 768;
            if (spanX >= spanY)
                h = Math.Max(256, (int)(768f * spanY / spanX));
            else
                w = Math.Max(256, (int)(768f * spanX / spanY));
            int[] pix = new int[w * h];
            float[] zb = new float[w * h];
            int sky = Color.FromArgb(118, 176, 220).ToArgb();
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = sky;
                zb[i] = -1e12f;
            }
            for (int ti = 0; ti < doc.Terrain.Count; ti++)
            {
                MapTri t = doc.Terrain[ti];
                float ax = t.X2 - t.X1, ay = t.Y2 - t.Y1, az = t.Z2 - t.Z1;
                float bx = t.X3 - t.X1, by = t.Y3 - t.Y1, bz = t.Z3 - t.Z1;
                float nx = ay * bz - az * by;
                float ny = az * bx - ax * bz;
                float nz = ax * by - ay * bx;
                float nl = (float)Math.Sqrt(nx * nx + ny * ny + nz * nz);
                if (nl < 0.01f) continue;
                float ndot = Math.Abs((nx * 0.2f + ny * -0.15f + nz * 0.97f) / nl);
                float light = 0.38f + 0.62f * ndot;
                float slope = Math.Abs(nz) / nl;
                float r, g, b;
                if (slope > 0.55f) { r = 86; g = 152; b = 74; }
                else if (slope > 0.25f) { r = 138; g = 126; b = 90; }
                else { r = 166; g = 148; b = 116; }
                int col = (255 << 24) | ((int)(r * light) << 16) | ((int)(g * light) << 8) | (int)(b * light);
                float x0 = (t.X1 - minx) / spanX * (w - 1);
                float y0 = (1f - (t.Y1 - miny) / spanY) * (h - 1);
                float x1 = (t.X2 - minx) / spanX * (w - 1);
                float y1 = (1f - (t.Y2 - miny) / spanY) * (h - 1);
                float x2 = (t.X3 - minx) / spanX * (w - 1);
                float y2 = (1f - (t.Y3 - miny) / spanY) * (h - 1);
                float z0 = t.Z1, z1 = t.Z2, z2 = t.Z3;
                int minPX = (int)Math.Min(x0, Math.Min(x1, x2));
                int maxPX = (int)Math.Max(x0, Math.Max(x1, x2));
                int minPY = (int)Math.Min(y0, Math.Min(y1, y2));
                int maxPY = (int)Math.Max(y0, Math.Max(y1, y2));
                if (maxPX < 0 || maxPY < 0 || minPX >= w || minPY >= h) continue;
                if (minPX < 0) minPX = 0; if (minPY < 0) minPY = 0;
                if (maxPX >= w) maxPX = w - 1; if (maxPY >= h) maxPY = h - 1;
                float area = (x1 - x0) * (y2 - y0) - (x2 - x0) * (y1 - y0);
                if (area > -0.01f && area < 0.01f) continue;
                float inv = 1f / area;
                for (int py = minPY; py <= maxPY; py++)
                {
                    for (int px = minPX; px <= maxPX; px++)
                    {
                        float w0 = ((x1 - px) * (y2 - py) - (x2 - px) * (y1 - py)) * inv;
                        float w1 = ((x2 - px) * (y0 - py) - (x0 - px) * (y2 - py)) * inv;
                        float w2 = 1f - w0 - w1;
                        if (w0 < -0.01f || w1 < -0.01f || w2 < -0.01f) continue;
                        float z = z0 * w0 + z1 * w1 + z2 * w2;
                        int idx = py * w + px;
                        if (z < zb[idx]) continue;
                        zb[idx] = z;
                        pix[idx] = col;
                    }
                }
            }
            try
            {
                var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                var rect = new Rectangle(0, 0, w, h);
                var bits = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                Marshal.Copy(pix, 0, bits.Scan0, pix.Length);
                bmp.UnlockBits(bits);
                doc.Overview = bmp;
            }
            catch { }
        }

        static void ExpandBoundsFromProps(MapDocument doc)
        {
            if (doc.Props == null || doc.Props.Count == 0) return;
            bool empty = doc.MaxX <= doc.MinX;
            float minx = empty ? float.MaxValue : doc.MinX;
            float maxx = empty ? float.MinValue : doc.MaxX;
            float miny = empty ? float.MaxValue : doc.MinY;
            float maxy = empty ? float.MinValue : doc.MaxY;
            float minz = empty ? float.MaxValue : doc.MinZ;
            float maxz = empty ? float.MinValue : doc.MaxZ;
            for (int i = 0; i < doc.Props.Count; i++)
            {
                var p = doc.Props[i];
                Acc(p.X, p.Y, p.Z, ref minx, ref maxx, ref miny, ref maxy, ref minz, ref maxz);
            }
            if (minx < float.MaxValue)
            {
                doc.MinX = minx; doc.MaxX = maxx;
                doc.MinY = miny; doc.MaxY = maxy;
                doc.MinZ = minz; doc.MaxZ = maxz;
            }
        }

        static void Acc(float x, float y, float z,
            ref float minx, ref float maxx, ref float miny, ref float maxy, ref float minz, ref float maxz)
        {
            if (x < minx) minx = x; if (x > maxx) maxx = x;
            if (y < miny) miny = y; if (y > maxy) maxy = y;
            if (z < minz) minz = z; if (z > maxz) maxz = z;
        }

        static void LoadProps(MapDocument doc, byte[] trv)
        {
            var names = new List<string>();
            Encoding enc;
            try { enc = Encoding.GetEncoding(949); }
            catch { enc = Encoding.Default; }
            int stop = IndexOf(trv, enc.GetBytes("terrain_staticobject.mat"), 0);
            if (stop < 0) stop = trv.Length;
            int i = 0x48000;
            if (i >= stop) i = 0;
            while (i + 8 < stop)
            {
                int n = BitConverter.ToUInt16(trv, i);
                if (n >= 6 && n <= 40 && i + 2 + n <= trv.Length)
                {
                    if (trv[i + n - 2] == (byte)'.' && trv[i + n - 1] == (byte)'p'
                        && trv[i + n] == (byte)'n' && trv[i + n + 1] == (byte)'g')
                    {
                        bool ok = true;
                        for (int k = 0; k < n; k++)
                            if (trv[i + 2 + k] == 0) { ok = false; break; }
                        if (ok)
                        {
                            names.Add(enc.GetString(trv, i + 2, n));
                            i += 2 + n;
                            continue;
                        }
                    }
                }
                i++;
            }
            if (names.Count == 0) return;
            CollectMatInstances(doc, trv, names, "terrain_staticobject.mat");
            CollectMatInstances(doc, trv, names, "waving.mat");
        }

        static void CollectMatInstances(MapDocument doc, byte[] trv, List<string> names, string mat)
        {
            Encoding enc;
            try { enc = Encoding.GetEncoding(949); }
            catch { enc = Encoding.Default; }
            byte[] sig = new byte[2 + mat.Length];
            BitConverter.GetBytes((ushort)mat.Length).CopyTo(sig, 0);
            enc.GetBytes(mat).CopyTo(sig, 2);
            int off = 0;
            while (true)
            {
                int hit = IndexOf(trv, sig, off);
                if (hit < 0) break;
                int o = hit;
                int n1 = BitConverter.ToUInt16(trv, o);
                o += 2 + n1;
                if (o + 2 >= trv.Length) break;
                int n2 = BitConverter.ToUInt16(trv, o);
                if (n2 < 1 || n2 > 80 || o + 2 + n2 >= trv.Length) { off = hit + 1; continue; }
                o += 2 + n2;
                int p = o;
                while (p < o + 40 && p < trv.Length && trv[p] == 0) p++;
                if (p + 64 >= trv.Length) { off = hit + 1; continue; }
                int tid = BitConverter.ToInt32(trv, p);
                float x = BitConverter.ToSingle(trv, p + 4);
                float y = BitConverter.ToSingle(trv, p + 8);
                float z = BitConverter.ToSingle(trv, p + 12);
                float fx = BitConverter.ToSingle(trv, p + 16);
                float fy = BitConverter.ToSingle(trv, p + 20);
                float w = BitConverter.ToSingle(trv, p + 16 + 40);
                float h = BitConverter.ToSingle(trv, p + 16 + 44);
                off = hit + 1;
                if (tid < 0 || tid >= names.Count) continue;
                if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z)) continue;
                if (Math.Abs(x) > 80000 || Math.Abs(y) > 80000 || Math.Abs(z) > 20000) continue;
                if (!(w > 10 && w < 8000 && h > 10 && h < 8000))
                {
                    w = 400; h = 400;
                }
                var prop = new MapProp();
                prop.Texture = names[tid];
                prop.X = x; prop.Y = y; prop.Z = z;
                prop.Width = w; prop.Height = h;
                prop.Fx = fx; prop.Fy = fy;
                doc.Props.Add(prop);
            }
        }

        static int IndexOf(byte[] data, byte[] needle, int start)
        {
            for (int i = start; i <= data.Length - needle.Length; i++)
            {
                bool ok = true;
                for (int k = 0; k < needle.Length; k++)
                    if (data[i + k] != needle[k]) { ok = false; break; }
                if (ok) return i;
            }
            return -1;
        }
    }
}
