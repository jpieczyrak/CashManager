using System;
using System.IO;
using System.Linq;

using Autofac;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.DbConnection;
using CashManager.Infrastructure.Query;
using CashManager.Logic.DefaultData;

using CashManager.WPF.CommonData;
using CashManager.WPF.Configuration.DI;
using CashManager.WPF.Features;
using CashManager.WPF.Features.Main;
using CashManager.WPF.Features.Search;
using CashManager.WPF.Model;

using GalaSoft.MvvmLight;

using LiteDB;

using DtoCategory = CashManager.Data.DTO.Category;
using DtoPosition = CashManager.Data.DTO.Position;
using DtoTransaction = CashManager.Data.DTO.Transaction;
using DtoTransactionType = CashManager.Data.DTO.TransactionType;
using DtoStock = CashManager.Data.DTO.Stock;
using DtoTag = CashManager.Data.DTO.Tag;
using MapperConfiguration = CashManager.WPF.Configuration.Mapping.MapperConfiguration;

namespace CashManager.Tests.ViewModels
{
    public sealed class ViewModelContext
    {
        public enum SetupDb
        {
            Categories,
            Tags,
            Types,
            Stocks,
            Positions,
            Transactions
        }

        public readonly IContainer Container;

        internal Lazy<Transaction[]> Transactions => new Lazy<Transaction[]>(() => Mapper.Map<Transaction[]>(DtoTransactions.Value));

        internal Lazy<Position[]> Positions => new Lazy<Position[]>(() => Mapper.Map<Transaction[]>(DtoTransactions.Value).SelectMany(x => x.Positions).ToArray());

        internal Lazy<Tag[]> Tags => new Lazy<Tag[]>(() => Mapper.Map<Tag[]>(DtoTags.Value));

        private Lazy<DtoTransaction[]> DtoTransactions { get; set; }

        private Lazy<DtoPosition[]> DtoPositions => new Lazy<DtoPosition[]>(() => DtoTransactions.Value.SelectMany(x => x.Positions).ToArray());

        private Lazy<DtoCategory[]> DtoCategories { get; set; }

        private Lazy<DtoTag[]> DtoTags { get; set; }

        private Lazy<DtoTransactionType[]> DtoTypes { get; set; }

        private Lazy<DtoStock[]> DtoStocks { get; set; }

        private Lazy<Category[]> Categories => new Lazy<Category[]>(() => Mapper.Map<Category[]>(DtoCategories.Value));

        private Lazy<TransactionType[]> Types => new Lazy<TransactionType[]>(() => Mapper.Map<TransactionType[]>(DtoTypes.Value));

        private Lazy<Stock[]> Stocks => new Lazy<Stock[]>(() => Mapper.Map<Stock[]>(DtoStocks.Value));

        public ViewModelContext()
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

        internal static IContainer GetContainer()
        {
            MapperConfiguration.Configure();
            var builder = AutofacConfiguration.ContainerBuilder();

            //override db register
            builder.Register(x => new LiteRepository(new LiteDatabase(new MemoryStream()))).SingleInstance().ExternallyOwned();

            builder.RegisterType<ApplicationViewModel>()
                   .Named<ViewModelBase>(nameof(ApplicationViewModel))
                   .As<ViewModelBase>()
                   .As<ApplicationViewModel>()
                   .SingleInstance();

            //search should be perform instantly in tests
            builder.Register(x =>
                   {
                       var vm = new SearchViewModel(
                           x.Resolve<IQueryDispatcher>(),
                           x.Resolve<ICommandDispatcher>(),
                           x.Resolve<ViewModelFactory>(),
                           x.Resolve<TransactionsProvider>())
                       {
                           IsDebounceable = false
                       };
                       return vm;
                   })
                   .As<SearchViewModel>()
                   .Named<ViewModelBase>(nameof(SearchViewModel));

            return builder.Build();
        }
    }
}