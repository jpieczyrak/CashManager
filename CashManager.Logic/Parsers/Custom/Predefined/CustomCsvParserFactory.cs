﻿using System.Collections.Generic;

namespace CashManager.Logic.Parsers.Custom.Predefined
{
    public class CustomCsvParserFactory
    {
        private readonly Dictionary<PredefinedCsvParsers, Rule[]> _predefined;

        public CustomCsvParserFactory()
        {
            _predefined = new Dictionary<PredefinedCsvParsers, Rule[]>
            {
                [PredefinedCsvParsers.Ing] = new[]
                {
                    new Rule { Column = 1, Property = TransactionField.CreationDate },
                    new Rule { Column = 2, Property = TransactionField.BookDate },
                    new Rule { Column = 3, Property = TransactionField.Note },
                    new Rule { Column = 4, Property = TransactionField.Title },
                    new Rule { Column = 7, Property = TransactionField.PositionTitle },
                    new Rule { Column = 9, Property = TransactionField.Value },
                    new Rule { Column = 15, Property = TransactionField.UserStock },
                    new Rule { Column = 16, Property = TransactionField.Balance },
                    new Rule { Column = 17, Property = TransactionField.Currency },
                }
            };
        }

        public CustomCsvParser Create(PredefinedCsvParsers type)
        {
            return _predefined.ContainsKey(type) ? new CustomCsvParser(_predefined[type]) : null;
        }
    }
}