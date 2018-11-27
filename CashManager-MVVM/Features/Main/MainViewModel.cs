using CashManager_MVVM.Features.Transaction;
using CashManager_MVVM.Model.DataProviders;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Main
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly ViewModelFactory _factory;

        public TrulyObservableCollection<Model.Transaction> Transactions { get; set; }

		public RelayCommand TransactionEditCommand => new RelayCommand(TransactionEdit, () => true);

		private void TransactionEdit()
		{
			var window = new TransactionView(SelectedTransaction, _factory.Create<TransactionViewModel>())
			{
				Title = SelectedTransaction?.Title ?? string.Empty,
				//Left = mainWindow.Left + mainWindow.Width,
				//Top = mainWindow.Top
			};
			window.Show();
		}

		public Model.Transaction SelectedTransaction { get; set; }

        public MainViewModel(IDataService dataService, ViewModelFactory factory)
        {
			_factory = factory;
            _dataService = dataService;
            _dataService.GetTransactions(
                (transactions, error) =>
                {
                    if (error != null) return;
                    Transactions = new TrulyObservableCollection<Model.Transaction>(transactions);
                });
        }
    }
}