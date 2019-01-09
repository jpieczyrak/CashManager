using System;
using System.Windows;

using CashManager_MVVM.Properties;

using log4net;

namespace CashManager_MVVM.Features.Main
{
    public partial class MainWindow : Window
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(MainWindow)));

        public MainWindow(ApplicationViewModel viewModel)
        {
            _logger.Value.Debug("Loading");
            DataContext = viewModel;
            InitializeComponent();
            _logger.Value.Debug("Loaded");
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Settings.Default.Save();
            Application.Current.Shutdown();
        }

        private void MinimizeOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseOnClick(object sender, RoutedEventArgs e)
        {
            OnClosed(sender, e);
        }

        private void RestoreOnClick(object sender, RoutedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
            }
        }
    }
}