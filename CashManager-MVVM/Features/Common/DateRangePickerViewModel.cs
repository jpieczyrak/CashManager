using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Common
{
    public class DateRangePickerViewModel : ViewModelBase
    {
        public RelayCommand SubtractDayCommand { get; }

        public RelayCommand SubtractMonthCommand { get; }

        public RelayCommand SubtractYearCommand { get; }

        public RelayCommand AddDayCommand { get; }

        public RelayCommand AddMonthCommand { get; }

        public RelayCommand AddYearCommand { get; }

        public DateRangePickerViewModel(DateRangePicker owner)
        {
            SubtractDayCommand = new RelayCommand(() =>
            {
                owner.DateFrame.From = owner.DateFrame.From.AddDays(-1);
                owner.DateFrame.To = owner.DateFrame.To.AddDays(-1);
            });
            SubtractMonthCommand = new RelayCommand(() =>
            {
                owner.DateFrame.From = owner.DateFrame.From.AddMonths(-1);
                owner.DateFrame.To = owner.DateFrame.To.AddMonths(-1);
            });
            SubtractYearCommand = new RelayCommand(() =>
            {
                owner.DateFrame.From = owner.DateFrame.From.AddYears(-1);
                owner.DateFrame.To = owner.DateFrame.To.AddYears(-1);
            });

            AddDayCommand = new RelayCommand(() =>
            {
                owner.DateFrame.From = owner.DateFrame.From.AddDays(1);
                owner.DateFrame.To = owner.DateFrame.To.AddDays(1);
            });
            AddMonthCommand = new RelayCommand(() =>
            {
                owner.DateFrame.From = owner.DateFrame.From.AddMonths(1);
                owner.DateFrame.To = owner.DateFrame.To.AddMonths(1);
            });
            AddYearCommand = new RelayCommand(() =>
            {
                owner.DateFrame.From = owner.DateFrame.From.AddYears(1);
                owner.DateFrame.To = owner.DateFrame.To.AddYears(1);
            });
        }
    }
}