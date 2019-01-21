using System.Windows;

using CashManager_MVVM.Features.Common;

namespace CashManager_MVVM.UserCommunication
{
    public class MessageBoxService : IMessagesService
    {
        #region IMessagesService

        public bool ShowQuestionMessage(string title = null, string question = null, Window parent = null)
        {
            var window = new SimpleMessageBoxWindow(question, title, parent, MessageBoxButton.YesNo, MessageBoxImage.Question);
            window.ShowDialog();
            return window.Result == MessageBoxResult.Yes;
        }

        #endregion
    }
}