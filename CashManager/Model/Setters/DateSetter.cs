using System;

using CashManager.Model.Selectors;
using CashManager.Properties;

namespace CashManager.Model.Setters
{
    public class DateSetter : BaseSelector
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

        public DateSetterType Type { get; }

        public DateSetter(DateSetterType type)
        {
            Type = type;
            switch (type)
            {
                case DateSetterType.BookDate:
                    Description = Strings.BookDate;
                    break;
            }

            _value = DateTime.Today;
        }

        public void Apply(DateSetter source)
        {
            Value = source.Value;
            IsChecked = source.IsChecked;
        }
    }
}