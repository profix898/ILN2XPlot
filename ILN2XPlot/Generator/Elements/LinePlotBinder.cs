using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using XPlot.Plotly;
using ILNLegend = ILNumerics.Drawing.Plotting.Legend;
using Line = XPlot.Plotly.Line;
using Marker = XPlot.Plotly.Marker;
using static ILNumerics.ILMath;

namespace ILN2XPlot.Generator.Elements
{
    public class LinePlotBinder : IPlotBinder
    {
        #region IPlotBinder Members

        public void Bind(Group group, Group root, List<Trace> traces, List<string> labels, Layout.Layout layout)
        {
            if (!(group is LinePlot linePlot))
                return;

            if (!(root is PlotCube plotCube))
                return;

            var scatter = new Scatter();

            Array<float> xValues = squeeze(linePlot.Positions[0, Globals.full]);
            Array<float> yValues = squeeze(linePlot.Positions[1, Globals.full]);
            if (plotCube.ScaleModes.XAxisScale == AxisScale.Logarithmic)
                xValues = pow(10f, xValues);
            if (plotCube.ScaleModes.YAxisScale == AxisScale.Logarithmic)
                yValues = pow(10f, yValues);
            scatter.x = xValues.ToArray();
            scatter.y = yValues.ToArray();

            // Line
            scatter.line = new Line
            {
                width = linePlot.Line.Width,
                color = (linePlot.Line.Color ?? Color.Black).ToXPlotColor(),
                dash = linePlot.Line.DashStyle.ToXPlotDashStyle()
            };

            // Marker
            if (linePlot.Marker.Style != MarkerStyle.None)
            {
                scatter.marker = new Marker
                {
                    size = Math.Max(linePlot.Marker.Size / 2, 1),
                    color = (linePlot.Marker.Fill.Color ?? linePlot.Line.Color ?? Color.Black).ToXPlotColor(),
                    symbol = linePlot.Marker.Style.ToXPlotMarkerSymbol()
                };
                scatter.mode = "lines+markers";
            }
            else
                scatter.mode = "lines";

            var legend = plotCube.First<ILNLegend>();
            if (legend != null)
            {
                var legendItems = legend.Find<LegendItem>().ToArray();
                var legendItem = legendItems.FirstOrDefault(item => item.ProviderID == linePlot.ID);
                if (legendItem != null)
                    scatter.name = legendItem.Text;
                else
                {
                    var idx = Array.IndexOf(plotCube.Find<LinePlot>().ToArray(), linePlot);
                    if (idx != -1 && idx < legendItems.Length)
                        legendItem = legendItems[idx];
                    scatter.name = legendItem?.Text ?? $"Line {labels.Count}";
                }
            }

            traces.Add(scatter);
            labels.Add(scatter.name);
        }

        #endregion
    }
}
