using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public class IdeaBankParser : IParser
    {
        private readonly List<Balance> _balances = new List<Balance>();
        private const string NOT_PERFORMED_TRANSACTION = "-";
        private const int LINES_PER_ENTRY = 4;

        public Balance Balance { get; private set; }

        #region IParser

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock, TransactionType defaultOutcome,
            TransactionType defaultIncome)
        {
            if (string.IsNullOrEmpty(input)) return null;

            var results = new List<Transaction>();
            var elements = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (elements.Length >= LINES_PER_ENTRY)
            {
                int i = 0;
                while (i + LINES_PER_ENTRY <= elements.Length)
                {
                    try
                    {
                        string title = elements[i];
                        DateTime date = DateTime.Parse(elements[i + 1]);
                        string stringValue = elements[i + 2].Replace(" PLN", string.Empty).Replace(" ", string.Empty);
                        decimal value = decimal.Parse(stringValue);
                        decimal balance = decimal.Parse(elements[i + 3].Replace(" ", string.Empty));

                        bool income = value > 0m;

                        var positions = new[] { new Position(title, Math.Abs(value)) };
                        string sourceInput = string.Join("\n", elements.Skip(i - 1).Take(LINES_PER_ENTRY));
                        var transaction = new Transaction(income ? defaultIncome : defaultOutcome, date, title,
                            $"Saldo: {balance:#,##0.00}",
                            positions, userStock, externalStock, sourceInput);

                        results.Add(transaction);
                        _balances.Add(new Balance(date, balance));
                    }
                    catch (Exception e)
                    {
                        bool hasTitle = !string.IsNullOrEmpty(elements[0]);
                        bool isSkippedTransaction = NOT_PERFORMED_TRANSACTION.Equals(elements[1].Trim());
                        if (hasTitle && isSkippedTransaction) i -= 2;
                    }

                    i += LINES_PER_ENTRY;
                }
            }

            Balance = _balances.OrderByDescending(x => x.LastEditDate).FirstOrDefault();
            _balances.Clear();

            return results.ToArray();
        }

        #endregion
    }
}