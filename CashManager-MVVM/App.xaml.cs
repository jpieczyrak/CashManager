using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Autofac;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Modules;
using CashManager.Logic.Extensions;
using CashManager.Logic.Wrappers;

using CashManager_MVVM.Configuration.DI;
using CashManager_MVVM.Configuration.Mapping;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Features.Main.Init;
using CashManager_MVVM.Properties;
using CashManager_MVVM.Skins;

using GalaSoft.MvvmLight.Threading;

using log4net;

using IContainer = Autofac.IContainer;

namespace CashManager_MVVM
{
    public partial class App : Application
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(App)));

        private const string DB_PATH = "results.litedb";

        internal static SkinColors SkinColors { get; set; }

        internal static SkinShapes SkinShape { get; set; }

        private string DatabaseFilepath => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty, DB_PATH);

        static App()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Settings.Default.Localization);
            DispatcherHelper.Initialize();
            MapperConfiguration.Configure();
            SkinColors = (SkinColors)Settings.Default.SkinColor;
            SkinShape = (SkinShapes)Settings.Default.SkinShape;
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
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            base.OnStartup(e);
            HandleSettingsUpgrade();
            _logger.Value.Debug("Startup");

            ContainerBuilder builder = null;
            using (new MeasureTimeWrapper(() => builder = AutofacConfiguration.ContainerBuilder(), "ContainerBuilder")) { }

            InitWindow init = null;
            if (File.Exists(DatabaseFilepath))
            {
                string connectionString = $"Filename={DatabaseFilepath};Journal=true";
                if (Settings.Default.IsPasswordNeeded)
                {
                    var passwordWindow = new PasswordPromptWindow();
                    await ShowWindow(passwordWindow);

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
                        return;
                    }
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
                IContainer container = null;
                using (new MeasureTimeWrapper(() => container = builder.Build(), "Container.Build")) { }

                if (init?.DataContext is InitViewModel vm)
                {
                    if (vm.CanStartApplication)
                    {
                        using (new MeasureTimeWrapper(
                            () => vm.GenerateData(container.Resolve<ICommandDispatcher>()), "GenerateData")) { }
                    }
                    else
                    {
                        Current.Shutdown();
                        return;
                    }
                }

                using (new MeasureTimeWrapper(() => container.Resolve<MainWindow>().Show(), "Resolve<MainWindow>().Show()")) { }
            }
            catch (Exception exception)
            {
                //todo: maybe password was needed (outdated settings) -> handle password?

                _logger.Value.Error("Loading app failed", exception);
                Console.WriteLine(exception);
                Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _logger.Value.Debug("Exit");
            base.OnExit(e);
        }

        #endregion

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
#if DEBUG
            string errorMessage = $"An unhandled exception: {e.Exception.Message}";
            MessageBox.Show(errorMessage, "Error [OnDispatcherUnhandledException]", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            _logger.Value.Fatal("An unhandled exception", e.Exception);
            e.Handled = false;
        }

        private static void HandleSettingsUpgrade()
        {
            if (Settings.Default.UpgradeNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.Save();
                Settings.Default.Reload();
                Settings.Default.UpgradeNeeded = false;
                Settings.Default.Save();
                _logger.Value.Debug("Settings upgraded");
            }
        }
    }
}