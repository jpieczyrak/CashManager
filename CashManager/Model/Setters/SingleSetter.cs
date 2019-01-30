using System;
using System.Linq;

using CashManager.Model.Common;
using CashManager.Model.Selectors;
using CashManager.Properties;

namespace CashManager.Model.Setters
{
    public class SingleSetter : BaseSelector
    {
        public MultiPickerType Type { get; private set; }

        public Selectable[] Input { get; set; }

        private Selectable _selected;

        public Selectable Selected
        {
            get => _selected;
            set
            {
                Set(ref _selected, value);
                IsChecked = true;
                if (value != null) SelectedId = value.Id;
            }
        }

        public Guid SelectedId { get; set; }

        private SingleSetter() { }

        public SingleSetter(MultiPickerType type, Selectable[] input)
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

        public void Apply(SingleSetter source)
        {
            Selected = Input.FirstOrDefault(x => x.Id == source.SelectedId);
            IsChecked = source.IsChecked;
            Type = source.Type;
        }
    }
}