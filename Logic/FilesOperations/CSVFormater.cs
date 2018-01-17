using System;
using System.Collections.Generic;
using System.Linq;

using Logic.Model;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

namespace Logic.FilesOperations
{
    public class CSVFormater
    {
        public static char TRANSACTION_SPLIT_ELEMENT = '\n';
        public static char TRANSACTION_ELEMENT_SPLIT_ELEMENT = ';';

        public static char SUBELEMENT_SPLIT_ELEMENT = '\t';
        public static char SUBELEMENT_ELEMENT_SPLIT_ELEMENT = '|';

        public static string DATE_FORMAT = "dd:MM:yyyy";

        public static string ToCSV(Transactions transactions)
        {
            return transactions.TransactionsList.Aggregate("", (current, transaction) => current + ToCSV(transaction));
        }

        public static string ToCSV(Transaction t)
        {
            string subtransactionsCSV = ToCSV(t.Subtransactions);

            return string.Format("{0}{10}{1}{10}{2}{10}{3}{10}{4}{10}{5}{10}{6}{10}{7}{10}{8}{9}", 
                t.BookDate.ToString(DATE_FORMAT), 
                t.InstanceCreationDate.ToString(DATE_FORMAT), 
                t.LastEditDate.ToString(DATE_FORMAT), 
                t.Title, 
                t.Note,
                t.Type, 
                subtransactionsCSV, 
                t.UserStock.Name + SUBELEMENT_ELEMENT_SPLIT_ELEMENT + t.ExternalStock.Name,
                TRANSACTION_SPLIT_ELEMENT,
                TRANSACTION_ELEMENT_SPLIT_ELEMENT);
        }

        private static string ToCSV(IEnumerable<Subtransaction> subtransactions)
        {
            return subtransactions.Aggregate("",
                (current, s) =>
                    current + string.Format("{0}{1}{2}{3}{4}{5}", s.Title, SUBELEMENT_ELEMENT_SPLIT_ELEMENT, s.Value,
                        SUBELEMENT_ELEMENT_SPLIT_ELEMENT, s.Category, SUBELEMENT_ELEMENT_SPLIT_ELEMENT));
        }
    }
}