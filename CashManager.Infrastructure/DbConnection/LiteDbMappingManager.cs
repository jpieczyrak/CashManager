using CashManager.Data.DTO;

using LiteDB;

namespace CashManager.Infrastructure.DbConnection
{
    public static class LiteDbMappingManager
    {
        public static void SetMappings()
        {
            BsonMapper.Global.Entity<Transaction>()
                      .Id(x => x.Id)
                      .DbRef(x => x.Positions)
                      .DbRef(x => x.ExternalStock)
                      .DbRef(x => x.UserStock);

            BsonMapper.Global.Entity<Position>()
                      .Id(x => x.Id)
                      .DbRef(x => x.Category)
                      .DbRef(x => x.Tags)
                      .DbRef(x => x.Value);

            BsonMapper.Global.Entity<Category>()
                      .Id(x => x.Id)
                      .DbRef(x => x.Parent);
        }
    }
}