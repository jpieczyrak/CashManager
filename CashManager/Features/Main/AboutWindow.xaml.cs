using System.Windows;

using CashManager.WPF.Features.Common;
using CashManager.WPF.Utils;

namespace CashManager.WPF.Features.Main
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