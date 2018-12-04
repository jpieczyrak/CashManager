using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Common
{
    public class MultiComboBoxViewModel : ViewModelBase
    {
        private readonly TrulyObservableCollection<BaseSelectable> _filtrableInput;
        private BaseSelectable _selectedValue;
        private string _text;
        private TrulyObservableCollection<BaseSelectable> _results;

        public string Text
        {
            get => _text;
            set
            {
                Set(nameof(Text), ref _text, value);
                var items = _filtrableInput.Where(x => x.Name.ToLower().Contains(_text.ToLower()))
                                           .OrderBy(x => !x.IsSelected)
                                           .ThenBy(x => x.Name);
                Results = new TrulyObservableCollection<BaseSelectable>(items);
            }
        }

        public BaseSelectable SelectedValue
        {
            get => _selectedValue;
            set => Set(nameof(SelectedValue), ref _selectedValue, value);
        }

        public TrulyObservableCollection<BaseSelectable> Results
        {
            get => _results;
            set => Set(nameof(Results), ref _results, value);
        }

        public string SelectedString => _filtrableInput != null
                                            ? string.Join(", ", _filtrableInput.Where(x => x.IsSelected).OrderBy(x => x.Name))
                                            : string.Empty;

        public MultiComboBoxViewModel(IEnumerable<BaseSelectable> input)
        {
            _filtrableInput = new TrulyObservableCollection<BaseSelectable>(input);
            Results = new TrulyObservableCollection<BaseSelectable>(_filtrableInput);
        }
    }
}