﻿using System.Collections.Generic;
using System.Linq;

using LogicOld.Model;
using LogicOld.TransactionManagement.TransactionElements;

namespace LogicOld.FilesOperations
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
            string positionsCSV = ToCSV(t.Positions);

            return string.Format("{0}{9}{1}{9}{2}{9}{3}{9}{4}{9}{5}{9}{6}{9}{7}{9}{8}", 
                t.BookDate.ToString(DATE_FORMAT), 
                t.InstanceCreationDate.ToString(DATE_FORMAT), 
                t.LastEditDate.ToString(DATE_FORMAT), 
                t.Title, 
                t.Note,
                t.Type, 
                positionsCSV, 
                t.UserStock.Name + SUBELEMENT_ELEMENT_SPLIT_ELEMENT + t.ExternalStock.Name,
				TRANSACTION_SPLIT_ELEMENT,
                TRANSACTION_ELEMENT_SPLIT_ELEMENT);
        }

        private static string ToCSV(IEnumerable<Position> positions)
        {
            return positions.Aggregate("",
                (current, s) =>
                    current + $"{s.Title}{SUBELEMENT_ELEMENT_SPLIT_ELEMENT}{s.Value}{SUBELEMENT_ELEMENT_SPLIT_ELEMENT}{s.Category}{SUBELEMENT_ELEMENT_SPLIT_ELEMENT}");
        }
    }
}