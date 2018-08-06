using System.Windows;

using CashManager_MVVM.Model;
using CashManager_MVVM.ViewModel;

namespace CashManager_MVVM.View
{
    /// <summary>
    /// Interaction logic for TransactionView.xaml
    /// </summary>
    public partial class TransactionView : Window
    {
        public TransactionView(Transaction transaction, TransactionViewModel transactionViewModel)
        {
            InitializeComponent();
			DataContext = transactionViewModel;
			transactionViewModel.Transaction = transaction;
        }
    }
}
