using System;
using System.Reflection;
using System.Windows;

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
            Application.Current.Shutdown();
        }
    }
}