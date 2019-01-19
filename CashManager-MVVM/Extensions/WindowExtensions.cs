using System.Threading.Tasks;
using System.Windows;

namespace CashManager_MVVM.Extensions
{
    internal static class WindowExtensions
    {
        internal static Task ShowBlocking<T>(this T window) where T : Window
        {
            var task = new TaskCompletionSource<object>();
            window.Closed += (sender, args) => task.SetResult(null);
            window.Show();
            window.Focus();
            return task.Task;
        }
    }
}