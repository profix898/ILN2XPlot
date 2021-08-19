using System;
using System.Collections.Generic;
using System.Drawing;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using XPlot.Plotly;
using ILNLegend = ILNumerics.Drawing.Plotting.Legend;

namespace ILN2XPlot.Generator.Elements
{
    public class PlotCubeBinder : IPlotBinder
    {
        public void Bind(Group group, Group root, List<Trace> traces, List<string> labels, Layout.Layout layout)
        {
            if (!(group is PlotCube plotCube))
                return;

            // TODO: Font: Family, Size, Bold/Italic, Color
            layout.autosize = true;

            // Global
            var title = group.First<Title>();
            layout.title = title?.Label?.Text ?? String.Empty;

            // XAxis
            var xaxis = new Xaxis();
            xaxis.title = plotCube.Axes.XAxis.Label.Text;
            xaxis.type = (plotCube.ScaleModes.XAxisScale == AxisScale.Linear) ? "linear" : "log";
            var xMin = plotCube.Axes.XAxis.Min ?? plotCube.Plots.Limits.XMin;
            var xMax = plotCube.Axes.XAxis.Max ?? plotCube.Plots.Limits.XMax;
            xaxis.range = (plotCube.ScaleModes.XAxisScale == AxisScale.Linear) ? $"{xMin}:{xMax}" : $"{Math.Pow(10.0, xMin)}:{Math.Pow(10.0, xMax)}";
            xaxis.showgrid = plotCube.Axes.XAxis.GridMajor.Visible;
            xaxis.gridcolor = (plotCube.Axes.XAxis.GridMajor.Color ?? Color.FromArgb(230, 230, 230)).ToXPlotColor();
            xaxis.gridwidth = plotCube.Axes.XAxis.GridMajor.Width;
            layout.xaxis = xaxis;

            // YAxis
            var yaxis = new Yaxis();
            yaxis.title = plotCube.Axes.YAxis.Label.Text;
            yaxis.type = (plotCube.ScaleModes.YAxisScale == AxisScale.Linear) ? "linear" : "log";
            var yMin = plotCube.Axes.YAxis.Min ?? plotCube.Plots.Limits.YMin;
            var yMax = plotCube.Axes.YAxis.Max ?? plotCube.Plots.Limits.YMax;
            yaxis.range = (plotCube.ScaleModes.YAxisScale == AxisScale.Linear) ? $"{yMin}:{yMax}" : $"{Math.Pow(10.0, yMin)}:{Math.Pow(10.0, yMax)}";
            yaxis.showgrid = plotCube.Axes.YAxis.GridMajor.Visible;
            yaxis.gridcolor = (plotCube.Axes.YAxis.GridMajor.Color ?? Color.FromArgb(230, 230, 230)).ToXPlotColor();
            yaxis.gridwidth = plotCube.Axes.YAxis.GridMajor.Width;
            layout.yaxis = yaxis;

            // Legend
            var legend = plotCube.First<ILNLegend>();
            //if (legend != null)
            //{
            //    LegendVisible = legend.Visible;
            //    LegendLocation = legend.Location;
            //    LegendBorderColor = legend.Border.Color ?? Color.Black;
            //    layout.Colors.Add(LegendBorderColor);
            //    LegendBackgroundColor = legend.Background.Color ?? Color.White;
            //    layout.Colors.Add(LegendBackgroundColor);
            //}

            // Map plots (LinePlot, Surface, etc.)

            // LinePlots
            foreach (var linePlot in plotCube.Find<LinePlot>())
                linePlot.Bind<LinePlotBinder>(plotCube, traces, labels, layout);

            //// SurfacePlots
            //foreach (var surface in plotCube.Find<ILNumerics.Drawing.Plotting.Surface>())
            //    surface.Bind<SurfaceBinder>(plotCube, traces, labels, layout);
        }
    }
}
