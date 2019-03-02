using CashManager.Properties;

namespace CashManager.Model.Selectors
{
    public class TextSelector : BaseSelector
    {
        private string _value = string.Empty;
        private bool _isCaseSensitive;
        private bool _isWildCard;
        private bool _isRegex;
        private bool _displayOnlyNotMatching;
        private bool _anyOfWords;

        public TextSelectorType Type { get; private set; }

        public string Value
        {
            get => _value;
            set
            {
                bool addictive = (value?.Length ?? 0) > (_value?.Length ?? 0);
                Set(nameof(Value), ref _value, value);
                if (addictive) IsChecked = true;
            }
        }

        public bool IsCaseSensitive
        {
            get => _isCaseSensitive;
            set => Set(ref _isCaseSensitive, value);
        }

        public bool IsWildCard
        {
            get => _isWildCard;
            set
            {
                Set(ref _isWildCard, value);
                if (value) _isRegex = false;
                RaisePropertyChanged(nameof(IsRegex));
            }
        }

        public bool IsRegex
        {
            get => _isRegex;
            set
            {
                Set(ref _isRegex, value);
                if (value) _isWildCard = false;
                RaisePropertyChanged(nameof(IsWildCard));
            }
        }

        public bool DisplayOnlyNotMatching
        {
            get => _displayOnlyNotMatching;
            set => Set(ref _displayOnlyNotMatching, value);
        }

        public bool AnyOfWords
        {
            get => _anyOfWords;
            set => Set(ref _anyOfWords, value);
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
            IsCaseSensitive = source.IsCaseSensitive;
            IsRegex = source.IsRegex;
            IsWildCard = source.IsWildCard;
            DisplayOnlyNotMatching = source.DisplayOnlyNotMatching;
        }
    }
}