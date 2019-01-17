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

        private Selectable[] _results;

        public MultiComboBoxViewModel ComboBox { get; private set; }

        public Selectable[] Results
        {
            get => _results;
            private set
            {
                Set(nameof(Results), ref _results, value);
                Selected = _results.Select(x => x.Id).ToArray();
                RaisePropertyChanged(nameof(AllSelected));
            }
        }

        public Guid[] Selected { get; set; }

        public bool? AllSelected
        {
            get =>
                _results.Length == ComboBox.InternalDisplayableSearchResults.Count
                    ? true
                    : _results.Any()
                        ? (bool?) null
                        : false;
            set
            {
                if (value.HasValue)
                {
                    if (value.Value)
                        foreach (var result in ComboBox.InternalDisplayableSearchResults)
                            result.IsSelected = true;
                    else
                        foreach (var result in ComboBox.InternalDisplayableSearchResults)
                            result.IsSelected = false;
                }
                else
                    foreach (var result in ComboBox.InternalDisplayableSearchResults)
                        result.IsSelected = false;
            }
        }

        private MultiPicker() { }

        public MultiPicker(MultiPickerType type, Selectable[] source, Selectable[] selected = null)
        {
            Type = type;
            Results = new Selectable[0];

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

        public void SetInput(Selectable[] source)
        {
            ComboBox.SetInput(source, Results);
        }

        public void Apply(MultiPicker source)
        {
            foreach (var x in ComboBox.InternalDisplayableSearchResults)
                x.IsSelected = source.Selected.Contains(x.Id);
            IsChecked = source.IsChecked;
        }
    }
}