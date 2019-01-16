using System;
using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Model.Common;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Common
{
    public class MultiComboBoxViewModel : ViewModelBase
    {
        private TrulyObservableCollection<Selectable> _internalDisplayableSearchResults;
        private TrulyObservableCollection<Selectable> _filtrableInput;
        private Selectable _selectedValue;
        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                Set(nameof(Text), ref _text, value);
                var items = _filtrableInput.Where(x => x.Name.ToLower().Contains(_text.ToLower())).OrderBy(x => !x.IsSelected);
                InternalDisplayableSearchResults = new TrulyObservableCollection<Selectable>(items);
            }
        }

        public Selectable SelectedValue
        {
            get => _selectedValue;
            set => Set(nameof(SelectedValue), ref _selectedValue, value);
        }

        /// <summary>
        /// Used for displaying data inside of control
        /// </summary>
        public TrulyObservableCollection<Selectable> InternalDisplayableSearchResults
        {
            get => _internalDisplayableSearchResults;
            private set => Set(nameof(InternalDisplayableSearchResults), ref _internalDisplayableSearchResults, value);
        }

        /// <summary>
        /// Returns only selected elements
        /// </summary>
        public Selectable[] Results => _filtrableInput.Where(x => x.IsSelected).OrderBy(x => x.Name).ToArray();

        public bool AnySelected => Results?.Any() ?? false;

        public string SelectedString => _filtrableInput != null
                                            ? string.Join(", ", _filtrableInput.Where(x => x.IsSelected).OrderBy(x => x.Name).Select(x => x.Name?.Trim()))
                                            : string.Empty;

        public RelayCommand AddCommand { get; private set; }

        public MultiComboBoxViewModel()
        {
            AddCommand = new RelayCommand(ExecuteAddCommand, CanExecuteAddCommand);
            //todo: receive msg (element updated) -> add to list as not selected
        }

        public void SetInput(IEnumerable<Selectable> input, IEnumerable<Selectable> selected = null)
        {
            if (selected != null)
            {
                var dict = input.ToDictionary(x => x.Id, x => x);
                foreach (var x in selected)
                    if (dict.ContainsKey(x.Id))
                        dict[x.Id].IsSelected = true;
            }

            _filtrableInput = new TrulyObservableCollection<Selectable>(input);
            InternalDisplayableSearchResults = new TrulyObservableCollection<Selectable>(_filtrableInput);

            _filtrableInput.CollectionChanged += (sender, args) =>
            {
                RaisePropertyChanged(nameof(SelectedString));
                RaisePropertyChanged(nameof(AnySelected));
            };
        }

        private void ExecuteAddCommand()
        {
            //todo: remove
            //var item = new BaseSelectable(Guid.NewGuid()) { Name = Text, IsSelected = true };
            //InternalDisplayableSearchResults.Add(item);
            //_filtrableInput.Add(item);
            ////todo: message new element (save it. update other lists)
            //Text = string.Empty;
        }

        private bool CanExecuteAddCommand()
        {
            return SelectedValue == null && !string.IsNullOrWhiteSpace(_text)
                                         && !_filtrableInput.Select(x => x.Name.ToLower()).Contains(_text.ToLower());
        }
    }
}