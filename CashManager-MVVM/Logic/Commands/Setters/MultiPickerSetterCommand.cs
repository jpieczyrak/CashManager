using System;
using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Model.Selectors;

namespace CashManager_MVVM.Logic.Commands.Setters
{
    public class MultiPickerSetterCommand : ISetter<Transaction>, ISetter<Position>
    {
        private readonly MultiPicker _selector;

        private MultiPickerSetterCommand(MultiPicker selector) { _selector = selector; }

        #region ISetter<Transaction>

        public bool CanExecute() => _selector?.IsChecked == true && (_selector.Results?.Any() ?? false);

        #endregion

        public IEnumerable<Position> Execute(IEnumerable<Position> elements)
        {
            if (!CanExecute()) return elements;
            var setter = GetPositionAction();
            var selected = _selector.Results.Select(x => x.Value).ToArray();
            foreach (var position in elements) setter(position, selected);
            return elements;
        }

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            if (!CanExecute()) return elements;
            var setter = GetTransactionAction();
            var selected = _selector.Results.Select(x => x.Value).ToArray();
            foreach (var transaction in elements) setter(transaction, selected);
            return elements;
        }

        public static MultiPickerSetterCommand Create(MultiPicker selector) => new MultiPickerSetterCommand(selector);

        private Action<Transaction, BaseObservableObject[]> GetTransactionAction()
        {
            switch (_selector.Type)
            {
                case MultiPickerType.Tag:
                    return (transaction, o) =>
                    {
                        foreach (var position in transaction.Positions)
                            position.Tags = o.OfType<Tag>().ToArray();
                    };
            }

            return null;
        }

        private Action<Position, BaseObservableObject[]> GetPositionAction()
        {
            switch (_selector.Type)
            {
                case MultiPickerType.Tag:
                    return (position, o) => position.Tags = o.OfType<Tag>().ToArray();
            }

            return null;
        }
    }
}