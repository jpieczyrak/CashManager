using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public class IntelligoBankParser : IParser
    {
        private const string REGEX_PATTERN =
            @"(?<Id>\d+)\s+((?<Year>\d{4})-(?<Month>\d{2})-(?<Day>\d{2})\s+){2}(?<OperationType>.*)\s+(?<Sign>([\-+]))(?<ValueWithSpaces>[0-9 ]+),(?<ValueAfterComma>\d*)\s+(?<Currency>\S*)\s+(?<BalanceValueWithSpaces>[0-9 ]+),(?<BalanceValueAfterComma>\d*)\s+(?<Note>(.*\n){1,8}(Data waluty: \d{4}-\d{2}-\d{2}))";

        private readonly List<Balance> _balances = new List<Balance>();

        public Balance Balance { get; private set; }

        #region IParser

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock,
            TransactionType defaultOutcome, TransactionType defaultIncome)
        {
            var output = new List<Transaction>();

            var regex = new Regex(REGEX_PATTERN);

            foreach (Match match in regex.Matches(input))
                output.Add(CreateTransaction(match, userStock, externalStock, defaultOutcome, defaultIncome));

            Balance = _balances.OrderByDescending(x => x.LastEditDate).FirstOrDefault();
            _balances.Clear();

            return output.ToArray();
        }

        #endregion

        private Transaction CreateTransaction(Match match, Stock userStock, Stock externalStock,
            TransactionType defaultOutcome, TransactionType defaultIncome)
        {
            int day = int.Parse(match.Groups["Day"].Value);
            int month = int.Parse(match.Groups["Month"].Value);
            int year = int.Parse(match.Groups["Year"].Value);

            string title = string.Join(" ",
                match.Groups["Note"].Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
            string currency = match.Groups["Currency"].Value.Trim();

            bool negativeSign = match.Groups["Sign"].Value.Equals("-");
            int bigValue = int.Parse(match.Groups["ValueWithSpaces"].Value.Replace(" ", string.Empty));
            int smallValue = int.Parse(match.Groups["ValueAfterComma"].Value);

            decimal value = bigValue + smallValue / 100m;
            var date = new DateTime(year, month, day);
            string note = match.Groups["OperationType"].Value.Trim();

            bool isBalanceNote = match.Groups["BalanceValueWithSpaces"].Success;
            if (isBalanceNote)
            {
                int bigValueBalance = int.Parse(match.Groups["BalanceValueWithSpaces"].Value.Replace(" ", string.Empty));
                int smallValueBalance = int.Parse(match.Groups["BalanceValueAfterComma"].Value);
                decimal balance = bigValueBalance + smallValueBalance / 100m;
                note = $"{note} saldo: {balance:#,##0.00} ({currency})";

                _balances.Add(new Balance(date, balance));
            }

            var transactionType = negativeSign ? defaultOutcome : defaultIncome;

            var position = new Position(title, value);

            return new Transaction(transactionType, date, title, note, new List<Position> { position }, userStock, externalStock,
                match.Value);
        }
    }
}