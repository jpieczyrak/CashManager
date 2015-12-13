using System;

namespace Logic.TransactionManagement
{
    public class TimeFrame
    {
        DateTime From { get; set; }

        DateTime To { get; set; }

        public TimeFrame(DateTime @from, DateTime to)
        {
            From = @from;
            To = to;
        }

        /// <summary>
        /// Checks if given date is between borders of timeframe
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool Contains(DateTime date)
        {
            return date >= From && date <= To;
        }
    }
}