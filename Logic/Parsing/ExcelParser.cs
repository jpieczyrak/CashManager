using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Logic.StocksManagement;
using Logic.TransactionManagement;

namespace Logic.Parsing
{
    public class ExcelParser : IParser
    {
        public List<Transaction> Parse(string input, Stock userStock)
        {
            List<Transaction> transactions = new List<Transaction>();
            string[] lines = input.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] values = line.Split(';');

                bool buying = string.IsNullOrEmpty(values[10]);


                eTransactionType type = string.IsNullOrEmpty(values[10]) ? eTransactionType.Buy : eTransactionType.Work;
                DateTime date = DateTime.ParseExact(values[0], "d.M.yy", CultureInfo.InvariantCulture);

                string title = values[12];
                Transaction transaction = new Transaction(type, date, title, "");
                Subtransaction subtransaction = new Subtransaction();

                string stringWithValue = buying ? values[11] : values[10];
                double value;
                double.TryParse(stringWithValue, out value);

                subtransaction.Value = value;
                subtransaction.Name = title;
                
                transaction.Subtransactions.Add(subtransaction);

                transaction.TargetStock = buying ? Stock.Unknown : userStock;

                Stock sourceStock = !buying ? Stock.Unknown : userStock;
                transaction.TransactionSoucePayments.Add(new TransactionPartPayment(sourceStock, 100, ePaymentType.Percent));

                transactions.Add(transaction);
            }
            return transactions;
        }
    }
}