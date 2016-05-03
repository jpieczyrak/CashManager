namespace Logic.StocksManagement
{
    public class StockStats
    {
        public StockStats(string name, double value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public double Value { get; set; }
    }
}