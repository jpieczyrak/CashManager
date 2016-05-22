using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Logic;
using Logic.FindingFilters;
using Logic.Parsing;
using Logic.StocksManagement;
using Logic.TransactionManagement;

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
    }
}
