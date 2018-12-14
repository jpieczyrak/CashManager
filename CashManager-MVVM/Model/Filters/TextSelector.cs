namespace CashManager_MVVM.Model.Filters
{
    public class TextSelector : BaseSelector
    {
        private string _value = string.Empty;

        public string Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }

        public TextSelector(string description)
        {
            Description = description;
        }
    }
}