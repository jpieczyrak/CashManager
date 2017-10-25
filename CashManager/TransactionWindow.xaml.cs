using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Logic;
using Logic.FindingFilters;
using Logic.LogicObjectsProviders;
using Logic.Model;
using Logic.StocksManagement;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for TransactionWindow.xaml
    /// </summary>
    public partial class TransactionWindow : Window
    {
        private readonly Wallet _wallet;
        private Transaction Transaction { get; set; }

        private ObservableCollection<Category> Categories { get; } 

        /// <summary>
        /// Creation window
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="wallet"></param>
        /// <param name="transactionDirection"></param>
        public TransactionWindow(Transaction transaction, Wallet wallet, eTransactionDirection transactionDirection)
        {
            _wallet = wallet;
            Transaction = transaction;
            //Transaction = transaction.Clone();
            InitializeComponent();

            //TODO: make copy of actual transaction

            DataContext = transaction;

            comboBoxContributionTypes.ItemsSource = Enum.GetValues(typeof(ePaymentType)).Cast<ePaymentType>();
            comboBoxContributionTypes.SelectedItem = ePaymentType.Percent;

            SetTransactionTypeCombobox(transactionDirection);

            SetSourceStocks();
            SetTargetStocks();
        }

        private void SetSourceStocks()
        {
            comboboxSourceStock.ItemsSource = StockProvider.GetStocks();
            if (comboboxSourceStock.Items.Count > 0)
            {
                comboboxSourceStock.SelectedIndex = 0;
            }
        }

        private void SetTargetStocks()
        {
            comboboxTargetStock.ItemsSource = StockProvider.GetStocks();
            if (comboboxTargetStock.Items.Count > 0)
            {
                comboboxTargetStock.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Sets proper available Transaction Type values for combobox (depends on transaction direction)
        /// </summary>
        /// <param name="transactionDirection"></param>
        private void SetTransactionTypeCombobox(eTransactionDirection transactionDirection)
        {
            switch (transactionDirection)
            {
                case eTransactionDirection.Income:
                    comboBoxTransactionType.ItemsSource = new List<eTransactionType>
                    {
                        eTransactionType.Work,
                        eTransactionType.Sell,
                        eTransactionType.Resell
                    };
                    comboBoxTransactionType.SelectedItem = eTransactionType.Work;
                    break;
                case eTransactionDirection.Outcome:
                    comboBoxTransactionType.ItemsSource = new List<eTransactionType>
                    {
                        eTransactionType.Buy,
                        eTransactionType.Reinvest
                    };
                    comboBoxTransactionType.SelectedItem = eTransactionType.Buy;
                    break;
                case eTransactionDirection.Transfer:
                    comboBoxTransactionType.ItemsSource = new List<eTransactionType>
                    {
                        eTransactionType.Transfer
                    };
                    comboBoxTransactionType.SelectedItem = eTransactionType.Transfer;
                    break;
                case eTransactionDirection.Uknown:
                    //We are editing some transaction so we can read actual transaction type
                    comboBoxTransactionType.ItemsSource = Enum.GetValues(typeof (eTransactionType)).Cast<eTransactionType>();
                    comboBoxTransactionType.SelectedItem = Transaction.Type;
                    break;
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
            _wallet.Save();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            //TODO: restore transaction from copy
            //Close();
        }

        private void comboBoxTransactionType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboBoxTransactionType.SelectedIndex > -1)
            {
                Transaction.Type = (eTransactionType) comboBoxTransactionType.SelectedItem;
            }
        }

        private void buttonAddSource_Click(object sender, RoutedEventArgs e)
        {
            double value;
            double.TryParse(textBoxSourceValue.Text, out value);
            textBoxSourceValue.Text = "";

            Stock sourceStock = (Stock) (comboboxSourceStock.SelectedIndex >= 0 ? comboboxSourceStock.SelectedItem : StockProvider.Default);

            Transaction.Payment = new Payment(sourceStock, sourceStock, value);
        }

        private void comboboxTargetStock_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
