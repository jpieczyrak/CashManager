using System.Windows;

using Logic;
using Logic.LogicObjectsProviders;
using Logic.Model;

namespace CashManager
{
    /// <summary>
    ///     Interaction logic for ManageStocks.xaml
    /// </summary>
    public partial class ManageStocks : Window
    {
        private readonly Wallet _wallet;

        public ManageStocks(Wallet wallet)
        {
            _wallet = wallet;
            //DataContext = wallet;
            InitializeComponent();

            dataGridStocks.ItemsSource = StockProvider.Stocks;
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            _wallet.Save();
            Close();
        }

        private void buttonAddEmpty_Click(object sender, RoutedEventArgs e)
        {
            StockProvider.AddNew("");
        }
    }
}