using System;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    public class TimeFrame : ObservableObject
    {
        private DateTime _from = DateTime.Today;
        private DateTime _to = DateTime.Today.AddDays(1);
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

        public string Title { get; }

        public TimeFrame(string title)
        {
            Title = title;
        }
    }
}