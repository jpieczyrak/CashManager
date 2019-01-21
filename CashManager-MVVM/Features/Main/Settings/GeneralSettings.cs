using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

using CashManager_MVVM.Skins;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Main.Settings
{
    internal class GeneralSettings : ObservableObject
    {
        private readonly Lazy<ColorsResourceDictionary> _colorsResourceDictionary;
        private readonly Lazy<ShapesResourceDictionary> _shapesDictionary;

        public bool IsSoundEnabled
        {
            get => Properties.Settings.Default.SoundEnabled;
            set => Properties.Settings.Default.SoundEnabled = value;
        }

        public IEnumerable<SkinColors> Skins { get; } = Enum.GetValues(typeof(SkinColors)).Cast<SkinColors>();

        public IEnumerable<SkinShapes> Shapes { get; } = Enum.GetValues(typeof(SkinShapes)).Cast<SkinShapes>();

        public SkinColors SelectedSkin
        {
            get => (SkinColors)Properties.Settings.Default.SkinColor;
            set
            {
                Properties.Settings.Default.SkinColor = (int)value;
                _colorsResourceDictionary.Value?.UpdateSource();
            }
        }

        public SkinShapes SelectedShape
        {
            get => (SkinShapes)Properties.Settings.Default.SkinShape;
            set
            {
                Properties.Settings.Default.SkinShape = (int)value;
                _shapesDictionary.Value?.UpdateSource();
            }
        }

        public string[] Localizations { get; }

        public bool UseExtendedDatePicker
        {
            get => Properties.Settings.Default.UseExtendedDatePicker;
            set
            {
                Properties.Settings.Default.UseExtendedDatePicker = value;
                RaisePropertyChanged(nameof(UseExtendedDatePicker));
            }
        }

        public string SelectedLocalization
        {
            get => Properties.Settings.Default.Localization;
            set
            {
                Properties.Settings.Default.Localization = value;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(value);
            }
        }

        public GeneralSettings()
        {
            Localizations = new[] { "pl-PL", "en-US" };
            _colorsResourceDictionary = new Lazy<ColorsResourceDictionary>(() =>
                Application.Current.Resources
                           .MergedDictionaries
                           .FirstOrDefault(x => x is ColorsResourceDictionary) as ColorsResourceDictionary);
            _shapesDictionary = new Lazy<ShapesResourceDictionary>(() =>
                Application.Current.Resources
                           .MergedDictionaries
                           .FirstOrDefault(x => x is ShapesResourceDictionary) as ShapesResourceDictionary);
        }
    }
}