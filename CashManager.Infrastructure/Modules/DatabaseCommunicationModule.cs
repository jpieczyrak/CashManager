﻿using System;
using System.IO;
using System.Reflection;

using Autofac;

using CashManager.DatabaseConnection;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

using CommandDispatcher = CashManager.Infrastructure.Command.CommandDispatcher;
using ICommandDispatcher = CashManager.Infrastructure.Command.ICommandDispatcher;
using ICommandHandler = CashManager.Infrastructure.Command.ICommandHandler;
using IQueryDispatcher = CashManager.Infrastructure.Query.IQueryDispatcher;
using IQueryHandler = CashManager.Infrastructure.Query.IQueryHandler;
using Module = Autofac.Module;
using QueryDispatcher = CashManager.Infrastructure.Query.QueryDispatcher;

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
            LiteDbMappingManager.SetMappings();

            RegisterCommandHandlers(builder);
            RegisterQueryHandlers(builder);
        }

        private static void RegisterCommandHandlers(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Command.ICommandHandler<>)))
                   .Where(x => x.IsAssignableTo<ICommandHandler>())
                   .AsImplementedInterfaces();

            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>();

            builder.Register<Func<Type, ICommandHandler>>(c =>
            {
                var context = c.Resolve<IComponentContext>();

                return type =>
                {
                    var handlerType = typeof(Command.ICommandHandler<>).MakeGenericType(type);
                    return (ICommandHandler) context.Resolve(handlerType);
                };
            });
        }

        private static void RegisterQueryHandlers(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Query.IQueryHandler<,>)))
                   .Where(x => x.IsAssignableTo<IQueryHandler>())
                   .AsImplementedInterfaces();

            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>();

            builder.Register<Func<Type, IQueryHandler>>(c =>
            {
                var context = c.Resolve<IComponentContext>();

                return type =>
                {
                    var returnType = ((Type[])((TypeInfo)type).ImplementedInterfaces)[0].GenericTypeArguments[0];
                    var handlerType = typeof(Query.IQueryHandler<,>).MakeGenericType(type, returnType);
                    return (IQueryHandler) context.Resolve(handlerType);
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