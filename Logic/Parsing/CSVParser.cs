using System;
using System.Collections.Generic;
using System.Globalization;
using Logic.FilesOperations;
using Logic.LogicObjectsProviders;
using Logic.StocksManagement;
using Logic.TransactionManagement;
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
                Guid stockId = Guid.Parse(transactionElements[5]);
                Stock stock = StockProvider.GetStock(stockId);
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
                
                string[] payments = transactionElements[8].Split(CSVFormater.SUBELEMENT_SPLIT_ELEMENT);
                List<TransactionPartPayment> partPayments = new List<TransactionPartPayment>();

                foreach (string payment in payments)
                {
                    if (string.IsNullOrEmpty(payment)) continue;

                    string[] paymentElements = payment.Split(CSVFormater.SUBELEMENT_ELEMENT_SPLIT_ELEMENT);

                    double value = double.Parse(paymentElements[0]);
                    ePaymentType paymentType;
                    ePaymentType.TryParse(paymentElements[1], true, out paymentType);
                    Guid paymentStockId = Guid.Parse(paymentElements[2]);
                    Stock paymentStock = StockProvider.GetStock(paymentStockId);

                    partPayments.Add(new TransactionPartPayment(paymentStock, value, paymentType));
                }

                output.Add(new Transaction(transactionType, date, title, note, stock, creationDate, lastEdit, subtransactions, partPayments));
            }

            return output;
        }
    }
}