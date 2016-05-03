using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Logic;
using Logic.FilesOperations;
using Logic.StocksManagement;
using Logic.TransactionManagement;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Wallet _wallet;
        private readonly ObservableCollection<StockStats> _stockStats = new ObservableCollection<StockStats>(); 
        public MainWindow()
        {
            _wallet = Deserializer.DeserializeXML<Wallet>(Wallet.Path);
            _wallet.UpdateStockStats(_stockStats);
            
            InitializeComponent();
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version;

            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);

            DataContext = _wallet.Transactions;
            dataGridStockStats.ItemsSource = _stockStats;
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            Transaction transaction = new Transaction();
            _wallet.Transactions.Add(transaction);
            TransactionWindow window = new TransactionWindow(transaction, _wallet);
            window.Show();
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            TransactionWindow window = new TransactionWindow((Transaction)DataGridTransactions.SelectedItem, _wallet);
            window.Show();
        }

        private void DataGridTransactions_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            _wallet.Save();
        }

        private void DataGridTransactions_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DataGridTransactions.SelectedIndex = -1;
        }

        private void buttonParse_Click(object sender, RoutedEventArgs e)
        {
            ParserWindow window = new ParserWindow(_wallet);
            window.Show();
        }
        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.S)
            {
                _wallet.Save();
            }
        }

        private void buttonManageStocks_Click(object sender, RoutedEventArgs e)
        {
            ManageStocks window = new ManageStocks(_wallet);
            window.Show();
        }
    }
}
