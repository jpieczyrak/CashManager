using System;

namespace CashManager.Data.ViewModelState.Setters
{
    public class MultiSetter : BaseSelector
    {
        public MultiPickerType Type { get; set; }

        public Guid[] Selected { get; set; }

        public bool Append { get; set; }
    }
}