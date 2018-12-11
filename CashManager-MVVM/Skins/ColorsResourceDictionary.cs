using System;
using System.Windows;

namespace CashManager_MVVM.Skins
{
    internal class ColorsResourceDictionary : ResourceDictionary
    {
        private Uri _darkSource;
        private Uri _defaultSource;

        public Uri DarkSource
        {
            get => _darkSource;
            set
            {
                _darkSource = value;
                UpdateSource();
            }
        }

        public Uri DefaultSource
        {
            get => _defaultSource;
            set
            {
                _defaultSource = value;
                UpdateSource();
            }
        }

        private void UpdateSource()
        {
            var selected = App.SkinColors == SkinColors.Dark ? DarkSource : DefaultSource;
            if (selected != null && Source != selected) Source = selected;
        }
    }
}