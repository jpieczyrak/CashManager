﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CashManager.Features.Common;

namespace CashManager.Features.Main
{
    public partial class PasswordPromptWindow : CustomWindow
    {
        public bool Success { get; private set; }

        public string PasswordText { get; private set; }

        public PasswordPromptWindow()
        {
            InitializeComponent();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Success = true;
            Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordText = ((PasswordBox) sender).Password;
        }

        private void PasswordBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButtonClick(sender, e);
            }
        }
    }
}