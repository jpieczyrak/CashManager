using System.Collections.ObjectModel;
using Logic.TransactionManagement;

namespace Logic
{
    public class CategoryProvider
    {
        public static ObservableCollection<StringWrapper> Categories { get; } = new ObservableCollection<StringWrapper>();

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
                if (!Categories.Contains(new StringWrapper(category)))
                {
                    Categories.Add(new StringWrapper(category));
                }
            }
        }

        public static void Add(StringWrapper category)
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
