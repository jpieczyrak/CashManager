using System;
using System.Windows;

using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Properties;

using log4net;

namespace CashManager_MVVM.Features.Main
{
    public partial class MainWindow : CustomWindow
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(MainWindow)));

        public MainWindow(ApplicationViewModel viewModel)
        {
            _logger.Value.Debug("Loading");
            DataContext = viewModel;
            InitializeComponent();
            _logger.Value.Debug("Loaded");
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            Settings.Default.Save();
            Application.Current.Shutdown();
        }
    }
}