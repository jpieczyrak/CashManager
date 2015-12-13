using System;
using System.Collections.Generic;
using Logic.StocksManagement;
using Logic.ValueCalculationStrategies;

namespace Logic.TransactionManagement
{
    public class Transaction
    {
        eTransactionType Type { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// Full transaction value (total payment)
        /// </summary>
        float RawValue { get; set; }

        /// <summary>
        /// Percent contribution in transaction <0; 100>
        /// </summary>
        float Contribution { get; set; }

        string Title { get; set; }

        string Note { get; set; }

        Category Category { get; set; }

        /// <summary>
        /// List of tags
        /// </summary>
        List<Tag> Tags { get; set; }

        ValueCalculationStrategy _strategy;

        Stock From { get; set; }

        Stock To { get; set; }
    }
}
