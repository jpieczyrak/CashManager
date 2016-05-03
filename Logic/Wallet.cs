using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Logic.FilesOperations;
using Logic.StocksManagement;
using Logic.TransactionManagement;

namespace Logic
{
    [DataContract(Namespace = "")]
    public class Wallet
    {
        public static string Path = "wallet.xml";
        private ObservableCollection<Stock> _availableStocks = new ObservableCollection<Stock>();

        [DataMember]
        public ObservableCollection<Stock> AvailableStocks
        {
            get
            {
                if (_availableStocks.Count == 0)
                {
                    _availableStocks.Add(Stock.Unknown);
                }
                return _availableStocks;
            }
            set { _availableStocks = value; }
        }

        [DataMember]
        public Transactions Transactions { get; set; } = new Transactions();
        
        public void AddStock(Stock stock)
        {
            //TODO: check if stock allready exists? (or check higher)
            AvailableStocks.Add(stock);
        }


        //TODO: to dict
        public Stock GetStockByName(string stockName)
        {
            return AvailableStocks.FirstOrDefault(stock => stock.ToString().ToLower().Equals(stockName.ToLower()));
        }

        public void Save()
        {
            Serializer.XMLSerializeObject(this, Path);
        }
    }
}
