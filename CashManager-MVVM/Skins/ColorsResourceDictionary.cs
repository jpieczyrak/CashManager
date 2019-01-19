using System;
using System.Windows;

using CashManager_MVVM.Properties;

namespace CashManager_MVVM.Skins
{
    internal class ColorsResourceDictionary : ResourceDictionary
    {
        private Uri _darkSource;
        private Uri _lightSource;
        private Uri _yellowSource;
        private Uri _blackWhiteSource;

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

        public Uri BlackWhite
        {
            get => _blackWhiteSource;
            set
            {
                _blackWhiteSource = value;
                UpdateSource();
            }
        }

        public void UpdateSource()
        {
            Uri selected;
            switch ((SkinColors)Settings.Default.SkinColor)
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
                case SkinColors.BlackWhite:
                    selected = BlackWhite;
                    break;
                default:
                    selected = DarkSource;
                    break;
            }

            if (selected != null && Source != selected) Source = selected;
        }
    }
}