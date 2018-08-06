using CashManager_MVVM.Model.DataProviders;
using CashManager_MVVM.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using Logic.Utils;

using Transaction = CashManager_MVVM.Model.Transaction;

namespace CashManager_MVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly ViewModelFactory _factory;

        public TrulyObservableCollection<Transaction> Transactions { get; set; }

		public RelayCommand TransactionEditCommand => new RelayCommand(TranactionEdit, () => true);

		private void TranactionEdit()
		{
			var window = new TransactionView(SelectedTransaction, _factory.Create<TransactionViewModel>())
			{
				Title = SelectedTransaction?.Title ?? string.Empty,
				//Left = mainWindow.Left + mainWindow.Width,
				//Top = mainWindow.Top
			};
			window.Show();
		}

		public Transaction SelectedTransaction { get; set; }

        public MainViewModel(IDataService dataService, ViewModelFactory factory)
        {
			_factory = factory;
            _dataService = dataService;
            _dataService.GetTransactions(
                (transactions, error) =>
                {
                    if (error != null) return;
                    Transactions = new TrulyObservableCollection<Transaction>(transactions);
                });
        }
    }
}