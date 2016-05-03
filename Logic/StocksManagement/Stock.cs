﻿using System;
using System.Runtime.Serialization;
using Logic.TransactionManagement;

namespace Logic.StocksManagement
{
    /// <summary>
    /// Stores name, starting value and actual value of "part of" your wallet.
    /// You can have more than one Stock in your Wallet (like bank account, phisic wallet, second bank acc ect)
    /// Actual value is based on starting value + income - outcome from transactions.
    /// </summary>
    [DataContract]
    public class Stock
    {
        [DataMember]
        protected string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Value of stock when stock is being added to wallet (in app)
        /// </summary>
        [DataMember]
        float StartingValue { get; set; }

        /// <summary>
        /// Value calculated for choosen date, based on transactions done on stock since created
        /// </summary>
        [DataMember]
        float ActualValue { get; set; }

        public float GetActualValue(Transactions transactions, TimeFrame timeframe)
        {
            ActualValue = CalculateActualValue(transactions, timeframe);
            return ActualValue;
        }

        private float CalculateActualValue(Transactions transactions, TimeFrame timeframe)
        {
            foreach (var transaction in transactions.TransactionsList)
            {
                //if stock == this stock; TODO: move level up (wallet should update stocks)
                if (timeframe.Contains(transaction.Date))
                {
                    
                }
            }
            throw new NotImplementedException();
        }

        public Stock(string name, float startingValue)
        {
            Name = name;
            StartingValue = startingValue;
        }
    }
}