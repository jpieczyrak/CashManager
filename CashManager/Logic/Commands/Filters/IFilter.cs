using System.Collections.Generic;
using System.ComponentModel;

namespace CashManager.Logic.Commands.Filters
{
    public interface IFilter<T> : INotifyPropertyChanged
    {
        IEnumerable<T> Execute(IEnumerable<T> elements);

        bool CanExecute();
    }
}