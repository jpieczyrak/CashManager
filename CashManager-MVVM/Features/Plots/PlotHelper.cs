using System.Windows;

using OxyPlot;

namespace CashManager_MVVM.Features.Plots
{
    internal static class PlotHelper
    {
        internal static PlotModel CreatePlotModel()
        {
            var application = Application.Current;

            var defaultColor = OxyColor.FromRgb(100, 100, 100);
            var textColor = application != null
                                ? OxyColor.Parse(application.Resources["ForegroundColor"].ToString())
                                : defaultColor;
            var borderColor = application != null
                                  ? OxyColor.Parse(application.Resources["BorderColor"].ToString())
                                  : defaultColor;
            var selectColor = application != null
                                  ? OxyColor.Parse(application.Resources["HoverBorderAccentColor"].ToString())
                                  : defaultColor;

            return new PlotModel
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendPosition = LegendPosition.BottomCenter,
                TextColor = textColor,
                LegendTextColor = textColor,
                LegendTitleColor = textColor,
                TitleColor = textColor,
                PlotAreaBorderColor = borderColor,
                SubtitleColor = textColor,
                SelectionColor = selectColor
            };
        }
    }
}