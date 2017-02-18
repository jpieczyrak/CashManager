using System.Collections.ObjectModel;

using Logic.TransactionManagement.TransactionElements;

namespace Logic.LogicObjectsProviders
{
    public class CategoryProvider
    {
        public static ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        /// <summary>
        /// Loads all categories from existing transactions.
        /// Should be called after loading transactions
        /// </summary>
        /// <param name="transactions"></param>
        public static void Load(ObservableCollection<Transaction> transactions)
        {
            foreach (Transaction transaction in transactions)
            {
                foreach (var sub in transaction.Subtransactions)
                {
                    Add(sub.Category);
                }
            }
        }

        public static void Add(string category)
        {
            if (!string.IsNullOrEmpty(category))
            {
                if (!Categories.Contains(new Category(category)))
                {
                    Categories.Add(new Category(category));
                }
            }
        }

        public static void Add(Category category)
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
