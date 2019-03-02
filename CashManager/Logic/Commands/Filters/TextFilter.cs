using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

using CashManager.Logic.Extensions;
using CashManager.Model;
using CashManager.Model.Selectors;

using GalaSoft.MvvmLight;

using log4net;

namespace CashManager.Logic.Commands.Filters
{
    public class TextFilter : ObservableObject, IFilter<Transaction>, IFilter<Position>
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(TextFilter)));

        private readonly TextSelector _textSelector;

        private TextFilter(TextSelector textSelector)
        {
            _textSelector = textSelector;
            _textSelector.PropertyChanged += TextSelectorOnPropertyChanged;
        }

        #region IFilter<Position>

        public IEnumerable<Position> Execute(IEnumerable<Position> elements)
        {
            var results = elements;
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

            return FilterPositions(selector, results);
        }

        #endregion

        #region IFilter<Transaction>

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            var results = elements;
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
                    return FilterPositions(x => x.Title, results.SelectMany(x => x.Positions)).Select(x => x.Parent).Distinct();
            }

            return FilterTransactions(selector, results);
        }

        public bool CanExecute() { return _textSelector.IsChecked && !string.IsNullOrEmpty(_textSelector.Value); }

        #endregion

        public static TextFilter Create(TextSelector textSelector) => new TextFilter(textSelector);

        private void TextSelectorOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) => RaisePropertyChanged();

        private IEnumerable<Transaction> FilterTransactions(Func<Transaction, string> selector, IEnumerable<Transaction> results)
        {
            if (selector == null) return new Transaction[0];
            if (_textSelector.IsRegex || _textSelector.IsWildCard)
            {
                string selectorValue = _textSelector.IsRegex ? _textSelector.Value : _textSelector.Value.WildCardToRegex();
                try
                {
                    var regex = new Regex(selectorValue);
                    results = results.Where(x => regex.IsMatch(selector(x)) != _textSelector.DisplayOnlyNotMatching);
                }
                catch (Exception e)
                {
                    _logger.Value.Info($"Invalid regex: {_textSelector.Value}", e);
                }
            }
            else
            {
                results = results.Where(x =>
                {
                    string value = selector(x);
                    if (string.IsNullOrEmpty(value)) return _textSelector.DisplayOnlyNotMatching;
                    string textSelectorValue = _textSelector.Value;
                    if (!_textSelector.IsCaseSensitive)
                    {
                        value = selector(x).ToLower();
                        textSelectorValue = textSelectorValue.ToLower();
                    }

                    bool isMatch = _textSelector.AnyOfWords
                                       ? textSelectorValue.Split(' ').Any(y => value.Contains(y))
                                       : value.Contains(textSelectorValue);
                    return isMatch != _textSelector.DisplayOnlyNotMatching;
                });
            }

            return results;
        }

        private IEnumerable<Position> FilterPositions(Func<Position, string> selector, IEnumerable<Position> results)
        {
            if (selector == null) return new Position[0];
            if (_textSelector.IsRegex || _textSelector.IsWildCard)
            {
                string selectorValue = _textSelector.IsRegex ? _textSelector.Value : _textSelector.Value.WildCardToRegex();
                try
                {
                    var regex = new Regex(selectorValue);
                    results = results.Where(x => regex.IsMatch(selector(x)) != _textSelector.DisplayOnlyNotMatching);
                }
                catch (Exception e)
                {
                    _logger.Value.Info($"Invalid regex: {_textSelector.Value}", e);
                }
            }
            else
            {
                results = results.Where(x =>
                {
                    string value = selector(x);
                    if (string.IsNullOrEmpty(value)) return _textSelector.DisplayOnlyNotMatching;
                    string textSelectorValue = _textSelector.Value;
                    if (!_textSelector.IsCaseSensitive)
                    {
                        value = selector(x).ToLower();
                        textSelectorValue = textSelectorValue.ToLower();
                    }

                    bool isMatch = _textSelector.AnyOfWords
                                       ? textSelectorValue.Split(' ').Any(y => value.Contains(y))
                                       : value.Contains(textSelectorValue);
                    return isMatch != _textSelector.DisplayOnlyNotMatching;
                });
            }

            return results;
        }

        #region Override

        ~TextFilter() => _textSelector.PropertyChanged -= TextSelectorOnPropertyChanged;

        #endregion
    }
}