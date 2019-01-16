using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Transactions;
using CashManager.Infrastructure.Query.TransactionTypes;
using CashManager.Logic.Wrappers;

using CashManager_MVVM.Model;

using DtoTransaction = CashManager.Data.DTO.Transaction;
using DtoType = CashManager.Data.DTO.TransactionType;
using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.CommonData
{
    public class TransactionsProvider
    {
        public TrulyObservableCollection<Transaction> AllTransactions { get; private set; }

        public TransactionsProvider(IQueryDispatcher queryDispatcher)
        {
            //ensure that types and stocks exists in mapper [after transaction query mapping optimization
            //such code is needed to prevent empty fields in gui (before first loading of stocks/tt data)]
            //todo: extract it to separate providers?
            using (new MeasureTimeWrapper(() => Mapper.Map<TransactionType[]>(queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(new TransactionTypesQuery())).ToArray(), "query types")) { }
            using (new MeasureTimeWrapper(() => Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery())), "query stocks")) { }

            var query = new TransactionQuery();
            DtoTransaction[] dtos = null;
            using (new MeasureTimeWrapper(() => dtos = queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(query), "query transactions")) { }

            List<Transaction> transactions = null;
            using (new MeasureTimeWrapper(() => transactions = Mapper.Map<List<Transaction>>(dtos), $"map transactions [{dtos.Length}]")) { }
            using (new MeasureTimeWrapper(() => AllTransactions = new TrulyObservableCollection<Transaction>(transactions), "create tru. ob. coll")) { }
        }
    }
}