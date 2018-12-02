using System;
using System.Collections.Generic;
using System.Globalization;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public class ExcelParser : IParser
    {
        public Balance Balance { get; private set; } = new Balance { Value = 0 };

        #region IParser

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock,
            TransactionType defaultOutcome, TransactionType defaultIncome)
        {
            List<Transaction> transactions = new List<Transaction>();
            string[] lines = input.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] values = line.Split(';');

                bool buying = !string.IsNullOrEmpty(values[11]);
                bool working = !string.IsNullOrEmpty(values[10]);

                if (buying) transactions.Add(MakeTransaction(userStock, externalStock, true, values, line, defaultOutcome, defaultIncome));
                if (working)
                    transactions.Add(MakeTransaction(userStock, externalStock, false, values, line, defaultOutcome, defaultIncome));
            }

            return transactions.ToArray();
        }

        #endregion

        private Transaction MakeTransaction(Stock userStock, Stock externalStock, bool outcome, IReadOnlyList<string> values, string input,
            TransactionType defaultOutcome, TransactionType defaultIncome)
        {
            var date = DateTime.ParseExact(values[0], "d.M.yy", CultureInfo.InvariantCulture);
            string title = values[12];

            var type = outcome ? defaultOutcome : defaultIncome;

            string stringWithValue = outcome ? values[11] : values[10];
            double.TryParse(stringWithValue, out double value);

            var position = new Position(title, value);

            return new Transaction(type, date, title, "", new List<Position> { position }, userStock, externalStock, input);
        }
    }
}