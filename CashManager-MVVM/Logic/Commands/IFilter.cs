using System.Collections.Generic;

namespace CashManager_MVVM.Logic.Commands
{
    public interface IFilter<T>
    {
        IEnumerable<T> Execute(IEnumerable<T> elements);

        bool CanExecute();
    }
}