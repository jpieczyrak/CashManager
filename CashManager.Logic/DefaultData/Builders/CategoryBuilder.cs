using System.Collections.Generic;

using CashManager.Data.DTO;

namespace CashManager.Logic.DefaultData.Builders
{
    public class CategoryBuilder
    {
        public Category LastCategory { get; private set; }

        public List<Category> Categories { get; }

        public CategoryBuilder()
        {
            Categories = new List<Category>();
        }

        public CategoryBuilder AddCategory(Category category)
        {
            LastCategory = category;
            if (category != null) Categories.Add(category);
            return this;
        }
    }
}