using System;

using Autofac;

using LiteDB;

namespace CashManager.Tests.ViewModels
{
    public class DatabaseFixture : IDisposable
    {
        public ViewModelTests ViewModelTests { get; private set; }

        public IContainer Container => ViewModelTests.Container;

        public DatabaseFixture()
        {
            ViewModelTests = new ViewModelTests();
            ViewModelTests.SetupDatabase();
        }

        #region IDisposable

        public void Dispose() { }

        #endregion
    }
    public class CleanableDatabaseFixture : IDisposable
    {
        public ViewModelTests ViewModelTests { get; private set; }

        public IContainer Container => ViewModelTests.Container;

        public CleanableDatabaseFixture()
        {
            ViewModelTests = new ViewModelTests();
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