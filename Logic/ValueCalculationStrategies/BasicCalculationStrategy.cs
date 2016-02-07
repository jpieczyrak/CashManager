namespace Logic.ValueCalculationStrategies
{
    public class BasicCalculationStrategy : IValueCalculationStrategy
    {
        public float CalculateValue(float rawValue, float contribution)
        {
            return rawValue * contribution / 100.0f;
        }
    }
}