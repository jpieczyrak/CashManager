using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Properties;

namespace CashManager_MVVM.Model.Selectors
{
    public class SinglePicker : BaseSelector
    {
        public MultiPickerType Type { get; }

        public Selectable[] Input { get; set; }

        private Selectable _selected;

        public Selectable Selected
        {
            get => _selected;
            set
            {
                Set(ref _selected, value);
                IsChecked = true;
            }
        }

        public SinglePicker(MultiPickerType type, Selectable[] input)
        {
            Type = type;
            Input = input;

            switch (type)
            {
                case MultiPickerType.Category:
                    Description = Strings.Categories;
                    break;
                case MultiPickerType.Tag:
                    Description = Strings.Tags;
                    break;
                case MultiPickerType.UserStock:
                    Description = Strings.UserStock;
                    break;
                case MultiPickerType.ExternalStock:
                    Description = Strings.ExternalStock;
                    break;
                case MultiPickerType.TransactionType:
                    Description = Strings.TransactionTypes;
                    break;
            }
        }
    }
}