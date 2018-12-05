namespace CashManager_MVVM.Model.Filters
{
    public class TextFilter : BaseFilter
    {
        private string _value;

        public string Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }

        public TextFilter(string description)
        {
            Description = description;
        }
    }
}