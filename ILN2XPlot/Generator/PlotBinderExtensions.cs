using System.Collections.Generic;
using ILNumerics.Drawing;
using XPlot.Plotly;

namespace ILN2XPlot.Generator
{
    public static class PlotBinderExtensions
    {
        public static void Bind<TBinder>(this Group group, Group root, List<Trace> traces, List<string> labels, Layout.Layout layout)
            where TBinder : IPlotBinder, new()
        {
            if (group == null)
                return;

            var binder = new TBinder();
            binder.Bind(group, root, traces, labels, layout);
        }
    }
}