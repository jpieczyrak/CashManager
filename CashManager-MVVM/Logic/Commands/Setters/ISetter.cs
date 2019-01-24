using System;
using System.Collections.Generic;

namespace CashManager_MVVM.Logic.Commands.Setters
{
    public interface ISetter<T>
    {
        void Execute(IEnumerable<T> elements, DateTime value);

        bool CanExecute();
    }
}