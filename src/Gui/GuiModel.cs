using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace GUITools
{
    public class GuiControl
    {
        public string CtrlType = "Button";
        public string Name = "";
        public int Ident;
        public int ParentId = -1;
        public int Flags = 0x40;
        public int X, Y, W = 80, H = 24;
        public bool Visible = true;
        public bool Enabled = true;
        public string Description = "";
        public string Command = "";
        public string Text = "";
        public string ImageFile = "";
        public int[] SourceRect;
        public string AspectRatio = "none";
        public Dictionary<string, object> Properties = new Dictionary<string, object>(StringComparer.Ordinal);
        public List<GuiControl> Children = new List<GuiControl>();

        public string Label
        {
            get { return string.IsNullOrEmpty(Name) ? CtrlType : Name; }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle(X, Y, Math.Max(W, 4), Math.Max(H, 4)); }
            set { X = value.X; Y = value.Y; W = value.Width; H = value.Height; }
        }

        public string GetPropString(string key)
        {
            object v;
            if (Properties != null && Properties.TryGetValue(key, out v) && v != null)
                return v.ToString();
            return "";
        }

        public int[] GetPropRect(string key)
        {
            object v;
            if (Properties != null && Properties.TryGetValue(key, out v))
            {
                var arr = v as int[];
                if (arr != null && arr.Length >= 4) return arr;
            }
            return null;
        }

        public string ResolveImageFile()
        {
            if (!string.IsNullOrEmpty(ImageFile)) return ImageFile;
            string[] keys = {
                "ImageFile", "FrameNormal.ImageFile", "backround.ImageFile",
                "button.ImageFile", "ImageButton.ImageNormal.ImageFile", "Frame.ImageFile"
            };
            foreach (var k in keys)
            {
                string s = GetPropString(k);
                if (!string.IsNullOrEmpty(s) && s != "True") return s;
            }
            if (Properties != null)
            {
                foreach (var kv in Properties)
                {
                    if (kv.Key.EndsWith("ImageFile", StringComparison.OrdinalIgnoreCase) && kv.Value is string)
                    {
                        string s = (string)kv.Value;
                        if (!string.IsNullOrEmpty(s)) return s;
                    }
                }
            }
            return "";
        }

        public int[] ResolveSourceRect()
        {
            if (SourceRect != null && SourceRect.Length >= 4) return SourceRect;
            string[] keys = {
                "ImageNormal.SourceRect", "FrameNormal.SourceRect", "SourceRect",
                "backround.SourceRect", "ImageButton.ImageNormal.SourceRect", "button.SourceRect"
            };
            foreach (var k in keys)
            {
                var r = GetPropRect(k);
                if (r != null) return r;
            }
            if (Properties != null)
            {
                foreach (var kv in Properties)
                {
                    if (kv.Key.EndsWith("SourceRect", StringComparison.OrdinalIgnoreCase))
                    {
                        var r = kv.Value as int[];
                        if (r != null && r.Length >= 4) return r;
                    }
                }
            }
            return null;
        }

        public GuiControl Clone()
        {
            var c = new GuiControl();
            c.CtrlType = CtrlType;
            c.Name = Name;
            c.Ident = Ident;
            c.ParentId = ParentId;
            c.Flags = Flags;
            c.X = X; c.Y = Y; c.W = W; c.H = H;
            c.Visible = Visible;
            c.Enabled = Enabled;
            c.Description = Description;
            c.Command = Command;
            c.Text = Text;
            c.ImageFile = ImageFile;
            c.SourceRect = SourceRect;
            c.AspectRatio = AspectRatio;
            foreach (var kv in Properties) c.Properties[kv.Key] = kv.Value;
            foreach (var ch in Children) c.Children.Add(ch.Clone());
            return c;
        }
    }

    public class GuiDocument
    {
        public int Version = 4;
        public GuiControl Root;
        public List<GuiControl> Templates = new List<GuiControl>();
        public string SourcePath = "";

        public IEnumerable<GuiControl> AllControls()
        {
            if (Root != null)
                foreach (var c in Walk(Root)) yield return c;
            foreach (var t in Templates)
                foreach (var c in Walk(t)) yield return c;
        }

        public static IEnumerable<GuiControl> Walk(GuiControl c)
        {
            yield return c;
            foreach (var ch in c.Children)
                foreach (var x in Walk(ch)) yield return x;
        }

        public static GuiDocument NewEmpty()
        {
            var doc = new GuiDocument();
            doc.Root = new GuiControl { CtrlType = "Dialog", Name = "Dialog", W = 800, H = 600, Ident = 1 };
            return doc;
        }
    }

    public static class ControlCatalog
    {
        public static readonly string[] Toolbox = {
            "Image Button", "Frame Button", "Static Text", "Edit",
            "Listbox", "Drop list combo", "Drop down combo", "Slider",
            "Image Control", "Frame", "Scrollbar", "User control",
            "Formatted Text", "Item image Control"
        };

        public static readonly string[] TreeGroups = {
            "Dialog settings", "Button", "Static Text", "Edit", "Listbox",
            "Drop list combo", "Slider", "Frame", "Image control",
            "Scrollbar", "User control", "Template"
        };

        public static string ToolboxToType(string label)
        {
            switch (label)
            {
                case "Image Button": return "ImageButton";
                case "Frame Button": return "FrameButton";
                case "Static Text": return "StaticText";
                case "Edit": return "EditCtrl";
                case "Listbox": return "ListBox";
                case "Drop list combo": return "ComboBoxDropList";
                case "Drop down combo": return "ComboBoxDropDown";
                case "Slider": return "Slider";
                case "Image Control": return "ImageCtrl";
                case "Frame": return "Frame";
                case "Scrollbar": return "Scrollbar";
                case "User control": return "UserCtrl";
                case "Formatted Text": return "FormatedStaticText";
                case "Item image Control": return "ItemImageCtrl";
                default: return "Button";
            }
        }

        public static string GroupOf(string ctrlType)
        {
            switch (ctrlType)
            {
                case "Button":
                case "ImageButton":
                case "FrameButton":
                case "RadioButton":
                case "CheckButton": return "Button";
                case "StaticText":
                case "FormatedStaticText": return "Static Text";
                case "EditCtrl": return "Edit";
                case "ListBox": return "Listbox";
                case "ComboBox":
                case "ComboBoxDropList":
                case "ComboBoxDropDown": return "Drop list combo";
                case "Slider": return "Slider";
                case "Frame": return "Frame";
                case "ImageCtrl":
                case "Image":
                case "ItemImageCtrl": return "Image control";
                case "Scrollbar": return "Scrollbar";
                case "UserCtrl": return "User control";
                case "Dialog":
                case "MessageWnd": return "Dialog settings";
                default: return "Button";
            }
        }

        public static Color FillOf(string ctrlType)
        {
            switch (ctrlType)
            {
                case "Dialog": return Color.FromArgb(82, 84, 88);
                case "FrameButton":
                case "ImageButton":
                case "Button": return Color.FromArgb(36, 92, 186);
                case "StaticText":
                case "FormatedStaticText": return Color.FromArgb(70, 70, 74);
                case "EditCtrl": return Color.FromArgb(245, 245, 245);
                case "ListBox": return Color.FromArgb(48, 50, 54);
                case "Slider": return Color.FromArgb(90, 70, 130);
                case "ImageCtrl":
                case "Image":
                case "ItemImageCtrl": return Color.FromArgb(40, 120, 110);
                default: return Color.FromArgb(60, 62, 66);
            }
        }

        public static bool IsBackground(string ctrlType)
        {
            return ctrlType == "Image" || ctrlType == "ImageCtrl" || ctrlType == "ItemImageCtrl"
                || ctrlType == "Dialog" || ctrlType == "Frame";
        }
    }

    public static class GuiFormat
    {
        static readonly HashSet<string> Types = new HashSet<string>(StringComparer.Ordinal) {
            "FormatedStaticText","FrameButton","ImageButton","RadioButton","CheckButton",
            "MessageWnd","ImageCtrl","StaticText","EditCtrl","ComboBox","ComboBoxDropList",
            "ComboBoxDropDown","Scrollbar","ListBox","UserCtrl","ImageRect","Dialog",
            "Button","Slider","Frame","Image","ItemImageCtrl"
        };

        static readonly HashSet<string> KnownKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "ImageFile","SourceRect","Text","Command","Name","Visible","Enable","Enabled",
            "BkColor","FgColor","Font","FontName","FontSize","HAlign","VAlign","ToolTip",
            "MaxNum","CharWidth","CharSpacing","CursorColor","HasDialogFrame","NoShadowRect",
            "ShadowRect","Align"
        };

        const int HeaderSize = 0x50;

        public static GuiDocument Load(string path)
        {
            if (path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                return LoadJson(path);
            return LoadGui(path);
        }

        public static GuiDocument LoadGui(string path)
        {
            var data = File.ReadAllBytes(path);
            if (data.Length < HeaderSize || Encoding.ASCII.GetString(data, 0, 13) != "TRGUIDescFile")
                throw new InvalidDataException("Not a TRGUIDescFile");
            var version = BitConverter.ToInt32(data, 0x34);
            var hits = new List<KeyValuePair<int, string>>();
            int off = HeaderSize;
            while (off + 4 < data.Length)
            {
                string s;
                int next;
                if (!TryPString(data, off, out s, out next)) { off++; continue; }
                if (Types.Contains(s))
                {
                    hits.Add(new KeyValuePair<int, string>(off, s));
                    off = next;
                    string n2; int n2off;
                    if (TryPString(data, off, out n2, out n2off)) off = n2off;
                }
                else off++;
            }

            var controls = new List<GuiControl>();
            for (int i = 0; i < hits.Count; i++)
            {
                int pos = hits[i].Key;
                string typ = hits[i].Value;
                int n = BitConverter.ToUInt16(data, pos);
                int after = pos + 2 + n;
                string name = typ;
                string n2; int n2off;
                if (TryPString(data, after, out n2, out n2off))
                {
                    name = n2;
                    after = n2off;
                }
                int stop = (i + 1 < hits.Count) ? hits[i + 1].Key : data.Length;
                int ident, flags, x, y, w, h;
                FindRect(data, after, Math.Min(96, stop - after), out ident, out flags, out x, out y, out w, out h);
                int parentId = -1;
                if (pos >= 4) parentId = BitConverter.ToInt32(data, pos - 4);
                var c = new GuiControl {
                    CtrlType = typ, Name = name, Ident = ident, Flags = flags,
                    X = x, Y = y, W = w, H = h, ParentId = parentId
                };
                ReadProps(data, after, stop, c);
                if (string.IsNullOrEmpty(c.Text) && !string.Equals(c.Name, c.CtrlType, StringComparison.Ordinal))
                    c.Text = "";
                controls.Add(c);
            }

            var doc = new GuiDocument { Version = version, SourcePath = path };
            if (controls.Count == 0)
            {
                doc.Root = new GuiControl { CtrlType = "Dialog", Name = "Dialog", W = 800, H = 600, Ident = 1 };
                return doc;
            }
            BuildTree(doc, controls);
            return doc;
        }

        static void BuildTree(GuiDocument doc, List<GuiControl> controls)
        {
            var byId = new Dictionary<int, GuiControl>();
            foreach (var c in controls)
            {
                if (c.Ident != 0 && !byId.ContainsKey(c.Ident))
                    byId[c.Ident] = c;
            }
            GuiControl root = controls[0];
            bool linked = false;
            for (int i = 1; i < controls.Count; i++)
            {
                var c = controls[i];
                if (c.CtrlType == "Image" && (c.Name ?? "").IndexOf("Template", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    doc.Templates.Add(c);
                    continue;
                }
                GuiControl parent;
                if (c.ParentId > 0 && byId.TryGetValue(c.ParentId, out parent) && parent != c)
                {
                    parent.Children.Add(c);
                    linked = true;
                }
                else
                    root.Children.Add(c);
            }
            if (!linked)
            {
                foreach (var c in controls)
                    c.Children.Clear();
                root.Children.Clear();
                doc.Templates.Clear();
                for (int i = 1; i < controls.Count; i++)
                {
                    var c = controls[i];
                    if (c.CtrlType == "Image" && (c.Name ?? "").IndexOf("Template", StringComparison.OrdinalIgnoreCase) >= 0)
                        doc.Templates.Add(c);
                    else
                        root.Children.Add(c);
                }
            }
            doc.Root = root;
        }

        static bool TryPString(byte[] data, int off, out string s, out int next)
        {
            return TryPStringEx(data, off, false, out s, out next);
        }

        static bool TryPStringValue(byte[] data, int off, out string s, out int next)
        {
            return TryPStringEx(data, off, true, out s, out next);
        }

        static bool TryPStringEx(byte[] data, int off, bool value, out string s, out int next)
        {
            s = null; next = off;
            if (off < 0 || off + 2 > data.Length) return false;
            int n = BitConverter.ToUInt16(data, off);
            int max = value ? 260 : 240;
            if (n < 1 || n > max || off + 2 + n > data.Length) return false;
            for (int i = 0; i < n; i++) if (data[off + 2 + i] == 0) return false;
            try { s = Encoding.GetEncoding(949).GetString(data, off + 2, n); }
            catch { s = Encoding.Default.GetString(data, off + 2, n); }
            if (string.IsNullOrEmpty(s)) return false;
            if (!value && !char.IsLetter(s[0])) return false;
            next = off + 2 + n;
            return true;
        }

        static void FindRect(byte[] data, int start, int limit, out int ident, out int flags, out int x, out int y, out int w, out int h)
        {
            ident = 0; flags = 0x40; x = 0; y = 0; w = 80; h = 24;
            int end = Math.Min(data.Length - 16, start + Math.Max(limit, 16));
            int bestScore = -1;
            for (int off = start; off < end; off++)
            {
                if (off + 16 > data.Length) break;
                int a = BitConverter.ToInt32(data, off);
                int b = BitConverter.ToInt32(data, off + 4);
                int c = BitConverter.ToInt32(data, off + 8);
                int d = BitConverter.ToInt32(data, off + 12);
                int rx, ry, rw, rh, score;
                if (c > a && d > b && (c - a) <= 2048 && (d - b) <= 2048 && a >= -200 && b >= -200 && a <= 2048 && b <= 2048)
                {
                    rx = a; ry = b; rw = c - a; rh = d - b;
                    score = 6;
                    if (rw >= 4 && rw <= 1280 && rh >= 4 && rh <= 1024) score += 4;
                    if (rw < 8 || rh < 4) score -= 3;
                }
                else if (c > 0 && c <= 2048 && d > 0 && d <= 2048 && a >= -200 && a <= 2048 && b >= -200 && b <= 2048)
                {
                    rx = a; ry = b; rw = c; rh = d;
                    score = 2;
                    if (rw >= 8 && rw <= 1280 && rh >= 8 && rh <= 1024) score += 2;
                }
                else continue;
                if (score > bestScore)
                {
                    bestScore = score;
                    x = rx; y = ry; w = rw; h = rh;
                    if (off >= start + 8)
                    {
                        ident = BitConverter.ToInt32(data, off - 8) & 0xFFFF;
                        flags = data[off - 4] == 0 ? 0x40 : data[off - 4];
                    }
                }
            }
        }

        static bool LooksLikePropKey(string key)
        {
            if (KnownKeys.Contains(key)) return true;
            if (key.IndexOf('.') >= 0) return true;
            if (key.EndsWith("ImageFile") || key.EndsWith("SourceRect") || key.EndsWith("Color")
                || key.EndsWith("Rect") || key.EndsWith("ShadowRect") || key.StartsWith("Frame")
                || key.StartsWith("Image") || key.StartsWith("ListBox") || key.StartsWith("Edit")
                || key.StartsWith("backround") || key.StartsWith("button") || key.StartsWith("Has")
                || key.EndsWith("ttf") || key.Contains("Font"))
                return true;
            return false;
        }

        static bool LooksLikeCaption(string key)
        {
            if (string.IsNullOrEmpty(key) || LooksLikePropKey(key)) return false;
            foreach (var ch in key)
                if (ch > 127) return true;
            return key.IndexOf(' ') >= 0;
        }

        static void ReadProps(byte[] data, int off, int stop, GuiControl c)
        {
            int cur = off;
            while (cur < stop - 2)
            {
                string key; int ncur;
                if (!TryPString(data, cur, out key, out ncur)) { cur++; continue; }
                if (Types.Contains(key)) break;
                cur = ncur;
                if (!LooksLikePropKey(key))
                {
                    if (LooksLikeCaption(key) && string.IsNullOrEmpty(c.Text)) c.Text = key;
                    else c.Properties[key] = true;
                    continue;
                }
                if (key.EndsWith("ImageFile") || key == "ImageFile")
                {
                    string val; int vo;
                    if (TryPStringValue(data, cur, out val, out vo))
                    {
                        c.Properties[key] = val;
                        if (key == "ImageFile" || string.IsNullOrEmpty(c.ImageFile))
                            c.ImageFile = val;
                        cur = vo;
                    }
                    continue;
                }
                if (key.EndsWith("SourceRect") || key == "SourceRect")
                {
                    if (cur + 20 <= data.Length)
                    {
                        var rect = new[] {
                            BitConverter.ToInt32(data, cur),
                            BitConverter.ToInt32(data, cur + 4),
                            BitConverter.ToInt32(data, cur + 8),
                            BitConverter.ToInt32(data, cur + 12),
                            BitConverter.ToInt32(data, cur + 16)
                        };
                        c.Properties[key] = rect;
                        if (key == "SourceRect") c.SourceRect = rect;
                        cur += 20;
                    }
                    else if (cur + 16 <= data.Length)
                    {
                        var rect = new[] {
                            BitConverter.ToInt32(data, cur),
                            BitConverter.ToInt32(data, cur + 4),
                            BitConverter.ToInt32(data, cur + 8),
                            BitConverter.ToInt32(data, cur + 12)
                        };
                        c.Properties[key] = rect;
                        if (key == "SourceRect") c.SourceRect = rect;
                        cur += 16;
                    }
                    continue;
                }
                if (key.IndexOf("Color") >= 0)
                {
                    if (cur + 16 <= data.Length)
                    {
                        c.Properties[key] = new[] {
                            BitConverter.ToSingle(data, cur),
                            BitConverter.ToSingle(data, cur + 4),
                            BitConverter.ToSingle(data, cur + 8),
                            BitConverter.ToSingle(data, cur + 12)
                        };
                        cur += 16;
                    }
                    continue;
                }
                if (key == "Text")
                {
                    string val; int vo;
                    if (TryPStringValue(data, cur, out val, out vo)) { c.Text = val; c.Properties[key] = val; cur = vo; }
                    continue;
                }
                if (key == "Command")
                {
                    string val; int vo;
                    if (TryPStringValue(data, cur, out val, out vo)) { c.Command = val; c.Properties[key] = val; cur = vo; }
                    continue;
                }
                c.Properties[key] = true;
            }
        }

        public static Rectangle AtlasRect(int[] v, int imgW, int imgH)
        {
            if (v == null || v.Length < 4) return Rectangle.Empty;
            int left, top, right, bottom;
            if (v.Length >= 5)
            {
                // flag, left, top, right, bottom  (flag is usually 16 = 9-slice border)
                left = v[1]; top = v[2]; right = v[3]; bottom = v[4];
            }
            else if (v[0] == 16 && v.Length == 4)
            {
                left = v[1]; top = v[2]; right = v[3]; bottom = v[2];
            }
            else
            {
                left = v[0]; top = v[1]; right = v[2]; bottom = v[3];
            }
            if (right < left) { int t = left; left = right; right = t; }
            if (bottom < top) { int t = top; top = bottom; bottom = t; }
            var r = Rectangle.FromLTRB(left, top, Math.Max(left + 1, right), Math.Max(top + 1, bottom));
            if (imgW > 0)
            {
                if (r.X < 0) r.X = 0;
                if (r.Y < 0) r.Y = 0;
                if (r.Right > imgW) r.Width = Math.Max(1, imgW - r.X);
                if (r.Bottom > imgH) r.Height = Math.Max(1, imgH - r.Y);
            }
            return r;
        }

        public static int AtlasBorder(int[] v)
        {
            if (v != null && v.Length >= 5 && v[0] > 0 && v[0] <= 64) return v[0];
            if (v != null && v.Length >= 1 && v[0] == 16) return 16;
            return 0;
        }

        public static void SaveGui(GuiDocument doc, string path)
        {
            var buf = new List<byte>(new byte[HeaderSize]);
            var magic = Encoding.ASCII.GetBytes("TRGUIDescFile");
            for (int i = 0; i < magic.Length; i++) buf[i] = magic[i];
            WriteI32(buf, 0x34, doc.Version);
            int n = 0;
            if (doc.Root != null)
            {
                n = 1;
                foreach (var _ in GuiDocument.Walk(doc.Root)) n++;
                n--;
            }
            WriteI32(buf, 0x4C, n);
            if (doc.Root != null) buf.AddRange(WriteControl(doc.Root, -1));
            File.WriteAllBytes(path, buf.ToArray());
        }

        static void WriteI32(List<byte> buf, int off, int v)
        {
            var b = BitConverter.GetBytes(v);
            for (int i = 0; i < 4; i++) buf[off + i] = b[i];
        }

        static byte[] PString(string s)
        {
            Encoding enc;
            try { enc = Encoding.GetEncoding(949); }
            catch { enc = Encoding.Default; }
            var raw = enc.GetBytes(s ?? "");
            var b = new byte[2 + raw.Length];
            BitConverter.GetBytes((ushort)raw.Length).CopyTo(b, 0);
            raw.CopyTo(b, 2);
            return b;
        }

        static byte[] WriteControl(GuiControl c, int parent)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write(parent);
            bw.Write(PString(c.CtrlType));
            bw.Write(PString(c.Label));
            while (ms.Length % 4 != 0) bw.Write((byte)0);
            bw.Write(c.Ident == 0 ? 1 : c.Ident);
            bw.Write((byte)(c.Flags & 0xFF));
            bw.Write(c.X); bw.Write(c.Y); bw.Write(c.X + c.W); bw.Write(c.Y + c.H);
            bw.Write(new byte[16]);
            if (!string.IsNullOrEmpty(c.ImageFile))
            {
                bw.Write(PString("ImageFile"));
                bw.Write(PString(c.ImageFile));
            }
            if (c.SourceRect != null && c.SourceRect.Length >= 4)
            {
                bw.Write(PString("SourceRect"));
                for (int i = 0; i < 4; i++) bw.Write(c.SourceRect[i]);
            }
            if (!string.IsNullOrEmpty(c.Text))
            {
                bw.Write(PString("Text"));
                bw.Write(PString(c.Text));
            }
            int ident = c.Ident == 0 ? 1 : c.Ident;
            int i2 = ident + 1;
            foreach (var ch in c.Children)
            {
                if (ch.Ident == 0) ch.Ident = i2++;
                var sub = WriteControl(ch, ident);
                bw.Write(sub);
            }
            return ms.ToArray();
        }

        public static void SaveJson(GuiDocument doc, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"format\": \"guitool-recovered\",");
            sb.AppendLine("  \"version\": " + doc.Version + ",");
            sb.Append("  \"root\": ");
            WriteJsonControl(sb, doc.Root, 2);
            sb.AppendLine();
            sb.AppendLine("}");
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        static void WriteJsonControl(StringBuilder sb, GuiControl c, int indent)
        {
            if (c == null) { sb.Append("null"); return; }
            string pad = new string(' ', indent);
            sb.AppendLine("{");
            sb.AppendLine(pad + "  \"ctrl_type\": " + Q(c.CtrlType) + ",");
            sb.AppendLine(pad + "  \"name\": " + Q(c.Name) + ",");
            sb.AppendLine(pad + "  \"x\": " + c.X + ", \"y\": " + c.Y + ", \"w\": " + c.W + ", \"h\": " + c.H + ",");
            sb.AppendLine(pad + "  \"visible\": " + (c.Visible ? "true" : "false") + ",");
            sb.AppendLine(pad + "  \"enabled\": " + (c.Enabled ? "true" : "false") + ",");
            sb.AppendLine(pad + "  \"description\": " + Q(c.Description) + ",");
            sb.AppendLine(pad + "  \"command\": " + Q(c.Command) + ",");
            sb.AppendLine(pad + "  \"text\": " + Q(c.Text) + ",");
            sb.AppendLine(pad + "  \"imageFile\": " + Q(c.ImageFile) + ",");
            sb.AppendLine(pad + "  \"aspectRatio\": " + Q(c.AspectRatio) + ",");
            sb.Append(pad + "  \"children\": [");
            if (c.Children.Count == 0) sb.AppendLine("]");
            else
            {
                sb.AppendLine();
                for (int i = 0; i < c.Children.Count; i++)
                {
                    sb.Append(pad + "    ");
                    WriteJsonControl(sb, c.Children[i], indent + 4);
                    sb.AppendLine(i + 1 < c.Children.Count ? "," : "");
                }
                sb.AppendLine(pad + "  ]");
            }
            sb.Append(pad + "}");
        }

        static string Q(string s)
        {
            if (s == null) s = "";
            return "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n") + "\"";
        }

        static GuiDocument LoadJson(string path)
        {
            var text = File.ReadAllText(path, Encoding.UTF8);
            var doc = new GuiDocument { SourcePath = path };
            doc.Root = ParseJsonControl(text);
            if (doc.Root == null) doc.Root = GuiDocument.NewEmpty().Root;
            return doc;
        }

        static GuiControl ParseJsonControl(string json)
        {
            try
            {
                var c = new GuiControl();
                c.CtrlType = Extract(json, "ctrl_type", "Dialog");
                c.Name = Extract(json, "name", "Dialog");
                c.X = ExtractInt(json, "x", 0);
                c.Y = ExtractInt(json, "y", 0);
                c.W = ExtractInt(json, "w", 800);
                c.H = ExtractInt(json, "h", 600);
                c.Visible = Extract(json, "visible", "true") != "false";
                c.Enabled = Extract(json, "enabled", "true") != "false";
                c.Description = Extract(json, "description", "");
                c.Command = Extract(json, "command", "");
                c.Text = Extract(json, "text", "");
                c.ImageFile = Extract(json, "imageFile", "");
                c.AspectRatio = Extract(json, "aspectRatio", "none");
                return c;
            }
            catch { return null; }
        }

        static string Extract(string json, string key, string def)
        {
            var token = "\"" + key + "\"";
            int i = json.IndexOf(token);
            if (i < 0) return def;
            i = json.IndexOf(':', i);
            if (i < 0) return def;
            i++;
            while (i < json.Length && char.IsWhiteSpace(json[i])) i++;
            if (i < json.Length && json[i] == '"')
            {
                int j = json.IndexOf('"', i + 1);
                if (j < 0) return def;
                return json.Substring(i + 1, j - i - 1);
            }
            int k = i;
            while (k < json.Length && ",}\r\n ".IndexOf(json[k]) < 0) k++;
            var raw = json.Substring(i, k - i).Trim();
            return raw.Length == 0 ? def : raw;
        }

        static int ExtractInt(string json, string key, int def)
        {
            int v;
            return int.TryParse(Extract(json, key, def.ToString()), out v) ? v : def;
        }
    }
}
