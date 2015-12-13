namespace Logic.ValueCalculationStrategies
{
    public interface ValueCalculationStrategy
    {
        float CalculateValue(float rawValue, float contribution);
    }
}