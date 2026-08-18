using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GUITools
{
    public class PkgIndexEntry
    {
        public string Pkg;
        public string Path;
        public int Parts;
        public int Offset;
    }

    public static class GameData
    {
        public static string GameRoot = "";
        public static string LastBrowseDir = "";
        public static string CacheRoot;
        public static string ToolRoot;
        public static string ToolGui;
        public static string CurrentGuiDir;

        static readonly Dictionary<string, PkgIndexEntry> ByPath =
            new Dictionary<string, PkgIndexEntry>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, List<PkgIndexEntry>> ByFileName =
            new Dictionary<string, List<PkgIndexEntry>>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, Image> ImageCache =
            new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        public static void EnsureInitialized()
        {
            if (!string.IsNullOrEmpty(CacheRoot)) return;
            string start = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(start)) start = Environment.CurrentDirectory;
            Initialize(start);
        }

        public static void Initialize(string toolRoot)
        {
            if (string.IsNullOrEmpty(toolRoot))
                toolRoot = AppDomain.CurrentDomain.BaseDirectory;
            ToolRoot = FindProjectRoot(toolRoot);
            if (string.IsNullOrEmpty(ToolRoot))
                ToolRoot = Environment.CurrentDirectory;
            ToolGui = Path.Combine(ToolRoot, "gui");
            CacheRoot = Path.Combine(ToolRoot, "cache");
            Directory.CreateDirectory(CacheRoot);
            Directory.CreateDirectory(Path.Combine(CacheRoot, "pkg_index"));
            LoadLastBrowse();
            LoadIndex();
            if (ByPath.Count == 0 && !string.IsNullOrEmpty(GameRoot) && Directory.Exists(GameRoot))
                RebuildIndex();
        }

        public static void RememberBrowse(string fileOrFolder)
        {
            if (string.IsNullOrEmpty(fileOrFolder)) return;
            string dir = fileOrFolder;
            if (File.Exists(fileOrFolder)) dir = Path.GetDirectoryName(fileOrFolder);
            if (!Directory.Exists(dir)) return;
            LastBrowseDir = dir;
            if (fileOrFolder.EndsWith(".pkg", StringComparison.OrdinalIgnoreCase))
                GameRoot = dir;
            try
            {
                File.WriteAllText(Path.Combine(CacheRoot, "pkg_index", "last_browse.txt"), dir);
            }
            catch { }
        }

        static void LoadLastBrowse()
        {
            try
            {
                string p = Path.Combine(CacheRoot, "pkg_index", "last_browse.txt");
                if (File.Exists(p))
                {
                    string dir = File.ReadAllText(p).Trim();
                    if (Directory.Exists(dir))
                    {
                        LastBrowseDir = dir;
                        GameRoot = dir;
                    }
                }
            }
            catch { }
        }

        public static string BrowseStartDir()
        {
            if (!string.IsNullOrEmpty(LastBrowseDir) && Directory.Exists(LastBrowseDir))
                return LastBrowseDir;
            if (!string.IsNullOrEmpty(GameRoot) && Directory.Exists(GameRoot))
                return GameRoot;
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (Directory.Exists(desktop)) return desktop;
            return Environment.CurrentDirectory;
        }

        static string FindProjectRoot(string start)
        {
            string root = start;
            for (int i = 0; i < 8 && !string.IsNullOrEmpty(root); i++)
            {
                if (File.Exists(Path.Combine(root, "build.bat"))
                    || Directory.Exists(Path.Combine(root, "src"))
                    || File.Exists(Path.Combine(root, "GUITools.csproj")))
                    return root;
                root = Path.GetDirectoryName(root);
            }
            return start;
        }

        static string IndexPath()
        {
            EnsureInitialized();
            return Path.Combine(CacheRoot, "pkg_index", "vfs.tsv");
        }

        static void LoadIndex()
        {
            ByPath.Clear();
            ByFileName.Clear();
            string indexPath = IndexPath();
            if (!File.Exists(indexPath)) return;
            foreach (var line in File.ReadAllLines(indexPath))
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("pkg\t")) continue;
                var p = line.Split('\t');
                if (p.Length < 4) continue;
                int parts, off;
                if (!int.TryParse(p[2], out parts) || !int.TryParse(p[3], out off)) continue;
                AddIndex(p[0], p[1], parts, off);
            }
        }

        static void AddIndex(string pkg, string path, int parts, int off)
        {
            var e = new PkgIndexEntry { Pkg = pkg, Path = path, Parts = parts, Offset = off };
            string key = Norm(path);
            ByPath[key] = e;
            string fn = Path.GetFileName(path.Replace('/', '\\'));
            List<PkgIndexEntry> list;
            if (!ByFileName.TryGetValue(fn, out list))
            {
                list = new List<PkgIndexEntry>();
                ByFileName[fn] = list;
            }
            list.Add(e);
        }

        public static int RebuildIndex()
        {
            if (!Directory.Exists(GameRoot)) return 0;
            ByPath.Clear();
            ByFileName.Clear();
            var lines = new List<string>();
            lines.Add("pkg\tpath\tparts\toffset");
            string[] pkgs = Directory.GetFiles(GameRoot, "*.pkg");
            Array.Sort(pkgs, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < pkgs.Length; i++)
            {
                string pkgPath = pkgs[i];
                string pkgName = Path.GetFileName(pkgPath);
                string n = pkgName.ToLowerInvariant();
                bool want = n.StartsWith("tr") || n.StartsWith("map") || n == "map_etc.pkg"
                    || n == "material.pkg" || n == "scripts.pkg" || n == "shaders.pkg";
                if (!want) continue;
                try
                {
                    var entries = PkgArchive.ListEntries(pkgPath);
                    for (int k = 0; k < entries.Count; k++)
                    {
                        PkgFileEntry e = entries[k];
                        AddIndex(pkgName, e.Path, e.Parts, e.Offset);
                        lines.Add(pkgName + "\t" + e.Path + "\t" + e.Parts + "\t" + e.Offset);
                    }
                }
                catch { }
            }
            File.WriteAllLines(IndexPath(), lines.ToArray());
            return ByPath.Count;
        }

        static bool _mapsIndexed;

        public static void EnsureMapsIndexed()
        {
            if (_mapsIndexed) return;
            _mapsIndexed = true;
            if (!Directory.Exists(GameRoot)) return;
            bool hasTrv = false;
            foreach (var key in ByPath.Keys)
            {
                if (key.EndsWith(".trv", StringComparison.OrdinalIgnoreCase)) { hasTrv = true; break; }
            }
            if (hasTrv) return;
            string[] pkgs = Directory.GetFiles(GameRoot, "map*.pkg");
            var extra = new List<string>();
            for (int i = 0; i < pkgs.Length; i++)
            {
                try
                {
                    string pkgName = Path.GetFileName(pkgs[i]);
                    var entries = PkgArchive.ListEntries(pkgs[i]);
                    for (int k = 0; k < entries.Count; k++)
                    {
                        PkgFileEntry e = entries[k];
                        AddIndex(pkgName, e.Path, e.Parts, e.Offset);
                        extra.Add(pkgName + "\t" + e.Path + "\t" + e.Parts + "\t" + e.Offset);
                    }
                }
                catch { }
            }
            if (extra.Count > 0)
            {
                string path = IndexPath();
                File.AppendAllLines(path, extra.ToArray());
            }
        }

        static bool _coreIndexed;

        public static void EnsureCorePacksIndexed()
        {
            if (_coreIndexed) return;
            _coreIndexed = true;
            IndexNamedPacks(new[] { "scripts.pkg", "shaders.pkg", "material.pkg" });
        }

        public static string FindPkg(string pkgName)
        {
            if (string.IsNullOrEmpty(pkgName)) return null;
            string[] roots = {
                GameRoot,
                ToolRoot,
                Path.Combine(ToolRoot ?? "", "..", "GUITool"),
                @"C:\Users\Administrator\Desktop\GUITool"
            };
            for (int i = 0; i < roots.Length; i++)
            {
                if (string.IsNullOrEmpty(roots[i]) || !Directory.Exists(roots[i])) continue;
                string p = Path.Combine(roots[i], pkgName);
                if (File.Exists(p)) return p;
            }
            return null;
        }

        static void IndexNamedPacks(string[] names)
        {
            var extra = new List<string>();
            for (int i = 0; i < names.Length; i++)
            {
                bool already = false;
                foreach (var kv in ByPath)
                {
                    if (string.Equals(kv.Value.Pkg, names[i], StringComparison.OrdinalIgnoreCase))
                    { already = true; break; }
                }
                if (already) continue;
                string pkg = FindPkg(names[i]);
                if (pkg == null || !PkgArchive.LooksLikePkg(pkg)) continue;
                try
                {
                    var entries = PkgArchive.ListEntries(pkg);
                    string pkgName = Path.GetFileName(pkg);
                    for (int k = 0; k < entries.Count; k++)
                    {
                        PkgFileEntry e = entries[k];
                        AddIndex(pkgName, e.Path, e.Parts, e.Offset);
                        extra.Add(pkgName + "\t" + e.Path + "\t" + e.Parts + "\t" + e.Offset);
                    }
                }
                catch { }
            }
            if (extra.Count > 0)
            {
                string path = IndexPath();
                if (!File.Exists(path))
                    File.WriteAllText(path, "pkg\tpath\tparts\toffset" + Environment.NewLine);
                File.AppendAllLines(path, extra.ToArray());
            }
        }

        public static Dictionary<string, PkgIndexEntry> AllEntries()
        {
            return ByPath;
        }

        static string Norm(string p)
        {
            return (p ?? "").Replace('/', '\\').TrimStart('\\');
        }

        static void AddSearch(List<string> search, string path)
        {
            if (!string.IsNullOrEmpty(path) && !search.Contains(path))
                search.Add(path);
        }

        static string CombineSafe(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return null;
            return Path.Combine(a, b);
        }

        static string CombineSafe(string a, string b, string c)
        {
            if (string.IsNullOrEmpty(a)) return null;
            return Path.Combine(a, b, c);
        }

        public static string ResolveExisting(string requested)
        {
            if (string.IsNullOrEmpty(requested)) return null;
            EnsureInitialized();
            string rel = StripDevPath(requested);
            string collapsed = CollapseDots(requested);
            var search = new List<string>();
            if (!string.IsNullOrEmpty(CurrentGuiDir)) search.Add(CurrentGuiDir);
            AddSearch(search, Path.GetDirectoryName(requested));
            AddSearch(search, ToolGui);
            AddSearch(search, CacheRoot);
            AddSearch(search, CombineSafe(CacheRoot, "gui"));
            AddSearch(search, CombineSafe(CacheRoot, "gui", "lobby"));
            AddSearch(search, CombineSafe(CacheRoot, "gui", "Total"));
            AddSearch(search, CombineSafe(CacheRoot, "gui2"));
            AddSearch(search, CombineSafe(CacheRoot, "gui3"));
            AddSearch(search, GameRoot);
            AddSearch(search, CombineSafe(GameRoot, "gui"));
            foreach (var root in search)
            {
                if (string.IsNullOrEmpty(root) || !Directory.Exists(root)) continue;
                try
                {
                    string a = Path.GetFullPath(Path.Combine(root, requested.Replace('/', '\\')));
                    if (File.Exists(a)) return a;
                }
                catch { }
                try
                {
                    string d = Path.GetFullPath(Path.Combine(root, collapsed));
                    if (File.Exists(d)) return d;
                }
                catch { }
                string b = Path.Combine(root, rel);
                if (File.Exists(b)) return b;
                string c = Path.Combine(root, Path.GetFileName(rel));
                if (File.Exists(c)) return c;
            }
            string found = MaterializeFromPkg(rel);
            if (found != null) return found;
            return MaterializeFromPkg(collapsed);
        }

        static string StripDevPath(string requested)
        {
            string s = requested.Replace('/', '\\');
            int gui = s.IndexOf("\\gui\\", StringComparison.OrdinalIgnoreCase);
            if (gui >= 0) return s.Substring(gui + 1);
            if (s.IndexOf(':') >= 0) return Path.GetFileName(s);
            return CollapseDots(s);
        }

        static string CollapseDots(string requested)
        {
            string s = (requested ?? "").Replace('/', '\\');
            while (s.StartsWith("..\\")) s = s.Substring(3);
            return s.TrimStart('\\');
        }

        static PkgIndexEntry Lookup(string rel)
        {
            PkgIndexEntry e;
            if (ByPath.TryGetValue(Norm(rel), out e)) return e;
            List<PkgIndexEntry> list;
            if (ByFileName.TryGetValue(Path.GetFileName(rel.Replace('/', '\\')), out list) && list.Count > 0)
                return list[0];
            return null;
        }

        static string MaterializeFromPkg(string rel)
        {
            PkgIndexEntry e = Lookup(rel);
            if (e == null) return null;
            return ExtractEntry(e);
        }

        public static string ExtractEntry(PkgIndexEntry e)
        {
            if (e == null) return null;
            EnsureInitialized();
            string dest = Path.Combine(CacheRoot, e.Path.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(dest) && new FileInfo(dest).Length > 0) return dest;
            string pkg = FindPkg(e.Pkg);
            if (pkg == null || !File.Exists(pkg)) return null;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.WriteAllBytes(dest, PkgArchive.Extract(pkg, e.Offset, e.Parts));
                return dest;
            }
            catch { return null; }
        }

        /// <summary>
        /// Extract a .trv and its sidecar files (sm/rd/png) into cache.
        /// </summary>
        public static string ExtractMap(string relativeTrv)
        {
            EnsureInitialized();
            string cached = Path.Combine(CacheRoot, Norm(relativeTrv));
            if (File.Exists(cached) && new FileInfo(cached).Length > 0) return cached;
            EnsureMapsIndexed();
            cached = Path.Combine(CacheRoot, Norm(relativeTrv));
            if (File.Exists(cached) && new FileInfo(cached).Length > 0) return cached;
            PkgIndexEntry e = Lookup(relativeTrv);
            if (e == null) return File.Exists(cached) ? cached : null;
            string folder = Path.GetDirectoryName(Norm(e.Path));
            ExtractFolder(folder);
            string dest = ExtractEntry(e);
            return dest;
        }

        public static int ExtractFolder(string folderPrefix)
        {
            string prefix = Norm(folderPrefix);
            if (!prefix.EndsWith("\\")) prefix += "\\";
            int n = 0;
            foreach (var kv in ByPath)
            {
                if (!kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
                string ext = Path.GetExtension(kv.Key).ToLowerInvariant();
                bool want = ext == ".trv" || ext == ".trv_sm" || ext == ".trv_rd"
                    || ext == ".png" || ext == ".jpg" || ext == ".tga" || ext == ".dds"
                    || ext == ".txt" || ext == ".ca3" || ext == ".m1" || ext == ".pt1";
                if (!want) continue;
                if (ExtractEntry(kv.Value) != null) n++;
            }
            return n;
        }

        public static string ParkMapPath()
        {
            return ExtractMap("map_park\\land.trv");
        }

        public static string DefaultRaceMapPath()
        {
            string[] names = {
                "map_hungbunolbu\\hungbunolbu1_beginner_re_league.trv",
                "map_hungbunolbu\\hungbunolbu1_beginner.trv"
            };
            for (int i = 0; i < names.Length; i++)
            {
                string p = ExtractMap(names[i]);
                if (p != null && File.Exists(p)) return p;
            }
            foreach (var kv in ByPath)
            {
                if (kv.Key.EndsWith(".trv", StringComparison.OrdinalIgnoreCase)
                    && kv.Key.IndexOf("hungbunolbu", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string p = ExtractMap(kv.Key);
                    if (p != null) return p;
                }
            }
            return null;
        }

        public static Image GetImage(string requested)
        {
            if (string.IsNullOrEmpty(requested)) return null;
            Image cached;
            if (ImageCache.TryGetValue(requested, out cached)) return cached;
            string path = ResolveExisting(requested);
            if (path == null || !File.Exists(path)) return null;
            try
            {
                cached = DdsImage.LoadFile(path);
                ImageCache[requested] = cached;
                ImageCache[path] = cached;
                return cached;
            }
            catch { return null; }
        }

        public static string DefaultGuiPath()
        {
            string[] preferred = {
                "gui\\lobby\\lobby.gui",
                "gui\\lobby\\lobbydlg.gui"
            };
            for (int i = 0; i < preferred.Length; i++)
            {
                string p = ResolveExisting(preferred[i]);
                if (!string.IsNullOrEmpty(p) && File.Exists(p)) return p;
            }
            return null;
        }
    }
}
