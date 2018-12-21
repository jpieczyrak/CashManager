using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Model.Common;

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
            private set => Set(nameof(Results), ref _results, value);
        }

        public MultiPicker(MultiPickerType type, BaseSelectable[] source, BaseSelectable[] selected = null)
        {
            Type = type;
            Results = new BaseSelectable[0];

            switch (type)
            {
                case MultiPickerType.Category:
                    Description = "Categories";
                    break;
                case MultiPickerType.Tag:
                    Description = "Tags";
                    break;
                case MultiPickerType.UserStock:
                    Description = "User stock";
                    break;
                case MultiPickerType.ExternalStock:
                    Description = "External stock";
                    break;
                case MultiPickerType.TransactionType:
                    Description = "Types";
                    break;
            }

            ComboBox = new MultiComboBoxViewModel();
            ComboBox.SetInput(source, selected);
            ComboBox.PropertyChanged += (sender, args) => Results = ComboBox.Results;
        }
    }
}