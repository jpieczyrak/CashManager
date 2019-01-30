
using GalaSoft.MvvmLight;

namespace CashManager.Features.Main.Settings
{
    internal class SettingsViewModel : ViewModelBase
    {
        public GeneralSettings General { get; }
        public WarningsSettings Warnings { get; }

        public SettingsViewModel()
        {
            General = new GeneralSettings();
            Warnings = new WarningsSettings();
        }
    }
}