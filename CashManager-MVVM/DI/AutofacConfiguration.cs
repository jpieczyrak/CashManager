using System;

using Autofac;

using CashManager.Infrastructure.Modules;

using CashManager_MVVM.Features;
using CashManager_MVVM.Features.Main;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.DI
{
    public static class AutofacConfiguration
    {
        public static ContainerBuilder ContainerBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(DatabaseCommunicationModule).Assembly);
            builder.RegisterType<MainWindow>();

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

            return builder;
        }
    }
}