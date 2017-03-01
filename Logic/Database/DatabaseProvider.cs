using DBInterface;

using LiteDBWrapper;

namespace Logic.Database
{
    public class DatabaseProvider
    {
        private static LiteDBFacade _db;
        private const string LITEDB_DB = "litedb.db";

        public static IDatabase DB => _db ?? (_db = new LiteDBFacade(LITEDB_DB));
    }
}