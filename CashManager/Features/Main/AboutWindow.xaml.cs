using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

using CashManager.Features.Common;
using CashManager.Properties;
using CashManager.Utils;

namespace CashManager.Features.Main
{
    public partial class AboutWindow : CustomWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            SoundPlayerHelper.PlaySound(SoundPlayerHelper.Sound.About);

            infoBlock.Text = $"Happy counting!{Environment.NewLine}{Environment.NewLine}Build date: "
                              + $"{File.GetLastWriteTime("CashManager.exe"):dd-MM-yyyy}";
        }

        protected override void RestoreOnClick(object sender, RoutedEventArgs e) { }

        private void WebsiteHyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Strings.WebsiteUrl);
        }

        private void ChangelogHyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Strings.ChangelogUrl);
        }
    }
}