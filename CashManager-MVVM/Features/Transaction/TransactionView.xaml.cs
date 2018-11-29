using System.Windows.Controls;

namespace CashManager_MVVM.Features.Transaction
{
    /// <summary>
    /// Interaction logic for TransactionView.xaml
    /// </summary>
    public partial class TransactionView : UserControl
    {
        public TransactionView(Model.Transaction transaction, TransactionViewModel transactionViewModel)
        {
            InitializeComponent();
			DataContext = transactionViewModel;
			transactionViewModel.Transaction = transaction;
        }
    }
}
