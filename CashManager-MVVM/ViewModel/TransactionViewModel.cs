using System;
using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.DataProviders;

using GalaSoft.MvvmLight;

using Logic.TransactionManagement.TransactionElements;

namespace CashManager_MVVM.ViewModel
{
    public class TransactionViewModel : ViewModelBase
    {
        private IEnumerable<Stock> _stocks;

        public IEnumerable<eTransactionType> TransactionTypes => Enum.GetValues(typeof(eTransactionType)).Cast<eTransactionType>();

        public Transaction Transaction { get; set; }

        public IEnumerable<Stock> ExternalStocks => _stocks.Where(x => !x.IsUserStock);

        public IEnumerable<Stock> UserStocks => _stocks.Where(x => x.IsUserStock);
        
        public TransactionViewModel(IDataService dataService)
        {
            dataService.GetStocks((stocks, exception) => { _stocks = stocks; });
        }
    }
}