using System;
using System.Linq;
using System.Linq.Expressions;

using LiteDB;

namespace LogicOld.Database
{
	public static class LiteDbExtensions
	{
		private const string ID_FIELD_NAME = "_id";

		public static bool Upsert<T>(this LiteDatabase db, T element) where T : class
		{
			var collection = db.GetCollection<T>();

			if (element is IId) return collection.Upsert(element);

			return collection.Upsert(element.GetHashCode(), element);
		}

		public static void Remove<T>(this LiteDatabase db, T element) where T : class
		{
			var collection = db.GetCollection<T>();

			var x = element as IId;
			collection.Delete(x != null
								  ? LiteDB.Query.EQ(ID_FIELD_NAME, new BsonValue(x.Id))
								  : LiteDB.Query.EQ(ID_FIELD_NAME, element.GetHashCode()));
		}

		public static T[] Read<T>(this LiteDatabase db) where T : class
		{
			var collection = db.GetCollection<T>();
			return collection.FindAll().ToArray();
		}

		public static T[] Query<T>(this LiteDatabase db, Expression<Func<T, bool>> query) where T : class
		{
			return db.GetCollection<T>().Find(query).ToArray();
		}

		public static int RemoveAll<T>(this LiteDatabase db, Expression<Func<T, bool>> query = null) where T : class
		{
		    return query == null 
		               ? db.GetCollection<T>().Delete(LiteDB.Query.All()) 
		               : db.GetCollection<T>().Delete(query);
		}
	}
}
