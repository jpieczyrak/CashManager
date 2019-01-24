using System;
using System.Collections.Generic;

namespace CashManager_MVVM.Logic.Commands.Setters
{
    public interface ISetter<T>
    {
        IEnumerable<T> Execute(IEnumerable<T> elements);

        bool CanExecute();
    }
}