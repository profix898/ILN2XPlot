using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ILN2XPlot;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace XPlotDemo
{
    public partial class XPlotDemoForm : Form
    {
        #region PlotVariants enum

        public enum PlotVariants
        {
            LinePlot,
            LinePlotLog,
            //ErrorBarPlot,
            //Surface,
            //FastSurface
        }

        #endregion

        public XPlotDemoForm()
        {
            InitializeComponent();

            comboBoxScene.DataSource = Enum.GetValues(typeof(PlotVariants));
        }

        private void XPlotDemoForm_Load(object sender, EventArgs e)
        {
            comboBoxScene.SelectedIndex = 0;
        }

        private void comboBoxScene_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxScene.SelectedIndex == -1)
                return;

            var plotCube = new PlotCube();
            plotCube.Axes.XAxis.Ticks.TickLength = -plotCube.Axes.XAxis.Ticks.TickLength; // Ticks Inside
            plotCube.Axes.YAxis.Ticks.TickLength = -plotCube.Axes.YAxis.Ticks.TickLength; // Ticks Inside
            plotCube.Axes.ZAxis.Ticks.TickLength = -plotCube.Axes.ZAxis.Ticks.TickLength; // Ticks Inside
            plotCube.AspectRatioMode = AspectRatioMode.MaintainRatios;

            switch (comboBoxScene.SelectedIndex)
            {
                // LinePlot
                case (int) PlotVariants.LinePlot:
                {
                    Array<float> A = tosingle(rand(13, 10)) + arange<float>(0, 12);
                    var linePlot = LinePlot.CreateXPlots(A,
                                                         lineStyles: new[] { DashStyle.Solid, DashStyle.Solid, DashStyle.Solid, DashStyle.Solid, DashStyle.Dashed,
                                                            DashStyle.Dashed, DashStyle.Dashed, DashStyle.PointDash, DashStyle.PointDash, DashStyle.PointDash,
                                                            DashStyle.Dotted, DashStyle.Dotted, DashStyle.Dotted },
                                                         lineWidth: new[] { 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1 });

                    plotCube.Add(linePlot);
                    break;
                }

                // LinePlot (log)
                case (int) PlotVariants.LinePlotLog:
                {
                    Array<double> x = logspace(0, 3);
                    Array<float> A = zeros<float>(3, x.Length);
                    A[0, full] = tosingle(x);

                    Array<double> y1 = 10 + (abs(randn(1, 50)) * x);
                    A[1, full] = tosingle(y1);
                    var linePlot1 = new LinePlot(A, markerStyle: MarkerStyle.Cross, markerColor: Color.Crimson);
                    plotCube.Add(linePlot1);

                    Array<double> y2 = abs(randn(1, 50)) * x * x;
                    A[1, full] = tosingle(y2);
                    var linePlot2 = new LinePlot(A, markerStyle: MarkerStyle.Cross, markerColor: Color.BlueViolet);
                    plotCube.Add(linePlot2);

                    plotCube.ScaleModes.XAxisScale = AxisScale.Logarithmic;
                    plotCube.ScaleModes.YAxisScale = AxisScale.Logarithmic;
                    plotCube.Axes.XAxis.Label.Text = "Voltage / V_{rms}";
                    plotCube.Axes.YAxis.Label.Text = "Area / m^2";

                    plotCube.Add(new Legend("one", "two"));
                    break;
                }

                //// Error Bar Plot
                //case (int) PlotVariants.ErrorBarPlot:
                //{
                //    // LinePlot points
                //    Array<float> XY = linspace<float>(0, 1, 20);
                //    XY["1;:"] = exp(XY["0;:"]);

                //    // Error margins
                //    Array<float> L = tosingle(rand(1, 20)) * 0.5f;
                //    Array<float> T = tosingle(rand(1, 20)) * 0.5f;

                //    var errorBarPlot = new ErrorBarPlot(XY, L, T);
                //    plotCube.Add(errorBarPlot);
                //    break;
                //}

                //// Surface
                //case (int) PlotVariants.Surface:
                //{
                //    var surface = new Surface((x, y) => (float) (Math.Sin(x) * Math.Cos(y) * Math.Exp(-((x * x) + (y * y)) / 46)),
                //                              -10, 10, 40, -5, 5, 40);
                //    surface.Colormap = Colormaps.Jet;

                //    plotCube.Add(surface);
                //    plotCube.TwoDMode = false;
                //    plotCube.Rotation = Matrix4.Rotation(new Vector3(1f,0.23f,1), 0.7f);
                //    break;
                //}

                //// FastSurface
                //case (int) PlotVariants.FastSurface:
                //{
                //    var fastSurface = new FastSurface();
                //    fastSurface.Update(Z: tosingle(SpecialData.terrain["30:5:350;10:5:350"]), colormap: Colormaps.Summer);

                //    plotCube.Add(fastSurface);
                //    plotCube.TwoDMode = false;
                //    plotCube.Rotation = Matrix4.Rotation(new Vector3(1f,0.23f,1), 0.7f);
                //    break;
                //}
            }

            ilPanel.Scene = new Scene();
            ilPanel.Scene.Add(plotCube);

            ilPanel.Configure();
            ilPanel.Refresh();
        }

        private void btnExportPlotly_Click(object sender, EventArgs e)
        {
            var currentScene = ilPanel.GetCurrentScene() ?? ilPanel.Scene;
            var plotlyChart = ILN2XPlotExport.Export(currentScene);

            if (plotlyChart != null)
                webView.CoreWebView2.NavigateToString(plotlyChart.GetHtml());
            else
            {
                var svgContent = WriteSVG(currentScene);

                // Known Issue: NavigateToString has a 2 MB size limit
                // (see https://docs.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.winforms.webview2.navigatetostring)
                // We write to a temporary file and load that one instead (as a workaround)

                //webView.CoreWebView2.NavigateToString(svgContent);

                var filePath = Path.GetTempFileName() + ".svg";
                File.WriteAllText(filePath, svgContent);
                webView.CoreWebView2.Navigate($"file:///{filePath}");
            }
        }

        #region Private

        public static string WriteSVG(Scene scene)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Render SVG into memory stream
                new SVGDriver(memoryStream, 800, 600, scene).Render();

                var svgBytes = memoryStream.ToArray();
                var svgContent = Encoding.UTF8.GetString(svgBytes, 0, svgBytes.Length);

                svgContent = svgContent.Replace("<?xml version='1.0' encoding='UTF-8'?>", ""); // Strip XML header
                svgContent = svgContent.Replace("<!DOCTYPE svg PUBLIC '-//W3C//DTD SVG 1.1//EN' 'http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd'>", ""); // Strip DocType header
                svgContent = svgContent.Replace("\r\n", ""); // Strip all line breaks

                return svgContent;
            }
        }

        #endregion
    }
}