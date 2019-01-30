namespace CashManager.Data.ViewModelState.Setters
{
    public class TextSetter : BaseSelector
    {
        public TextSetterType Type { get; set; }

        public string Value { get; set; }

        public bool AppendMode { get; set; }

        public bool ReplaceMatch { get; set; }
    }
}