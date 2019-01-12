using System;
using System.Windows;

namespace CashManager_MVVM.Skins
{
    internal class ColorsResourceDictionary : ResourceDictionary
    {
        private Uri _darkSource;
        private Uri _lightSource;
        private Uri _yellowSource;

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

        public Uri YellowSource
        {
            get => _yellowSource;
            set
            {
                _yellowSource = value;
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
                case SkinColors.Yellow:
                    selected = YellowSource;
                    break;
                default:
                    selected = DarkSource;
                    break;
            }

            if (selected != null && Source != selected) Source = selected;
        }
    }
}