using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CashManager.Model;
using CashManager.Model.Common;
using CashManager.Model.Selectors;

using GalaSoft.MvvmLight;

namespace CashManager.Logic.Commands.Filters
{
    public class DateFrameFilter : ObservableObject, IFilter<Transaction>, IFilter<Position>
    {
        private readonly DateFrameSelector _dateFrameSelector;
        private readonly Func<IBookable, DateTime> _selector;

        private DateFrameFilter(DateFrameSelector dateFrameSelector, Func<IBookable, DateTime> selector)
        {
            _dateFrameSelector = dateFrameSelector;
            _selector = selector;
            _dateFrameSelector.PropertyChanged += DateFrameOnPropertyChanged;
        }

        ~DateFrameFilter() { _dateFrameSelector.PropertyChanged -= DateFrameOnPropertyChanged; }

        public IEnumerable<Position> Execute(IEnumerable<Position> elements) => Filter(elements).OfType<Position>();

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements) => Filter(elements).OfType<Transaction>();

        public bool CanExecute() => _dateFrameSelector.IsChecked;

        public IEnumerable<IBookable> Filter(IEnumerable<IBookable> elements)
        {
            return elements.Where(x => _selector(x) >= _dateFrameSelector.From && _selector(x) <= _dateFrameSelector.To);
        }

        public static DateFrameFilter Create(DateFrameSelector dateFrameSelector)
        {
            switch (dateFrameSelector.Type)
            {
                case DateFrameType.BookDate:
                    return new DateFrameFilter(dateFrameSelector, x => x.BookDate);
                case DateFrameType.CreationDate:
                    return new DateFrameFilter(dateFrameSelector, x => x.InstanceCreationDate);
                case DateFrameType.EditDate:
                    return new DateFrameFilter(dateFrameSelector, x => x.LastEditDate);
            }

            return null;
        }

        private void DateFrameOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            RaisePropertyChanged();
        }
    }
}