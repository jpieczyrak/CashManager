using System;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Logic.DefaultData.InputParsers;

namespace CashManager.Logic.DefaultData
{
    public class DefaultDataProvider : IDataProvider
    {
        protected TransactionType _workType;
        protected TransactionType _buyType;
        protected TransactionType _transferInType;
        protected TransactionType _transferOutType;
        protected TransactionType _giftsType;
        protected Tag[] _tags;

        public DefaultDataProvider()
        {
            GetCategories();
            GetTransactionTypes();
            GetTags();
        }

        public virtual Transaction[] GetTransactions(Stock[] stocks, Category[] categories, TransactionType[] types, Tag[] tags)
        {
            return new Transaction[] { };
        }

        public Category[] GetCategories()
        {
            return new CategoryParser().Parse(Strings.DefaultCategories).Concat(new [] { Category.Default }).ToArray();
        }

        public virtual Stock[] GetStocks()
        {
            return new[]
            {
                new Stock { Name = Strings.DefaultUserBankAccount, IsUserStock = true, Balance = new Balance(DateTime.Today, 0) },
                new Stock { Name = Strings.DefaultWallet, IsUserStock = true, Balance = new Balance(DateTime.Today, 0) },
                new Stock { Name = Strings.DefaultExternalStock },
            };
        }

        public TransactionType[] GetTransactionTypes()
        {
            _workType = new TransactionType { Income = true, Name = Strings.Work, IsDefault = true };
            _buyType = new TransactionType { Outcome = true, Name = Strings.Buy, IsDefault = true };
            _transferInType = new TransactionType { Name = Strings.TransferIn, Income = true, IsTransfer = true };
            _transferOutType = new TransactionType { Name = Strings.TransferOut, Outcome = true, IsTransfer = true };
            _giftsType = new TransactionType { Income = true, Name = Strings.Gifts };
            return new[]
            {
                _workType,
                _buyType,
                _transferInType,
                _transferOutType,
                _giftsType
            };
        }

        public Tag[] GetTags()
        {
            _tags = new[]
            {
                new Tag { Name = "tag 1" },
                new Tag { Name = "tag 2" },
                new Tag { Name = "tag 3" },
            };
            return _tags;
        }
    }
}