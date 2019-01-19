using System;
using System.Configuration;
using System.IO;
using System.Reflection;

using CashManager_MVVM.Properties;

using log4net;

namespace CashManager_MVVM.Utils
{
    internal static class SettingsManager
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(SettingsManager)));

        private static string BackupPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\last.config";

        private static string SettingsPath =>
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

        internal static void BackupSettings()
        {
            try
            {
                File.Copy(SettingsPath, BackupPath, true);
                _logger.Value.Info("Settings backup done");
            }
            catch (Exception e)
            {
                _logger.Value.Error("Could not backup settings", e);
            }
        }

        internal static void HandleSettingsUpgrade()
        {
            RestoreSettings();
            if (Settings.Default.UpgradeNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.Save();
                Settings.Default.Reload();
                Settings.Default.UpgradeNeeded = false;
                Settings.Default.Save();
                _logger.Value.Debug("Settings upgraded");
            }
        }

        private static void RestoreSettings()
        {
            if (!File.Exists(BackupPath))
            {
                _logger.Value.Debug("There is no settings backup file");
                return;
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
            }
            catch (Exception e)
            {
                _logger.Value.Info("Could not created directory", e);
            }
            if (!File.Exists(SettingsPath)) _logger.Value.Debug($"Settings path does not exists: {SettingsPath}");

            try
            {
                File.Copy(BackupPath, SettingsPath, true);
            }
            catch (Exception e)
            {
                _logger.Value.Warn("Could not perform settings restore", e);
                return;
            }

            _logger.Value.Info("Settings backup restored");
            Settings.Default.Reload();
            _logger.Value.Info("Settings reloaded");

            try
            {
                File.Delete(BackupPath);
            }
            catch (Exception e)
            {
                _logger.Value.Info("Could not delete settings backup", e);
            }
        }
    }
}