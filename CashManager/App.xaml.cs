using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Autofac;

using CashManager.Configuration.DI;
using CashManager.Configuration.Mapping;
using CashManager.Extensions;
using CashManager.Features.Main;
using CashManager.Features.Main.Init;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Modules;
using CashManager.Logic.Extensions;
using CashManager.Logic.Wrappers;
using CashManager.Properties;
using CashManager.Utils;
using CashManager.Utils.Updates;

using GalaSoft.MvvmLight.Threading;

using log4net;

namespace CashManager
{
    public partial class App : Application
    {
        private const string DB_PATH = "results.litedb";
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(App)));
        private IUpdatesManager _updater;

        private string DatabaseFilepath
        {
            get
            {
#if PORTABLE
                return DB_PATH;
#else
                return Path.Combine(SolutionSettingsDir, DB_PATH);
#endif
            }
        }

        private string SolutionSettingsDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), typeof(App).Namespace ?? string.Empty);

        static App()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Settings.Default.Localization);
            DispatcherHelper.Initialize();
            MapperConfiguration.Configure();
        }

        private async Task PerformStart()
        {
            SettingsManager.HandleSettingsUpgrade();
            while (true)
            {
                ContainerBuilder builder = null;
                using (new MeasureTimeWrapper(() => builder = AutofacConfiguration.ContainerBuilder(), "ContainerBuilder")) { }

                var init = await HandleApplicationInit(builder);

                try
                {
                    //it could be using, but then there is problem with resolving func factory... anyway it will die with app.
                    IContainer container = null;
                    using (new MeasureTimeWrapper(() => container = builder.Build(), "Container.Build")) { }

                    if (HandleInitDataGeneration(init, container)) return;

                    using (new MeasureTimeWrapper(() => container.Resolve<MainWindow>().Show(), "Resolve<MainWindow>.Show")) { }

                    await _updater.HandleApplicationUpdatesCheck();
                    break;
                }
                catch (Exception exception)
                {
                    //todo: catch only litedb exceptions?
                    _logger.Value.Error("Loading app failed", exception);
                }
            }
        }

        private async Task<InitWindow> HandleApplicationInit(ContainerBuilder builder)
        {
            InitWindow init = null;
            if (File.Exists(DatabaseFilepath))
            {
                string connectionString = $"Filename={DatabaseFilepath};Journal=true";
                if (Settings.Default.IsPasswordNeeded)
                {
                    var passwordWindow = new PasswordPromptWindow();
                    await passwordWindow.ShowBlocking();

                    if (passwordWindow.Success)
                    {
                        string password = string.Empty;
                        using (new MeasureTimeWrapper(
                            () => password = passwordWindow.PasswordText.Encrypt(), "Password encryption")) { }

                        connectionString += $";password={password}";
                    }
                    else
                    {
                        _logger.Value.Debug("Password window closed by user");
                        Current.Shutdown();
                        return null;
                    }
                }

                builder.Register(x => connectionString).Keyed<string>(DatabaseCommunicationModule.DB_KEY);
            }
            else
            {
                init = new InitWindow(builder, DatabaseFilepath);
                await init.ShowBlocking();
            }

            return init;
        }

        private static bool HandleInitDataGeneration(InitWindow init, IContainer container)
        {
            if (init?.DataContext is InitViewModel vm)
                if (vm.CanStartApplication)
                {
                    using (new MeasureTimeWrapper(() => vm.GenerateData(container.Resolve<ICommandDispatcher>()), "GenerateData")) { }
                }
                else
                {
                    Current.Shutdown();
                    return true;
                }

            return false;
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Value.Fatal("An unhandled exception", e.Exception);
#if DEBUG
            e.Handled = false;
#else
            e.Handled = true;
            Current.Shutdown();
#endif
        }

        #region Override

        protected override void OnExit(ExitEventArgs e)
        {
            _updater.Cleanup();
            _logger.Value.Info("Exit");
            base.OnExit(e);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
#if PORTABLE
            _updater = new DefaultUpdatesManager();
#else
            _updater = new SquirrelUpdatesManager();
#endif
            _updater.HandleEvents();

            base.OnStartup(e);
            _logger.Value.Info($"Startup [{Assembly.GetExecutingAssembly().GetName().Version}]");

            await PerformStart();
        }

#endregion
    }
}