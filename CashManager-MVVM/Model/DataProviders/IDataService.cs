using System;
using System.Collections;
using System.Collections.Generic;

using Logic.Utils;

namespace CashManager_MVVM.Model.DataProviders
{
    public interface IDataService
    {
        void GetData(Action<TrulyObservableCollection<Transaction>, Exception> callback);

        void GetStocks(Action<IEnumerable<Stock>, Exception> callback);
    }
}
