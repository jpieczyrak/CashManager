using System;
using System.Collections.Generic;
using System.Windows;

using Autofac;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Modules;
using CashManager.Infrastructure.Query;

using CashManager_MVVM.Design;
using CashManager_MVVM.Features;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Model.DataProviders;

using GalaSoft.MvvmLight;
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
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var builder = ContainerBuilder();

			var container = builder.Build(); //it could be using, but then there is problem with resolving func factory... anyway it will die with app.
			TestCommands(container); //todo: remove after test / commands real implementation
			container.Resolve<MainWindow>().Show();
		}

		private static ContainerBuilder ContainerBuilder()
		{
			var builder = new ContainerBuilder();
			builder.RegisterAssemblyModules(typeof(DatabaseCommunicationModule).Assembly);
			builder.RegisterType<MainWindow>();

			if (ViewModelBase.IsInDesignModeStatic) builder.RegisterType<DesignDataService>().As<IDataService>();
			else builder.RegisterType<DataService>().As<IDataService>();

			builder.RegisterAssemblyTypes(typeof(MainViewModel).Assembly)
				   .Where(t => t.IsSubclassOf(typeof(ViewModelBase)))
				   .Named<ViewModelBase>(x => x.Name)
				   .As(t => t);

			builder.RegisterType<ViewModelFactory>().As<ViewModelFactory>();

			builder.Register<Func<Type, ViewModelBase>>(c =>
			{
				var context = c.Resolve<IComponentContext>();
                return type => context.ResolveNamed<ViewModelBase>(type.Name);
			});

            return builder;
		}

		private static void TestCommands(IContainer container)
		{
			var commandDispatcher = container.Resolve<ICommandDispatcher>();
			commandDispatcher.Execute(new NoCommand());

			var queryDispatcher = container.Resolve<IQueryDispatcher>();
			var test = queryDispatcher.Execute<NoQuery, IEnumerable<string>>(new NoQuery());
		}
	}
}
