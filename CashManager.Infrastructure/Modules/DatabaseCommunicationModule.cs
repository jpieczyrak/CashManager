using System;
using System.IO;
using System.Reflection;

using Autofac;

using CashManager.DatabaseConnection;

using LiteDB;

using Logic.Infrastructure.Command;
using Logic.Infrastructure.Query;

using Module = Autofac.Module;

namespace CashManager.Infrastructure.Modules
{
    public class DatabaseCommunicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            string dbPath = "results.litedb"; //todo: from settings
            EnsureDirectoryExists(dbPath);
            builder.Register(x => new LiteRepository($"Filename={dbPath};Journal=true"));

            RegisterCommandHandlers(builder);
            RegisterQueryHandlers(builder);
        }

        private static void RegisterCommandHandlers(ContainerBuilder builder)
        {
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
                    return (ICommandHandler) ctx.Resolve(handlerType);
                };
            });
        }

        private static void RegisterQueryHandlers(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(IQueryHandler<,>)))
                   .Where(x => x.IsAssignableTo<IQueryHandler>())
                   .AsImplementedInterfaces();

            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>();

            builder.Register<Func<Type, IQueryHandler>>(c =>
            {
                var ctx = c.Resolve<IComponentContext>();

                return t =>
                {
                    var type = typeof(IQueryHandler<,>);
                    var returnType = ((Type[])((TypeInfo)t).ImplementedInterfaces)[0].GenericTypeArguments[0];
                    var handlerType = type.MakeGenericType(t, returnType);
                    return (IQueryHandler) ctx.Resolve(handlerType);
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