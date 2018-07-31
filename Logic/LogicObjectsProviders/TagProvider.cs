using System;
using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using Logic.Database;
using Logic.Model;
using Logic.Utils;

namespace Logic.LogicObjectsProviders
{
    public class TagProvider
    {
        private static TrulyObservableCollection<Tag> _categories;

        public static TrulyObservableCollection<Tag> Categories => _categories ?? (_categories = Load());

        public static TrulyObservableCollection<Tag> Load()
        {
            var dtos = DatabaseProvider.DB.Read<DTO.Tag>();
            var list = dtos.Select(Mapper.Map<DTO.Tag, Tag>);
            _categories = new TrulyObservableCollection<Tag>(list);
            _categories.CollectionChanged += CategoriesOnCollectionChanged;

            return _categories;
        }

        /// <summary>
        ///     Finds Tag - or if not exist - creates the new one and returns result
        /// </summary>
        /// <param name="tagName">Unique Tag name</param>
        /// <returns>Found or created Tag</returns>
        public static Tag FindOrCreate(string tagName)
        {
            if (!string.IsNullOrEmpty(tagName))
            {
                var tag =
                    Categories.FirstOrDefault(c => string.Equals(c.Name, tagName, StringComparison.CurrentCultureIgnoreCase));

                if (tag == null)
                {
                    tag = new Tag(tagName);
                    Store(tag);
                }
                return tag;
            }
            throw new ArgumentNullException("Tag name can not be empty!");
        }

        private static void CategoriesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.NewItems != null)
                foreach (Tag tag in notifyCollectionChangedEventArgs.NewItems)
                    DatabaseProvider.DB.Upsert(Mapper.Map<Tag, DTO.Tag>(tag));
            if (notifyCollectionChangedEventArgs.OldItems != null)
                foreach (Tag tag in notifyCollectionChangedEventArgs.OldItems)
                    DatabaseProvider.DB.Remove(Mapper.Map<Tag, DTO.Tag>(tag));
        }

        /// <summary>
        ///     Stores (for provider purpose) - loaded Tag
        /// </summary>
        /// <param name="tag">Existsing Tag from main source (e.g. transactions)</param>
        private static void Store(Tag tag)
        {
            if (tag != null)
                if (!Categories.Contains(tag))
                {
                    Categories.Add(tag);
                    DatabaseProvider.DB.Upsert(Mapper.Map<Tag, DTO.Tag>(tag));
                }
        }
    }
}