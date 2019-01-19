using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using log4net;

using Squirrel;

namespace CashManager_MVVM.Utils
{
    internal static class UpdatesManager
    {
        private const string UPDATES_URL = "http://cmh.eu5.org/";
        private const string ICON_NAME = "app.ico";
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(UpdatesManager)));

        internal static async Task HandleApplicationUpdatesCheck()
        {
            try
            {
                using (var mgr = new UpdateManager(UPDATES_URL))
                {
                    var result = await mgr.UpdateApp();
                    _logger.Value.Debug(result != null ? $"Updated to: {result.Version}" : "Up-to-date");
                }
            }
            catch (Exception e)
            {
                _logger.Value.Debug("Updated failed", e);
            }
        }

        internal static void HandleSquirrelEvents()
        {
            try
            {
                using (var mgr = new UpdateManager(UPDATES_URL))
                {
                    void Install(Version v)
                    {
                        string location = Assembly.GetEntryAssembly().Location;
                        string iconPath = Path.Combine(Path.GetDirectoryName(location) ?? string.Empty, @"..\", ICON_NAME);
                        mgr.CreateShortcutsForExecutable(Path.GetFileName(location), ShortcutLocation.StartMenu|ShortcutLocation.Desktop,
                            !Environment.CommandLine.Contains("squirrel-install"), null, iconPath);
                    }

                    SquirrelAwareApp.HandleEvents(Install, Install, onAppUninstall: v => mgr.RemoveShortcutForThisExe());
                }
            }
            catch (Exception) { }
        }
    }
}