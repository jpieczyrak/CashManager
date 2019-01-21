using System.Windows;

namespace CashManager_MVVM.UserCommunication
{
    public class MessageBoxService : IMessagesService
    {
        #region IMessagesService

        public bool ShowQuestionMessage(string title = null, string question = null)
        {
            var result = MessageBox.Show(question, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        #endregion
    }
}