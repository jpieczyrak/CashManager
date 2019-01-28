using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

using log4net;

namespace CashManager.Utils.Updates
{
    internal class DefaultUpdatesManager : IUpdatesManager
    {
        private const string USER_CONFIG_PATH = "user.config";
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(DefaultUpdatesManager)));

        #region IUpdatesManager

        public void HandleEvents()
        {
            SettingsManager.HandleSettingsUpgrade(USER_CONFIG_PATH);
        }

        public void Cleanup()
        {
            SettingsManager.BackupSettings(USER_CONFIG_PATH);
            string settingsPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            string path = Path.GetDirectoryName(settingsPath);
            if (string.IsNullOrEmpty(path)) return;

            RemoveSettingsDirectory(path);
        }

        public async Task HandleApplicationUpdatesCheck() { }

        #endregion

        private static void RemoveSettingsDirectory(string path)
        {
            var directory = new DirectoryInfo(path);
            try
            {
                if (directory.Exists && (directory.Parent?.Exists ?? false))
                {
                    directory.Parent.Delete(true);
                    _logger.Value.Debug($"Settings directory removed: {directory.Parent.FullName}");
                }
                else
                {
                    _logger.Value.Debug("Settings directory does not exists");
                }
            }
            catch (Exception e)
            {
                _logger.Value.Info($"Could not remove settings directory: {directory.Parent?.FullName}", e);
            }
        }
    }
}