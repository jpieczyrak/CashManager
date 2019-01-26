using System;
using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Model.Selectors;
using CashManager_MVVM.Model.Setters;

namespace CashManager_MVVM.Logic.Commands.Setters
{
    public class SingleSetterCommand : ISetter<Transaction>, ISetter<Position>
    {
        private readonly SingleSetter _setter;

        private SingleSetterCommand(SingleSetter setter) { _setter = setter; }

        #region ISetter<Transaction>

        public bool CanExecute() => _setter?.IsChecked == true && _setter.Selected?.Value != null;

        #endregion

        public IEnumerable<Position> Execute(IEnumerable<Position> elements)
        {
            if (!CanExecute()) return elements;
            if (_setter.Type == MultiPickerType.Category)
            {
                var setter = GetPositionAction();
                foreach (var position in elements) setter(position, _setter.Selected.Value);
            }
            else
            {
                Execute(elements.Select(x => x.Parent).Distinct());
            }
            return elements;
        }

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            if (!CanExecute()) return elements;
            var setter = GetTransactionAction();
            foreach (var transaction in elements) setter(transaction, _setter.Selected.Value);
            return elements;
        }

        public static SingleSetterCommand Create(SingleSetter setter) => new SingleSetterCommand(setter);

        private Action<Transaction, BaseObservableObject> GetTransactionAction()
        {
            switch (_setter.Type)
            {
                case MultiPickerType.Category:
                    return (transaction, o) =>
                    {
                        foreach (var position in transaction.Positions)
                            position.Category = o as Category;
                    };
                case MultiPickerType.UserStock:
                    return (transaction, o) => transaction.UserStock = o as Stock;
                case MultiPickerType.ExternalStock:
                    return (transaction, o) => transaction.ExternalStock = o as Stock;
                case MultiPickerType.TransactionType:
                    return (transaction, o) => transaction.Type = o as TransactionType;
            }

            return null;
        }

        private Action<Position, BaseObservableObject> GetPositionAction()
        {
            switch (_setter.Type)
            {
                case MultiPickerType.Category:
                    return (position, o) => position.Category = o as Category;
                case MultiPickerType.UserStock:
                    return (position, o) => position.Parent.UserStock = o as Stock;
                case MultiPickerType.ExternalStock:
                    return (position, o) => position.Parent.ExternalStock = o as Stock;
                case MultiPickerType.TransactionType:
                    return (position, o) => position.Parent.Type = o as TransactionType;
            }

            return null;
        }
    }
}