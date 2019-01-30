using System;

namespace CashManager.Data.ViewModelState.Setters
{
    public class SingleSetter : BaseSelector
    {

        public MultiPickerType Type { get; set; }

        public Guid SelectedId { get; set; }
    }
}