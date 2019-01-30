using System;
using System.Diagnostics;

using log4net;

namespace CashManager.Logic.Wrappers
{
    public class MeasureTimeWrapper : IDisposable
    {
        private readonly string _description;
        private readonly Stopwatch _stopwatch;

        public MeasureTimeWrapper(Action action, string description = null)
        {
            _description = description;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            action();
            _stopwatch.Stop();
        }

        public void Dispose()
        {
            string msPart = _stopwatch.ElapsedMilliseconds < 1000 ? $" [{_stopwatch.ElapsedMilliseconds,3}] ms" : string.Empty;
            LogManager.GetLogger(typeof(MeasureTimeWrapper)).Info($"{_description,-30} Time: {_stopwatch.Elapsed:c}{msPart}");
        }
    }
}