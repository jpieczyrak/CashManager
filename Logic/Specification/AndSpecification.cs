﻿namespace LogicOld.Specification
{
    public class AndSpecification<T> : CompositeSpecification<T>
    {
        private readonly ISpecification<T> _leftSpecification;
        private readonly ISpecification<T> _rightSpecification;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _leftSpecification = left;
            _rightSpecification = right;
        }

        public override bool IsSatisfiedBy(T transaction)
        {
            return _leftSpecification.IsSatisfiedBy(transaction)
                   && _rightSpecification.IsSatisfiedBy(transaction);
        }
    }
}