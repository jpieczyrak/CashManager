using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public class GetinBankParser : IParser
    {
        private readonly List<Balance> _balances = new List<Balance>();
        private const string TRANSFER_REGEX =
            @"(?<Day>\d{2})\.(?<Month>\d{2})\.(?<Year>\d{4}) \– (?<OperationType>(\S| )*)(\r\n|\n)(?<SourceName>(\S| )*) \– (?<Title>(\S| )*)(\r\n|\n)*(?<Sign>(-|\+))(?<ValueWithSpaces>[0-9 ]+),(?<ValueAfterComma>\d*) (?<Currency>\S*)( saldo po operacji: (?<BalanceValueWithSpaces>[0-9 ]+),(?<BalanceValueAfterComma>\d*) (?<BalanceCurrency>\S*))?";

        private const string CARD_OPERATION_REGEX =
            @"(?<Day>\d{2})\.(?<Month>\d{2})\.(?<Year>\d{4}) (\–|\-) (?<OperationType>(\S| )*)(\r\n|\n)(?<Title>(\S| )*)(\r\n|\n)*(?<Sign>(-|\+))(?<ValueWithSpaces>[0-9 ]+),(?<ValueAfterComma>\d*) (?<Currency>\S*)( saldo po operacji: (?<BalanceValueWithSpaces>[0-9 ]+),(?<BalanceValueAfterComma>\d*) (?<BalanceCurrency>\S*))?";

        public Dictionary<Stock, Balance> Balances { get; private set; } = new Dictionary<Stock, Balance>();

        #region IParser

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock,
            TransactionType defaultOutcome, TransactionType defaultIncome, bool generateMissingStocks = false)
        {
            var output = new List<Transaction>();

            var transfer = new Regex(TRANSFER_REGEX);
            var cardOperation = new Regex(CARD_OPERATION_REGEX);

            foreach (Match match in cardOperation.Matches(input))
            {
                //card operation matches to all results
                //transfer is special option (with additional info),
                //so we want to check that info if possible
                if (transfer.IsMatch(match.Value))
                {
                    var m = transfer.Match(match.Value);
                    output.Add(CreateTransaction(m, userStock, externalStock, defaultOutcome, defaultIncome));
                }
                else
                {
                    output.Add(CreateTransaction(match, userStock, externalStock, defaultOutcome, defaultIncome));
                }
            }

            Balances[userStock] = _balances.OrderByDescending(x => x.LastEditDate).FirstOrDefault();
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

            string title = match.Groups["Title"].Value;
            string operationType = match.Groups["OperationType"].Value;
            string sourceName = match.Groups["SourceName"]?.Value ?? string.Empty;
            string currency = match.Groups["Currency"].Value;

            bool positiveSign = match.Groups["Sign"].Value.Equals("+");
            int bigValue = int.Parse(match.Groups["ValueWithSpaces"].Value.Replace(" ", string.Empty));
            int smallValue = int.Parse(match.Groups["ValueAfterComma"].Value);

            decimal value = bigValue + smallValue / 100m;
            var date = new DateTime(year, month, day);
            string note = $"{sourceName}{(sourceName != string.Empty ? ": " : string.Empty)}{operationType} ({currency})";

            bool isBalanceNote = match.Groups["BalanceValueWithSpaces"].Success;
            if (isBalanceNote)
            {
                int bigValueBalance = int.Parse(match.Groups["BalanceValueWithSpaces"].Value.Replace(" ", string.Empty));
                int smallValueBalance = int.Parse(match.Groups["BalanceValueAfterComma"].Value);
                decimal balance = bigValueBalance + smallValueBalance / 100m;
                note += $" Saldo: {balance:#,##0.00}";

                _balances.Add(new Balance(date, balance));
            }

            var transactionType = positiveSign ? defaultIncome : defaultOutcome;
            var position = new Position(title, value);

            return new Transaction(transactionType, date, title, note, new List<Position> { position }, userStock, externalStock,
                match.Value);
        }
    }
}