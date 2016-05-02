using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Logic;
using Logic.StocksManagement;
using Logic.TransactionManagement;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for TransactionWindow.xaml
    /// </summary>
    public partial class TransactionWindow : Window
    {
        private readonly Wallet _wallet;
        private Transaction Transaction { get; set; }

        public TransactionWindow(Transaction transaction, Wallet wallet)
        {
            _wallet = wallet;
            Transaction = transaction;     //make copy not copy ref!
            InitializeComponent();

            //TODO: make copy of actual transaction

            DataContext = transaction;

            comboBoxContributionTypes.ItemsSource = Enum.GetValues(typeof(ePaymentType)).Cast<ePaymentType>();
            comboBoxContributionTypes.SelectedItem = Transaction.ContributionType;

            comboBoxTransactionType.ItemsSource = Enum.GetValues(typeof(eTransactionType)).Cast<eTransactionType>();
            comboBoxTransactionType.SelectedItem = Transaction.Type;

            comboboxSourceStock.ItemsSource = wallet.AvailableStocks;
            comboboxTargetStock.ItemsSource = wallet.AvailableStocks;

            dataGridSubtransactions.ItemsSource = Transaction.Subtransactions;

            //            comboBoxContributionTypes.ItemsSource = Transaction.StocksList;

            //dataGridSources.ItemsSource = Transaction.TransactionSoucePayments.TransactionPartPayments;

            //dataGridTargets.ItemsSource = Transaction.TransactionTargetPayments;

            //dataGridSubtransactions.ItemsSource = transaction.Subtransactions;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            //TODO: restore transaction from copy
            //Close();
        }

        private void comboBoxContributionTypes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboBoxContributionTypes.SelectedIndex > -1)
            {
                Transaction.ContributionType = (ePaymentType) comboBoxContributionTypes.SelectedItem;
            }
        }

        private void comboBoxTransactionType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboBoxTransactionType.SelectedIndex > -1)
            {
                Transaction.Type = (eTransactionType)comboBoxTransactionType.SelectedItem;
            }
        }

        private void buttonAddSource_Click(object sender, RoutedEventArgs e)
        {
            double value = 0;
            double.TryParse(textBoxSourceValue.Text, out value);
            textBoxSourceValue.Text = "";

            string sourceStock = comboboxSourceStock.SelectedIndex >= 0 ? comboboxSourceStock.SelectedItem.ToString() : "";

            Transaction.TransactionSoucePayments.Add(new TransactionPartPayment(sourceStock, value));
        }

        private void buttonAddTarget_Click(object sender, RoutedEventArgs e)
        {
            double value = 0;
            double.TryParse(textBoxTargetValue.Text, out value);
            textBoxTargetValue.Text = "";

            string targetStock = comboboxTargetStock.SelectedIndex >= 0 ? comboboxTargetStock.SelectedItem.ToString() : "";

            Transaction.TransactionTargetPayments.Add(new TransactionPartPayment(targetStock, value));
        }
    }
}
