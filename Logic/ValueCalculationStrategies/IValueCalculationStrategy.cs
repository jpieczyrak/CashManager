using System.Collections.ObjectModel;
using Logic.TransactionManagement;

namespace Logic.ValueCalculationStrategies
{
    public interface IValueCalculationStrategy
    { 
        double CalculateValue(eTransactionType transactionType, ObservableCollection<TransactionPartPayment> transactionSoucePayments, ObservableCollection<Subtransaction> subtransactions);
    }
}