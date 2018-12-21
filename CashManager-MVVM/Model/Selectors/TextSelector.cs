using System;

namespace CashManager_MVVM.Model.Selectors
{
    public class TextSelector : BaseSelector
    {
        private string _value = string.Empty;

        public TextSelectorType Type { get; }

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
                    Description = "Title";
                    break;
                case TextSelectorType.Note:
                    Description = "Note";
                    break;
                case TextSelectorType.PositionTitle:
                    Description = "Position title";
                    break;
            }
        }
    }
}