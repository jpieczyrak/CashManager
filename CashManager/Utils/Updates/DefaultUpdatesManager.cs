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
            {
                _logger.Value.Debug("Settings directory does not exists");
            }
        }
    }
}