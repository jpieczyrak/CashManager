using System;

namespace CashManager_MVVM.Model
{
    public class Balance : BaseObservableObject
    {
        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                Set(nameof(Value), ref _value, value);
                LastEditDate = DateTime.Now;
            }
        }
    }
}