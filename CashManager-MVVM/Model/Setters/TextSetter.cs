using CashManager_MVVM.Model.Selectors;
using CashManager_MVVM.Properties;

namespace CashManager_MVVM.Model.Setters
{
    public class TextSetter : BaseSelector
    {
        private string _value = string.Empty;
        private bool _displayOnlyNotMatching;

        public TextSetterType Type { get; private set; }

        public string Value
        {
            get => _value;
            set
            {
                bool addictive = value.Length > _value.Length;
                Set(nameof(Value), ref _value, value);
                if (addictive) IsChecked = true;
            }
        }

        public bool DisplayOnlyNotMatching
        {
            get => _displayOnlyNotMatching;
            set => Set(ref _displayOnlyNotMatching, value);
        }

        private bool _appendMode;

        public bool AppendMode
        {
            get => _appendMode;
            set => Set(ref _appendMode, value);
        }

        private bool _replaceMatch;

        public bool ReplaceMatch
        {
            get => _replaceMatch;
            set => Set(ref _replaceMatch, value);
        }

        public TextSetter(TextSetterType type)
        {
            Type = type;
            switch (type)
            {
                case TextSetterType.Title:
                    Description = Strings.Title;
                    break;
                case TextSetterType.Note:
                    Description = Strings.Note;
                    break;
                case TextSetterType.PositionTitle:
                    Description = Strings.PositionTitle;
                    break;
            }
        }

        public void Apply(TextSetter source)
        {
            Value = source.Value;
            Type = source.Type;
            IsChecked = source.IsChecked;
        }
    }
}