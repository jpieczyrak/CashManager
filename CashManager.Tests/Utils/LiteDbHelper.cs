using System.IO;

using LiteDB;

namespace CashManager.Tests.Utils
{
    internal static class LiteDbHelper
    {
        internal static LiteRepository CreateMemoryDb() => new LiteRepository(new LiteDatabase(new MemoryStream()));
    }
}