using System.IO;
using System.Linq;

using Autofac;

using AutoMapper;

using CashManager.Infrastructure.DbConnection;
using CashManager.Logic.DefaultData;

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

        protected DtoTransaction[] DtoTransactions { get; set; }
        protected DtoPosition[] DtoPositions => DtoTransactions.SelectMany(x => x.Positions).ToArray();
        protected DtoCategory[] DtoCategories { get; set; }
        protected DtoTag[] DtoTags { get; set; }
        protected DtoTransactionType[] DtoTypes { get; set; }
        protected DtoStock[] DtoStocks { get; set; }

        protected internal Transaction[] Transactions => Mapper.Map<Transaction[]>(DtoTransactions);
        protected internal Position[] Positions => Mapper.Map<Transaction[]>(DtoTransactions).SelectMany(x => x.Positions).ToArray();
        protected Category[] Categories => Mapper.Map<Category[]>(DtoCategories);
        protected internal Tag[] Tags => Mapper.Map<Tag[]>(DtoTags);
        protected internal TransactionType[] Types => Mapper.Map<TransactionType[]>(DtoTypes);
        protected internal Stock[] Stocks => Mapper.Map<Stock[]>(DtoStocks);

        public ViewModelTests()
        {
            Container = GetContainer();

            var defaultDataProvider = new TestDataProvider();
            DtoTags = defaultDataProvider.GetTags();
            DtoStocks = defaultDataProvider.GetStocks();
            DtoCategories = defaultDataProvider.GetCategories();
            DtoTypes = defaultDataProvider.GetTransactionTypes();
            DtoTransactions = defaultDataProvider.GetTransactions(DtoStocks, DtoCategories, DtoTypes, DtoTags);
        }

        public void SetupDatabase()
        {
            var repo = Container.Resolve<LiteRepository>();
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

            //override - we dont want to have singletons in tests
            builder.RegisterAssemblyTypes(typeof(ApplicationViewModel).Assembly)
                   .Where(t => t.IsSubclassOf(typeof(ViewModelBase)))
                   .Named<ViewModelBase>(x => x.Name)
                   .As(t => t);

            return builder.Build();
        }
    }
}