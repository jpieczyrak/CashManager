using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using Logic;
using Logic.Parsing;
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
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            TransactionWindow window = new TransactionWindow((Transaction)DataGridTransactions.SelectedItem, _wallet);
            window.Show();
        }

        private void buttonParse_Click(object sender, RoutedEventArgs e)
        {
            string input = textBoxDataToParse.Text;

            IParser parser = ParserFactory.Create(eParserInputType.Excel);

            List<Transaction> parsedTransactions = parser.Parse(input, _wallet.AvailableStocks);
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
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
