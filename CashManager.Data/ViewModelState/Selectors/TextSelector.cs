namespace CashManager.Data.ViewModelState.Selectors
{
    public class TextSelector
    {
        public int Type { get; set; }

        public string Value { get; set; }

        public bool IsChecked { get; set; }

        public bool IsCaseSensitive { get; set; }

        public bool IsWildCard { get; set; }

        public bool IsRegex { get; set; }

        public bool DisplayOnlyNotMatching { get; set; }
    }
}