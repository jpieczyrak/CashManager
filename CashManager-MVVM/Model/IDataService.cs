using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Logic.Model;
using Logic.Utils;

namespace CashManager_MVVM.Model
{
    public interface IDataService
    {
        void GetData(Action<TrulyObservableCollection<Transaction>, Exception> callback);
    }
}
