using System;
using System.Collections.Generic;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Model.Selectors;

namespace CashManager_MVVM.Logic.Commands
{
    public class DateFrameReplacer : IReplacer<Transaction>, IReplacer<Position>
    {
        private readonly DateFrame _dateFrame;
        private readonly Action<IBookable, DateTime> _setter;

        private DateFrameReplacer(DateFrame dateFrame, Action<IBookable, DateTime> setter)
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

        public static DateFrameReplacer Create(DateFrame dateFrame)
        {
            switch (dateFrame.Type)
            {
                case DateFrameType.BookDate:
                    return new DateFrameReplacer(dateFrame, (bookable, time) => bookable.BookDate = time);
                case DateFrameType.CreationDate:
                    return new DateFrameReplacer(dateFrame, (bookable, time) => { } );
                case DateFrameType.EditDate:
                    return new DateFrameReplacer(dateFrame, (bookable, time) => { } );
            }

            return null;
        }
    }
}