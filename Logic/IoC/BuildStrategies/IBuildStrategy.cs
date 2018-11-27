using Autofac;

namespace LogicOld.IoC.BuildStrategies
{
    public interface IBuildStrategy
    {
        IContainer Build();
    }
}