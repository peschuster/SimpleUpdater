using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SimpleUpdater.Core;
using System.Threading;

namespace SimpleUpdater
{
    public static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        public static void Main(params string[] parameter)
        {
#if DEBUG
            if (!Debugger.IsAttached)
                Debugger.Launch();
#endif

            if (parameter == null)
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var settings = ParseArguments(parameter);

            if (!settings.ContainsKey("feedUrl") 
                || !settings.ContainsKey("publicKey")
                || !settings.ContainsKey("version") 
                || !settings.ContainsKey("restart") 
                || !settings.ContainsKey("binDir")
                || !settings.ContainsKey("current"))
                return;

            string processName = Path.GetFileNameWithoutExtension("current");

            if (!settings.ContainsKey("appTitle"))
            {
                settings.Add("appTitle", processName);
            }

            Process[] app = Process.GetProcessesByName(processName);

            if (app.Length > 0)
            {
                app[0].WaitForExit(10000);

                if (!app[0].HasExited)
                    return;
            }

            EventWaitHandle asyncHandle = new ManualResetEvent(false);

            using (var updater = new Updater(
                new Version(settings["version"]),
                settings["binDir"],
                settings["feedUrl"],
                settings["publicKey"],
                settings["appTitle"]))
            {
                var dialog = updater.UpdateApplication(asyncHandle);

                dialog.ShowDialog();
                asyncHandle.WaitOne(TimeSpan.FromSeconds(60));
            }

            if (bool.Parse(settings["restart"]))
            {
                Process.Start(settings["current"], "--update-performed");
            }
        }

        private static Dictionary<string, string> ParseArguments(string[] arguments)
        {
            var result = new Dictionary<string, string>();

            Regex parser = new Regex(@"^\s*(?:\-{0,2})(?<key>[^\-]+?)\=\""{0,1}(?<value>[^""]+)\""{0,1}\s*$", RegexOptions.IgnoreCase);

            foreach (string item in arguments)
            {
                Match match = parser.Match(item);

                if (!match.Success)
                    continue;

                string key = match.Groups["key"].Value;
                string value = match.Groups["value"].Value;

                if (result.ContainsKey(key))
                {
                    result[key] = value;
                }
                else
                {
                    result.Add(key, value);
                }
            }
            
            return result;
        }
    }
}
