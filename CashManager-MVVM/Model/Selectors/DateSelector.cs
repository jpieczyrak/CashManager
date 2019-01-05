using System;

namespace CashManager_MVVM.Model.Selectors
{
    public class DateSelector : BaseSelector
    {
        private DateTime _value;

        public DateTime Value
        {
            get => _value;
            set
            {
                if (value > DateTime.Today) value = DateTime.Today;
                Set(nameof(Value), ref _value, value);
            }
        }

        public DateSelector(string description)
        {
            Description = description;
            _value = DateTime.Today;
        }

        public void Apply(DateSelector source)
        {
            Value = source.Value;
            IsChecked = source.IsChecked;
        }
    }
}