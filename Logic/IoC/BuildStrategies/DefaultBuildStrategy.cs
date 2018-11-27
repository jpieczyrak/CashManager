using Autofac;

using LiteDB;

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
            containerBuilder.RegisterInstance(new LiteDatabase("Filename=litedb.db;Journal=true"))
                            .As<LiteDatabase>()
                            .SingleInstance();

            return containerBuilder;
        }
    }
}