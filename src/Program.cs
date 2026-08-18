using System;
using System.IO;
using System.Windows.Forms;

namespace GUITools
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (s, e) => ShowFail(e.Exception, "GUITools error");
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception ex = e.ExceptionObject as Exception;
                if (ex == null) ex = new Exception(Convert.ToString(e.ExceptionObject));
                ShowFail(ex, "GUITools error");
            };
            try
            {
                GameData.Initialize(Application.StartupPath);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                ShowFail(ex, "GUITools failed to start");
            }
        }

        static void ShowFail(Exception ex, string title)
        {
            try
            {
                string dir = Application.StartupPath;
                if (string.IsNullOrEmpty(dir)) dir = Environment.CurrentDirectory;
                File.WriteAllText(Path.Combine(dir, "GUITools_crash.txt"),
                    DateTime.Now.ToString() + Environment.NewLine + ex);
            }
            catch { }
            try { MessageBox.Show(ex.ToString(), title); }
            catch { }
        }
    }
}
