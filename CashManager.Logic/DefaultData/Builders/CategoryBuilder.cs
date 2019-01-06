using System.Collections.Generic;

using CashManager.Data.DTO;
using CashManager.Data.Extensions;

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

        public CategoryBuilder AddTopCategory(string name) => AddTopCategory(new Category(name.GenerateGuid()) { Name = name });

        public CategoryBuilder AddTopCategory(Category category)
        {
            LastCategory = category;
            if (category != null)
            {
                Categories.Add(category);
            }
            return this;
        }

        public CategoryBuilder AddChildrenCategory(string name) => AddChildrenCategory(new Category((LastCategory.Name + name).GenerateGuid()) { Name = name });

        public CategoryBuilder AddChildrenCategory(Category category)
        {
            if (LastCategory != null)
            {
                category.Parent = LastCategory;
                LastCategory = category;
                Categories.Add(category);
            }

            return this;
        }

        public CategoryBuilder AddChildrenCategoryAndGoUp(string name) => AddChildrenCategoryAndGoUp(new Category((LastCategory.Name + name).GenerateGuid()) { Name = name });

        public CategoryBuilder AddChildrenCategoryAndGoUp(Category category)
        {
            if (LastCategory != null)
            {
                category.Parent = LastCategory;
                Categories.Add(category);
            }

            return this;
        }

        public CategoryBuilder GoUp()
        {
            LastCategory = LastCategory?.Parent;
            return this;
        }

        public Category[] Build() => Categories.ToArray();
    }
}