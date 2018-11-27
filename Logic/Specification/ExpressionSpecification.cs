using System;

namespace LogicOld.Specification
{
    public class ExpressionSpecification<T> : CompositeSpecification<T>
    {
        private readonly Func<T, bool> _expression;

        public ExpressionSpecification(Func<T, bool> expression)
        {
            _expression = expression ?? throw new ArgumentNullException();
        }

        public override bool IsSatisfiedBy(T transaction)
        {
            return _expression(transaction);
        }
    }
}