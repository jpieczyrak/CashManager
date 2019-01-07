using Xunit;

namespace CashManager.Tests.ViewModels
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }

    [CollectionDefinition("Cleanable database collection")]
    public class EmptyDatabaseCollection : ICollectionFixture<CleanableDatabaseFixture> { }
}