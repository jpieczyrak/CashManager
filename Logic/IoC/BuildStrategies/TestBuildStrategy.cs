using Autofac;

using DBInterface;

using LiteDBWrapper;

namespace Logic.IoC.BuildStrategies
{
    public class TestBuildStrategy : IBuildStrategy
    {
        private readonly DefaultBuildStrategy _defaultStrategy = new DefaultBuildStrategy();
        
        public IContainer Build()
        {
            //lets get default settings
            var containerBuilder = _defaultStrategy.ConfigureBuilder();

            //and override what we need
            containerBuilder.RegisterInstance(new LiteDBFacade(100))
                            .As<IDatabase>()
                            .SingleInstance();

            return containerBuilder.Build();
        }
    }
}