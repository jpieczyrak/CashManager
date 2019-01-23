using System.Windows;

using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Utils;

namespace CashManager_MVVM.Features.Main
{
    public partial class AboutWindow : CustomWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            SoundPlayerHelper.PlaySound(SoundPlayerHelper.Sound.About);
        }

        protected override void RestoreOnClick(object sender, RoutedEventArgs e) { }
    }
}