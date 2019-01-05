using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

using Autofac;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Modules;

using CashManager_MVVM.Configuration.DI;
using CashManager_MVVM.Configuration.Mapping;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Features.Main.Init;
using CashManager_MVVM.Properties;
using CashManager_MVVM.Skins;

using GalaSoft.MvvmLight.Threading;

namespace CashManager_MVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string DB_PATH = "results.litedb";

        internal static SkinColors SkinColors { get; private set; }

        internal static SkinShapes SkinShape { get; private set; }

        private string DatabaseFilepath => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DB_PATH);

        static App()
        {
            DispatcherHelper.Initialize();
            MapperConfiguration.Configure();
            SkinColors = SkinColors.Dark;
            SkinShape = SkinShapes.Round;
        }

        private static Task ShowWindow<T>(T window) where T : Window
        {
            var task = new TaskCompletionSource<object>();
            window.Closed += (sender, args) => task.SetResult(null);
            window.Show();
            window.Focus();
            return task.Task;
        }

        #region Override

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = AutofacConfiguration.ContainerBuilder();

            InitWindow init = null;
            if (File.Exists(DatabaseFilepath))
            {
                string connectionString = $"Filename={DatabaseFilepath};Journal=true";
                if (Settings.Default.IsPasswordNeeded)
                {
                    var passwordWindow = new PasswordPromptWindow();
                    await ShowWindow(passwordWindow);
                    string password = passwordWindow.PasswordText;
                    connectionString += $";password={password}";
                }
                builder.Register(x => connectionString).Keyed<string>(DatabaseCommunicationModule.DB_KEY);
            }
            else
            {
                init = new InitWindow(builder, DatabaseFilepath);
                await ShowWindow(init);
            }

            try
            {
                //it could be using, but then there is problem with resolving func factory... anyway it will die with app.
                var container = builder.Build();

                if (init?.DataContext is InitViewModel vm) vm.GenerateData(container.Resolve<ICommandDispatcher>());

                container.Resolve<MainWindow>().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}