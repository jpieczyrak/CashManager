using System;

namespace CashManager_MVVM.Model
{
    public class Balance : BaseObservableObject
    {
        private double _value;
        private DateTime _date;

        public double Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }

        public DateTime Date
        {
            get => _date;
            set => Set(nameof(Date), ref _date, value);
        }
    }
}