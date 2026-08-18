using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GUITools
{
    public class GameFontDef
    {
        public int Index;
        public string Face;
        public bool Bold;
        public int Size;
        public bool GdiPlus;
        public bool Antialias = true;
        public Font Font;
    }

    /// <summary>
    /// Loads scripts.pkg, shaders.pkg, and startup.py the same way the original
    /// GUITool / in-game client boots (fonts, properties, shader catalog).
    /// </summary>
    public static class GameRuntime
    {
        public static readonly Dictionary<int, GameFontDef> Fonts =
            new Dictionary<int, GameFontDef>();
        public static readonly Dictionary<string, string> Properties =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public static readonly List<string> ShaderNames = new List<string>();
        public static readonly List<string> ScriptNames = new List<string>();

        static readonly PrivateFontCollection _privateFonts = new PrivateFontCollection();
        static readonly Dictionary<string, FontFamily> _privateFamilies =
            new Dictionary<string, FontFamily>(StringComparer.OrdinalIgnoreCase);
        static Font _fallback;

        public static int ShaderCount { get { return ShaderNames.Count; } }
        public static int ScriptCount { get { return ScriptNames.Count; } }
        public static int FontCount { get { return Fonts.Count; } }
        public static string StartupPath;
        public static string LastStatus = "";

        public static string Boot()
        {
            try
            {
                GameData.EnsureInitialized();
                GameData.EnsureCorePacksIndexed();
                CollectPackLists();
                LoadStartup();
                ExtractUiShaders();
                LastStatus = BuildStatus();
            }
            catch (Exception ex)
            {
                LastStatus = "ready (game packs not loaded yet)";
                try
                {
                    string dir = GameData.CacheRoot ?? AppDomain.CurrentDomain.BaseDirectory;
                    File.WriteAllText(Path.Combine(dir, "boot_error.txt"), ex.ToString());
                }
                catch { }
            }
            return LastStatus;
        }

        static string BuildStatus()
        {
            bool lobby = false;
            try
            {
                string p = Path.Combine(GameData.CacheRoot ?? "", "gui", "lobby", "lobby.gui");
                lobby = File.Exists(p);
            }
            catch { }
            string gui = lobby ? "lobby.gui" : "gui from pkg";
            return "scripts " + ScriptCount
                + "  ·  shaders " + ShaderCount
                + "  ·  fonts " + FontCount
                + "  ·  " + gui;
        }

        static void CollectPackLists()
        {
            ShaderNames.Clear();
            ScriptNames.Clear();
            foreach (var kv in GameData.AllEntries())
            {
                string ext = Path.GetExtension(kv.Key).ToLowerInvariant();
                if (ext == ".vs" || ext == ".ps" || ext == ".gs" || ext == ".vss")
                    ShaderNames.Add(kv.Key);
                else if (ext == ".py" || ext == ".pyc")
                    ScriptNames.Add(kv.Key);
            }
            ShaderNames.Sort(StringComparer.OrdinalIgnoreCase);
            ScriptNames.Sort(StringComparer.OrdinalIgnoreCase);
        }

        static void ExtractUiShaders()
        {
            string dir = Path.Combine(GameData.CacheRoot ?? "", "shaders");
            int n = 0;
            for (int i = 0; i < ShaderNames.Count && n < 40; i++)
            {
                string name = Path.GetFileName(ShaderNames[i]).ToLowerInvariant();
                if (name.StartsWith("ui_") || name.StartsWith("gui_"))
                {
                    if (GameData.ResolveExisting(ShaderNames[i]) != null) n++;
                }
            }
        }

        static void LoadStartup()
        {
            Fonts.Clear();
            Properties.Clear();
            StartupPath = FindStartup();
            if (string.IsNullOrEmpty(StartupPath) || !File.Exists(StartupPath))
            {
                AddDefaultFonts();
                return;
            }
            Encoding enc;
            try { enc = Encoding.GetEncoding(949); }
            catch { enc = Encoding.Default; }
            string text = File.ReadAllText(StartupPath, enc);
            ParseStartup(text);
            if (Fonts.Count == 0) AddDefaultFonts();
            RealizeFonts();
        }

        static string FindStartup()
        {
            string[] paths = {
                Path.Combine(GameData.ToolRoot ?? "", "script", "startup.py"),
                Path.Combine(ApplicationStartup(), "script", "startup.py"),
                @"C:\Users\Administrator\Desktop\GUITool\script\startup.py",
                Path.Combine(GameData.CacheRoot ?? "", "script", "startup.py"),
                Path.Combine(GameData.GameRoot ?? "", "script", "startup.py")
            };
            for (int i = 0; i < paths.Length; i++)
                if (!string.IsNullOrEmpty(paths[i]) && File.Exists(paths[i])) return paths[i];
            string fromPkg = GameData.ResolveExisting("startup.py");
            if (fromPkg != null) return fromPkg;
            fromPkg = GameData.ResolveExisting("script_startup.py");
            return fromPkg;
        }

        static string ApplicationStartup()
        {
            try { return System.Windows.Forms.Application.StartupPath; }
            catch { return ""; }
        }

        static void ParseStartup(string text)
        {
            var fontRe = new Regex(
                @"addTextFont(?:Ex)?\s*\(\s*(\d+)\s*,\s*""([^""]+)""\s*,\s*(True|False)\s*,\s*(\d+)\s*,\s*(True|False)(?:\s*,\s*(True|False))?",
                RegexOptions.IgnoreCase);
            Match m = fontRe.Match(text);
            while (m.Success)
            {
                var def = new GameFontDef();
                def.Index = int.Parse(m.Groups[1].Value);
                def.Face = m.Groups[2].Value;
                def.Bold = string.Equals(m.Groups[3].Value, "True", StringComparison.OrdinalIgnoreCase);
                def.Size = int.Parse(m.Groups[4].Value);
                def.GdiPlus = string.Equals(m.Groups[5].Value, "True", StringComparison.OrdinalIgnoreCase);
                def.Antialias = m.Groups[6].Success
                    ? string.Equals(m.Groups[6].Value, "True", StringComparison.OrdinalIgnoreCase)
                    : true;
                Fonts[def.Index] = def;
                m = m.NextMatch();
            }
            var propRe = new Regex(@"setProperty\s*\(\s*""([^""]+)""\s*,\s*""([^""]*)""\s*\)");
            m = propRe.Match(text);
            while (m.Success)
            {
                Properties[m.Groups[1].Value] = m.Groups[2].Value;
                m = m.NextMatch();
            }
        }

        static void AddDefaultFonts()
        {
            AddDef(0, "Gulim", true, 16);
            AddDef(1, "Gulim", true, 26);
            AddDef(2, "Gulim", false, 12);
            AddDef(3, "Gulim", false, 11);
        }

        static void AddDef(int index, string face, bool bold, int size)
        {
            var def = new GameFontDef();
            def.Index = index; def.Face = face; def.Bold = bold; def.Size = size;
            Fonts[index] = def;
        }

        static void RealizeFonts()
        {
            if (_fallback == null)
                _fallback = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular);
            foreach (var kv in Fonts)
            {
                GameFontDef def = kv.Value;
                try { def.Font = MakeFont(def); }
                catch { def.Font = _fallback; }
            }
        }

        static Font MakeFont(GameFontDef def)
        {
            float size = def.Size > 0 ? def.Size : 12f;
            FontStyle style = def.Bold ? FontStyle.Bold : FontStyle.Regular;
            FontFamily family = ResolveFamily(def.Face);
            return new Font(family, size, style, GraphicsUnit.Pixel);
        }

        static FontFamily ResolveFamily(string face)
        {
            if (string.IsNullOrEmpty(face)) return FontFamily.GenericSansSerif;
            string name = face;
            if (name.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith(".otf", StringComparison.OrdinalIgnoreCase))
            {
                FontFamily fromFile = LoadPrivateFace(name);
                if (fromFile != null) return fromFile;
                name = Path.GetFileNameWithoutExtension(name);
            }
            if (name == "굴림" || name.IndexOf("Gulim", StringComparison.OrdinalIgnoreCase) >= 0
                || name == "±¼¸²")
                name = "Gulim";
            try { return new FontFamily(name); }
            catch { }
            try { return new FontFamily("Gulim"); }
            catch { }
            try { return new FontFamily("Malgun Gothic"); }
            catch { }
            return FontFamily.GenericSansSerif;
        }

        static FontFamily LoadPrivateFace(string fileName)
        {
            FontFamily existing;
            if (_privateFamilies.TryGetValue(fileName, out existing)) return existing;
            string path = GameData.ResolveExisting(fileName);
            if (path == null)
            {
                string win = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fileName);
                if (File.Exists(win)) path = win;
            }
            if (path == null || !File.Exists(path)) return null;
            try
            {
                _privateFonts.AddFontFile(path);
                FontFamily fam = _privateFonts.Families[_privateFonts.Families.Length - 1];
                _privateFamilies[fileName] = fam;
                return fam;
            }
            catch { return null; }
        }

        public static Font FontFor(GuiControl c)
        {
            int index = 0;
            if (c != null)
            {
                string s = c.GetPropString("Font");
                int parsed;
                if (!string.IsNullOrEmpty(s) && int.TryParse(s, out parsed)) index = parsed;
            }
            GameFontDef def;
            if (Fonts.TryGetValue(index, out def) && def.Font != null) return def.Font;
            if (Fonts.TryGetValue(0, out def) && def.Font != null) return def.Font;
            return _fallback ?? SystemFonts.DefaultFont;
        }

        public static string GetProperty(string key)
        {
            string v;
            if (Properties.TryGetValue(key, out v)) return v;
            return "";
        }
    }
}
