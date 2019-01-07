using System;
using System.IO;
using System.Linq;

using Autofac;

using AutoMapper;

using CashManager.Infrastructure.DbConnection;
using CashManager.Logic.DefaultData;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Configuration.DI;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;

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
        public readonly IContainer Container;

        protected Lazy<DtoTransaction[]> DtoTransactions { get; set; }
        protected Lazy<DtoPosition[]> DtoPositions => new Lazy<DtoPosition[]>(() => DtoTransactions.Value.SelectMany(x => x.Positions).ToArray());
        protected Lazy<DtoCategory[]> DtoCategories { get; set; }
        protected Lazy<DtoTag[]> DtoTags { get; set; }
        protected Lazy<DtoTransactionType[]> DtoTypes { get; set; }
        protected Lazy<DtoStock[]> DtoStocks { get; set; }

        protected internal Lazy<Transaction[]> Transactions => new Lazy<Transaction[]>(() => Mapper.Map<Transaction[]>(DtoTransactions.Value));
        protected internal Lazy<Position[]> Positions => new Lazy<Position[]>(() => Mapper.Map<Transaction[]>(DtoTransactions.Value).SelectMany(x => x.Positions).ToArray());
        protected Lazy<Category[]> Categories => new Lazy<Category[]>(() => Mapper.Map<Category[]>(DtoCategories.Value));
        protected internal Lazy<Tag[]> Tags => new Lazy<Tag[]>(() => Mapper.Map<Tag[]>(DtoTags.Value));
        protected internal Lazy<TransactionType[]> Types => new Lazy<TransactionType[]>(() => Mapper.Map<TransactionType[]>(DtoTypes.Value));
        protected internal Lazy<Stock[]> Stocks => new Lazy<Stock[]>(() => Mapper.Map<Stock[]>(DtoStocks.Value));

        public ViewModelTests()
        {
            Container = GetContainer();

            var defaultDataProvider = new TestDataProvider();
            DtoTags = new Lazy<DtoTag[]>(() => defaultDataProvider.GetTags());
            DtoStocks = new Lazy<DtoStock[]>(() => defaultDataProvider.GetStocks());
            DtoCategories = new Lazy<DtoCategory[]>(() => defaultDataProvider.GetCategories());
            DtoTypes = new Lazy<DtoTransactionType[]>(() => defaultDataProvider.GetTransactionTypes());
            DtoTransactions = new Lazy<DtoTransaction[]>(() => defaultDataProvider.GetTransactions(DtoStocks.Value, DtoCategories.Value, DtoTypes.Value, DtoTags.Value));
        }

        public void SetupDatabase()
        {
            var repo = Container.Resolve<LiteRepository>();
            repo.Database.UpsertBulk(DtoCategories.Value);
            repo.Database.UpsertBulk(DtoTags.Value);
            repo.Database.UpsertBulk(DtoTypes.Value);
            repo.Database.UpsertBulk(DtoStocks.Value);
            repo.Database.UpsertBulk(DtoPositions.Value);
            repo.Database.UpsertBulk(DtoTransactions.Value);
        }

        public void SetupDatabase(params SetupDb[] setup)
        {
            var repo = Container.Resolve<LiteRepository>();

            foreach (var type in setup)
            {
                switch (type)
                {
                    case SetupDb.Categories:
                        repo.Database.UpsertBulk(DtoCategories.Value);
                        break;
                    case SetupDb.Tags:
                        repo.Database.UpsertBulk(DtoTags.Value);
                        break;
                    case SetupDb.Types:
                        repo.Database.UpsertBulk(DtoTypes.Value);
                        break;
                    case SetupDb.Stocks:
                        repo.Database.UpsertBulk(DtoStocks.Value);
                        break;
                    case SetupDb.Positions:
                        repo.Database.UpsertBulk(DtoPositions.Value);
                        break;
                    case SetupDb.Transactions:
                        repo.Database.UpsertBulk(DtoTransactions.Value);
                        break;
                }
            }
        }

        protected static IContainer GetContainer()
        {
            MapperConfiguration.Configure();
            var builder = AutofacConfiguration.ContainerBuilder();

            //override db register
            builder.Register(x => new LiteRepository(new LiteDatabase(new MemoryStream()))).SingleInstance().ExternallyOwned();

            //override - we dont want to have singletons in tests
            builder.RegisterAssemblyTypes(typeof(ApplicationViewModel).Assembly)
                   .Where(t => t.IsSubclassOf(typeof(ViewModelBase)))
                   .Named<ViewModelBase>(x => x.Name)
                   .As(t => t);
            builder.RegisterType<TransactionsProvider>().As<TransactionsProvider>();

            return builder.Build();
        }

        public enum SetupDb
        {
            Categories,
            Tags,
            Types,
            Stocks,
            Positions,
            Transactions
        }
    }
}