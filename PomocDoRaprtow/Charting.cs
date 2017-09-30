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
        public static void BarChart(Chart Bar_chart_control, DataTable chart_data, int x_name_column_index, int x_value_columns_index)
        {
            List<string> x_names = new List<string>();
            List<double> x_values = new List<double>();

            foreach (DataRow row in chart_data.Rows)
            {
                double val = 0;
                if (double.TryParse(row[x_value_columns_index].ToString(), out val))
                {
                    x_names.Add(row[x_name_column_index].ToString());
                    x_values.Add(val);
                }
            }

            Bar_chart_control.Series.Clear();
            Bar_chart_control.ChartAreas.Clear();

            Series ser = new Series();
            ser.IsVisibleInLegend = false;
            ser.IsValueShownAsLabel = false;
            ser.ChartType = SeriesChartType.Column;

            ChartArea area = new ChartArea();
            area.AxisX.IsLabelAutoFit = true;
            area.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep45;
            area.AxisX.LabelStyle.Enabled = true;
            area.AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 12);

            Bar_chart_control.Series.Add(ser);
            Bar_chart_control.ChartAreas.Add(area);

            for (int i = 0; i < x_values.Count; i++)
            {
                Bar_chart_control.Series[0].Points.AddXY(x_names[i], x_values[i]);
            }



        }

        public static void Shift_Line_chart(Chart Line_chart_control,DataTable chart_data,int date_col_index, int shift_col_index, int quantity_col_index)
        {
            Line_chart_control.Series.Clear();
            Line_chart_control.ChartAreas.Clear();

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


            Line_chart_control.Series.Add(ser1);
            Line_chart_control.Series.Add(ser2);
            Line_chart_control.Series.Add(ser3);
            Line_chart_control.ChartAreas.Add(area);

            foreach (DataRow row in chart_data.Rows)
            {
                if (row[shift_col_index].ToString() == "1") Line_chart_control.Series[0].Points.AddXY(row[date_col_index].ToString(), row[quantity_col_index].ToString());
                if (row[shift_col_index].ToString() == "2") Line_chart_control.Series[1].Points.AddXY(row[date_col_index].ToString(), row[quantity_col_index].ToString());
                if (row[shift_col_index].ToString() == "3") Line_chart_control.Series[2].Points.AddXY(row[date_col_index].ToString(), row[quantity_col_index].ToString());
            }

            
        }
    }
}
