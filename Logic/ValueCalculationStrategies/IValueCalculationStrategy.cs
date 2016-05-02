using System.Collections.ObjectModel;
using Logic.TransactionManagement;

namespace Logic.ValueCalculationStrategies
{
    public interface IValueCalculationStrategy
    { 
        float CalculateValue(eTransactionType transactionType, ObservableCollection<TransactionPartPayment> transactionSoucePayments, float contribution, ePaymentType contributionType);
    }
}