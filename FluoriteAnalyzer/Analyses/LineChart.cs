using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FluoriteAnalyzer.Common;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Analyses
{
    internal partial class LineChart : UserControl, IRedrawable
    {
        #region Delegates

        public delegate void ChartDoubleClickHandler(int timevalue);

        #endregion

        private static readonly int XAXIS_DIVISOR = 1000;

        public LineChart(ILogProvider log)
        {
            InitializeComponent();

            chartLine.MouseEnter += chartLine_MouseEnter;
            chartLine.MouseWheel += chartLine_MouseWheel;

            LogProvider = log;

            LineChartScale = 1.0;
        }

        private ILogProvider LogProvider { get; set; }

        private double LineChartScale { get; set; }

        public event ChartDoubleClickHandler ChartDoubleClick;

        private void SetLineChartAxisYTitle()
        {
            Axis axisY = chartLine.ChartAreas[0].AxisY;

            if (radioDocumentLength.Checked)
            {
                axisY.Title = "Number of Characters";
            }
            else if (radioActiveCodeLength.Checked)
            {
                axisY.Title = "Number of Characters in Active Code";
            }
            else if (radioExpressionCount.Checked)
            {
                axisY.Title = "Number of Expression Nodes";
            }
            else if (radioASTNodeCount.Checked)
            {
                axisY.Title = "Number of AST Nodes";
            }
            else
            {
                axisY.Title = "";
            }
        }

        private int GetLineChartYValue(DocumentChange documentChange)
        {
            if (radioDocumentLength.Checked)
            {
                return documentChange.DocumentLength;
            }
            else if (radioActiveCodeLength.Checked)
            {
                return documentChange.ActiveCodeLength;
            }
            else if (radioExpressionCount.Checked)
            {
                return documentChange.ExpressionCount;
            }
            else if (radioASTNodeCount.Checked)
            {
                return documentChange.ASTNodeCount;
            }
            else
            {
                return 0;
            }
        }

        // How to merge with the previous method??
        private int GetLineChartYValue(FileOpenCommand fileOpenCommand)
        {
            if (radioDocumentLength.Checked)
            {
                return fileOpenCommand.DocumentLength;
            }
            else if (radioActiveCodeLength.Checked)
            {
                return fileOpenCommand.ActiveCodeLength;
            }
            else if (radioExpressionCount.Checked)
            {
                return fileOpenCommand.ExpressionCount;
            }
            else if (radioASTNodeCount.Checked)
            {
                return fileOpenCommand.ASTNodeCount;
            }
            else
            {
                return 0;
            }
        }

        private void general_CheckedChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void chartLine_MouseEnter(object sender, EventArgs e)
        {
            chartLine.Focus();
        }

        private void chartLine_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                LineChartScale += 0.1;
            }
            else if (e.Delta < 0 && LineChartScale >= 1.1)
            {
                LineChartScale -= 0.1;
            }

            double originalSize = LogProvider.LoggedEvents.Last().Timestamp/XAXIS_DIVISOR;
            double size = originalSize/LineChartScale;

            Axis axis = chartLine.ChartAreas[0].AxisX;

            // Get current position value
            double currentValue = axis.PixelPositionToValue(e.X);
            double ratio = (currentValue - axis.Minimum)/(axis.Maximum - axis.Minimum);

            double viewStart = currentValue - size*ratio;
            if (viewStart < 0.0)
            {
                viewStart = 0.0;
            }

            double viewEnd = viewStart + size;
            if (viewEnd > originalSize)
            {
                viewEnd = originalSize;
                viewStart = viewEnd - size;
            }

            axis.ScaleView.Zoom((int) viewStart, (int) viewEnd);
        }

        private void chartLine_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HitTestResult result = chartLine.HitTest(e.X, e.Y);
            if (result.ChartArea != null)
            {
                var timevalue = (int) (result.ChartArea.AxisX.PixelPositionToValue(e.X)*XAXIS_DIVISOR);

                if (ChartDoubleClick != null)
                {
                    ChartDoubleClick(timevalue);
                }
            }
        }

        #region IRedrawable 멤버

        public void Redraw()
        {
            chartLine.Series.Clear();

            string currentFile = null;

            if (radioAltogether.Checked)
            {
                chartLine.Series.Add("Total");
                Series series = chartLine.Series[0];

                series.ChartType = SeriesChartType.StepLine;
                series.BorderWidth = 1;
            }

            var fileValueMap = new Dictionary<string, int>();
            // Input the first values for each file
            IEnumerable<IGrouping<string, FileOpenCommand>> fileGroups =
                LogProvider.LoggedEvents.OfType<FileOpenCommand>().GroupBy(x => Path.GetFileName(x.FilePath));
            foreach (var group in fileGroups)
            {
                fileValueMap.Add(group.Key, GetLineChartYValue(group.First()));

                //if (this.radioPerFile.Checked)
                {
                    chartLine.Series.Add(Path.GetFileName(group.Key));
                    Series series = chartLine.Series[Path.GetFileName(group.Key)];

                    series.ChartType = SeriesChartType.StepLine;
                    series.BorderWidth = 1;
                }
            }

            SetLineChartAxisYTitle();

            foreach (
                Event element in
                    LogProvider.LoggedEvents.Where(x => x is DocumentChange || x is FileOpenCommand).OrderBy(
                        x => x.Timestamp))
            {
                if (element is DocumentChange)
                {
                    if (currentFile == null)
                    {
                        continue;
                    }

                    var documentChange = (DocumentChange) element;

                    int length = documentChange.Length;
                    long timestamp = documentChange.Timestamp;

                    int value = GetLineChartYValue(documentChange);
                    fileValueMap[Path.GetFileName(currentFile)] = value;

                    if (radioPerFile.Checked)
                    {
                        chartLine.Series[Path.GetFileName(currentFile)].Points.AddXY(timestamp/XAXIS_DIVISOR, value);
                    }
                    else
                    {
                        chartLine.Series[0].Points.AddXY(timestamp/XAXIS_DIVISOR, fileValueMap.Values.Sum());
                    }
                }
                else if (element is FileOpenCommand)
                {
                    var fileOpenCommand = (FileOpenCommand) element;
                    if (fileOpenCommand.FilePath == null || fileOpenCommand.FilePath == "null")
                    {
                        currentFile = null;
                        continue;
                    }

                    currentFile = fileOpenCommand.FilePath;
                }

                // Make sure that the X axis starts with 0
                chartLine.ChartAreas[0].AxisX.Minimum = 0;
            }
        }

        #endregion
    }
}