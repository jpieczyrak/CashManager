using System;
using System.Windows;

namespace CashManager_MVVM.Skins
{
    internal class ColorsResourceDictionary : ResourceDictionary
    {
        private Uri _darkSource;
        private Uri _lightSource;

        public Uri DarkSource
        {
            get => _darkSource;
            set
            {
                _darkSource = value;
                UpdateSource();
            }
        }

        public Uri LightSource
        {
            get => _lightSource;
            set
            {
                _lightSource = value;
                UpdateSource();
            }
        }

        private void UpdateSource()
        {
            Uri selected;
            switch (App.SkinColors)
            {
                case SkinColors.Dark:
                    selected = DarkSource;
                    break;
                case SkinColors.Light:
                    selected = LightSource;
                    break;
                default:
                    selected = LightSource;
                    break;
            }

            if (selected != null && Source != selected) Source = selected;
        }
    }
}