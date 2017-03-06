using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using Logic.Database;
using Logic.Model;
using Logic.Utils;

namespace Logic.LogicObjectsProviders
{
    public static class TransactionProvider
    {
        private static TrulyObservableCollection<Transaction> _transactions;

        public static TrulyObservableCollection<Transaction> Transactions => _transactions ?? (_transactions = Load());

        public static TrulyObservableCollection<Transaction> Load()
        {
            var dtos = DatabaseProvider.DB.Read<DTO.Transaction>();
            var list = dtos.Select(Mapper.Map<DTO.Transaction, Transaction>);
            _transactions = new TrulyObservableCollection<Transaction>(list);
            _transactions.CollectionChanged += TransactionsOnCollectionChanged;

            return _transactions;
        }

        public static void Add(Transaction transaction)
        {
            Transactions.Add(transaction);
            DatabaseProvider.DB.Update(Mapper.Map<Transaction, DTO.Transaction>(transaction));
        }

        private static void TransactionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.NewItems != null)
            {
                foreach (Transaction transaction in notifyCollectionChangedEventArgs.NewItems)
                {
                    DatabaseProvider.DB.Update(Mapper.Map<Transaction, DTO.Transaction>(transaction));
                }
            }
            if (notifyCollectionChangedEventArgs.OldItems != null)
            {
                foreach (Transaction transaction in notifyCollectionChangedEventArgs.OldItems)
                {
                    DatabaseProvider.DB.Remove(Mapper.Map<Transaction, DTO.Transaction>(transaction));
                }
            }
        }
    }
}