using System;
using System.Collections.Generic;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public class IdeaBankParser : IParser
    {
        private const string NOT_PERFORMED_TRANSACTION = "-";
        private const int LINES_PER_ENTRY = 4;

        public Dictionary<Stock, Dictionary<DateTime, decimal>> Balances { get; } = new Dictionary<Stock, Dictionary<DateTime, decimal>>();

        #region IParser

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock, TransactionType defaultOutcome,
            TransactionType defaultIncome, bool generateMissingStocks = false)
        {
            Balances.Clear();
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
                        var transaction = new Transaction(income ? defaultIncome : defaultOutcome, date, title,
                            $"Saldo: {balance:#,##0.00}",
                            positions, userStock, externalStock);

                        results.Add(transaction);
                        if (!Balances.ContainsKey(userStock)) Balances[userStock] = new Dictionary<DateTime, decimal>();
                        Balances[userStock].Add(date, balance);
                    }
                    catch (Exception)
                    {
                        bool hasTitle = !string.IsNullOrEmpty(elements[i]);
                        bool isSkippedTransaction = NOT_PERFORMED_TRANSACTION.Equals(elements[i + 1].Trim());
                        if (hasTitle && isSkippedTransaction) i -= 2;
                    }

                    i += LINES_PER_ENTRY;
                }
            }

            return results.ToArray();
        }

        #endregion
    }
}