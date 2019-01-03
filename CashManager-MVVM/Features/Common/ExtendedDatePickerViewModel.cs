using System;

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

        public RelayCommand FirstDayCommand { get; }

        public RelayCommand LastDayCommand { get; }

        public RelayCommand FirstMonthCommand { get; }

        public RelayCommand LastMonthCommand { get; }

        public ExtendedDatePickerViewModel(ExtendedDatePicker extendedDatePicker)
        {
            Owner = extendedDatePicker;

            SubtractDayCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddDays(-1));
            SubtractMonthCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddMonths(-1));
            SubtractYearCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddYears(-1));

            AddDayCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddDays(1));
            AddMonthCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddMonths(1));
            AddYearCommand = new RelayCommand(() => Owner.SelectedValue = Owner.SelectedValue.AddYears(1));

            FirstDayCommand = new RelayCommand(() => Owner.SelectedValue = new DateTime(Owner.SelectedValue.Year, Owner.SelectedValue.Month, 1));
            LastDayCommand = new RelayCommand(() => Owner.SelectedValue = new DateTime(Owner.SelectedValue.Year, Owner.SelectedValue.Month, 1).AddMonths(1).AddDays(-1));

            FirstMonthCommand = new RelayCommand(() => Owner.SelectedValue = new DateTime(Owner.SelectedValue.Year, 1, Owner.SelectedValue.Day));
            LastMonthCommand = new RelayCommand(() => Owner.SelectedValue = new DateTime(Owner.SelectedValue.Year, 12, Owner.SelectedValue.Day));
        }
    }
}