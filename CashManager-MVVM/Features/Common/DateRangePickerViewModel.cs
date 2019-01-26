using System;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Common
{
    public class DateRangePickerViewModel : ViewModelBase
    {
        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable - it has to be a field to work properly
        private readonly DateRangePicker _owner;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

        public RelayCommand SubtractDayCommand { get; }

        public RelayCommand SubtractMonthCommand { get; }

        public RelayCommand SubtractYearCommand { get; }

        public RelayCommand AddDayCommand { get; }

        public RelayCommand AddMonthCommand { get; }

        public RelayCommand AddYearCommand { get; }

        public RelayCommand CurrentMonthCommand { get; }

        public DateRangePickerViewModel(DateRangePicker owner)
        {
            _owner = owner;

            SubtractDayCommand = new RelayCommand(() =>
            {
                _owner.DateFrameSelector.From = _owner.DateFrameSelector.From.AddDays(-1);
                _owner.DateFrameSelector.To = _owner.DateFrameSelector.To.AddDays(-1);
            });
            SubtractMonthCommand = new RelayCommand(() =>
            {
                _owner.DateFrameSelector.From = _owner.DateFrameSelector.From.AddMonths(-1);
                _owner.DateFrameSelector.To = _owner.DateFrameSelector.To.AddMonths(-1);
            });
            SubtractYearCommand = new RelayCommand(() =>
            {
                _owner.DateFrameSelector.From = _owner.DateFrameSelector.From.AddYears(-1);
                _owner.DateFrameSelector.To = _owner.DateFrameSelector.To.AddYears(-1);
            });

            AddDayCommand = new RelayCommand(() =>
            {
                _owner.DateFrameSelector.From = _owner.DateFrameSelector.From.AddDays(1);
                _owner.DateFrameSelector.To = _owner.DateFrameSelector.To.AddDays(1);
            });
            AddMonthCommand = new RelayCommand(() =>
            {
                _owner.DateFrameSelector.From = _owner.DateFrameSelector.From.AddMonths(1);
                _owner.DateFrameSelector.To = _owner.DateFrameSelector.To.AddMonths(1);
            });
            AddYearCommand = new RelayCommand(() =>
            {
                _owner.DateFrameSelector.From = _owner.DateFrameSelector.From.AddYears(1);
                _owner.DateFrameSelector.To = _owner.DateFrameSelector.To.AddYears(1);
            });

            CurrentMonthCommand = new RelayCommand(() =>
            {
                var firstDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                _owner.DateFrameSelector.From = firstDay;
                _owner.DateFrameSelector.To = firstDay.AddMonths(1).AddDays(-1);
            });
        }
    }
}