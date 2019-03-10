using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public class IngBankParser : IParser
    {
        private const string REGEX_PATTERN =
            @"(?<Day>\d{2})\.(?<Month>\d{2})\.(?<Year>\d{4}).*\r?\nKategoria +(?<Category>.*)\r?\n.*\r?\n(?<Title>(.*\r?\n)(.*\r?\n)?(.*\r?\n)?)\r?\nKwota\r?\n(.*\r?\n)?(.*\r?\n)?(?<Sign>(\-)?)(?<ValueWithSpaces>[0-9 ]+),(?<ValueAfterComma>\d*) (?<Currency>\S*)(\r?\n)*Konto\r?\n(?<Account>.*)(\r?\n)*Saldo po transakcji(\r?\n)*(?<BalanceValueWithSpaces>[0-9 ]+),(?<BalanceValueAfterComma>\d*) +(?<BalanceCurrency>\S*)";

        public Dictionary<Stock, Dictionary<DateTime, decimal>> Balances { get; } = new Dictionary<Stock, Dictionary<DateTime, decimal>>();

        #region IParser

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock,
            TransactionType defaultOutcome, TransactionType defaultIncome, bool generateMissingStocks = false)
        {
            Balances.Clear();
            var output = new List<Transaction>();

            var regex = new Regex(REGEX_PATTERN);

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

            string title = string.Join(" ",
                match.Groups["Title"].Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
            string sourceName = match.Groups["Account"]?.Value.Trim() ?? string.Empty;
            string currency = match.Groups["Currency"].Value.Trim();
            string category = match.Groups["Category"].Value.Trim();

            bool negativeSign = match.Groups["Sign"].Value.Equals("-");
            int bigValue = int.Parse(match.Groups["ValueWithSpaces"].Value.Replace(" ", string.Empty));
            int smallValue = int.Parse(match.Groups["ValueAfterComma"].Value);

            decimal value = bigValue + smallValue / 100m;
            var date = new DateTime(year, month, day);
            string note = "";

            bool isBalanceNote = match.Groups["BalanceValueWithSpaces"].Success;
            if (isBalanceNote)
            {
                int bigValueBalance = int.Parse(match.Groups["BalanceValueWithSpaces"].Value.Replace(" ", string.Empty));
                int smallValueBalance = int.Parse(match.Groups["BalanceValueAfterComma"].Value);
                decimal balance = bigValueBalance + smallValueBalance / 100m;
                note = $"{category}, {sourceName} saldo: {balance:#,##0.00} ({currency})";

                if (!Balances.ContainsKey(userStock)) Balances[userStock] = new Dictionary<DateTime, decimal>();
                if (!Balances[userStock].ContainsKey(date)) Balances[userStock][date] = balance;
            }

            var transactionType = negativeSign ? defaultOutcome : defaultIncome;

            var position = new Position(title, value);

            return new Transaction(transactionType, date, title, note, new List<Position> { position }, userStock, externalStock);
        }
    }
}