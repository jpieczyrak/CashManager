using System;

using Autofac;

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
}