using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model.Filters
{
    public class MultiPicker : BaseFilter
    {
        private BaseSelectable[] _results;

        public MultiComboBoxViewModel ComboBox { get; private set; }

        public BaseSelectable[] Results
        {
            get => _results;
            private set => Set(nameof(Results), ref _results, value);
        }

        public MultiPicker(string description, BaseSelectable[] source, BaseSelectable[] selected = null)
        {
            Description = description;
            ComboBox = new MultiComboBoxViewModel();
            ComboBox.SetInput(source, selected);
            ComboBox.PropertyChanged += (sender, args) => Results = ComboBox.Results;
        }
    }
}