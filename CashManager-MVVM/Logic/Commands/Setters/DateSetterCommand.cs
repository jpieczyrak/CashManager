using System;
using System.Collections.Generic;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Model.Setters;

namespace CashManager_MVVM.Logic.Commands.Setters
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
            Replace(elements);
            return elements;
        }

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            Replace(elements);
            return elements;
        }

        public void Replace(IEnumerable<IBookable> elements)
        {
            foreach (var bookable in elements) _setterAction(bookable, _dateFrame.Value);
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