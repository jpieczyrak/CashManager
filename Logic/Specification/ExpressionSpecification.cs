using System;

namespace LogicOld.Specification
{
    public class ExpressionSpecification<T> : CompositeSpecification<T>
    {
        private readonly Func<T, bool> _expression;

        public ExpressionSpecification(Func<T, bool> expression, Func<T, bool> expression1)
        {
            _expression = expression1;
            if (expression == null)
            {
                throw new ArgumentNullException();
            }
            _expression = expression;
        }

        public override bool IsSatisfiedBy(T transaction)
        {
            return _expression(transaction);
        }
    }
}