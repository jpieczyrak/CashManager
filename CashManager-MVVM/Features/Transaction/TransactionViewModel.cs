using System;
using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Features.Category;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.DataProviders;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Logic.TransactionManagement.TransactionElements;

namespace CashManager_MVVM.Features.Transaction
{
	public class TransactionViewModel : ViewModelBase
	{
		private readonly Func<Type, ViewModelBase> _factory;
		private IEnumerable<Stock> _stocks;
		private Model.Transaction _transaction;

		public TransactionViewModel(IDataService dataService, Func<Type, ViewModelBase> factory)
		{
			_factory = factory;
			dataService.GetStocks((stocks, exception) => { _stocks = stocks; });
			ChooseCategoryCommand = new RelayCommand<Subtransaction>(subtransaction =>
			{
				var viewmodel = _factory.Invoke(typeof(CategoryViewModel)) as CategoryViewModel;
				var window = new CategoryPickerView(viewmodel, subtransaction.Category);
				window.Show();
				window.Closing += (sender, args) => { subtransaction.Category = viewmodel?.SelectedCategory; };
			});
		}

		public IEnumerable<eTransactionType> TransactionTypes => Enum.GetValues(typeof(eTransactionType)).Cast<eTransactionType>();

		public Model.Transaction Transaction
		{
			get => _transaction;
			set => Set(nameof(Transaction), ref _transaction, value);
		}

		public IEnumerable<Stock> ExternalStocks => _stocks.Where(x => !x.IsUserStock);

		public IEnumerable<Stock> UserStocks => _stocks.Where(x => x.IsUserStock);

		public RelayCommand<Subtransaction> ChooseCategoryCommand { get; set; }
	}
}
