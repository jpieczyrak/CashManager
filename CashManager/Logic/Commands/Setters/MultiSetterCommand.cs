using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Model;
using CashManager.Model.Common;
using CashManager.Model.Selectors;
using CashManager.Model.Setters;

namespace CashManager.Logic.Commands.Setters
{
    public class MultiSetterCommand : ISetter<Transaction>, ISetter<Position>
    {
        private readonly MultiSetter _setter;

        private MultiSetterCommand(MultiSetter setter) { _setter = setter; }

        #region ISetter<Transaction>

        public bool CanExecute() => _setter?.IsChecked == true && (_setter.Results?.Any() ?? false);

        #endregion

        public IEnumerable<Position> Execute(IEnumerable<Position> elements)
        {
            if (!CanExecute()) return elements;
            var setter = GetPositionAction();
            var selected = _setter.Results.Select(x => x.Value).ToArray();
            foreach (var position in elements) setter(position, selected);
            return elements;
        }

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            if (!CanExecute()) return elements;
            var setter = GetTransactionAction();
            var selected = _setter.Results.Select(x => x.Value).ToArray();
            foreach (var transaction in elements) setter(transaction, selected);
            return elements;
        }

        public static MultiSetterCommand Create(MultiSetter setter) => new MultiSetterCommand(setter);

        private Action<Transaction, BaseObservableObject[]> GetTransactionAction()
        {
            switch (_setter.Type)
            {
                case MultiPickerType.Tag:
                    if (_setter.Append)
                        return (transaction, o) =>
                        {
                            foreach (var position in transaction.Positions)
                                position.Tags = position.Tags.Concat(o.OfType<Tag>()).Distinct().ToArray();
                        };
                    else
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
            switch (_setter.Type)
            {
                case MultiPickerType.Tag:
                    if (_setter.Append)
                        return (position, o) => position.Tags = position.Tags.Concat(o.OfType<Tag>()).Distinct().ToArray();
                    else
                        return (position, o) => position.Tags = o.OfType<Tag>().ToArray();
            }

            return null;
        }
    }
}