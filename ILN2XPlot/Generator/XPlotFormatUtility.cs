using System;
using System.Drawing;
using ILNumerics.Drawing;

namespace ILN2XPlot.Generator
{
    public static class XPlotFormatUtility
    {
        public static string ToXPlotColor(this Color color)
        {
            return $"rgb({color.R}, {color.G}, {color.B})";
        }

        public static string ToXPlotDashStyle(this DashStyle dashStyle)
        {
            switch (dashStyle)
            {
                case DashStyle.Solid:
                    return "solid";
                case DashStyle.Dashed:
                    return "dash";
                case DashStyle.PointDash:
                    return "dashdot";
                case DashStyle.Dotted:
                    return "dot";
                case DashStyle.UserPattern:
                    return "solid"; // Not supported: Fallback to solid
                default:
                    throw new ArgumentOutOfRangeException(nameof(dashStyle), dashStyle, null);
            }
        }

        public static string ToXPlotMarkerSymbol(this MarkerStyle markerStyle)
        {
            switch (markerStyle)
            {
                case MarkerStyle.Dot:
                    return "circle";
                case MarkerStyle.Circle:
                    return "circle";
                case MarkerStyle.Diamond:
                    return "diamond";
                case MarkerStyle.Square:
                    return "square";
                case MarkerStyle.Rectangle:
                    return "square-x";
                case MarkerStyle.TriangleUp:
                    return "triangle-up";
                case MarkerStyle.TriangleDown:
                    return "triangle-down";
                case MarkerStyle.TriangleLeft:
                    return "triangle-left";
                case MarkerStyle.TriangleRight:
                    return "triangle-right";
                case MarkerStyle.Plus:
                    return "cross";
                case MarkerStyle.Cross:
                    return "x";
                case MarkerStyle.Custom:
                    return "star"; // Not supported: Fallback to star
                case MarkerStyle.None:
                    return "";
                default:
                    throw new ArgumentOutOfRangeException(nameof(markerStyle), markerStyle, null);
            }
        }
    }
}
