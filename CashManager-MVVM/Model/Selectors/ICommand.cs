using System.Collections.Generic;

using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model.Selectors
{
    public interface ICommand
    {
        IEnumerable<IBookable> Execute(IEnumerable<IBookable> elements);

        bool CanExecute();
    }
}