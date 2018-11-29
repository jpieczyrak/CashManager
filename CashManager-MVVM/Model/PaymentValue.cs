using System;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    public class PaymentValue : ObservableObject
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        private double _value;

        public double Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }
    }
}