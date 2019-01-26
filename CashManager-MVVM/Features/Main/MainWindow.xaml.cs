using System;
using System.Windows;

using CashManager.Logic.Wrappers;

using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Utils;

namespace CashManager_MVVM.Features.Main
{
    public partial class MainWindow : CustomWindow
    {
        public MainWindow(ApplicationViewModel viewModel)
        {
            DataContext = viewModel;

            using (new MeasureTimeWrapper(InitializeComponent, "MainWindow.Init")) { }

            SoundPlayerHelper.PlaySound(SoundPlayerHelper.Sound.AppStart);
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                Properties.Settings.Default.Width = (int)Width;
                Properties.Settings.Default.Height = (int)Height;
            }
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }
    }
}