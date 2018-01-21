using System;
using System.Collections.Generic;

namespace CashManager_MVVM.Model.DataProviders
{
    public interface IDataService
    {
        void GetTransactions(Action<IEnumerable<Transaction>, Exception> callback);
        void GetCategories(Action<IEnumerable<Category>, Exception> callback);

        void GetStocks(Action<IEnumerable<Stock>, Exception> callback);
    }
}
