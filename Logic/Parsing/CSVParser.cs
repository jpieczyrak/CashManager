using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Logic.FilesOperations;
using Logic.LogicObjectsProviders;
using Logic.Model;
using Logic.TransactionManagement.TransactionElements;

namespace Logic.Parsing
{
    public class CSVParser : IParser
    {
        public List<Transaction> Parse(string input, Stock userStock)
        {
            List<Transaction> output = new List<Transaction>();
            string[] transactions = input.Split(CSVFormater.TRANSACTION_SPLIT_ELEMENT);

            foreach (string transaction in transactions)
            {
                if(string.IsNullOrEmpty(transaction)) continue;
                
                string[] transactionElements = transaction.Split(CSVFormater.TRANSACTION_ELEMENT_SPLIT_ELEMENT);

                DateTime date = DateTime.ParseExact(transactionElements[0], CSVFormater.DATE_FORMAT, CultureInfo.InvariantCulture);
                DateTime creationDate =  DateTime.ParseExact(transactionElements[1], CSVFormater.DATE_FORMAT, CultureInfo.InvariantCulture);
                DateTime lastEdit = DateTime.ParseExact(transactionElements[2], CSVFormater.DATE_FORMAT, CultureInfo.InvariantCulture);
                string title = transactionElements[3];
                string note = transactionElements[4];
                Stock stock = StockProvider.GetStock(transactionElements[5]);
                eTransactionType transactionType;
                eTransactionType.TryParse(transactionElements[6], true, out transactionType);

                string[] subs = transactionElements[7].Split(CSVFormater.SUBELEMENT_SPLIT_ELEMENT);

                List<Subtransaction> subtransactions = new List<Subtransaction>();

                foreach (string sub in subs)
                {
                    if(string.IsNullOrEmpty(sub)) continue;

                    string[] subtransactionElement = sub.Split(CSVFormater.SUBELEMENT_ELEMENT_SPLIT_ELEMENT);

                    string name = subtransactionElement[0];
                    double value = double.Parse(subtransactionElement[1]);
                    string category = subtransactionElement[2];
                    string tags = subtransactionElement[3];

                    subtransactions.Add(new Subtransaction(name, value, category, tags));
                }

                string[] paymentString = transactionElements[8].Split(CSVFormater.SUBELEMENT_SPLIT_ELEMENT); //probably is only one in list.. todo: check later!
                string paymentProperString = paymentString.FirstOrDefault(x => !string.IsNullOrEmpty(x));

                if (paymentProperString != null)
                {
                    string[] paymentElements = paymentProperString?.Split(CSVFormater.SUBELEMENT_ELEMENT_SPLIT_ELEMENT);

                    double value = double.Parse(paymentElements[0]);
                    value = value == 0 ? subtransactions.Sum(x => x.Value) : value;

                    ePaymentType paymentType;
                    ePaymentType.TryParse(paymentElements[1], true, out paymentType);
                    Stock paymentStock = StockProvider.GetStock(paymentElements[2]);

                    var payment = new Payment(stock, paymentStock, value);

                    output.Add(new Transaction(transactionType, date, title, note, creationDate, lastEdit, subtransactions, payment));
                }
            }

            return output;
        }
    }
}