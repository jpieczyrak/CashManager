using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Logic.Commands
{
    public class TextFilter : ObservableObject, IFilter<Transaction>, IFilter<Position>
    {
        private readonly TextSelector _textSelector;

        private TextFilter(TextSelector textSelector)
        {
            _textSelector = textSelector;
            _textSelector.PropertyChanged += TextSelectorOnPropertyChanged;
        }

        public IEnumerable<Position> Execute(IEnumerable<Position> elements)
        {
            Func<Position, string> selector = null;
            switch (_textSelector.Type)
            {
                case TextSelectorType.Title:
                    selector = x => x.Parent.Title;
                    break;
                case TextSelectorType.Note:
                    selector = x => x.Parent.Note;
                    break;
                case TextSelectorType.PositionTitle:
                    selector = x => x.Title;
                    break;
            }

            if (selector == null) return new Position[0];
            return elements.Where(x => !string.IsNullOrEmpty(selector(x)) && selector(x).ToLower().Contains(_textSelector.Value.ToLower()));
        }

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            Func<Transaction, string> selector = null;
            switch (_textSelector.Type)
            {
                case TextSelectorType.Title:
                    selector = x => x.Title;
                    break;
                case TextSelectorType.Note:
                    selector = x => x.Note;
                    break;
                case TextSelectorType.PositionTitle:
                    return elements.Where(x => x.Positions.Any(y => y.Title.ToLower().Contains(_textSelector.Value.ToLower())));
            }

            if (selector == null) return new Transaction[0];
            return elements.Where(x => !string.IsNullOrEmpty(selector(x)) && selector(x).ToLower().Contains(_textSelector.Value.ToLower()));
        }

        public bool CanExecute()
        {
            return _textSelector.IsChecked && !string.IsNullOrEmpty(_textSelector.Value);
        }

        public static TextFilter Create(TextSelector textSelector)
        {
            return new TextFilter(textSelector);
        }

        private void TextSelectorOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            RaisePropertyChanged();
        }

        ~TextFilter()
        {
            _textSelector.PropertyChanged -= TextSelectorOnPropertyChanged;
        }
    }
}