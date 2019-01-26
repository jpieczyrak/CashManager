using System;

using CashManager_MVVM.Properties;

namespace CashManager_MVVM.Model.Selectors
{
    public class DateFrameSelector : BaseSelector
    {
        private DateTime _from;
        private DateTime _to;

        public DateFrameType Type { get; private set; }

        public DateTime From
        {
            get => _from;
            set
            {
                Set(nameof(From), ref _from, value);
                IsChecked = true;
            }
        }

        public DateTime To
        {
            get => _to;
            set
            {
                Set(nameof(To), ref _to, value);
                IsChecked = true;
            }
        }

        public DateFrameSelector(DateFrameType type)
        {
            Type = type;
            switch (type)
            {
                case DateFrameType.BookDate:
                    Description = Strings.BookDate;
                    break;
                case DateFrameType.CreationDate:
                    Description = Strings.CreationDate;
                    break;
                case DateFrameType.EditDate:
                    Description = Strings.EditDate;
                    break;
            }

            var today = DateTime.Today;
            _from = new DateTime(today.Year, today.Month, 1);
            _to = _from.AddMonths(1).AddDays(-1);
        }

        public void Apply(DateFrameSelector source)
        {
            From = source.From;
            To = source.To;
            Type = source.Type;
            IsChecked = source.IsChecked;
        }
    }
}