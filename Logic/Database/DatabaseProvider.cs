using Autofac;

using LiteDB;

using LogicOld.IoC;

namespace LogicOld.Database
{
    public class DatabaseProvider
    {
        private static LiteDatabase _db;

        public static LiteDatabase DB => _db ?? (_db = AutofacConfiguration.Container.Resolve<LiteDatabase>());
    }
}