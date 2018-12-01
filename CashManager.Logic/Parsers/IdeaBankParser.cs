using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public class IdeaBankParser : IParser
    {
        #region IParser

        public List<Transaction> Parse(string input, Stock userStock, Stock externalStock, TransactionType defaultOutcome,
            TransactionType defaultIncome)
        {
            if (string.IsNullOrEmpty(input)) return null;

            var results = new List<Transaction>();
            var elements = input.Split(new [] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (elements.Length >= 4)
            {
                int i = 0;
                while ((i+4) <= elements.Length)
                {
                    try
                    {
                        string title = elements[i];
                        DateTime date = DateTime.Parse(elements[i + 1]);
                        string stringValue = elements[i + 2].Replace(" PLN", string.Empty).Replace(" ", string.Empty);
                        double value = double.Parse(stringValue);
                        double saldo = double.Parse(elements[i + 3].Replace(" ", string.Empty));

                        bool income = value > 0.0d;

                        var positions = new[] { new Position(title, Math.Abs(value)) };
                        var transaction = new Transaction(income ? defaultIncome : defaultOutcome, date, title, $"Saldo: {saldo:#,##0.00}",
                            positions, userStock, externalStock, string.Join("\n", elements.Skip(i - 1).Take(4)));
                        
                        results.Add(transaction);
                    }
                    catch (Exception e)
                    {
                        
                    }
                    i += 4;
                }
            }

            return results;
        }

        #endregion
    }
}