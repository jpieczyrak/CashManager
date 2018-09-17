namespace Logic.Specification
{
    public class OrSpecification<T> : CompositeSpecification<T>
    {
        readonly ISpecification<T> _leftSpecification;
        readonly ISpecification<T> _rightSpecification;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _leftSpecification = left;
            _rightSpecification = right;
        }

        public override bool IsSatisfiedBy(T transaction)
        {
            return _leftSpecification.IsSatisfiedBy(transaction)
                   || _rightSpecification.IsSatisfiedBy(transaction);
        }
    }
}