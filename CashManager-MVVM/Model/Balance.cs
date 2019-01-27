﻿using CashManager.WPF.Model.Common;

namespace CashManager.WPF.Model
{
    public class Balance : BaseObservableObject
    {
        private decimal _value;

        public decimal Value
        {
            get => _value;
            set
            {
                PreviousValue = Value;
                Set(nameof(Value), ref _value, value);
            }
        }

        public decimal PreviousValue { get; private set; }
    }
}