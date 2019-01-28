using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

using CashManager.Properties;
using CashManager.Skins;

using log4net;

namespace CashManager.Utils
{
    internal static class SettingsManager
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(SettingsManager)));

        private static string BackupPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\last.config";

        private static string SettingsPath =>
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

        internal static void BackupSettings(string path = null)
        {
            try
            {
                Settings.Default.Save();
                File.Copy(SettingsPath, path ?? BackupPath, true);
                _logger.Value.Debug("Settings backup done");
            }
            catch (Exception e)
            {
                _logger.Value.Error("Could not backup settings", e);
            }
        }

        internal static void HandleSettingsUpgrade(string path = null)
        {
            RestoreSettings(path);
            if (Settings.Default.UpgradeNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.Save();
                Settings.Default.Reload();
                Settings.Default.UpgradeNeeded = false;
                Settings.Default.Save();
                _logger.Value.Debug("Settings upgraded");
            }

            (Application.Current.Resources
                        .MergedDictionaries
                        .FirstOrDefault(x => x is ColorsResourceDictionary) as ColorsResourceDictionary)?.UpdateSource();
            (Application.Current.Resources
                        .MergedDictionaries
                        .FirstOrDefault(x => x is ShapesResourceDictionary) as ShapesResourceDictionary)?.UpdateSource();
        }

        private static void RestoreSettings(string path = null)
        {
            string sourceFileName = path ?? BackupPath;
            if (!File.Exists(sourceFileName))
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
                _logger.Value.Debug("Could not create settings target directory", e);
            }
            if (!File.Exists(SettingsPath)) _logger.Value.Debug($"Settings path does not exists: {SettingsPath}");

            try
            {
                File.Copy(sourceFileName, SettingsPath, true);
            }
            catch (Exception e)
            {
                _logger.Value.Debug("Could not perform settings restore", e);
                return;
            }

            _logger.Value.Debug("Settings backup restored");
            Settings.Default.Reload();
            _logger.Value.Debug("Settings reloaded");

            try
            {
                File.Delete(sourceFileName);
            }
            catch (Exception e)
            {
                _logger.Value.Debug("Could not delete settings backup", e);
            }
        }
    }
}