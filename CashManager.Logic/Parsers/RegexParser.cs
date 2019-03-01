using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public class RegexParser : IParser
    {
        public Dictionary<Stock, Dictionary<DateTime, decimal>> Balances { get; } = new Dictionary<Stock, Dictionary<DateTime, decimal>>();

        public string RegexValue { get; set; }

        #region IParser

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock,
            TransactionType defaultOutcome, TransactionType defaultIncome, bool generateMissingStocks = false)
        {
            Balances.Clear();
            var output = new List<Transaction>();

            var regex = new Regex(RegexValue);

            foreach (Match match in regex.Matches(input))
                output.Add(CreateTransaction(match, userStock, externalStock, defaultOutcome, defaultIncome));

            return output.ToArray();
        }

        #endregion

        private Transaction CreateTransaction(Match match, Stock userStock, Stock externalStock,
            TransactionType defaultOutcome, TransactionType defaultIncome)
        {
            int day = int.Parse(match.Groups["Day"].Value);
            int month = int.Parse(match.Groups["Month"].Value);
            int year = int.Parse(match.Groups["Year"].Value);

            int id = int.Parse(match.Groups["Id"].Value);

            string operationType = match.Groups["OperationType"].Value.Trim();
            var noteLines = match.Groups["Note"].Value.Split('\n');
            string title = match.Groups["Title"].Value.Trim();
            string currency = match.Groups["Currency"].Value.Trim();

            bool negativeSign = match.Groups["Sign"].Value.Equals("-");
            int bigValue = int.Parse(match.Groups["ValueWithSpaces"].Value.Replace(" ", string.Empty));
            int smallValue = int.Parse(match.Groups["ValueAfterComma"].Value);

            decimal value = bigValue + smallValue / 100m;
            var date = new DateTime(year, month, day);
            string note =
                $"{id} - {string.Join(" ", noteLines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))} {operationType}";

            bool isBalanceNote = match.Groups["BalanceValueWithSpaces"].Success;
            if (isBalanceNote)
            {
                int bigValueBalance = int.Parse(match.Groups["BalanceValueWithSpaces"].Value.Replace(" ", string.Empty));
                int smallValueBalance = int.Parse(match.Groups["BalanceValueAfterComma"].Value);
                decimal balance = bigValueBalance + smallValueBalance / 100m;
                note = $"{note} saldo: {balance:#,##0.00} ({currency})";

                if (!Balances.ContainsKey(userStock)) Balances[userStock] = new Dictionary<DateTime, decimal>();
                Balances[userStock].Add(date, balance);
            }

            var transactionType = negativeSign ? defaultOutcome : defaultIncome;

            var position = new Position(title, value);

            return new Transaction(transactionType, date, title, note, new List<Position> { position }, userStock, externalStock);
        }
    }
}