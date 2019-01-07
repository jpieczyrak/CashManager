using System;
using System.Reflection;
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
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version;
            _logger.Value.Debug("Loaded");
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Settings.Default.Save();
            Application.Current.Shutdown();
        }
    }
}