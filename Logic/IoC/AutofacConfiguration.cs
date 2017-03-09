using Autofac;

using Logic.IoC.BuildStrategies;

namespace Logic.IoC
{
    public static class AutofacConfiguration
    {
        private static IContainer _container;
        private static IBuildStrategy _strategy = new DefaultBuildStrategy();
        private static eBuildStrategyType _strategyType = eBuildStrategyType.Normal;

        public static IContainer Container => _container ?? (_container = CreateContainer());

        public static void UseStrategy(eBuildStrategyType strategyType)
        {
            if (_strategyType == strategyType)
            {
                return;
            }

            _strategyType = strategyType;

            switch (strategyType)
            {
                case eBuildStrategyType.Normal:
                    _strategy = new DefaultBuildStrategy();
                    break;
                case eBuildStrategyType.Test:
                    _strategy = new TestBuildStrategy();
                    break;
            }

            CreateContainer();
        }

        private static IContainer CreateContainer()
        {
            return _strategy.Build();
        }
    }
}