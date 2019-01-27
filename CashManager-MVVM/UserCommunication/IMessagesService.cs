using System.Windows;

namespace CashManager.WPF.UserCommunication
{
    public interface IMessagesService
    {
        bool ShowQuestionMessage(string title = null, string question = null, Window parent = null);
    }
}