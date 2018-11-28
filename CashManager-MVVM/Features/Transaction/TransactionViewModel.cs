using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Data;
using CashManager.Data.DTO;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.Features.Category;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CashManager_MVVM.Features.Transaction
{
	public class TransactionViewModel : ViewModelBase
	{
	    private readonly IQueryDispatcher _queryDispatcher;
	    private readonly ICommandDispatcher _commandDispatcher;
	    private readonly Func<Type, ViewModelBase> _factory;
		private readonly IEnumerable<Model.Stock> _stocks;
		private Model.Transaction _transaction;

		public TransactionViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, Func<Type, ViewModelBase> factory)
		{
		    _queryDispatcher = queryDispatcher;
		    _commandDispatcher = commandDispatcher;
		    _factory = factory;

		    var dtos = _queryDispatcher.Execute<StockQuery, Stock[]>(new StockQuery());
            _stocks = dtos.Select(Mapper.Map<Model.Stock>);

			ChooseCategoryCommand = new RelayCommand<Model.Position>(position =>
			{
				var viewmodel = _factory.Invoke(typeof(CategoryViewModel)) as CategoryViewModel;
				var window = new CategoryPickerView(viewmodel, position.Category);
				window.Show();
				window.Closing += (sender, args) =>
				{
				    position.Category = viewmodel?.SelectedCategory;
                    _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<CashManager.Data.DTO.Transaction>(_transaction)));
				};
			});
		}

		public IEnumerable<eTransactionType> TransactionTypes => Enum.GetValues(typeof(eTransactionType)).Cast<eTransactionType>();

		public Model.Transaction Transaction
		{
			get => _transaction;
			set => Set(nameof(Transaction), ref _transaction, value);
		}

		public IEnumerable<Model.Stock> ExternalStocks => _stocks.Where(x => !x.IsUserStock);

		public IEnumerable<Model.Stock> UserStocks => _stocks.Where(x => x.IsUserStock);

		public RelayCommand<Model.Position> ChooseCategoryCommand { get; set; }
	}
}
