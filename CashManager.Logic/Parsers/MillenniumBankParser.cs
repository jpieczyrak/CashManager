using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public class MillenniumBankParser : IParser
    {
        private const int LINES_PER_ENTRY = 7;
        private readonly List<Balance> _balances = new List<Balance>();

        public Dictionary<Stock, Balance> Balances { get; private set; } = new Dictionary<Stock, Balance>();

        #region IParser

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock, TransactionType defaultOutcome,
            TransactionType defaultIncome, bool generateMissingStocks = false)
        {
            if (string.IsNullOrEmpty(input)) return null;

            var results = new List<Transaction>();
            try
            {
                var elements = input.Remove(0, input.IndexOf("Zaznacz", StringComparison.Ordinal))
                                    .Replace("\t", string.Empty)
                                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                elements = elements.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                if (elements.Length >= LINES_PER_ENTRY)
                {
                    int i = 0;
                    while (i + LINES_PER_ENTRY <= elements.Length)
                    {
                        try
                        {
                            string type = elements[i + 2].Split(':')[1].Trim();
                            string title = elements[i + 3].Split(':')[1].Trim();
                            DateTime date = DateTime.Parse(elements[i + 1].Split('/')[0].Trim());
                            string stringValue = elements[i + 4].Replace(" PLN", string.Empty).Replace(" ", string.Empty).Trim();
                            decimal value = decimal.Parse(stringValue);
                            string balanceString = elements[i + 5]
                                                   .Replace("Saldo:", string.Empty)
                                                   .Replace(" ", string.Empty)
                                                   .Replace("PLN", string.Empty);
                            decimal balance = decimal.Parse(balanceString);

                            bool income = value > 0m;

                            var positions = new[] { new Position(title, Math.Abs(value)) };
                            string note = $"{type}, Saldo: {balance:#,##0.00}";
                            var transaction = new Transaction(income ? defaultIncome : defaultOutcome, date, title, note,
                                positions, userStock, externalStock, string.Join("\n", elements.Skip(i - 1).Take(4)));

                            results.Add(transaction);
                            _balances.Add(new Balance(date, balance));
                        }
                        catch (Exception e) { }

                        i += LINES_PER_ENTRY;
                    }
                }

                Balances[userStock] = _balances.OrderByDescending(x => x.LastEditDate).FirstOrDefault();
                _balances.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return results.ToArray();
        }

        #endregion
    }
}