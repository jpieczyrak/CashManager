using Autofac;

using LiteDB;

using Logic.IoC;

namespace Logic.Database
{
    public class DatabaseProvider
    {
        private static LiteDatabase _db;

        public static LiteDatabase DB => _db ?? (_db = AutofacConfiguration.Container.Resolve<LiteDatabase>());
    }
}