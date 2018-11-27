using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using LogicOld.Model;
using LogicOld.TransactionManagement.TransactionElements;

namespace LogicOld.Parsing
{
    public class GetinBankParser : IParser
    {
        private const string TRANSFER_REGEX =
            @"(?<Day>\d{2})\.(?<Month>\d{2})\.(?<Year>\d{4}) \– (?<OperationType>(\S| )*)(\r\n|\n)(?<SourceName>(\S| )*) \– (?<Title>(\S| )*)(\r\n|\n)*(?<Sign>(-|\+))(?<ValueWithSpaces>[0-9 ]+),(?<ValueAfterComma>\d*) (?<Currency>\S*)";

        private const string CARD_OPERATION_REGEX =
            @"(?<Day>\d{2})\.(?<Month>\d{2})\.(?<Year>\d{4}) \– (?<OperationType>(\S| )*)(\r\n|\n)(?<Title>(\S| )*)(\r\n|\n)*(?<Sign>(-|\+))(?<ValueWithSpaces>[0-9 ]+),(?<ValueAfterComma>\d*) (?<Currency>\S*)";

        #region IParser

        public List<Transaction> Parse(string input, Stock userStock, Stock externalStock)
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
                    output.Add(CreateTransaction(m, userStock, externalStock));
                }
                else
                {
                    output.Add(CreateTransaction(match, userStock, externalStock));
                }
            }

            return output;
        }

        #endregion

        private Transaction CreateTransaction(Match match, Stock userStock, Stock externalStock)
        {
            int day = int.Parse(match.Groups["Day"].Value);
            int month = int.Parse(match.Groups["Month"].Value);
            int year = int.Parse(match.Groups["Year"].Value);

            string title = match.Groups["Title"].Value;
            string operationType = match.Groups["OperationType"].Value;
            string sourceName = match.Groups["SourceName"]?.Value ?? "";
            string currency = match.Groups["Currency"].Value;

            bool positiveSign = match.Groups["Sign"].Value.Equals("+");
            int bigValue = int.Parse(match.Groups["ValueWithSpaces"].Value.Replace(" ", ""));
            int smallValue = int.Parse(match.Groups["ValueAfterComma"].Value);

            double value = bigValue + smallValue / 100.0;
            var date = new DateTime(year, month, day);
            string note = $"{sourceName}{(sourceName != "" ? ": " : "")}{operationType} ({currency})";
            var transactionType = positiveSign ? eTransactionType.Work : eTransactionType.Buy;
            
            var position = new Position(title, value);

			return new Transaction(transactionType, date, title, note, new List<Position> { position }, userStock, externalStock, match.Value);
        }
    }
}