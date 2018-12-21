using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Logic.Commands
{
    public class MultiPickerFilter : ObservableObject, IFilter<Transaction>, IFilter<Position>
    {
        private readonly MultiPicker _picker;

        private MultiPickerFilter(MultiPicker picker)
        {
            _picker = picker;
            _picker.PropertyChanged += PickerOnPropertyChanged;
        }

        public IEnumerable<Position> Execute(IEnumerable<Position> elements)
        {
            switch (_picker.Type)
            {
                case MultiPickerType.Category:
                    var categories = _picker.Results.OfType<Category>().ToArray();
                    elements = elements.Where(x => categories.Any(y => y.MatchCategoryFilter(x.Category)));
                    break;
                case MultiPickerType.Tag:
                    var tags = new HashSet<Tag>(_picker.Results.OfType<Tag>());
                    elements = elements.Where(x => x.Tags.Any(y => tags.Contains(y)));
                        break;
                case MultiPickerType.UserStock:
                    var userStocks = new HashSet<Stock>(_picker.Results.OfType<Stock>());
                    elements = elements.Where(x => userStocks.Contains(x.Parent.UserStock));
                    break;
                case MultiPickerType.ExternalStock:
                    var exStocks = new HashSet<Stock>(_picker.Results.OfType<Stock>());
                    elements = elements.Where(x => exStocks.Contains(x.Parent.ExternalStock));
                    break;
                case MultiPickerType.TransactionType:
                    var types = new HashSet<TransactionType>(_picker.Results.OfType<TransactionType>());
                    elements = elements.Where(x => types.Contains(x.Parent.Type));
                    break;
            }

            return elements;
        }

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            switch (_picker.Type)
            {
                case MultiPickerType.Category:
                    var categories = _picker.Results.OfType<Category>().ToArray();
                    elements = elements.Where(x => x.Positions.Select(y => y.Category)
                                                    .Any(y => categories.Any(z => z.MatchCategoryFilter(y))));
                    break;
                case MultiPickerType.Tag:
                    var tags = new HashSet<Tag>(_picker.Results.OfType<Tag>());
                    elements = elements.Where(x => x.Positions.SelectMany(y => y.Tags).Any(y => tags.Contains(y)));
                    break;
                case MultiPickerType.UserStock:
                    var userStocks = new HashSet<Stock>(_picker.Results.OfType<Stock>());
                    elements = elements.Where(x => userStocks.Contains(x.UserStock));
                    break;
                case MultiPickerType.ExternalStock:
                    var exStocks = new HashSet<Stock>(_picker.Results.OfType<Stock>());
                    elements = elements.Where(x => exStocks.Contains(x.ExternalStock));
                    break;
                case MultiPickerType.TransactionType:
                    var types = new HashSet<TransactionType>(_picker.Results.OfType<TransactionType>());
                    elements = elements.Where(x => types.Contains(x.Type));
                    break;
            }

            return elements;
        }

        public bool CanExecute()
        {
            return _picker.IsChecked && _picker.Results.Any();
        }

        public static MultiPickerFilter Create(MultiPicker picker)
        {
            return new MultiPickerFilter(picker);
        }

        private void PickerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            RaisePropertyChanged();
        }
        
        ~MultiPickerFilter()
        {
            _picker.PropertyChanged -= PickerOnPropertyChanged;
        }
    }
}