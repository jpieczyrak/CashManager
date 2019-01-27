using System;
using System.Windows;

using CashManager.WPF.Properties;

namespace CashManager.WPF.Skins
{
    internal class ColorsResourceDictionary : ResourceDictionary
    {
        private Uri _darkRedSource;
        private Uri _darkOrangeSource;
        private Uri _lightSource;
        private Uri _yellowSource;
        private Uri _blackWhiteSource;

        public Uri DarkRedSource
        {
            get => _darkRedSource;
            set
            {
                _darkRedSource = value;
                UpdateSource();
            }
        }

        public Uri DarkOrangeSource
        {
            get => _darkOrangeSource;
            set
            {
                _darkOrangeSource = value;
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
            switch ((SkinColors) Settings.Default.SkinColor)
            {
                case SkinColors.DarkOrange:
                    selected = DarkOrangeSource;
                    break;
                case SkinColors.DarkRed:
                    selected = DarkRedSource;
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
                    selected = DarkOrangeSource;
                    break;
            }

            if (selected != null && Source != selected) Source = selected;
        }
    }
}