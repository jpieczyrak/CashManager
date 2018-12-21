using System.Collections.Generic;
using System.ComponentModel;

namespace CashManager_MVVM.Logic.Commands
{
    public interface IFilter<T> : INotifyPropertyChanged
    {
        IEnumerable<T> Execute(IEnumerable<T> elements);

        bool CanExecute();
    }
}