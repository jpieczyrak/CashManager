using System.Collections.Generic;

namespace CashManager.WPF.Logic.Commands.Setters
{
    public interface ISetter<T>
    {
        IEnumerable<T> Execute(IEnumerable<T> elements);

        bool CanExecute();
    }
}