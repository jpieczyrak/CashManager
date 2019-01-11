using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Common
{
    public class DateRangePickerViewModel : ViewModelBase
    {
        private readonly DateRangePicker _owner;

        public RelayCommand SubtractDayCommand { get; }

        public RelayCommand SubtractMonthCommand { get; }

        public RelayCommand SubtractYearCommand { get; }

        public RelayCommand AddDayCommand { get; }

        public RelayCommand AddMonthCommand { get; }

        public RelayCommand AddYearCommand { get; }

        public DateRangePickerViewModel(DateRangePicker owner)
        {
            _owner = owner;

            SubtractDayCommand = new RelayCommand(() =>
            {
                _owner.DateFrame.From = _owner.DateFrame.From.AddDays(-1);
                _owner.DateFrame.To = _owner.DateFrame.To.AddDays(-1);
            });
            SubtractMonthCommand = new RelayCommand(() =>
            {
                _owner.DateFrame.From = _owner.DateFrame.From.AddMonths(-1);
                _owner.DateFrame.To = _owner.DateFrame.To.AddMonths(-1);
            });
            SubtractYearCommand = new RelayCommand(() =>
            {
                _owner.DateFrame.From = _owner.DateFrame.From.AddYears(-1);
                _owner.DateFrame.To = _owner.DateFrame.To.AddYears(-1);
            });

            AddDayCommand = new RelayCommand(() =>
            {
                _owner.DateFrame.From = _owner.DateFrame.From.AddDays(1);
                _owner.DateFrame.To = _owner.DateFrame.To.AddDays(1);
            });
            AddMonthCommand = new RelayCommand(() =>
            {
                _owner.DateFrame.From = _owner.DateFrame.From.AddMonths(1);
                _owner.DateFrame.To = _owner.DateFrame.To.AddMonths(1);
            });
            AddYearCommand = new RelayCommand(() =>
            {
                _owner.DateFrame.From = _owner.DateFrame.From.AddYears(1);
                _owner.DateFrame.To = _owner.DateFrame.To.AddYears(1);
            });
        }
    }
}