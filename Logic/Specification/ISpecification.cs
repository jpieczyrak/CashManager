namespace LogicOld.Specification
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T transaction);

        ISpecification<T> And(ISpecification<T> specification);
        ISpecification<T> Or(ISpecification<T> specification);
        ISpecification<T> Not(ISpecification<T> specification);
    }
}
