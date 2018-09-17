namespace Logic.Specification
{
    public class NotSpecification<T> : CompositeSpecification<T>
    {
        private readonly ISpecification<T> _theSpecification;

        public NotSpecification(ISpecification<T> specification)
        {
            _theSpecification = specification;
        }

        public override bool IsSatisfiedBy(T transaction)
        {
            return !_theSpecification.IsSatisfiedBy(transaction);
        }
    }
}