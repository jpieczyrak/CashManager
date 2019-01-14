using Xunit;

namespace CashManager.Tests.ViewModels.Fixtures
{
    [CollectionDefinition("Cleanable database collection")]
    public class EmptyDatabaseCollection : ICollectionFixture<CleanableDatabaseFixture> { }
}