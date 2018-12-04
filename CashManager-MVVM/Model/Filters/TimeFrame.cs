using System;

namespace CashManager_MVVM.Model.Filters
{
    public class TimeFrame : BaseFilter
    {
        private DateTime _from;
        private DateTime _to;
        private bool _isChecked;

        public DateTime From
        {
            get => _from;
            set => Set(nameof(From), ref _from, value);
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => Set(nameof(IsChecked), ref _isChecked, value);
        }

        public DateTime To
        {
            get => _to;
            set => Set(nameof(To), ref _to, value);
        }

        public TimeFrame(string title)
        {
            Title = title;

            var today = DateTime.Today;
            _from = new DateTime(today.Year, today.Month, 1);
            _to = _from.AddMonths(1).AddDays(-1);
        }
    }
}