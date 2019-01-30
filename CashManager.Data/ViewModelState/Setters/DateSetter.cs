using System;

namespace CashManager.Data.ViewModelState.Setters
{
    public class DateSetter : BaseSelector
    {
        public DateTime Value { get; set; }

        public DateSetterType Type { get; set; }
    }
}