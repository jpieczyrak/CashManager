using System.IO;
using System.Linq;

using Autofac;

using AutoMapper;

using CashManager.Infrastructure.DbConnection;
using CashManager.Logic.DefaultData;

using CashManager_MVVM.Configuration.DI;
using CashManager_MVVM.Model;

using LiteDB;

using MapperConfiguration = CashManager_MVVM.Configuration.Mapping.MapperConfiguration;

using DtoCategory = CashManager.Data.DTO.Category;
using DtoPosition = CashManager.Data.DTO.Position;
using DtoTransaction = CashManager.Data.DTO.Transaction;
using DtoTransactionType = CashManager.Data.DTO.TransactionType;
using DtoStock = CashManager.Data.DTO.Stock;
using DtoTag = CashManager.Data.DTO.Tag;

namespace CashManager.Tests.ViewModels
{
    public class ViewModelTests
    {
        protected readonly IContainer _container;

        protected DtoTransaction[] DtoTransactions { get; set; }
        protected DtoPosition[] DtoPositions => DtoTransactions.SelectMany(x => x.Positions).ToArray();
        protected DtoCategory[] DtoCategories { get; set; }
        protected DtoTag[] DtoTags { get; set; }
        protected DtoTransactionType[] DtoTypes { get; set; }
        protected DtoStock[] DtoStocks { get; set; }

        protected Transaction[] Transactions => Mapper.Map<Transaction[]>(DtoTransactions);
        protected Position[] Positions => Mapper.Map<Transaction[]>(DtoTransactions).SelectMany(x => x.Positions).ToArray();
        protected Category[] Categories => Mapper.Map<Category[]>(DtoCategories);
        protected Tag[] Tags => Mapper.Map<Tag[]>(DtoTags);
        protected TransactionType[] Types => Mapper.Map<TransactionType[]>(DtoTypes);
        protected Stock[] Stocks => Mapper.Map<Stock[]>(DtoStocks);

        public ViewModelTests()
        {
            _container = GetContainer();

            var defaultDataProvider = new DefaultDataProvider();
            DtoTags = defaultDataProvider.GetTags();
            DtoStocks = defaultDataProvider.GetStocks();
            DtoCategories = defaultDataProvider.GetCategories();
            DtoTypes = defaultDataProvider.GetTransactionTypes();
            DtoTransactions = defaultDataProvider.GetTransactions(DtoStocks, DtoCategories, DtoTypes, DtoTags);
        }

        protected void SetupDatabase()
        {
            var repo = _container.Resolve<LiteRepository>();
            repo.Database.UpsertBulk(DtoCategories);
            repo.Database.UpsertBulk(DtoTags);
            repo.Database.UpsertBulk(DtoTypes);
            repo.Database.UpsertBulk(DtoStocks);
            repo.Database.UpsertBulk(DtoPositions);
            repo.Database.UpsertBulk(DtoTransactions);
        }

        protected static IContainer GetContainer()
        {
            MapperConfiguration.Configure();
            var builder = AutofacConfiguration.ContainerBuilder();

            //override db register
            builder.Register(x => new LiteRepository(new LiteDatabase(new MemoryStream()))).SingleInstance().ExternallyOwned();

            return builder.Build();
        }
    }
}