using System.Reflection;
using System.Windows;
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
        private readonly Wallet _wallet;
        public MainWindow()
        {
            _wallet = Deserializer.DeserializeXML<Wallet>(Wallet.Path);
            DataContext = _wallet.Transactions;
            
            InitializeComponent();
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version;
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
    }
}
