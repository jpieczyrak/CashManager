using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Logic;
using Logic.FilesOperations;
using Logic.TransactionManagement;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowDataContext _dataContext = new MainWindowDataContext();
        public MainWindow()
        {
            _dataContext.Wallet = Deserializer.DeserializeXML<Wallet>(Wallet.Path);
            _dataContext.Timeframe = new TimeFrame(DateTime.Now.AddYears(-5), DateTime.Now);
            _dataContext.Wallet.UpdateStockStats(_dataContext.StockStats, _dataContext.Timeframe);
            
            InitializeComponent();
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version;

            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);

            DataContext = _dataContext;
            dataGridStockStats.ItemsSource = _dataContext.StockStats;
            
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            Transaction transaction = new Transaction();
            _dataContext.Wallet.Transactions.Add(transaction);
            TransactionWindow window = new TransactionWindow(transaction, _dataContext.Wallet);
            window.Show();
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            TransactionWindow window = new TransactionWindow((Transaction)DataGridTransactions.SelectedItem, _dataContext.Wallet);
            window.Show();
        }

        private void DataGridTransactions_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            _dataContext.Wallet.Save();
        }

        private void DataGridTransactions_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DataGridTransactions.SelectedIndex = -1;
        }

        private void buttonParse_Click(object sender, RoutedEventArgs e)
        {
            ParserWindow window = new ParserWindow(_dataContext.Wallet);
            window.Show();
        }
        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.S)
            {
                _dataContext.Wallet.Save();
            }
            if (Keyboard.IsKeyDown(Key.F5))
            {
                _dataContext.Wallet.UpdateStockStats(_dataContext.StockStats, _dataContext.Timeframe);
            }
        }

        private void buttonManageStocks_Click(object sender, RoutedEventArgs e)
        {
            ManageStocks window = new ManageStocks(_dataContext.Wallet);
            window.Show();
        }

        private void buttonFind_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.Wallet.UpdateStockStats(_dataContext.StockStats, _dataContext.Timeframe);
        }
    }
}
