using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CashManager_MVVM.Logic
{
    public class Debouncer : IDisposable
    {
        private readonly int _wait;
        private readonly List<CancellationTokenSource> _tokens;

        public Debouncer(int wait = 250)
        {
            _wait = wait;
            _tokens = new List<CancellationTokenSource>();
        }

        public void Debouce(Action func)
        {
            CancelAll();
            var cancellationTokenSource = new CancellationTokenSource();
            _tokens.Add(cancellationTokenSource);
            Task.Delay(_wait, cancellationTokenSource.Token)
                .ContinueWith(task =>
                {
                    if (cancellationTokenSource.IsCancellationRequested) return;
                    func();
                    CancelAll();
                    _tokens.Clear();
                }, cancellationTokenSource.Token);
        }

        private void CancelAll()
        {
            foreach (var token in _tokens) if (!token.IsCancellationRequested) token.Cancel();
        }

        public void Dispose()
        {
            CancelAll();
        }
    }
}