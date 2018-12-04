using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model.Filters
{
    public class MultiPicker : BaseFilter
    {
        public MultiComboBoxViewModel ComboBox { get; private set; }

        public BaseSelectable[] Results { get; private set; }

        public MultiPicker(string description, BaseSelectable[] source, BaseSelectable[] selected = null)
        {
            Description = description;
            ComboBox = new MultiComboBoxViewModel();
            ComboBox.SetInput(source, selected);
            ComboBox.PropertyChanged += (sender, args) => Results = ComboBox.Results;
        }
    }
}