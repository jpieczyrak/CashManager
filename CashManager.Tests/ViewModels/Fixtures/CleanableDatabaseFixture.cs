using System;

using Autofac;

using LiteDB;

namespace CashManager.Tests.ViewModels.Fixtures
{
    public class CleanableDatabaseFixture : IDisposable
    {
        private Lazy<IContainer> ContainerWrapper { get; set; }

        public IContainer Container => ContainerWrapper.Value;

        public CleanableDatabaseFixture()
        {
            ContainerWrapper = new Lazy<IContainer>(ViewModelContext.GetContainer);
        }

        #region IDisposable

        public void Dispose() { }

        #endregion

        public void Reset()
        {
            if (ContainerWrapper.IsValueCreated) ContainerWrapper = new Lazy<IContainer>(ViewModelContext.GetContainer);
        }
    }
}