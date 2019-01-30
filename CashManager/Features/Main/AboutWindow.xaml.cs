using System.Windows;

using CashManager.Features.Common;
using CashManager.Utils;

namespace CashManager.Features.Main
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