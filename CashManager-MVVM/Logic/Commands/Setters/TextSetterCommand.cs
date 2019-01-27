using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CashManager.Logic.Extensions;

using CashManager.WPF.Model;
using CashManager.WPF.Model.Selectors;
using CashManager.WPF.Model.Setters;

using log4net;

namespace CashManager.WPF.Logic.Commands.Setters
{
    public class TextSetterCommand : ISetter<Transaction>, ISetter<Position>
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(TextSetterCommand)));

        private readonly TextSetter _textSetter;
        private readonly TextSelector _selector;

        private TextSetterCommand(TextSetter textSetter, TextSelector selector)
        {
            _textSetter = textSetter;
            _selector = selector;
        }

        #region ISetter<Position>

        public IEnumerable<Position> Execute(IEnumerable<Position> elements)
        {
            var action = GetPositionAction(_textSetter.Type);
            var positions = _textSetter.Type == TextSetterType.Title || _textSetter.Type == TextSetterType.Note
                                 ? elements.Select(x => x.Parent).Distinct().SelectMany(x => x.Positions)
                                 : elements;
            foreach (var position in positions)
                action(position, _textSetter.Value);
            return elements;
        }

        #endregion

        #region ISetter<Transaction>

        public bool CanExecute() => _textSetter.IsChecked && !string.IsNullOrWhiteSpace(_textSetter.Value);

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            var action = GetTransactionAction(_textSetter.Type);
            foreach (var transaction in elements) action(transaction, _textSetter.Value);
            return elements;
        }

        #endregion

        public static TextSetterCommand Create(TextSetter setter, TextSelector selector = null)
            => new TextSetterCommand(setter, selector);

        private Action<Transaction, string> GetTransactionAction(TextSetterType type)
        {
            if (_textSetter.AppendMode)
            {
                switch (type)
                {
                    case TextSetterType.Title:
                        return (transaction, s) => transaction.Title += s;
                    case TextSetterType.Note:
                        return (transaction, s) => transaction.Note += s;
                    case TextSetterType.PositionTitle:
                        return (transaction, s) => { foreach (var position in transaction.Positions) position.Title += s; };
                }
            }
            else
            {
                if (_textSetter.ReplaceMatch && (_selector?.IsChecked ?? false))
                {
                    switch (type)
                    {
                        case TextSetterType.Title:
                            return (x, s) =>
                            {
                                string matchValue = GetMatchValue(x);
                                if (!string.IsNullOrWhiteSpace(matchValue))
                                    x.Title = x.Title.Replace(matchValue, _textSetter.Value);
                                else
                                    _logger.Value.Debug("No match");
                            };
                        case TextSetterType.Note:
                            return (x, s) =>
                            {
                                string matchValue = GetMatchValue(x);
                                if (!string.IsNullOrWhiteSpace(matchValue))
                                    x.Note = x.Note.Replace(matchValue, _textSetter.Value);
                                else
                                    _logger.Value.Debug("No match");
                            };
                        case TextSetterType.PositionTitle:
                            return (x, s) =>
                            {
                                foreach (var position in x.Positions)
                                {
                                    string matchValue = GetMatchValue(position);
                                    if (!string.IsNullOrWhiteSpace(matchValue))
                                    {
                                        position.Title = position.Title.Replace(matchValue, _textSetter.Value);
                                    }
                                    else
                                        _logger.Value.Debug("No match");
                                }
                            };
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TextSetterType.Title:
                            return (transaction, s) => transaction.Title = s;
                        case TextSetterType.Note:
                            return (transaction, s) => transaction.Note = s;
                        case TextSetterType.PositionTitle:
                            return (transaction, s) => { foreach (var position in transaction.Positions) position.Title = s; };
                    }
                }
            }

            return null;
        }

        private Action<Position, string> GetPositionAction(TextSetterType type)
        {
            if (_textSetter.AppendMode)
            {
                switch (type)
                {
                    case TextSetterType.Title:
                        return (position, s) => position.Parent.Title += s;
                    case TextSetterType.Note:
                        return (position, s) => position.Parent.Note += s;
                    case TextSetterType.PositionTitle:
                        return (position, s) => { position.Title += s; };
                }
            }
            else
            {
                if (_textSetter.ReplaceMatch && (_selector?.IsChecked ?? false))
                {
                    switch (type)
                    {
                        case TextSetterType.Title:
                            return (x, s) =>
                            {
                                string matchValue = GetMatchValue(x);
                                if (!string.IsNullOrWhiteSpace(matchValue))
                                    x.Parent.Title = x.Parent.Title.Replace(matchValue, _textSetter.Value);
                                else
                                    _logger.Value.Debug("No match");
                            };
                        case TextSetterType.Note:
                            return (x, s) =>
                            {
                                string matchValue = GetMatchValue(x);
                                if (!string.IsNullOrWhiteSpace(matchValue))
                                    x.Parent.Note = x.Parent.Note.Replace(matchValue, _textSetter.Value);
                                else
                                    _logger.Value.Debug("No match");
                            };
                        case TextSetterType.PositionTitle:
                            return (x, s) =>
                            {
                                string matchValue = GetMatchValue(x);
                                if (!string.IsNullOrWhiteSpace(matchValue))
                                    x.Title = x.Title.Replace(matchValue, _textSetter.Value);
                                else
                                    _logger.Value.Debug("No match");
                            };
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TextSetterType.Title:
                            return (position, s) => position.Parent.Title = s;
                        case TextSetterType.Note:
                            return (position, s) => position.Parent.Note = s;
                        case TextSetterType.PositionTitle:
                            return (position, s) => position.Title = s;
                    }
                }
            }

            return null;
        }

        public string GetMatchValue(Transaction transaction)
        {
            //todo: [DisplayOnlyNotMatching]
            Func<Transaction, string> selector = null;
            switch (_selector.Type)
            {
                case TextSelectorType.Title:
                    selector = t => t.Title;
                    break;
                case TextSelectorType.Note:
                    selector = t => t.Note;
                    break;
            }

            if (_selector.IsRegex || _selector.IsWildCard)
            {
                string selectorValue = _selector.IsRegex ? _selector.Value : _selector.Value.WildCardToRegex();
                try
                {
                    var regex = new Regex(selectorValue);
                    return regex.Match(selector(transaction)).Value;
                }
                catch (Exception e)
                {
                    _logger.Value.Info($"Invalid regex: {_selector.Value}", e);
                }
            }
            else
            {
                return _selector.Value;
            }

            return string.Empty;
        }

        public string GetMatchValue(Position position)
        {
            //todo: [DisplayOnlyNotMatching]
            Func<Position, string> selector = null;
            switch (_selector.Type)
            {
                case TextSelectorType.Title:
                    selector = t => t.Parent.Title;
                    break;
                case TextSelectorType.Note:
                    selector = t => t.Parent.Note;
                    break;
                case TextSelectorType.PositionTitle:
                    selector = t => t.Title;
                    break;
            }

            if (_selector.IsRegex || _selector.IsWildCard)
            {
                string selectorValue = _selector.IsRegex ? _selector.Value : _selector.Value.WildCardToRegex();
                try
                {
                    var regex = new Regex(selectorValue);
                    return regex.Match(selector(position)).Value;
                }
                catch (Exception e)
                {
                    _logger.Value.Info($"Invalid regex: {_selector.Value}", e);
                }
            }
            else
            {
                return _selector.Value;
            }

            return string.Empty;
        }
    }
}