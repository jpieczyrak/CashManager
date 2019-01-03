using System;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Common
{
    public class ExtendedDatePickerViewModel : ViewModelBase
    {
        private DateTime _value = DateTime.Today;

        public DateTime Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public RelayCommand SubtractDayCommand { get; }
        public RelayCommand AddDayCommand { get; }

        public ExtendedDatePickerViewModel()
        {
            SubtractDayCommand = new RelayCommand(() => Value = Value.AddDays(-1));
            AddDayCommand = new RelayCommand(() => Value = Value.AddDays(1));
        }
    }
}