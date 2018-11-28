using System.IO;

using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Tests.Utils
{
    internal static class LiteDbHelper
    {
        internal static LiteRepository CreateMemoryDb()
        {
            LiteDbMappingManager.SetMappings();
            return new LiteRepository(new LiteDatabase(new MemoryStream()));
        }
    }
}