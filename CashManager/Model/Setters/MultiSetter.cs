using System;
using System.Linq;

using CashManager.Features.Common;
using CashManager.Model.Common;
using CashManager.Model.Selectors;
using CashManager.Properties;

namespace CashManager.Model.Setters
{
    public class MultiSetter : BaseSelector
    {
        public MultiPickerType Type { get; }

        private Selectable[] _results;
        private bool _append;

        public MultiComboBoxViewModel ComboBox { get; private set; }

        public Selectable[] Results
        {
            get => _results;
            private set
            {
                Set(nameof(Results), ref _results, value);
                if (_results.Length > (Selected?.Length ?? 0)) IsChecked = true;
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

        public bool Append
        {
            get => _append;
            set => Set(ref _append, value);
        }

        private MultiSetter() { }

        public MultiSetter(MultiPickerType type, Selectable[] source, Selectable[] selected = null)
        {
            Type = type;
            Results = selected ?? new Selectable[0];

            switch (type)
            {
                case MultiPickerType.Tag:
                    Description = Strings.Tags;
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

        public void Apply(MultiSetter source)
        {
            foreach (var x in ComboBox.InternalDisplayableSearchResults)
                x.IsSelected = source.Selected.Contains(x.Id);
            IsChecked = source.IsChecked;
        }
    }
}