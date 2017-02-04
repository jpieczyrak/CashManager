using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Logic;
using Logic.FilesOperations;
using Logic.FindingFilters;
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
            //_dataContext.Wallet.UpdateStockStats(_dataContext.StockStats, _dataContext.Timeframe);

            InitializeComponent();
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version;

            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);

            DataContext = _dataContext;
            dataGridStockStats.ItemsSource = _dataContext.StockStats;
            DataGridTransactions.ItemsSource = _dataContext.Wallet.Transactions.TransactionsList;

            CategoryProvider.Load(_dataContext.Wallet.Transactions.TransactionsList);

            _dataContext.Wallet.Transactions.TransactionsList.Where(x=>x.Title != null && x.Title.Contains("Obiad")).Sum(x => x.ValueAsProfit);
        }

        private void AddTransactionButtonClick(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button)?.Name;
            Transaction transaction = new Transaction();
            _dataContext.Wallet.Transactions.Add(transaction);
            TransactionWindow window = null;

            if (name.ToLower().Contains("income"))
            {
                window =  new TransactionWindow(transaction, _dataContext.Wallet, eTransactionDirection.Income);
            }
            else if (name.ToLower().Contains("outcome"))
            {
                window = new TransactionWindow(transaction, _dataContext.Wallet, eTransactionDirection.Outcome);
            }
            else if (name.ToLower().Contains("transfer"))
            {
                window = new TransactionWindow(transaction, _dataContext.Wallet, eTransactionDirection.Transfer);
            }
            
            window?.Show();
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            TransactionWindow window = new TransactionWindow((Transaction)DataGridTransactions.SelectedItem, _dataContext.Wallet, eTransactionDirection.Uknown);
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

        private void buttonManageCategories_Click(object sender, RoutedEventArgs e)
        {
            ManageCategoriesWindow window = new ManageCategoriesWindow();
            window.Show();
        }
    }
}
