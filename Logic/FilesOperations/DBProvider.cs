using DBInterface;

using LiteDBWrapper;

namespace Logic.FilesOperations
{
    public class DBProvider
    {
        private static IDatabase _db;
        private const string DB_PATH = "litedb.db";

        public static IDatabase DB => _db ?? (_db = new LiteDBFacade(DB_PATH));
    }
}