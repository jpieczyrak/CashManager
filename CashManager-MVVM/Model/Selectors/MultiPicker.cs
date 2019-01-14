using System;
using System.Linq;

using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Properties;

namespace CashManager_MVVM.Model.Selectors
{
    public class MultiPicker : BaseSelector
    {
        public MultiPickerType Type { get; }

        private BaseSelectable[] _results;

        public MultiComboBoxViewModel ComboBox { get; private set; }

        public BaseSelectable[] Results
        {
            get => _results;
            private set
            {
                Set(nameof(Results), ref _results, value);
                Selected = _results.Select(x => x.Id).ToArray();
            }
        }

        public Guid[] Selected { get; set; }

        private MultiPicker() { }

        public MultiPicker(MultiPickerType type, BaseSelectable[] source, BaseSelectable[] selected = null)
        {
            Type = type;
            Results = new BaseSelectable[0];

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

            ComboBox = new MultiComboBoxViewModel();
            ComboBox.SetInput(source, selected);
            ComboBox.PropertyChanged += (sender, args) => Results = ComboBox.Results;
        }

        public void SetInput(BaseSelectable[] source)
        {
            ComboBox.SetInput(source, Selected.Select(x => new BaseSelectable(x)).ToArray());
        }

        public void Apply(MultiPicker source)
        {
            foreach (var x in ComboBox.InternalDisplayableSearchResults)
                x.IsSelected = source.Selected.Contains(x.Id);
            IsChecked = source.IsChecked;
        }
    }
}