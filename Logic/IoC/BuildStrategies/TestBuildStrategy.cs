using System;
using System.IO;

using Autofac;

using LiteDB;

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
            containerBuilder.RegisterInstance(new LiteDatabase(new StreamDiskService(new MemoryStream()), null, null, TimeSpan.FromMilliseconds(100)))
                            .As<LiteDatabase>()
                            .SingleInstance();

            return containerBuilder.Build();
        }
    }
}