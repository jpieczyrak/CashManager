namespace Logic.ValueCalculationStrategies
{
    public interface IValueCalculationStrategy
    {
        float CalculateValue(float rawValue, float contribution);
    }
}