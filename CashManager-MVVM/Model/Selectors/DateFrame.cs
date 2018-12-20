using System;
using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model.Selectors
{
    public class DateFrame : BaseSelector, ICommand
    {
        private readonly Func<IBookable, DateTime> _selector;
        private DateTime _from;
        private DateTime _to;

        public DateTime From
        {
            get => _from;
            set => Set(nameof(From), ref _from, value);
        }

        public DateTime To
        {
            get => _to;
            set => Set(nameof(To), ref _to, value);
        }

        private DateFrame(string description, Func<IBookable, DateTime> selector)
        {
            _selector = selector;
            Description = description;

            var today = DateTime.Today;
            _from = new DateTime(today.Year, today.Month, 1);
            _to = _from.AddMonths(1).AddDays(-1);
        }

        #region ICommand

        public IEnumerable<IBookable> Execute(IEnumerable<IBookable> elements)
        {
            return elements.Where(x => _selector(x) >= From && _selector(x) <= To);
        }

        public bool CanExecute()
        {
            return IsChecked;
        }

        #endregion

        public static DateFrame Create(DateFrameType type)
        {
            switch (type)
            {
                case DateFrameType.BookDate:
                    return new DateFrame("Book date", x => x.BookDate);
                case DateFrameType.CreationDate:
                    return new DateFrame("Creation date", x => x.InstanceCreationDate);
                case DateFrameType.EditDate:
                    return new DateFrame("Edit date", x => x.LastEditDate);
            }

            return null;
        }
    }
}