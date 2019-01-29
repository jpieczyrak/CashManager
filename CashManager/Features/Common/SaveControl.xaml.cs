using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager.Features.Common
{
    public partial class SaveControl : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen), typeof(bool), typeof(SaveControl), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command), typeof(RelayCommand<string>), typeof(SaveControl), new PropertyMetadata(default(RelayCommand<string>)));

        public bool IsOpen
        {
            get => (bool) GetValue(IsOpenProperty);
            set
            {
                SetValue(IsOpenProperty, value);
                TextValue = string.Empty;
            }
        }

        public static readonly DependencyProperty TextValueProperty = DependencyProperty.Register(
            nameof(TextValue), typeof(string), typeof(SaveControl), new PropertyMetadata(default(string)));

        public string TextValue
        {
            get => (string) GetValue(TextValueProperty);
            set => SetValue(TextValueProperty, value);
        }

        public RelayCommand<string> Command
        {
            get => (RelayCommand<string>) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public SaveControl() => InitializeComponent();

        private void ButtonOpenClick(object sender, RoutedEventArgs e)
        {
            IsOpen = true;
            EntryBox.Focus();
        }

        private void ButtonCloseClick(object sender, RoutedEventArgs e) => IsOpen = false;

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Command.CanExecute(TextValue))
                {
                    Command.Execute(TextValue);
                    ButtonCloseClick(sender, e);
                }
            }

            if (e.Key == Key.Escape)
                ButtonCloseClick(sender, e);
        }
    }
}