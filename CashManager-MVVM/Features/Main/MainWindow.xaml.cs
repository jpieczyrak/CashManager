using System.Reflection;
using System.Windows;

namespace CashManager_MVVM.Features.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow(ApplicationViewModel viewModel)
		{
			DataContext = viewModel;
            InitializeComponent();
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}