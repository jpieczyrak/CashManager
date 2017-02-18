using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Logic;
using Logic.FindingFilters;
using Logic.LogicObjectsProviders;
using Logic.Parsing;
using Logic.StocksManagement;
using Logic.TransactionManagement;
using Logic.TransactionManagement.BulkModifications;
using Logic.TransactionManagement.TransactionElements;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for ParserWindow.xaml
    /// </summary>
    public partial class ParserWindow : Window
    {
        private readonly Wallet _wallet;

        private readonly Transactions _transactions = new Transactions();

        public ParserWindow(Wallet wallet)
        {
            _wallet = wallet;
            InitializeComponent();

            DataContext = _transactions;

            comboboxUserStock.ItemsSource = StockProvider.GetStocks();
            comboboxInputType.ItemsSource = Enum.GetValues(typeof(eParserInputType)).Cast<eParserInputType>();

            if (comboboxUserStock.Items.Count > 0) comboboxUserStock.SelectedIndex = 0;
            if (comboboxInputType.Items.Count > 0) comboboxInputType.SelectedIndex = 0;
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            TransactionWindow window = new TransactionWindow((Transaction)DataGridTransactions.SelectedItem, _wallet, eTransactionDirection.Uknown);
            window.Show();
        }

        private void buttonParse_Click(object sender, RoutedEventArgs e)
        {
            string input = textBoxDataToParse.Text;

            if (comboboxUserStock.SelectedIndex < 0)    return;
            if (comboboxInputType.SelectedIndex < 0)    return;

            Stock userStock = (Stock) comboboxUserStock.SelectedItem;
            eParserInputType parserInputType = (eParserInputType) comboboxInputType.SelectedItem;
            IParser parser = ParserFactory.Create(parserInputType);

            List <Transaction> parsedTransactions = parser.Parse(input, userStock);
            foreach (Transaction transaction in parsedTransactions)
            {
                _transactions.TransactionsList.Add(transaction);
            }

            if (parsedTransactions.Count > 0)
            {
                buttonApplyRules.IsEnabled = true;
            }
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            foreach (var transaction in _transactions.TransactionsList)
            {
                _wallet.Transactions.Add(transaction);
            }
            _wallet.Save();
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonApplyRules_Click(object sender, RoutedEventArgs e)
        {
            //apply stored rules

            //now only hardcode:
            Func<Transaction, bool> PBDinner = x => x.Title.Contains("PROGRESS BAR");
            Action<Transaction> PRTitle = x => x.Title = "Obiad FP";
            //Action<Transaction> PBCategory = x => x.Subtransactions[0].Category = new StringWrapper("Jedzenie w FP");
            BulkTransactionParametersChanger.Change(_transactions, PBDinner,
            new []{ PRTitle
                //, PBCategory 
            });

            Func<Transaction, bool> FPIncome = x => x.Title.Contains("Wynagrodzenie z tytulu umowy cywilnoprawnej");
            Action<Transaction> FPCategory = x => x.Subtransactions[0].Category = new StringWrapper("Wypłata");
            BulkTransactionParametersChanger.Change(_transactions, FPIncome,
            new[] { FPCategory });
        }
    }
}
