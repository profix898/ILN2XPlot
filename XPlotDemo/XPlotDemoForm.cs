using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ILN2XPlot;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;

namespace TikzDemo
{
    public partial class XPlotDemoForm : Form
    {
        public XPlotDemoForm()
        {
            InitializeComponent();
            


            comboBoxScene.Items.Add("LinePlot");
            comboBoxScene.Items.Add("LinePlot (log)");
            comboBoxScene.Items.Add("Surface");
        }

        private async void XPlotDemoForm_Load(object sender, EventArgs e)
        {
            await webView.EnsureCoreWebView2Async();

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

            if (comboBoxScene.SelectedIndex == 0) // LinePlot
            {
                Array<float> A = ILMath.tosingle(ILMath.rand(13, 10)) + ILMath.arange<float>(0, 12);
                var linePlot = LinePlot.CreateXPlots(A,
                    lineStyles: new[]
                    {
                        DashStyle.Solid, DashStyle.Solid, DashStyle.Solid, DashStyle.Solid, DashStyle.Dashed,
                        DashStyle.Dashed, DashStyle.Dashed, DashStyle.PointDash, DashStyle.PointDash,
                        DashStyle.PointDash, DashStyle.Dotted, DashStyle.Dotted, DashStyle.Dotted
                    },
                    lineWidth: new[] {1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1});

                plotCube.Add(linePlot);
            }

            if (comboBoxScene.SelectedIndex == 1) // LinePlot (log)
            {
                Array<double> x = ILMath.logspace(0, 3);
                Array<float> A = ILMath.zeros<float>(3, x.Length);
                A[0, Globals.full] = ILMath.tosingle(x);

                Array<double> y1 = 10 + ILMath.abs(ILMath.randn(1, 50)) * x;
                A[1, Globals.full] = ILMath.tosingle(y1);
                var linePlot1 = new LinePlot(A, markerStyle: MarkerStyle.Cross, markerColor: Color.Crimson);
                plotCube.Add(linePlot1);

                Array<double> y2 = ILMath.abs(ILMath.randn(1, 50)) * x * x;
                A[1, Globals.full] = ILMath.tosingle(y2);
                var linePlot2 = new LinePlot(A, markerStyle: MarkerStyle.Cross, markerColor: Color.BlueViolet);
                plotCube.Add(linePlot2);

                plotCube.ScaleModes.XAxisScale = AxisScale.Logarithmic;
                plotCube.ScaleModes.YAxisScale = AxisScale.Logarithmic;
                plotCube.Axes.XAxis.Label.Text = "Voltage / V_{rms}";

                plotCube.Add(new Legend("one", "two"));
            }

            if (comboBoxScene.SelectedIndex == 2) // Surface
            {
                var surface = new Surface((x, y) =>
                        (float) (Math.Sin(x) * Math.Cos(y) * Math.Exp(-(x * x + y * y) / 46)),
                    -10, 10, 40, -5, 5, 40);
                surface.Colormap = Colormaps.ILNumerics;

                plotCube.Add(surface);
                plotCube.TwoDMode = false;
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