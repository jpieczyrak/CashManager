using System;
using System.Collections.ObjectModel;
using System.Linq;
using Logic.TransactionManagement;

namespace Logic.ValueCalculationStrategies
{
    public class BasicCalculationStrategy : IValueCalculationStrategy
    {
        public float CalculateValue(float rawValue, float contribution)
        {
            return rawValue * contribution / 100.0f;
        }

        public float CalculateValue(eTransactionType transactionType, ObservableCollection<TransactionPartPayment> transactionSoucePayments,
           float contribution, ePaymentType contributionType)
        {
            float sourcesValue = (float) transactionSoucePayments.Sum(payment => payment.Value);

            bool profit = false;

            switch (transactionType)
            {
                case eTransactionType.Sell:
                case eTransactionType.Resell:
                case eTransactionType.Work:
                    profit = true;
                    break;
                case eTransactionType.Transfer:
                    return 0;
                case eTransactionType.Buy:
                case eTransactionType.Reinvest:
                    profit = true;
                    break;
            }

            if (contributionType.Equals(ePaymentType.Percent))
                return sourcesValue * contribution / 100 * (profit ? 1 : -1);
            else
                return contribution * (profit ? 1 : -1);
        }
    }
}