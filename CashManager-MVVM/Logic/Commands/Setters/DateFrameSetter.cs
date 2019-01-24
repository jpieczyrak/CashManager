using System;
using System.Collections.Generic;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Model.Selectors;

namespace CashManager_MVVM.Logic.Commands.Setters
{
    public class DateFrameSetter : ISetter<Transaction>, ISetter<Position>
    {
        private readonly DateFrame _dateFrame;
        private readonly Action<IBookable, DateTime> _setter;

        private DateFrameSetter(DateFrame dateFrame, Action<IBookable, DateTime> setter)
        {
            _dateFrame = dateFrame;
            _setter = setter;
        }

        public void Execute(IEnumerable<Position> elements, DateTime value) => Replace(elements, value);

        public void Execute(IEnumerable<Transaction> elements, DateTime value) => Replace(elements, value);

        public bool CanExecute() => _dateFrame.IsChecked;

        public void Replace(IEnumerable<IBookable> elements, DateTime value)
        {
            foreach (var bookable in elements) _setter(bookable, value);
        }

        public static DateFrameSetter Create(DateFrame dateFrame)
        {
            switch (dateFrame.Type)
            {
                case DateFrameType.BookDate:
                    return new DateFrameSetter(dateFrame, (bookable, time) => bookable.BookDate = time);
                case DateFrameType.CreationDate:
                    return new DateFrameSetter(dateFrame, (bookable, time) => { } );
                case DateFrameType.EditDate:
                    return new DateFrameSetter(dateFrame, (bookable, time) => { } );
            }

            return null;
        }
    }
}