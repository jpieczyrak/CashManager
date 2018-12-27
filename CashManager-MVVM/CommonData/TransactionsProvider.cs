using System.Collections.Generic;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Transactions;

using CashManager_MVVM.Model;

using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.CommonData
{
    public class TransactionsProvider
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public TrulyObservableCollection<Transaction> AllTransactions { get; }

        public TransactionsProvider(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;

            var query = new TransactionQuery();
            var dtos = _queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(query);
            AllTransactions = new TrulyObservableCollection<Transaction>(Mapper.Map<List<Transaction>>(dtos));
        }
    }
}