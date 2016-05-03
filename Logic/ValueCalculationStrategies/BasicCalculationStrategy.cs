using System.Collections.ObjectModel;
using System.Linq;
using Logic.TransactionManagement;

namespace Logic.ValueCalculationStrategies
{
    public class BasicCalculationStrategy : IValueCalculationStrategy
    {
        public double CalculateValue(eTransactionType transactionType,
            ObservableCollection<TransactionPartPayment> transactionSoucePayments,
            ObservableCollection<Subtransaction> subtransactions)
        {
            double subtransactionCost = subtransactions.Sum(subtransaction => subtransaction.Value);

            double value =
                transactionSoucePayments.Sum(
                    payment =>
                        payment.PaymentType.Equals(ePaymentType.Value)
                            ? payment.Value
                            : subtransactionCost*payment.Value/100);
            return value;
        }
    }
}