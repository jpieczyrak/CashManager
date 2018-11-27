using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using LogicOld;
using LogicOld.FindingFilters;
using LogicOld.LogicObjectsProviders;
using LogicOld.Model;
using LogicOld.TransactionManagement.TransactionElements;

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
            
            SetTransactionTypeCombobox(transactionDirection);

			comboboxUserStock.ItemsSource = StockProvider.GetStocks().Where(x => x.IsUserStock).ToArray();
			comboboxTargetStock.ItemsSource = StockProvider.GetStocks().Where(x => !x.IsUserStock).ToArray();
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
	}
}
