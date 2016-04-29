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
        public MainWindow()
        {
            InitializeComponent();
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version;
            


            //temp use:
            Wallet wallet = new Wallet();

            Stock mystock = new UserStock("Jejek", 10000);

            Stock FP = new Stock("FP");
            Stock proline = new Stock("Proline");

            wallet.AddStock(mystock);
            wallet.AddStock(FP);

            Transactions transactions = new Transactions();
            transactions.Add(new Transaction(eTransactionType.Transfer, DateTime.Now.Subtract(TimeSpan.FromHours(65)), 1000, 100, "Wypłata FP", "Note: Miesięczne wynagrodzenie", new Category("Wypłata"), new List<Tag>(), wallet.GetStockByName("FP"), mystock));
            transactions.Add(new Transaction(eTransactionType.Buy, DateTime.Now, 200, 100, "Dysk do kompa", "Note: Zakup części komputerowych", new Category("PC"), new List<Tag>(), wallet.GetStockByName("Jejek"), proline));


            DataGridTransactions.ItemsSource = transactions.ToArray();

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
    }
}
