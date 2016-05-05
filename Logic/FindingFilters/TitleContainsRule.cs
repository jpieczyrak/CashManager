using Logic.Specification;
using Logic.TransactionManagement;

namespace Logic.FindingFilters
{
    public class TitleContainsRule : CompositeSpecification<Transaction>
    {
        private readonly string _title;

        public TitleContainsRule(string title)
        {
            _title = title;
        }
        
        public override bool IsSatisfiedBy(Transaction o)
        {
            return o.Title.ToLower().Contains(_title.ToLower());
        }
    }
}