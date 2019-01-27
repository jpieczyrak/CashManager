using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using CashManager.Messages.App;
using CashManager.Properties;

using GalaSoft.MvvmLight.Messaging;

using log4net;
#if !PORTABLE
using Squirrel;
#endif
namespace CashManager.Utils
{
    internal static class UpdatesManager
    {
#if !PORTABLE
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
                    if (result != null)
                    {
                        Messenger.Default.Send(new ApplicationUpdateMessage($"{Strings.Installed}: {result.Version}", Strings.PleaseRestartApplicationToUseNewerVersion));
                        SettingsManager.BackupSettings();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Value.Info("Application updated failed", e);
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
#else
        public static void HandleSquirrelEvents() { }

        public static async Task HandleApplicationUpdatesCheck() { }
#endif
    }
}