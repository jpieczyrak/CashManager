namespace Logic.StocksManagement
{
    /// <summary>
    /// Stores information of unknown stocks
    /// </summary>
    public class Stock
    {
        public Stock(string name)
        {
            Name = name;
        }

        protected string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}