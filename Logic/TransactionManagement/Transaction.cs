﻿using System;
using System.Collections.Generic;
using Logic.StocksManagement;
using Logic.ValueCalculationStrategies;

namespace Logic.TransactionManagement
{
    public class Transaction
    {
        public eTransactionType Type { get; private set; }

        public DateTime Date { get; private set; }

        /// <summary>
        /// Full transaction value (total payment)
        /// </summary>
        private float RawValue { get; set; }

        /// <summary>
        /// Percent contribution in transaction (0; 100)
        /// </summary>
        private float Contribution { get; set; }

        public string Title { get; private set; }

        public string Note { get; private set; }

        public Category Category { get; private set; }

        /// <summary>
        /// List of tags
        /// </summary>
        public List<Tag> Tags { get; private set; }

        IValueCalculationStrategy _strategy;

        public Stock From { get; private set; }

        public Stock To { get; private set; }

        public float Value => _strategy.CalculateValue(RawValue, Contribution);

        public Transaction(eTransactionType type, DateTime date, float rawValue, string title, string note, Category category, List<Tag> tags, Stock @from, Stock to)
        {
            _strategy = new BasicCalculationStrategy();
            Type = type;
            Date = date;
            RawValue = rawValue;
            Title = title;
            Note = note;
            Category = category;
            Tags = tags;
            From = @from;
            To = to;
        }

        public Transaction(eTransactionType type, DateTime date, float rawValue, float contribution, string title, string note, Category category, List<Tag> tags, Stock @from, Stock to) : this(type, date, rawValue, title, note, category, tags, from, to)
        {
            Contribution = contribution;
        }
    }
}
