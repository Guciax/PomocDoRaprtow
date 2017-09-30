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

            foreach (DataRow row in chartData.Rows)
            {
                double val = 0;
                if (double.TryParse(row[xValueColumnsIndex].ToString(), out val))
                {
                    xNames.Add(row[xNameColumnIndex].ToString());
                    xValues.Add(val);
                }
            }

            barChartControl.Series.Clear();
            barChartControl.ChartAreas.Clear();

            Series ser = new Series();
            ser.IsVisibleInLegend = false;
            ser.IsValueShownAsLabel = false;
            ser.ChartType = SeriesChartType.Column;

            ChartArea area = new ChartArea();
            area.AxisX.IsLabelAutoFit = true;
            area.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep45;
            area.AxisX.LabelStyle.Enabled = true;
            area.AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 12);

            barChartControl.Series.Add(ser);
            barChartControl.ChartAreas.Add(area);

            for (int i = 0; i < xValues.Count; i++)
            {
                barChartControl.Series[0].Points.AddXY(xNames[i], xValues[i]);
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