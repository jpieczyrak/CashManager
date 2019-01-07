using System;

using Autofac;

namespace CashManager.Tests.ViewModels.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        internal ViewModelContext ViewModelContext { get; private set; }

        public IContainer Container => ViewModelContext.Container;

        public DatabaseFixture()
        {
            ViewModelContext = new ViewModelContext();
            ViewModelContext.SetupDatabase();
        }

        #region IDisposable

        public void Dispose() { }

        #endregion
    }
}