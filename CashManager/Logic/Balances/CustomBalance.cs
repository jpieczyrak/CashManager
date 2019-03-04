using System.ComponentModel;
using System.Linq;

using CashManager.Features.Common;
using CashManager.Features.Search;
using CashManager.Model.Common;

namespace CashManager.Logic.Balances
{
    public sealed class CustomBalance : BaseObservableObject
    {
        private string _name;

        public SearchState[] Searches { get; private set; }

        public MultiComboBoxViewModel SearchesPicker { get; private set; }

        public override string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private CustomBalance()
        {
            Searches = new SearchState[0];
            SearchesPicker = new MultiComboBoxViewModel();
            SearchesPicker.PropertyChanged += SavedSearchesOnPropertyChanged;
        }

        private void SavedSearchesOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Searches = SearchesPicker.Results.Select(x => x.Value as SearchState).ToArray();
            RaisePropertyChanged();
        }

        public CustomBalance(string name) : this()
        {
            Name = name;
        }
    }
}