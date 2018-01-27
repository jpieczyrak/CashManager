using System.Windows;

using Autofac;

using CashManager.Infrastructure.Modules;

using GalaSoft.MvvmLight.Threading;

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

            }
        }
    }
}
