using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using CashManager.Messages.App;
using CashManager.Properties;

using GalaSoft.MvvmLight.Messaging;

using log4net;

namespace CashManager.Utils.Updates
{
    internal class DefaultUpdatesManager : IUpdatesManager
    {
#if BETA
        private const string UPDATES_URL = "http://cash-manager.pl/files/beta/RELEASES";
#else
        private const string UPDATES_URL = "http://cash-manager.pl/files/releases/RELEASES";
#endif
        private const string USER_CONFIG_PATH = "user.config";
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(DefaultUpdatesManager)));

        public void HandleEvents() { SettingsManager.HandleSettingsUpgrade(USER_CONFIG_PATH); }

        public void Cleanup()
        {
            SettingsManager.BackupSettings(USER_CONFIG_PATH);
            string settingsPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            string path = Path.GetDirectoryName(settingsPath);
            if (string.IsNullOrEmpty(path)) return;

            RemoveSettingsDirectory(path);
        }

        public async Task HandleApplicationUpdatesCheck()
        {
            await Task.Run(() =>
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Settings.Default.Localization);
                PerformVersionCheck();
            });
        }

        private static void PerformVersionCheck()
        {
            string content = new WebReader().Read(UPDATES_URL);
            if (!string.IsNullOrWhiteSpace(content))
            {
                var regex = new Regex(@"\d+\.\d+\.\d+");
                var lines = new List<string>();
                foreach (Match match in regex.Matches(content)) lines.Add(match.Value);

                var versions = lines
                               .OrderByDescending(VersionToNumber)
                               .ToArray();
                string topVersion = versions.FirstOrDefault();

                long actualVersion = VersionToNumber(Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
                if (actualVersion < VersionToNumber(topVersion))
                {
                    _logger.Value.Debug($"There is newer version: {topVersion}");
                    Messenger.Default.Send(new ApplicationUpdateMessage($"{Strings.UpdateAvailable}: {topVersion}", string.Empty));
                }
                else
                {
                    _logger.Value.Debug("Up-to-date");
                }
            }
        }

        private static long VersionToNumber(string version)
        {
            var elements = version.Split('.');
            return int.Parse(elements[0]) * 1000000 + int.Parse(elements[1]) * 10000 + int.Parse(elements[2]);
        }

        private static void RemoveSettingsDirectory(string path)
        {
            var directory = new DirectoryInfo(path);
            if (directory.Exists && (directory.Parent?.Exists ?? false))
            {
                try
                {
                    directory.Parent.Delete(true);
                    _logger.Value.Debug($"Settings directory removed: {directory.Parent.FullName}");

                    try
                    {
                        directory.Parent.Parent.Delete();
                        _logger.Value.Debug($"App directory removed: {directory.Parent.Parent.FullName}");
                    }
                    catch (Exception)
                    {
                        _logger.Value.Debug("App directory is not empty");
                    }
                }
                catch (Exception e)
                {
                    _logger.Value.Debug("Could not remove directory", e);
                }
            }
            else
                _logger.Value.Debug("Settings directory does not exists");
        }
    }
}