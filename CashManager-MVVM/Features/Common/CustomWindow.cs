using System;
using System.Windows;
using System.Windows.Controls;

using CashManager.WPF.Features.Main;

namespace CashManager.WPF.Features.Common
{
    public class CustomWindow : Window
    {
        public static readonly DependencyProperty NotificationPanelProperty = DependencyProperty.Register(
            "NotificationPanel", typeof(NotificationViewModel), typeof(CustomWindow), new PropertyMetadata(default(NotificationViewModel)));

        public NotificationViewModel NotificationPanel
        {
            get => (NotificationViewModel) GetValue(NotificationPanelProperty);
            internal set => SetValue(NotificationPanelProperty, value);
        }

        private Button _minimize;
        private Button _size;
        private Button _close;

        public CustomWindow() { Loaded += OnLoaded; }

        protected virtual void OnClosed(object sender, EventArgs e) { Close(); }

        protected virtual void CloseOnClick(object sender, RoutedEventArgs e) { OnClosed(sender, e); }

        protected virtual void RestoreOnClick(object sender, RoutedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
            }
        }

        protected virtual void MinimizeOnClick(object sender, RoutedEventArgs e) { WindowState = WindowState.Minimized; }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _minimize = (Button) Template.FindName("WindowMinimizeButton", this);
            _size = (Button) Template.FindName("WindowSizeButton", this);
            _close = (Button) Template.FindName("WindowCloseButton", this);

            _minimize.Click += MinimizeOnClick;
            _size.Click += RestoreOnClick;
            _close.Click += CloseOnClick;
        }

        #region Override

        ~CustomWindow()
        {
            _minimize.Click -= MinimizeOnClick;
            _size.Click -= RestoreOnClick;
            _close.Click -= CloseOnClick;
        }

        #endregion
    }
}