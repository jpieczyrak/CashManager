﻿using CashManager.Data.DTO;

using LiteDB;

namespace CashManager.Infrastructure.DbConnection
{
    public static class LiteDbMappingManager
    {
        public static void SetMappings()
        {
            BsonMapper.Global.Entity<Transaction>()
                      .Id(x => x.Id)
                      .DbRef(x => x.Type)
                      .DbRef(x => x.ExternalStock)
                      .DbRef(x => x.UserStock);

            BsonMapper.Global.Entity<Position>()
                      .Id(x => x.Id)
                      .DbRef(x => x.Category)
                      .DbRef(x => x.Tags);

            BsonMapper.Global.Entity<Category>()
                      .Id(x => x.Id)
                      .DbRef(x => x.Parent);
        }
    }
}