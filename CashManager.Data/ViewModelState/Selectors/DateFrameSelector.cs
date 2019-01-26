using System;

namespace CashManager.Data.ViewModelState.Selectors
{
    public class DateFrameSelector
    {
        public int Type { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public bool IsChecked { get; set; }
    }
}