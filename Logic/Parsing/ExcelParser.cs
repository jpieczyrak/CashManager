using System;
using System.Collections.Generic;
using System.Globalization;

using Logic.LogicObjectsProviders;
using Logic.Model;
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
                    transactions.Add(MakeTransaction(userStock, true, values, line));
                }
                if (working)
                {
                    transactions.Add(MakeTransaction(userStock, false, values, line));
                }
            }

            return transactions;
        }

        private Transaction MakeTransaction(Stock userStock, bool outcome, string[] values, string input)
        {
            DateTime date = DateTime.ParseExact(values[0], "d.M.yy", CultureInfo.InvariantCulture);
            string title = values[12];

            eTransactionType type = outcome ? eTransactionType.Buy : eTransactionType.Work;
            
            string stringWithValue = outcome ? values[11] : values[10];
            double value;
            double.TryParse(stringWithValue, out value);

            Subtransaction subtransaction = new Subtransaction(title, value);

            return new Transaction(type, date, title, "", new List<Subtransaction> { subtransaction }, userStock, StockProvider.Default, input);
        }
    }
}