using System.Linq;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Common
{
    public class MultiComboBoxViewModel : ViewModelBase
    {
        private readonly TrulyObservableCollection<ISelectable> _filtrableInput;
        private ISelectable _selectedValue;
        private string _text;
        private TrulyObservableCollection<ISelectable> _results;

        public string Text
        {
            get => _text;
            set
            {
                Set(nameof(Text), ref _text, value);
                var items = _filtrableInput.Where(x => x.Name.ToLower().Contains(_text.ToLower()))
                                           .OrderBy(x => !x.IsSelected)
                                           .ThenBy(x => x.Name);
                Results = new TrulyObservableCollection<ISelectable>(items);
            }
        }

        public ISelectable SelectedValue
        {
            get => _selectedValue;
            set => Set(nameof(SelectedValue), ref _selectedValue, value);
        }

        public TrulyObservableCollection<ISelectable> Results
        {
            get => _results;
            set => Set(nameof(Results), ref _results, value);
        }

        public string SelectedString => _filtrableInput != null
                                            ? string.Join(", ", _filtrableInput.Where(x => x.IsSelected).OrderBy(x => x.Name))
                                            : string.Empty;

        public MultiComboBoxViewModel()
        {
            //todo: remove
            _filtrableInput = new TrulyObservableCollection<ISelectable>
            {
                new Tag { Name = "aaa", IsSelected = true },
                new Tag { Name = "aab", IsSelected = false },
                new Tag { Name = "abc", IsSelected = true }
            };
            Results = new TrulyObservableCollection<ISelectable>(_filtrableInput);
        }
    }
}