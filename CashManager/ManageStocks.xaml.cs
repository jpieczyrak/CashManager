using System.Windows;
using Logic;
using Logic.StocksManagement;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for ManageStocks.xaml
    /// </summary>
    public partial class ManageStocks : Window
    {
        private readonly Wallet _wallet;

        public ManageStocks(Wallet wallet)
        {
            _wallet = wallet;
            DataContext = wallet;
            InitializeComponent();
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            Close(); 
        }

        private void buttonAddEmpty_Click(object sender, RoutedEventArgs e)
        {
            _wallet.AvailableStocks.Add(new Stock("", 0));
        }
    }
}
