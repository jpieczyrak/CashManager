using System;
using System.Reflection;
using System.Windows;

using CashManager_MVVM.Properties;

namespace CashManager_MVVM.Features.Main
{
    public partial class MainWindow : Window
    {
        public MainWindow(ApplicationViewModel viewModel)
		{
			DataContext = viewModel;
            InitializeComponent();
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Settings.Default.Save();
            Application.Current.Shutdown();
        }
    }
}