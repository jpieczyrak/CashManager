using Xunit;

namespace CashManager.Tests.ViewModels.Fixtures
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
}