using Logic.Model;
using Logic.Specification;

namespace Logic.FindingFilters
{
    public class TitleContainsRule : CompositeSpecification<Transaction>
    {
        private readonly string _title;

        public TitleContainsRule(string title)
        {
            _title = title;
        }
        
        public override bool IsSatisfiedBy(Transaction transaction)
        {
            return transaction?.Title.ToLower().Contains(_title.ToLower()) ?? false;
        }
    }
}