using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Common
{
    public class ExtendedDatePickerViewModel : ViewModelBase
    {
        public ExtendedDatePicker Owner { get; set; }

        public RelayCommand SubtractDayCommand { get; }

        public RelayCommand SubtractMonthCommand { get; }

        public RelayCommand SubtractYearCommand { get; }

        public RelayCommand AddDayCommand { get; }
        public RelayCommand AddMonthCommand { get; }
        public RelayCommand AddYearCommand { get; }

        public ExtendedDatePickerViewModel(ExtendedDatePicker extendedDatePicker)
        {
            Owner = extendedDatePicker;

            SubtractDayCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddDays(-1));
            SubtractMonthCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddMonths(-1));
            SubtractYearCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddYears(-1));

            AddDayCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddDays(1));
            AddMonthCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddMonths(1));
            AddYearCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddYears(1));
        }
    }
}