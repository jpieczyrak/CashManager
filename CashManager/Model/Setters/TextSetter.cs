using CashManager.Model.Selectors;
using CashManager.Properties;

namespace CashManager.Model.Setters
{
    public class TextSetter : BaseSelector
    {
        private string _value = string.Empty;

        public TextSetterType Type { get; private set; }

        public string Value
        {
            get => _value;
            set
            {
                bool addictive = value != null && _value != null && value.Length > _value.Length;
                Set(nameof(Value), ref _value, value);
                if (addictive) IsChecked = true;
            }
        }

        private bool _appendMode;

        public bool AppendMode
        {
            get => _appendMode;
            set
            {
                Set(ref _appendMode, value);
                if (value)
                {
                    _replaceMatch = false;
                    RaisePropertyChanged(nameof(ReplaceMatch));
                }
            }
        }

        private bool _replaceMatch;

        public bool ReplaceMatch
        {
            get => _replaceMatch;
            set
            {
                Set(ref _replaceMatch, value);
                if (value)
                {
                    _appendMode = false;
                    RaisePropertyChanged(nameof(AppendMode));
                }
            }
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
                    Description = Strings.Notes;
                    break;
                case TextSetterType.PositionTitle:
                    Description = Strings.PositionTitle;
                    break;
            }
        }

        public void Apply(TextSetter source)
        {
            Value = source.Value;
            AppendMode = source.AppendMode;
            ReplaceMatch = source.ReplaceMatch;
            Type = source.Type;
            IsChecked = source.IsChecked;
        }
    }
}