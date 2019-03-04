using System;
using System.IO;
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

            infoBlock.Text += $"{Environment.NewLine}{Environment.NewLine}Build date: "
                              + $"{File.GetLastWriteTime("CashManager.exe"):dd-MM-yyyy}";
        }

        protected override void RestoreOnClick(object sender, RoutedEventArgs e) { }
    }
}