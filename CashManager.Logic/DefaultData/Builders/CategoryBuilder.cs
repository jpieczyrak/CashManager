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

        public CategoryBuilder AddTopCategory(Category category)
        {
            LastCategory = category;
            if (category != null) Categories.Add(category);
            return this;
        }

        public CategoryBuilder AddChildrenCategory(Category category)
        {
            if (LastCategory != null)
            {
                category.Parent = LastCategory;
                LastCategory = category;
            }

            return this;
        }
    }
}