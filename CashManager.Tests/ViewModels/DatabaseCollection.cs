using Xunit;

namespace CashManager.Tests.ViewModels
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }

    [CollectionDefinition("Empty database collection")]
    public class EmptyDatabaseCollection : ICollectionFixture<EmptyDatabaseFixture> { }
}