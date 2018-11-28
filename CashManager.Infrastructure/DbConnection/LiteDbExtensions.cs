using System;
using System.Linq;
using System.Linq.Expressions;

using CashManager.Data.DTO;

using LiteDB;

namespace CashManager.Infrastructure.DbConnection
{
    public static class LiteDbExtensions
    {
        private const string ID_FIELD_NAME = "_id";

        /// <returns>True if added, false if updated or fail</returns>
        public static bool Upsert<T>(this LiteDatabase db, T element) where T : class
        {
            var collection = db.GetCollection<T>();

            if (element == null) return false;
            if (element is Dto) return collection.Upsert(element);

            return collection.Upsert(element.GetHashCode(), element);
        }

        public static int UpsertBulk<T>(this LiteDatabase db, T[] elements) where T : class
        {
            int count = 0;
            var collection = db.GetCollection<T>();

            if (elements == null) return 0;

            var matching = elements.OfType<Dto>().Select(x => x as T).ToArray();
            if (matching.Any()) count += collection.Upsert(matching);

            var notMatching = elements.Except(matching).ToArray();
            count += notMatching.Where(x => x != null).Sum(x => collection.Upsert(x.GetHashCode(), x) ? 1 : 0);

            return count;
        }

        public static int Remove<T>(this LiteDatabase db, T element) where T : class
        {
            var collection = db.GetCollection<T>();

            if (element == null) return 0;
            var x = element as Dto;
            return collection.Delete(x != null
                                  ? LiteDB.Query.EQ(ID_FIELD_NAME, new BsonValue(x.Id))
                                  : LiteDB.Query.EQ(ID_FIELD_NAME, element.GetHashCode()));
        }

        public static T[] Query<T>(this LiteDatabase db, Expression<Func<T, bool>> query = null) where T : class
        {
            var collection = db.GetCollection<T>();
            return query != null 
                       ? collection.Find(query).ToArray() 
                       : collection.FindAll().ToArray();
        }

        public static int RemoveAll<T>(this LiteDatabase db, Expression<Func<T, bool>> query = null) where T : class
        {
            return query == null
                       ? db.GetCollection<T>().Delete(LiteDB.Query.All())
                       : db.GetCollection<T>().Delete(query);
        }
    }
}