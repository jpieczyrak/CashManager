using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CashManager_MVVM.Properties;
using CashManager_MVVM.Skins;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Main
{
    internal class SettingsViewModel : ViewModelBase
    {
        private readonly Lazy<ColorsResourceDictionary> _colorsResourceDictionary;
        private readonly Lazy<ShapesResourceDictionary> _shapesDictionary;

        private SkinColors _selectedSkin;
        private SkinShapes _selectedShape;

        public bool IsSoundEnabled
        {
            get => Settings.Default.SoundEnabled;
            set => Settings.Default.SoundEnabled = value;
        }

        public IEnumerable<SkinColors> Skins { get; } = Enum.GetValues(typeof(SkinColors)).Cast<SkinColors>();
        public IEnumerable<SkinShapes> Shapes { get; } = Enum.GetValues(typeof(SkinShapes)).Cast<SkinShapes>();

        public SkinColors SelectedSkin
        {
            get => _selectedSkin;
            set
            {
                Set(ref _selectedSkin, value);
                App.SkinColors = value;
                _colorsResourceDictionary.Value?.UpdateSource();
                //todo: save to settings
            }
        }
        public SkinShapes SelectedShape
        {
            get => _selectedShape;
            set
            {
                Set(ref _selectedShape, value);
                App.SkinShape = value;
                _shapesDictionary.Value?.UpdateSource();
                //todo: save to settings
            }
        }

        public SettingsViewModel()
        {
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