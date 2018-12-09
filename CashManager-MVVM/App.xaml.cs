using System.Windows;

using Autofac;

using CashManager_MVVM.DI;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Temps;

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

			var builder = AutofacConfiguration.ContainerBuilder();
			var container = builder.Build(); //it could be using, but then there is problem with resolving func factory... anyway it will die with app.

#if DEBUG
		    DbFiller.Fill(container); //fill with default data
#endif

            container.Resolve<MainWindow>().Show();
		}
	}
}
