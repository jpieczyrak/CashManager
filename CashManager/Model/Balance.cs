using System;

using CashManager.Model.Common;

namespace CashManager.Model
{
    public class Balance : BaseObservableObject, IBookable
    {
        private decimal _value;

        private DateTime _bookDate;

        public decimal Value
        {
            get => _value;
            set
            {
                PreviousValue = Value;
                Set(nameof(Value), ref _value, value);
                if (IsPropertyChangedEnabled) BookDate = DateTime.Today;
            }
        }

        public DateTime BookDate
        {
            get => _bookDate;
            set => Set(ref _bookDate, value);
        }

        public decimal PreviousValue { get; private set; }
    }
}