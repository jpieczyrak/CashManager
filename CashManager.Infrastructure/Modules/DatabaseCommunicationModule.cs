using System;
using System.IO;
using System.Reflection;

using Autofac;

using CashManager.DatabaseConnection;

using LiteDB;

using Logic.Infrastructure.Command;

using Module = Autofac.Module;

namespace CashManager.Infrastructure.Modules
{
    public class DatabaseCommunicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var dbPath = "results.litedb"; //todo: from settings
            EnsureDirectoryExists(dbPath);
            builder.Register(x => new LiteRepository($"Filename={dbPath};Journal=true"));

            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(ICommandHandler<>)))
                   .Where(x => x.IsAssignableTo<ICommandHandler>())
                   .AsImplementedInterfaces();

            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>();

            builder.Register<Func<Type, ICommandHandler>>(c =>
            {
                var ctx = c.Resolve<IComponentContext>();

                return t =>
                {
                    var handlerType = typeof(ICommandHandler<>).MakeGenericType(t);
                    return (ICommandHandler)ctx.Resolve(handlerType);
                };
            });
        }

        private static void RegisterDatabasesBasedOnKey(ContainerBuilder builder, string dbPath)
        {
            builder.Register(x => new LiteRepository(new MemoryStream(File.ReadAllBytes(dbPath))))
                   .Keyed<LiteRepository>(eDatabaseConnection.InMemory);
            builder.Register(x => new LiteRepository($"Filename={dbPath};Journal=true"))
                   .Keyed<LiteRepository>(eDatabaseConnection.Local);
            builder.Register<Func<eDatabaseConnection, LiteRepository>>(ctx =>
            {
                var cc = ctx.Resolve<IComponentContext>();
                return dbType => cc.ResolveKeyed<LiteRepository>(dbType);
            });
        }

        /// <summary>
        /// Checks if filepath directory exists - if not - creates one.
        /// </summary>
        /// <param name="value">Filepath to check if parent dir exists</param>
        private static void EnsureDirectoryExists(string value)
        {
            var dir = Path.GetDirectoryName(value);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}