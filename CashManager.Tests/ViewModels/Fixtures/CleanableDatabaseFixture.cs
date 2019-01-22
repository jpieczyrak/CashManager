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

        public void CleanDatabase()
        {
            var repository = Container.Resolve<LiteRepository>();
            foreach (var name in repository.Database.GetCollectionNames())
            {
                repository.Database.GetCollection(name).Delete(Query.All());
            }
        }

        public void Reset()
        {
            if (ContainerWrapper.IsValueCreated) ContainerWrapper = new Lazy<IContainer>(ViewModelContext.GetContainer);
        }
    }
}