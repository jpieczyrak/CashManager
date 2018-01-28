using System.Collections.Generic;
using System.Windows;

using Autofac;

using CashManager.Infrastructure.Modules;

using GalaSoft.MvvmLight.Threading;

using Logic.Infrastructure.Command;
using Logic.Infrastructure.Query;

namespace CashManager_MVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
            Mapping.MapperConfiguration.Configure();

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(DatabaseCommunicationModule).Assembly);
            using (IContainer container = builder.Build())
            {
                var commandDispatcher = container.Resolve<ICommandDispatcher>();
                commandDispatcher.Execute(new NoCommand());

                var queryDispatcher = container.Resolve<IQueryDispatcher>();
                var test = queryDispatcher.Execute<NoQuery, IEnumerable<string>>(new NoQuery());
            }
        }
    }
}
