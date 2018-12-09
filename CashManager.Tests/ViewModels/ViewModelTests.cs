using System;
using System.IO;
using System.Linq;

using Autofac;

using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;
using CashManager.Infrastructure.Modules;
using CashManager.Logic.DefaultData;

using CashManager_MVVM.Features;
using CashManager_MVVM.Features.Main;

using GalaSoft.MvvmLight;

using LiteDB;

using MapperConfiguration = CashManager_MVVM.Mapping.MapperConfiguration;

namespace CashManager.Tests.ViewModels
{
    public class ViewModelTests
    {
        protected readonly IContainer _container;

        protected Transaction[] DtoTransactions { get; set; }
        protected Position[] DtoPositions => DtoTransactions.SelectMany(x => x.Positions).ToArray();
        protected Category[] DtoCategories { get; set; }
        protected Tag[] DtoTags { get; set; }
        protected TransactionType[] DtoTypes { get; set; }
        protected Stock[] DtoStocks { get; set; }

        public ViewModelTests()
        {
            _container = GetContainer();

            DtoTags = DefaultDataProvider.GetTags();
            DtoStocks = DefaultDataProvider.GetStocks();
            DtoCategories = DefaultDataProvider.GetCategories();
            DtoTypes = DefaultDataProvider.GetTransactionTypes();
            DtoTransactions = DefaultDataProvider.GetTransactions(DtoStocks, DtoCategories, DtoTypes, DtoTags);
        }

        protected void SetupDatabase()
        {
            var repo = _container.Resolve<LiteRepository>();
            repo.Database.UpsertBulk(DtoCategories);
            repo.Database.UpsertBulk(DtoTags);
            repo.Database.UpsertBulk(DtoTypes);
            repo.Database.UpsertBulk(DtoStocks);
            repo.Database.UpsertBulk(DtoTransactions);
        }

        protected static IContainer GetContainer()
        {
            MapperConfiguration.Configure();
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(DatabaseCommunicationModule).Assembly);

            builder.RegisterAssemblyTypes(typeof(ApplicationViewModel).Assembly)
                   .Where(t => t.IsSubclassOf(typeof(ViewModelBase)) && !string.Equals(t.Name, nameof(ApplicationViewModel)))
                   .Named<ViewModelBase>(x => x.Name)
                   .As(t => t);
            builder.RegisterType<ApplicationViewModel>()
                   .As<ApplicationViewModel>()
                   .Named<ViewModelBase>(nameof(ApplicationViewModel))
                   .SingleInstance()
                   .ExternallyOwned();

            builder.RegisterType<ViewModelFactory>().As<ViewModelFactory>();

            builder.Register<Func<Type, ViewModelBase>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return type => context.ResolveNamed<ViewModelBase>(type.Name);
            });
            builder.Register(x => new LiteRepository(new LiteDatabase(new MemoryStream()))).SingleInstance().ExternallyOwned();

            return builder.Build();
        }
    }
}