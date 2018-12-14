using System;
using System.Windows;

namespace CashManager_MVVM.Skins
{
    internal class ShapesResourceDictionary : ResourceDictionary
    {
        private Uri _roundShape;
        private Uri _rectShape;
        private Uri _defaultSource;

        public Uri RoundShape
        {
            get => _roundShape;
            set
            {
                _roundShape = value;
                UpdateSource();
            }
        }

        public Uri RectShape
        {
            get => _rectShape;
            set
            {
                _rectShape = value;
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
            Uri selected;
            switch (App.SkinShape)
            {
                case SkinShapes.Round:
                    selected = RoundShape;
                    break;
                case SkinShapes.Rect:
                    selected = RectShape;
                    break;
                case SkinShapes.Default:
                default:
                    selected = DefaultSource;
                    break;
            }
            if (selected != null && Source != selected) Source = selected;
        }
    }
}