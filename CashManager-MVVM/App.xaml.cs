﻿using System;
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
using CashManager_MVVM.Extensions;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Features.Main.Init;
using CashManager_MVVM.Properties;
using CashManager_MVVM.Utils;

using GalaSoft.MvvmLight.Threading;

using log4net;

using Squirrel;

namespace CashManager_MVVM
{
    public partial class App : Application
    {
        private const string DB_PATH = "results.litedb";
        private const string UPDATES_URL = "http://cmh.eu5.org/";
        private const string ICON_NAME = "app.ico";
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(App)));

        private string DatabaseFilepath
        {
            get
            {
#if DEBUG
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
            SettingsManager.HandleSettingsUpgrade(_logger);
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

                    await HandleApplicationUpdates();
                    break;
                }
                catch (Exception exception)
                {
                    //todo: catch only litedb exceptions?
                    _logger.Value.Error("Loading app failed", exception);
                }
            }
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

        private static void HandleSquirrelEvents()
        {
            try
            {
                using (var mgr = new UpdateManager(UPDATES_URL))
                {
                    void Install(Version v)
                    {
                        string location = Assembly.GetEntryAssembly().Location;
                        string iconPath = Path.Combine(Path.GetDirectoryName(location) ?? string.Empty, @"..\", ICON_NAME);
                        mgr.CreateShortcutsForExecutable(Path.GetFileName(location), ShortcutLocation.StartMenu|ShortcutLocation.Desktop,
                            !Environment.CommandLine.Contains("squirrel-install"), null, iconPath);
                    }

                    SquirrelAwareApp.HandleEvents(Install, Install, onAppUninstall: v => mgr.RemoveShortcutForThisExe());
                }
            }
            catch (Exception) { }
        }

        private async Task HandleApplicationUpdates()
        {
            try
            {
                using (var mgr = new UpdateManager(UPDATES_URL))
                {
                    var result = await mgr.UpdateApp();
                    _logger.Value.Debug(result != null ? $"Updated to: {result.Version}" : "Up-to-date");
                }
            }
            catch (Exception e)
            {
                _logger.Value.Debug("Updated failed", e);
            }
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
#if DEBUG
            string errorMessage = $"An unhandled exception: {e.Exception.Message}";
            MessageBox.Show(errorMessage, "Error [OnDispatcherUnhandledException]", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            _logger.Value.Fatal("An unhandled exception", e.Exception);
            e.Handled = false;
        }

        #region Override

        protected override void OnExit(ExitEventArgs e)
        {
            _logger.Value.Debug("Exit");
            base.OnExit(e);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;

            HandleSquirrelEvents();
            base.OnStartup(e);
            _logger.Value.Debug("Startup");

            await PerformStart();
        }

        #endregion
    }
}