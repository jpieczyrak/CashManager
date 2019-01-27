using System;

using Autofac;

using CashManager.CommonData;
using CashManager.Features;
using CashManager.Features.Common;
using CashManager.Features.Main;
using CashManager.Infrastructure.Modules;
using CashManager.Logic.Calculators;
using CashManager.Logic.Creators;
using CashManager.UserCommunication;

using GalaSoft.MvvmLight;

namespace CashManager.Configuration.DI
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
            builder.RegisterType<CorrectionsCreator>().As<ICorrectionsCreator>().SingleInstance().ExternallyOwned();
            builder.RegisterType<TransactionsProvider>().As<TransactionsProvider>().SingleInstance().ExternallyOwned();

            builder.RegisterType<MessageBoxService>().As<IMessagesService>();

            builder.RegisterType<MultiComboBoxViewModel>()
                   .Named<ViewModelBase>(nameof(MultiComboBoxViewModel))
                   .As<MultiComboBoxViewModel>();

            builder.Register<Func<Type, ViewModelBase>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return type => context.ResolveNamed<ViewModelBase>(type.Name);
            });

            return builder;
        }
    }
}