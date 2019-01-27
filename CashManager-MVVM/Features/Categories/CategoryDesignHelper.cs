using System.Collections.Generic;
using System.Linq;

using CashManager.WPF.Model;

namespace CashManager.WPF.Features.Categories
{
    public static class CategoryDesignHelper
    {
        public static Category[] BuildGraphicalOrder(Category[] categories, Category root = null, int index = 0)
        {
            var results = new List<Category>();
            if (root != null) results.Add(root);
            var children = categories.Where(x => Equals(x.Parent, root)).ToArray();
            foreach (var category in children)
            {
                category.Name = $"{string.Join(string.Empty, Enumerable.Range(0, index).Select(x => " "))}{category.Name}";
                results.AddRange(BuildGraphicalOrder(categories, category, index + 1));
            }

            return results.ToArray();
        }
    }
}