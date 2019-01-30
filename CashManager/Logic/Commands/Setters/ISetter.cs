using System.Collections.Generic;

namespace CashManager.Logic.Commands.Setters
{
    public interface ISetter<T>
    {
        IEnumerable<T> Execute(IEnumerable<T> elements);

        bool CanExecute();
    }
}