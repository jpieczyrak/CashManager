using System;
using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using LogicOld.Database;
using LogicOld.Model;
using LogicOld.Utils;

namespace LogicOld.LogicObjectsProviders
{
    public class CategoryProvider
    {
        private static TrulyObservableCollection<Category> _categories;

        public static TrulyObservableCollection<Category> Categories => _categories ?? (_categories = Load());

        public static TrulyObservableCollection<Category> Load()
        {
            var dtos = DatabaseProvider.DB.Read<DTO.Category>();
            var list = dtos.Select(Mapper.Map<DTO.Category, Category>);
            _categories = new TrulyObservableCollection<Category>(list);
            _categories.CollectionChanged += CategoriesOnCollectionChanged;

            return _categories;
        }

        /// <summary>
        ///     Finds category - or if not exist - creates the new one and returns result
        /// </summary>
        /// <param name="categoryName">Unique category name</param>
        /// <returns>Found or created category</returns>
        public static Category FindOrCreate(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                var category =
                    Categories.FirstOrDefault(c => string.Equals(c.Value, categoryName, StringComparison.CurrentCultureIgnoreCase));

                if (category == null)
                {
                    category = new Category(categoryName);
                    Store(category);
                }
                return category;
            }
            throw new ArgumentNullException("Category name can not be empty!");
        }

        public static Category GetById(Guid id) => Categories.FirstOrDefault(c => c.Id.Equals(id));

        private static void CategoriesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.NewItems != null)
            {
                foreach (Category category in notifyCollectionChangedEventArgs.NewItems)
                {
                    DatabaseProvider.DB.Upsert(Mapper.Map<Category, DTO.Category>(category));
                }
            }
            if (notifyCollectionChangedEventArgs.OldItems != null)
            {
                foreach (Category category in notifyCollectionChangedEventArgs.OldItems)
                {
                    DatabaseProvider.DB.Remove(Mapper.Map<Category, DTO.Category>(category));
                }
            }
        }

        /// <summary>
        ///     Stores (for provider purpose) - loaded category
        /// </summary>
        /// <param name="category">Existsing category from main source (e.g. transactions)</param>
        private static void Store(Category category)
        {
            if (category != null)
            {
                if (!Categories.Contains(category))
                {
                    Categories.Add(category);
                    DatabaseProvider.DB.Upsert(Mapper.Map<Category, DTO.Category>(category));
                }
            }
        }
    }
}