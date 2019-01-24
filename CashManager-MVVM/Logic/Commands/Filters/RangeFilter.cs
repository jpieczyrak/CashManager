using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Logic.Commands.Filters
{
    public class RangeFilter : ObservableObject, IFilter<Transaction>, IFilter<Position>
    {
        private readonly RangeSelector _rangeSelector;

        private RangeFilter(RangeSelector rangeSelector)
        {
            _rangeSelector = rangeSelector;
            _rangeSelector.PropertyChanged += RangeSelectorOnPropertyChanged;
        }

        public IEnumerable<Position> Execute(IEnumerable<Position> elements)
        {
            switch (_rangeSelector.Type)
            {
                case RangeSelectorType.GrossValue:
                    return elements.Where(x => x.Value.GrossValue >= _rangeSelector.Min && x.Value.GrossValue <= _rangeSelector.Max);
            }

            return elements;
        }

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            switch (_rangeSelector.Type)
            {
                case RangeSelectorType.GrossValue:
                    return elements.Where(x => x.ValueAsProfit >= _rangeSelector.Min && x.ValueAsProfit <= _rangeSelector.Max);
            }

            return elements;
        }

        public bool CanExecute()
        {
            return _rangeSelector.IsChecked;
        }

        public static RangeFilter Create(RangeSelector rangeSelector)
        {
            return new RangeFilter(rangeSelector);
        }

        private void RangeSelectorOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            RaisePropertyChanged();
        }

        ~RangeFilter()
        {
            _rangeSelector.PropertyChanged -= RangeSelectorOnPropertyChanged;
        }
    }
}