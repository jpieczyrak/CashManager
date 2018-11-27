using System;
using System.Collections.Generic;
using System.Globalization;

using LogicOld.Model;
using LogicOld.TransactionManagement.TransactionElements;

namespace LogicOld.Parsing
{
    public class ExcelParser : IParser
    {
        public List<Transaction> Parse(string input, Stock userStock, Stock externalStock)
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
                    transactions.Add(MakeTransaction(userStock, externalStock, true, values, line));
                }
                if (working)
                {
                    transactions.Add(MakeTransaction(userStock, externalStock, false, values, line));
                }
            }

            return transactions;
        }

		private Transaction MakeTransaction(Stock userStock, Stock externalStock, bool outcome, IReadOnlyList<string> values, string input)
		{
			var date = DateTime.ParseExact(values[0], "d.M.yy", CultureInfo.InvariantCulture);
			string title = values[12];

			var type = outcome ? eTransactionType.Buy : eTransactionType.Work;

			string stringWithValue = outcome ? values[11] : values[10];
			double.TryParse(stringWithValue, out double value);

			var position = new Position(title, value);

			return new Transaction(type, date, title, "", new List<Position> {position}, userStock, externalStock, input);
		}
    }
}