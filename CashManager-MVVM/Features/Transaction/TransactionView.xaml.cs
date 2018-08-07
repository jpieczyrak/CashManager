using System.Windows;

namespace CashManager_MVVM.Features.Transaction
{
    /// <summary>
    /// Interaction logic for TransactionView.xaml
    /// </summary>
    public partial class TransactionView : Window
    {
        public TransactionView(Model.Transaction transaction, TransactionViewModel transactionViewModel)
        {
            InitializeComponent();
			DataContext = transactionViewModel;
			transactionViewModel.Transaction = transaction;
        }
    }
}
