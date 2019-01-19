using System;

using CashManager_MVVM.Properties;

using log4net;

namespace CashManager_MVVM.Utils
{
    internal static class SettingsManager
    {
        internal static void HandleSettingsUpgrade(Lazy<ILog> logger)
        {
            if (Settings.Default.UpgradeNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.Save();
                Settings.Default.Reload();
                Settings.Default.UpgradeNeeded = false;
                Settings.Default.Save();
                logger.Value.Debug("Settings upgraded");
            }
        }
    }
}