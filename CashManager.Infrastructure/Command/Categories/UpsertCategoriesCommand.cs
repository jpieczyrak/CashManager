using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Categories
{
    public class UpsertCategoriesCommand : ICommand
    {
        public Category[] Categories { get; }

        public UpsertCategoriesCommand(Category[] categories)
        {
            Categories = categories;
        }
    }
}