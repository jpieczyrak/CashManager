using System;

using Logic.Utils;

namespace CashManager_MVVM.Model.DataProviders
{
    public interface IDataService
    {
        void GetData(Action<TrulyObservableCollection<Transaction>, Exception> callback);
    }
}
