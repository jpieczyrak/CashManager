using Autofac;

using DBInterface;

using Logic.IoC;

namespace Logic.Database
{
    public class DatabaseProvider
    {
        private static IDatabase _db;

        public static IDatabase DB => _db ?? (_db = AutofacConfiguration.Container.Resolve<IDatabase>());
    }
}