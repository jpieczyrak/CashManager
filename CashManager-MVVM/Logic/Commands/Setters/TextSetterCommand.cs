﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CashManager.Logic.Extensions;

using CashManager_MVVM.Logic.Commands.Filters;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;
using CashManager_MVVM.Model.Setters;

using log4net;

namespace CashManager_MVVM.Logic.Commands.Setters
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
            foreach (var position in elements) action(position, _textSetter.Value);
            return elements;
        }

        #endregion

        #region ISetter<Transaction>

        public bool CanExecute() => _textSetter.IsChecked;

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements)
        {
            var action = GetTransactionAction(_textSetter.Type);
            foreach (var transaction in elements) action(transaction, _textSetter.Value);
            return elements;
        }

        #endregion

        public static TextSetterCommand Create(TextSetter setter, TextSelector selector = null)
        {
            switch (setter.Type)
            {
                case TextSetterType.Title:
                    return new TextSetterCommand(setter, selector);
                case TextSetterType.Note:
                    break;
                case TextSetterType.PositionTitle:
                    break;
            }

            return null;
        }

        private Action<Transaction, string> GetTransactionAction(TextSetterType type)
        {
            if (_textSetter.AppendMode)
            {
                switch (type)
                {
                    case TextSetterType.Title:
                        return (transaction, s) => transaction.Title += s;
                    case TextSetterType.Note:
                        break;
                    case TextSetterType.PositionTitle:
                        break;
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
                            break;
                        case TextSetterType.PositionTitle:
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TextSetterType.Title:
                            return (transaction, s) => transaction.Title = s;
                        case TextSetterType.Note:
                            break;
                        case TextSetterType.PositionTitle:
                            break;
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
                        break;
                    case TextSetterType.PositionTitle:
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case TextSetterType.Title:
                        return (position, s) => position.Parent.Title = s;
                    case TextSetterType.Note:
                        break;
                    case TextSetterType.PositionTitle:
                        break;
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
                case TextSelectorType.PositionTitle:
                    selector = t => t.Title; //todo:
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
    }
}