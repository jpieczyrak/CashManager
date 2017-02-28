using System;
using System.Collections.ObjectModel;
using System.Linq;

using Logic.Model;
using Logic.TransactionManagement.TransactionElements;

namespace Logic.LogicObjectsProviders
{
    public class CategoryProvider
    {
        public static ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        /// <summary>
        ///     Loads all categories from existing transactions.
        ///     Should be called after loading transactions
        /// </summary>
        /// <param name="transactions"></param>
        public static void Load(ObservableCollection<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                foreach (var sub in transaction.Subtransactions)
                {
                    Store(sub.Category);
                }
            }
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
                    Categories.Add(category);
                }
                return category;
            }
            throw new ArgumentNullException("Category name can not be empty!");
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
                }
            }
        }
    }
}