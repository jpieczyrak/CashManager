﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

using CashManager.Logic.Extensions;

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
            IEnumerable<Position> results = new Position[0];
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

            if (selector == null) return results;
            results = _textSelector.IsCaseSensitive
                          ? elements.Where(x => !string.IsNullOrEmpty(selector(x)) && selector(x).Contains(_textSelector.Value))
                          : elements.Where(x => !string.IsNullOrEmpty(selector(x)) && selector(x).ToLower().Contains(_textSelector.Value.ToLower()));

            return results;
        }

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            IEnumerable<Transaction> results = new Transaction[0];
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
                    return elements.Where(x => x.Positions.Any(y => (_textSelector.IsCaseSensitive ? y.Title.Contains(_textSelector.Value) : y.Title.ToLower().Contains(_textSelector.Value.ToLower()))));
            }

            if (selector == null) return results;

            if (_textSelector.IsRegex)
            {
                var regex = new Regex(_textSelector.Value);
                results = elements.Where(x => regex.IsMatch(selector(x)) != _textSelector.DisplayOnlyNotMatching);
            }
            else if (_textSelector.IsWildCard)
            {
                var regex = new Regex(_textSelector.Value.WildCardToRegex());
                results = elements.Where(x => regex.IsMatch(selector(x)) != _textSelector.DisplayOnlyNotMatching);
            }
            else
            {
                results = _textSelector.IsCaseSensitive
                              ? elements.Where(x => !string.IsNullOrEmpty(selector(x)) && selector(x).Contains(_textSelector.Value) != _textSelector.DisplayOnlyNotMatching)
                              : elements.Where(x => !string.IsNullOrEmpty(selector(x)) && selector(x).ToLower().Contains(_textSelector.Value.ToLower()) != _textSelector.DisplayOnlyNotMatching);
            }

            return results;
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