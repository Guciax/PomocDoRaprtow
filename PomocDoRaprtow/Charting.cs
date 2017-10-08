using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace PomocDoRaprtow
{
    class Charting
    {
        public static void BarChart(Chart barChartControl, DataTable chartData, int xNameColumnIndex,
            int xValueColumnsIndex)
        {
            List<string> xNames = new List<string>();
            List<double> xValues = new List<double>();
            List<double> xPercentage = new List<double>();

            foreach (DataRow row in chartData.Rows)
            {
                double val = 0;
                if (double.TryParse(row[xValueColumnsIndex].ToString(), out val))
                {
                    xNames.Add(row[xNameColumnIndex].ToString());
                    xValues.Add(val);
                }
            }
            var valuesSum = xValues.Sum();
            double runningSum = 0;

            foreach (var val in xValues)
            {
                if (valuesSum > 0) xPercentage.Add((val + runningSum) / valuesSum * 100); else xPercentage.Add(0);
                runningSum += val;
            }


            barChartControl.Series.Clear();
            barChartControl.ChartAreas.Clear();

            Series serColumn = new Series();
            serColumn.IsVisibleInLegend = false;
            serColumn.IsValueShownAsLabel = false;
            serColumn.ChartType = SeriesChartType.Column;
            serColumn.Color = System.Drawing.Color.DarkBlue;

            Series serLine = new Series();
            serLine.ChartType = SeriesChartType.Line;
            serLine.YAxisType = AxisType.Secondary;
            serLine.IsVisibleInLegend = false;
            serLine.IsValueShownAsLabel = true;
            serLine.LabelFormat = "{0} %";
            serLine.Color = System.Drawing.Color.Orange;
            serLine.BorderWidth = 2;

            ChartArea area = new ChartArea();
            area.AxisX.IsLabelAutoFit = true;
            area.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep45;
            area.AxisX.LabelStyle.Enabled = true;
            area.AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 12);
            area.AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            area.AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            area.AxisY2.MajorGrid.Enabled = false;
            area.AxisY2.Enabled = AxisEnabled.Auto;
            area.AxisY2.LabelStyle.Format = "{0} %";

            barChartControl.Series.Add(serColumn);
            barChartControl.Series.Add(serLine);
            barChartControl.ChartAreas.Add(area);

            for (int i = 0; i < xValues.Count; i++)
            {
                barChartControl.Series[0].Points.AddXY(xNames[i], xValues[i]);
                barChartControl.Series[1].Points.AddXY(xNames[i], xPercentage[i]);
            }
        }

        public static void ShiftLineChart(Chart lineChartControl, DataTable chartData, int dateColIndex,
            int shiftColIndex, int quantityColIndex)
        {
            lineChartControl.Series.Clear();
            lineChartControl.ChartAreas.Clear();

            Series ser1 = new Series();
            ser1.IsVisibleInLegend = false;
            ser1.IsValueShownAsLabel = false;
            ser1.ChartType = SeriesChartType.FastLine;
            ser1.Color = System.Drawing.Color.BlueViolet;
            ser1.LegendText = "1";
            ser1.BorderWidth = 2;

            Series ser2 = new Series();
            ser2.IsVisibleInLegend = false;
            ser2.IsValueShownAsLabel = false;
            ser2.ChartType = SeriesChartType.Line;
            ser2.Color = System.Drawing.Color.Chocolate;
            ser2.LegendText = "2";

            Series ser3 = new Series();
            ser3.IsVisibleInLegend = false;
            ser3.IsValueShownAsLabel = false;
            ser3.ChartType = SeriesChartType.Line;
            ser3.Color = System.Drawing.Color.Lime;
            ser3.LegendText = "3";

            ChartArea area = new ChartArea();
            area.AxisX.IsLabelAutoFit = true;
            area.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep45;
            area.AxisX.LabelStyle.Enabled = true;
            area.AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 12);
            area.AxisX.Interval = 1;
            area.AxisY.Interval = 500;
            area.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            area.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            
            lineChartControl.Series.Add(ser1);
            lineChartControl.Series.Add(ser2);
            lineChartControl.Series.Add(ser3);
            lineChartControl.ChartAreas.Add(area);

            foreach (DataRow row in chartData.Rows)
            {
                if (row[shiftColIndex].ToString() == "1")
                    lineChartControl.Series[0].Points
                        .AddXY(row[dateColIndex].ToString(), row[quantityColIndex].ToString());
                if (row[shiftColIndex].ToString() == "2")
                    lineChartControl.Series[1].Points
                        .AddXY(row[dateColIndex].ToString(), row[quantityColIndex].ToString());
                if (row[shiftColIndex].ToString() == "3")
                    lineChartControl.Series[2].Points
                        .AddXY(row[dateColIndex].ToString(), row[quantityColIndex].ToString());
            }
        }
    }
}