using System;

using Autofac;

using CashManager.Infrastructure.Modules;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Features;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Logic.Calculators;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Configuration.DI
{
    public static class AutofacConfiguration
    {
        public static ContainerBuilder ContainerBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(DatabaseCommunicationModule).Assembly);
            builder.RegisterType<MainWindow>();

            builder.RegisterAssemblyTypes(typeof(ApplicationViewModel).Assembly)
                   .Where(t => t.IsSubclassOf(typeof(ViewModelBase)))
                   .Named<ViewModelBase>(x => x.Name)
                   .As(t => t)
                   .SingleInstance()
                   .ExternallyOwned();

            builder.RegisterType<ViewModelFactory>().As<ViewModelFactory>();
            builder.RegisterType<TransactionBalanceCalculator>().As<TransactionBalanceCalculator>();
            builder.RegisterType<TransactionsProvider>().As<TransactionsProvider>().SingleInstance().ExternallyOwned();

            builder.Register<Func<Type, ViewModelBase>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return type => context.ResolveNamed<ViewModelBase>(type.Name);
            });

            return builder;
        }
    }
}