using System;
using System.Collections.Generic;

namespace CashManager_MVVM.Logic.Commands
{
    public interface IReplacer<T>
    {
        void Execute(IEnumerable<T> elements, DateTime value);

        bool CanExecute();
    }
}