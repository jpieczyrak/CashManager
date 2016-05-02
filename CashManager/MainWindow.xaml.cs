using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Logic;
using Logic.StocksManagement;
using Logic.TransactionManagement;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Transaction _incomeTransaction;
        private readonly Wallet _wallet = new Wallet();

        private Transactions Transactions { get; set; } = new Transactions();

        public MainWindow()
        {
            //temp use:


            Stock mystock = new UserStock("Jejek", 10000);

            Stock FP = new Stock("FP");
            Stock proline = new Stock("Proline");

            _wallet.AddStock(mystock);
            _wallet.AddStock(FP);
            InitializeComponent();
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version;

            _incomeTransaction = new Transaction(eTransactionType.Work, DateTime.Now.Subtract(TimeSpan.FromHours(65)), 100, "Wypłata FP", "Note: Miesięczne wynagrodzenie");
            _incomeTransaction.TransactionSoucePayments.Add(new TransactionPartPayment(FP.ToString(), 1000));
            _incomeTransaction.TransactionTargetPayments.Add(new TransactionPartPayment(mystock.ToString(), 1000));
            _incomeTransaction.Subtransactions.Add(new Subtransation() { Category = new Category("Praca"), Value = 1000, Name = "Wypłata", Tags = "FP;praca"});

            Transactions.Add(_incomeTransaction);
            Transactions.Add(new Transaction(eTransactionType.Buy, DateTime.Now, 200, 100, "Dysk do kompa", "Note: Zakup części komputerowych"));


            DataGridTransactions.ItemsSource = Transactions.TransactionsList;

            //CollectionViewSource itemCollectionViewSource = (CollectionViewSource)FindResource("ItemCollectionViewSource");
            //itemCollectionViewSource.Source = x.ToArray();
        }

        private void DataGridTransactions_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            string content = "";
            foreach (object o in DataGridTransactions.ItemsSource)
            {
                content += o.ToString() + "\n";
            }
            File.WriteAllText(@"D:\test.txt", content);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Transaction transaction = new Transaction();
            Transactions.Add(transaction);
            TransactionWindow window = new TransactionWindow(transaction, _wallet);
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TransactionWindow window = new TransactionWindow((Transaction) DataGridTransactions.SelectedItem, _wallet);
            window.Show();
        }
    }
}
