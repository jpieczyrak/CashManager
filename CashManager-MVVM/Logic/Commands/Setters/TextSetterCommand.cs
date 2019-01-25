using System;
using System.Collections.Generic;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Setters;

namespace CashManager_MVVM.Logic.Commands.Setters
{
    public class TextSetterCommand : ISetter<Transaction>, ISetter<Position>
    {
        private readonly TextSetter _textSetter;

        private TextSetterCommand(TextSetter textSetter)
        {
            _textSetter = textSetter;
        }

        #region ISetter<Position>

        public IEnumerable<Position> Execute(IEnumerable<Position> elements)
        {
            var action = GetPositionAction(_textSetter.Type);
            foreach (var position in elements) action(position, _textSetter.Value);
            return elements;
        }

        #endregion

        #region ISetter<Transaction>

        public bool CanExecute() => _textSetter.IsChecked;

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            var action = GetTransactionAction(_textSetter.Type);
            foreach (var transaction in elements) action(transaction, _textSetter.Value);
            return elements;
        }

        #endregion

        public static TextSetterCommand Create(TextSetter setter)
        {
            switch (setter.Type)
            {
                case TextSetterType.Title:
                    return new TextSetterCommand(setter);
                case TextSetterType.Note:
                    break;
                case TextSetterType.PositionTitle:
                    break;
            }

            return null;
        }

        private Action<Transaction, string> GetTransactionAction(TextSetterType type)
        {
            switch (type)
            {
                case TextSetterType.Title:
                    return (transaction, s) => transaction.Title = s;
                case TextSetterType.Note:
                    break;
                case TextSetterType.PositionTitle:
                    break;
            }

            return null;
        }

        private Action<Position, string> GetPositionAction(TextSetterType type)
        {
            switch (type)
            {
                case TextSetterType.Title:
                    return (position, s) => position.Title = s;
                case TextSetterType.Note:
                    break;
                case TextSetterType.PositionTitle:
                    break;
            }

            return null;
        }
    }
}