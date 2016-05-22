using System.Collections.Generic;
using System.Linq;
using Logic.TransactionManagement;

namespace Logic.FilesOperations
{
    public class CSVFormater
    {
        public static char TRANSACTION_SPLIT_ELEMENT = '\n';
        public static char TRANSACTION_ELEMENT_SPLIT_ELEMENT = ';';
        public static char SUBTRANSACTION_SPLIT_ELEMENT = '\t';
        public static char SUBTRANSACTION_ELEMENT_SPLIT_ELEMENT = '|';
        public static char PARTTRANSACTION_SPLIT_ELEMENT = '>';
        public static char PARTTRANSACTION_ELEMENT_SPLIT_ELEMENT = '-';

        public static string DATE_FORMAT = "dd:MM:yyyy";

        public static string ToCSV(Transactions transactions)
        {
            return transactions.TransactionsList.Aggregate("", (current, transaction) => current + ToCSV(transaction));
        }

        public static string ToCSV(Transaction t)
        {
            string subtransactionCSV = ToCSV(t.Subtransactions);
            string sourcesCSV = ToCSV(t.TransactionSoucePayments);

            return string.Format("{0}{10}{1}{10}{2}{10}{3}{10}{4}{10}{5}{10}{6}{10}{7}{10}{8}{9}", 
                t.Date.ToString(DATE_FORMAT), 
                t.CreationDate.ToString(DATE_FORMAT), 
                t.LastEditDate.ToString(DATE_FORMAT), 
                t.Title, 
                t.Note, 
                t.TargetStock.Id, 
                t.Type, 
                subtransactionCSV, 
                sourcesCSV,
                TRANSACTION_SPLIT_ELEMENT,
                TRANSACTION_ELEMENT_SPLIT_ELEMENT);
        }

        private static string ToCSV(IEnumerable<Subtransaction> subtransactions)
        {
            return subtransactions.Aggregate("",
                (current, s) =>
                    current + string.Format("{0}{5}{1}{5}{2}{5}{3}{4}",
                        s.Name,
                        s.Value,
                        s.Category,
                        s.Tags,
                        SUBTRANSACTION_SPLIT_ELEMENT,
                        SUBTRANSACTION_ELEMENT_SPLIT_ELEMENT));
        }

        private static string ToCSV(IEnumerable<TransactionPartPayment> parts)
        {
            return parts.Aggregate("",
                (current, p) =>
                    current + string.Format("{0}{4}{1}{4}{2}{4}{3}",
                        p.Value,
                        p.PaymentType,
                        p.Stock.Id,
                        SUBTRANSACTION_SPLIT_ELEMENT,
                        SUBTRANSACTION_ELEMENT_SPLIT_ELEMENT));
        }
    }
}