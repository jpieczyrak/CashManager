using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model.Filters
{
    public class MultiPicker : BaseFilter
    {
        public MultiComboBoxViewModel ComboBox { get; private set; }

        public BaseSelectable[] Results { get; private set; }

        public MultiPicker(string userStock, BaseSelectable[] source, BaseSelectable[] selected = null)
        {
            Title = userStock;
            ComboBox = new MultiComboBoxViewModel();
            ComboBox.SetInput(source, selected);
            ComboBox.PropertyChanged += (sender, args) => Results = ComboBox.Results;
        }
    }
}