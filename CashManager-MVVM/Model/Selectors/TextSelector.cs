using CashManager_MVVM.Properties;

namespace CashManager_MVVM.Model.Selectors
{
    public class TextSelector : BaseSelector
    {
        private string _value = string.Empty;

        public TextSelectorType Type { get; private set; }

        public string Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }

        public TextSelector(TextSelectorType type)
        {
            Type = type;
            switch (type)
            {
                case TextSelectorType.Title:
                    Description = Strings.Title;
                    break;
                case TextSelectorType.Note:
                    Description = Strings.Note;
                    break;
                case TextSelectorType.PositionTitle:
                    Description = Strings.PositionTitle;
                    break;
            }
        }

        public void Apply(TextSelector source)
        {
            Value = source.Value;
            Type = source.Type;
            IsChecked = source.IsChecked;
        }
    }
}