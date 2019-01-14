using System;

using Autofac;

using LiteDB;

namespace CashManager.Tests.ViewModels.Fixtures
{
    public class CleanableDatabaseFixture : IDisposable
    {
        public ViewModelContext ViewModelContext { get; private set; }

        public IContainer Container => ViewModelContext.Container;

        public CleanableDatabaseFixture()
        {
            ViewModelContext = new ViewModelContext();
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
    }
}