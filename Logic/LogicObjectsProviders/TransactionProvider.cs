using System.Collections.ObjectModel;

using AutoMapper;

using Logic.Database;
using Logic.Model;
using Logic.Utils;

namespace Logic.LogicObjectsProviders
{
    public static class TransactionProvider
    {
        private static TrulyObservableCollection<Transaction> _transactions;

        public static ReadOnlyCollection<Transaction> Transactions => new ReadOnlyCollection<Transaction>(_transactions);

        public static void Load()
        {
            _transactions = new TrulyObservableCollection<Transaction>();

            var dtos = DatabaseProvider.DB.Read<DTO.Transaction>();
            foreach (var dto in dtos)
            {
                _transactions.Add(Mapper.Map<DTO.Transaction, Transaction>(dto));
            }
        }

        public static void Add(Transaction transaction)
        {
            _transactions.Add(transaction);
            DatabaseProvider.DB.Save(Mapper.Map<Transaction, DTO.Transaction>(transaction));
        }
    }
}