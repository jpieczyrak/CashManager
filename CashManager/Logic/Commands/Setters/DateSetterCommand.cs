using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Model;
using CashManager.Model.Common;
using CashManager.Model.Setters;

namespace CashManager.Logic.Commands.Setters
{
    public class DateSetterCommand : ISetter<Transaction>, ISetter<Position>
    {
        private readonly DateSetter _dateFrame;
        private readonly Action<IBookable, DateTime> _setterAction;

        private DateSetterCommand(DateSetter dateFrame, Action<IBookable, DateTime> setterAction)
        {
            _dateFrame = dateFrame;
            _setterAction = setterAction;
        }

        #region ISetter<Transaction>

        public bool CanExecute() => _dateFrame.IsChecked;

        #endregion

        public IEnumerable<Position> Execute(IEnumerable<Position> elements)
        {
            foreach (var position in elements) _setterAction(position, _dateFrame.Value);
            foreach (var transaction in elements.Select(x => x.Parent).Distinct()) _setterAction(transaction, _dateFrame.Value);
            return elements;
        }

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            foreach (var transaction in elements) _setterAction(transaction, _dateFrame.Value);
            return elements;
        }

        public static DateSetterCommand Create(DateSetter setter)
        {
            switch (setter.Type)
            {
                case DateSetterType.BookDate:
                    return new DateSetterCommand(setter, (bookable, time) => bookable.BookDate = time);
            }

            return null;
        }
    }
}