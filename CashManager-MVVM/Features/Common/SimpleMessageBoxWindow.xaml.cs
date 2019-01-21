using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CashManager_MVVM.Features.Common
{
    public partial class SimpleMessageBoxWindow : CustomWindow
    {
        private readonly Dictionary<MessageBoxResult, Button> _availableButtons;

        public MessageBoxResult Result { get; private set; }

        public SimpleMessageBoxWindow() { }

        public SimpleMessageBoxWindow(string message, string title, Window parent, MessageBoxButton buttons, MessageBoxImage icon)
        {
            InitializeComponent();
            _availableButtons = new Dictionary<MessageBoxResult, Button>
            {
                [MessageBoxResult.OK] = OkButton,
                [MessageBoxResult.Cancel] = CancelButton,
                [MessageBoxResult.Yes] = YesButton,
                [MessageBoxResult.No] = NoButton
            };
            TextBlockMessage.Text = message;
            Title = title;
            Owner = parent;

            ShowButtons(buttons);
        }

        private void ShowButtons(MessageBoxButton buttons)
        {
            if (buttons == MessageBoxButton.OK)
            {
                _availableButtons[MessageBoxResult.OK].Visibility = Visibility.Visible;
            }
            else if (buttons == MessageBoxButton.OKCancel)
            {
                _availableButtons[MessageBoxResult.OK].Visibility = Visibility.Visible;
                _availableButtons[MessageBoxResult.Cancel].Visibility = Visibility.Visible;
            }
            else if (buttons == MessageBoxButton.YesNo)
            {
                _availableButtons[MessageBoxResult.Yes].Visibility = Visibility.Visible;
                _availableButtons[MessageBoxResult.No].Visibility = Visibility.Visible;
            }
            else if (buttons == MessageBoxButton.YesNoCancel)
            {
                _availableButtons[MessageBoxResult.Yes].Visibility = Visibility.Visible;
                _availableButtons[MessageBoxResult.No].Visibility = Visibility.Visible;
                _availableButtons[MessageBoxResult.Cancel].Visibility = Visibility.Visible;
            }
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult) ((Button) sender).Tag;
            Close();
        }

        #region Override

        protected override void CloseOnClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.None;
            base.CloseOnClick(sender, e);
        }

        #endregion
    }
}