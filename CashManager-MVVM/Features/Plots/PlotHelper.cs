using System.Windows;

using OxyPlot;

namespace CashManager_MVVM.Features.Plots
{
    internal static class PlotHelper
    {
        internal static PlotModel CreatePlotModel()
        {
            var textColor = OxyColor.Parse(Application.Current.Resources["ForegroundColor"].ToString());
            var borderColor = OxyColor.Parse(Application.Current.Resources["BorderColor"].ToString());
            var selectColor = OxyColor.Parse(Application.Current.Resources["HoverBorderAccentColor"].ToString());

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