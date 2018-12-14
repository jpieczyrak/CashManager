using System;

namespace CashManager_MVVM.Model.Selectors
{
    public class DateSelector : BaseSelector
    {
        private DateTime _value;

        public DateTime Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }

        public DateSelector(string description)
        {
            Description = description;
            _value = DateTime.Today;
        }
    }
}