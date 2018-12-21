using System;

namespace CashManager.Data.ViewModelState.Selectors
{
    public class MultiPicker
    {
        public int Type { get; set; }

        public Guid[] Selected { get; set; }
    }
}