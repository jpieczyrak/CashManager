using System;

using Autofac;

using CashManager.Infrastructure.Modules;

using CashManager.WPF.CommonData;
using CashManager.WPF.Features;
using CashManager.WPF.Features.Common;
using CashManager.WPF.Features.Main;
using CashManager.WPF.Logic.Calculators;
using CashManager.WPF.Logic.Creators;
using CashManager.WPF.UserCommunication;

using GalaSoft.MvvmLight;

namespace CashManager.WPF.Configuration.DI
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