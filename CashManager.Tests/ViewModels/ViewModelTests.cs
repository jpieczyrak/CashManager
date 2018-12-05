using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Autofac;

using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;
using CashManager.Infrastructure.Modules;

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

        protected Transaction[] Transactions { get; set; }
        protected Position[] Positions => Transactions.SelectMany(x => x.Positions).ToArray();
        protected Category[] Categories { get; set; }
        protected Tag[] Tags { get; set; }
        protected TransactionType[] Types { get; set; }
        protected Stock[] Stocks { get; set; }

        public ViewModelTests()
        {
            _container = GetContainer();
            Types = new TransactionType[]
            {
                new TransactionType() { Income = true, Name = "Work" },
                new TransactionType() { Outcome = true, Name = "Buy" },
                new TransactionType() { Name = "Transfer" },
            };
            Stocks = new Stock[]
            {
                new Stock() { Name = "1st", IsUserStock = true }, 
                new Stock() { Name = "2nd", IsUserStock = true }, 
                new Stock() { Name = "3rd", IsUserStock = false }, 
            };
            Tags = new[]
            {
                new Tag() { Name = "tag1" },
                new Tag() { Name = "tag2" },
            };
            var root = new Category { Name = "Root" };
            var home = new Category { Name = "Home", Parent = root };
            var fun = new Category { Name = "Fun", Parent = root };
            var fun_PC = new Category { Name = "PC", Parent = fun };
            var fun_books = new Category { Name = "Books", Parent = fun };
            var fun_games = new Category { Name = "Games", Parent = fun };
            var fun_games_strategy = new Category { Name = "Strategy", Parent = fun_games };
            var fun_games_fps = new Category { Name = "FPS", Parent = fun_games };
            var home_cleaning = new Category { Name = "Cleaning", Parent = home };
            var home_food = new Category { Name = "Food", Parent = home };
            var home_food_base = new Category { Name = "Base food", Parent = home_food };
            var home_food_chocolates = new Category { Name = "Chocolates", Parent = home_food };
            var home_food_tea = new Category { Name = "Tea", Parent = home_food };
            Categories = new[]
            {
                root,
                home,
                fun,
                fun_PC,
                fun_books,
                fun_games,
                fun_games_strategy,
                fun_games_fps,
                home_cleaning,
                home_food,
                home_food_base,
                home_food_chocolates,
                home_food_tea
            };

            Transactions = new[]
            {
                new Transaction(Types[0], DateTime.Today, "Title 1 - work", "Just my note", new[]
                {
                    new Position("Income", 2145.45) { Category = root },
                }, Stocks[0], Stocks[2], "income1"),
                new Transaction(Types[1], DateTime.Today, "Title 1 - buying apples", "apples", new[]
                {
                    new Position("Outcome", 12.34) { Category = home_food, Tags = new List<Tag>() { Tags[0] } },
                    new Position("Outcome", 1.34) { Category = home, Tags = new List<Tag>() { Tags[0], Tags[1] } },
                }, Stocks[1], Stocks[2], "outcome1"),
            };
        }

        protected void SetupDatabase()
        {
            var repo = _container.Resolve<LiteRepository>();
            repo.Database.UpsertBulk(Categories);
            repo.Database.UpsertBulk(Tags);
            repo.Database.UpsertBulk(Types);
            repo.Database.UpsertBulk(Stocks);
            repo.Database.UpsertBulk(Transactions);
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