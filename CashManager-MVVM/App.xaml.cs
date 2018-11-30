using System;
using System.Windows;

using Autofac;

using CashManager.Infrastructure.Modules;

using CashManager_MVVM.Features;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Temps;

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
		    DbFiller.Fill(container);
			container.Resolve<MainWindow>().Show();
		}

	    private static ContainerBuilder ContainerBuilder()
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
