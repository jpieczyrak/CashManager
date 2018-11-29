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
    }
}