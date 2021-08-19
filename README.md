ILN2XPlot Export
==========

Export functionality for ILNumerics (http://ilnumerics.net/) scene graphs
and plot cubes to XPlot [XPlot](https://fslab.org/XPlot/) (interactive data visualization package) with [plotly](https://plotly.com/) backend.

## How to use

Export scene to XPlot (plotly) chart:
```csharp
var plotlyChart = ILN2XPlotExport.Export(scene);
```
As of today (August 2021) only _LinePlot_ is supported.

### License
ILN2XPlot is licensed under the terms of the MIT license (<http://opensource.org/licenses/MIT>, see LICENSE.txt).
