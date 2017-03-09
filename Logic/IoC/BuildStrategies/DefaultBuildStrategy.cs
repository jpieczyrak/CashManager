using Autofac;

using DBInterface;

using LiteDBWrapper;

namespace Logic.IoC.BuildStrategies
{
    public class DefaultBuildStrategy : IBuildStrategy
    {
        #region IBuildStrategy

        public IContainer Build()
        {
            return ConfigureBuilder().Build();
        }

        #endregion

        public ContainerBuilder ConfigureBuilder()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterInstance(new LiteDBFacade("litedb.db", 1000))
                            .As<IDatabase>()
                            .SingleInstance();

            return containerBuilder;
        }
    }
}