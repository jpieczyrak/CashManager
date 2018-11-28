using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Transactions;

using CashManager_MVVM.Features.Transaction;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Main
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
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

        public MainViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            _factory = factory;
            var items = _queryDispatcher.Execute<TransactionQuery, CashManager.Data.DTO.Transaction[]>(new TransactionQuery());
            var transactions = items.Select(Mapper.Map<Model.Transaction>).ToArray();
            Transactions = new TrulyObservableCollection<Model.Transaction>(transactions);
        }
    }
}