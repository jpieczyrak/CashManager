﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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

        public bool IsSoundEnabled
        {
            get => Settings.Default.SoundEnabled;
            set => Settings.Default.SoundEnabled = value;
        }

        public IEnumerable<SkinColors> Skins { get; } = Enum.GetValues(typeof(SkinColors)).Cast<SkinColors>();

        public IEnumerable<SkinShapes> Shapes { get; } = Enum.GetValues(typeof(SkinShapes)).Cast<SkinShapes>();

        public SkinColors SelectedSkin
        {
            get => (SkinColors) Settings.Default.SkinColor;
            set
            {
                Settings.Default.SkinColor = (int) value;
                App.SkinColors = value;
                _colorsResourceDictionary.Value?.UpdateSource();
            }
        }

        public SkinShapes SelectedShape
        {
            get => (SkinShapes) Settings.Default.SkinShape;
            set
            {
                Settings.Default.SkinShape = (int) value;
                App.SkinShape = value;
                _shapesDictionary.Value?.UpdateSource();
            }
        }

        public string[] Localizations { get; }

        public string SelectedLocalization
        {
            get => Settings.Default.Localization;
            set
            {
                Settings.Default.Localization = value;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(value);
            }
        }

        public SettingsViewModel()
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