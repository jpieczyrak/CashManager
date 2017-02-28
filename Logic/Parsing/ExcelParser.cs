using System;
using System.Collections.Generic;
using System.Globalization;

using Logic.Model;
using Logic.StocksManagement;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

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

                bool buying = !string.IsNullOrEmpty(values[11]);
                bool working = !string.IsNullOrEmpty(values[10]);

                if (buying)
                {
                    transactions.Add(MakeTransaction(userStock, true, values));
                }
                if (working)
                {
                    transactions.Add(MakeTransaction(userStock, false, values));
                }
            }
            return transactions;
        }

        private Transaction MakeTransaction(Stock userStock, bool outcome, string[] values)
        {
            DateTime date = DateTime.ParseExact(values[0], "d.M.yy", CultureInfo.InvariantCulture);
            string title = values[12];

            eTransactionType type = outcome ? eTransactionType.Buy : eTransactionType.Work;

            Transaction transaction = new Transaction(type, date, title, "");

            Subtransaction subtransaction = new Subtransaction();

            string stringWithValue = outcome ? values[11] : values[10];
            double value;
            double.TryParse(stringWithValue, out value);

            subtransaction.Value = value;
            subtransaction.Name = title;

            transaction.Subtransactions.Add(subtransaction);

            transaction.TargetStockId = outcome ? Stock.Unknown.Id : userStock.Id;

            Stock sourceStock = !outcome ? Stock.Unknown : userStock;
            transaction.TransactionSoucePayments.Add(new TransactionPartPayment(sourceStock, 100, ePaymentType.Percent));

            return transaction;
        }
    }
}