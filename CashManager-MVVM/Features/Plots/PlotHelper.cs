using System.Windows;

using OxyPlot;

namespace CashManager.WPF.Features.Plots
{
    internal static class PlotHelper
    {
        internal static PlotModel CreatePlotModel()
        {
            var application = Application.Current;

            var defaultColor = OxyColor.FromRgb(100, 100, 100);
            var borderColor = application != null
                                  ? OxyColor.Parse(application.Resources["BorderColor"].ToString())
                                  : defaultColor;
            var textColor = borderColor;
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
                PlotAreaBorderColor = OxyColors.Gray,
                SubtitleColor = textColor,
                SelectionColor = selectColor
            };
        }
    }
}