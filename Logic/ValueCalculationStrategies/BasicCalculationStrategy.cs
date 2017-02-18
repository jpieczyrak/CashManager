using System.Collections.ObjectModel;
using System.Linq;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

namespace Logic.ValueCalculationStrategies
{
    public class BasicCalculationStrategy : IValueCalculationStrategy
    {
        public double CalculateValue(eTransactionType transactionType,
            ObservableCollection<TransactionPartPayment> transactionSoucePayments,
            ObservableCollection<Subtransaction> subtransactions)
        {
            double subtransactionCost = subtransactions.Sum(subtransaction => subtransaction.Value);

            //Not needed since Transaction.Validate()
            //double value =
            //    transactionSoucePayments.Sum(
            //        payment =>
            //            payment.PaymentType.Equals(ePaymentType.Value)
            //                ? payment.Value
            //                : subtransactionCost*payment.Value/100);
            return subtransactionCost;
        }
    }
}