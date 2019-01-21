
using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Main.Settings
{
    internal class SettingsViewModel : ViewModelBase
    {
        public GeneralSettings General { get; }

        public SettingsViewModel()
        {
            General = new GeneralSettings();
        }
    }
}