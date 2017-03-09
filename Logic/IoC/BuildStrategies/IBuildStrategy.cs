using Autofac;

namespace Logic.IoC.BuildStrategies
{
    public interface IBuildStrategy
    {
        IContainer Build();
    }
}