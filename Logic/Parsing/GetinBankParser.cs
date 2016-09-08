using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Logic.StocksManagement;
using Logic.TransactionManagement;

namespace Logic.Parsing
{
    public class GetinBankParser : IParser
    {
        private const string TRANSFER_REGEX = @"(?<Day>\d{2})\.(?<Month>\d{2})\.(?<Year>\d{4}) \� (?<OperationType>(\S| )*)(\r\n|\n)(?<SourceName>(\S| )*) \� (?<Title>(\S| )*)(\r\n|\n)*(?<Sign>(-|\+))(?<ValueWithSpaces>[0-9 ]+),(?<ValueAfterComma>\d*) (?<Currency>\S*)";
        private const string CARD_OPERATION_REGEX = @"(?<Day>\d{2})\.(?<Month>\d{2})\.(?<Year>\d{4}) \� (?<OperationType>(\S| )*)(\r\n|\n)(?<Title>(\S| )*)(\r\n|\n)*(?<Sign>(-|\+))(?<ValueWithSpaces>[0-9 ]+),(?<ValueAfterComma>\d*) (?<Currency>\S*)";

        public List<Transaction> Parse(string input, Stock userStock)
        {
            List<Transaction> output = new List<Transaction>();

            Regex transfer = new Regex(TRANSFER_REGEX);
            Regex cardOperation = new Regex(CARD_OPERATION_REGEX);
            
            foreach (Match match in cardOperation.Matches(input))
            {
                //card operation matches to all results
                //transfer is special option (with additional info),
                //so we want to check that info if possible
                if (transfer.IsMatch(match.Value))
                {
                    Match m = transfer.Match(match.Value);
                    output.Add(CreateTransaction(m, userStock));
                }
                else
                {
                    output.Add(CreateTransaction(match, userStock));
                }
            }

            return output;
        }

        private Transaction CreateTransaction(Match match, Stock userStock)
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

            float value = (float)(bigValue + smallValue / 100.0);
            DateTime date = new DateTime(year, month, day);
            string note = string.Format("{0}: {1} ({2})", sourceName, operationType, currency);
            eTransactionType transactionType = positiveSign ? eTransactionType.Work : eTransactionType.Buy;
            
            Transaction transaction = new Transaction(transactionType, date, title, note);
            Subtransaction subtransaction = new Subtransaction
            {
                Value = value,
                Name = title
            };
            transaction.Subtransactions.Add(subtransaction);

            transaction.TargetStock = positiveSign ? userStock : Stock.Unknown;
            Stock sourceStock = positiveSign ? Stock.Unknown : userStock;
            transaction.TransactionSoucePayments.Add(new TransactionPartPayment(sourceStock, 100, ePaymentType.Percent));

            return transaction;
        }
    }
}