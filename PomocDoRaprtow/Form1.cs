using System;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;

namespace PomocDoRaprtow
{
    public partial class Form1 : Form
    {
        private readonly OptionProvider optionProvider;
        private LedStorage leds;

        public Form1()
        {
            InitializeComponent();
            dateTimePicker_odpad_do.Format = DateTimePickerFormat.Custom;
            dateTimePicker_odpad_od.Format = DateTimePickerFormat.Custom;

            dateTimePicker_odpad_do.CustomFormat = "dd-MM-yyyy HH:mm:ss";
            dateTimePicker_odpad_od.CustomFormat = "dd-MM-yyyy HH:mm:ss";

            optionProvider = new OptionProvider(this);
        }

        public DateTimePicker WasteSinceTimePicker => dateTimePicker_odpad_od;
        public DateTimePicker WasteToTimePicker => dateTimePicker_odpad_do;

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        DataTable Odpady_table = new DataTable();
        DataTable Tester_table = new DataTable();
        static DataTable LOT_Module_Short = new DataTable();

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable hist = TableOperations.HistogramTable(Odpady_table, new[] {3, 4, 5, 6, 7, 8, 9, 10},
                optionProvider);
            dataGridView1.DataSource = hist;
            Charting.BarChart(chart1, hist, 0, 1);
        }


        public static string LOT_to_Model(string lot)
        {
            foreach (DataRow row in LOT_Module_Short.Rows)
            {
                if (row[0].ToString() == lot)
                {
                    //Debug.WriteLine(row[1]);
                    return row[1].ToString();
                }
            }
            return "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTable shifts = TableOperations.Tester_IloscNaZmiane(Tester_table);
            dataGridView1.DataSource = shifts;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Charting.ShiftLineChart(chart1, shifts, 0, 1, 2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Odpady_table = SqlTableLoader.LoadWasteTable();
            Tester_table = SqlTableLoader.LoadTesterWorkCard();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            leds = new LedStorageLoader().BuildStorage();
            ModelcheckedListBox.Items.Clear();
            foreach (var model in leds.Models)
            {
                ModelcheckedListBox.Items.Add(model);
                ModelcheckedListBox.SetItemChecked(ModelcheckedListBox.Items.Count - 1, true);
            }
        }

        private void DrawCapaChart(Chart DestinationChart, DataGridView DestinationGrid)
        {
            DataTable GridSource = new DataTable();
            GridSource.Columns.Add("Day");
            GridSource.Columns.Add("Shift III", typeof(int));
            GridSource.Columns.Add("Shift I", typeof(int));
            GridSource.Columns.Add("Shift II", typeof(int));

            List<int> initializedDays = new List<int>();

            foreach (var led in FilterLeds())
            {
                if (led.TesterData.TimeOfTest > dateTimePicker_wyd_od.Value &&
                    led.TesterData.TimeOfTest < dateTimePicker_wyd_do.Value)
                {
                    DateUtilities.ShiftInfo shiftInfo = DateUtilities.DateToShiftInfo(led.TesterData.TimeOfTest);
                    if (!initializedDays.Contains(shiftInfo.DayOfTheMonth))
                    {
                        initializedDays.Add(shiftInfo.DayOfTheMonth);
                        GridSource.Rows.Add(shiftInfo.Month.ToString("d2")+"-"+ shiftInfo.DayOfTheMonth.ToString("d2"), 0, 0, 0);
                    }

                    int gridColumn = 1;
                    switch (shiftInfo.ShiftNo)
                    {
                        case 3:
                            gridColumn = 1;
                            break;
                        case 1:
                            gridColumn = 2;
                            break;
                        case 2:
                            gridColumn = 3;
                            break;
                    }

                    var indexInInitializedDays = initializedDays.IndexOf(shiftInfo.DayOfTheMonth);
                    GridSource.Rows[indexInInitializedDays][gridColumn] =
                        (int) GridSource.Rows[indexInInitializedDays][gridColumn] + 1;
                }
            }

            DataView dv = GridSource.DefaultView;
            dv.Sort = "Day asc";
            GridSource = dv.ToTable();

            dataGridView_Capacity_Test.DataSource = GridSource;
            foreach (DataGridViewColumn col in dataGridView_Capacity_Test.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            

            DestinationChart.Series.Clear();
            DestinationChart.ChartAreas.Clear();

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


            DestinationChart.Series.Add(ser1);
            DestinationChart.Series.Add(ser2);
            DestinationChart.Series.Add(ser3);
            DestinationChart.ChartAreas.Add(area);

            foreach (DataRow row in GridSource.Rows)
            {
                DestinationChart.Series[0].Points
                    .AddXY(row[0].ToString(), (int) row[1]);

                DestinationChart.Series[1].Points
                    .AddXY(row[0].ToString(), (int) row[2]);

                DestinationChart.Series[2].Points
                    .AddXY(row[0].ToString(), (int) row[3]);
            }
        }

        private HashSet<String> enabledModels = new HashSet<string>();
        private bool PassesFilter(Led led)
        {
            return enabledModels.Contains(led.Lot.Model);
        }
        private IEnumerable<Led> FilterLeds()
        {
            enabledModels.Clear();
            foreach(var model in ModelcheckedListBox.CheckedItems)
            {
                enabledModels.Add(model as String);
            }
            return leds.SerialNumbersToLed.Values.Where(PassesFilter);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Tab.SelectedTab.Name == "tab_Waste")
            {
            }
            if (Tab.SelectedTab.Name == "tab_Capacity")
            {
                DrawCapaChart(chart_Capacity_Test, dataGridView_Capacity_Test);
            }
        }

        private void DrawWasteHistogram()
        {
            DataTable hist = TableOperations.HistogramTable(Odpady_table, new[] {3, 4, 5, 6, 7, 8, 9, 10},
                optionProvider);

            dataGridView_odpad.DataSource = hist;
            Charting.BarChart(chart_odpad, hist, 0, 1);
        }

        private void dateTimePicker_odpad_od_ValueChanged(object sender, EventArgs e)
        {
            DrawWasteHistogram();
        }

        private void dateTimePicker_odpad_do_ValueChanged(object sender, EventArgs e)
        {
            DrawWasteHistogram();
        }

        private void checkedListBox1_MouseEnter(object sender, EventArgs e)
        {
            ModelcheckedListBox.Height = 200;
        }

        private void checkedListBox1_MouseLeave(object sender, EventArgs e)
        {
            ModelcheckedListBox.Height = 50;
        }

        private void ModelcheckedListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            DrawCapaChart(chart_Capacity_Test, dataGridView_Capacity_Test);
        }

        private void dateTimePicker_wyd_od_ValueChanged(object sender, EventArgs e)
        {
            DrawCapaChart(chart_Capacity_Test, dataGridView_Capacity_Test);
        }

        private void dateTimePicker_wyd_do_ValueChanged(object sender, EventArgs e)
        {
            DrawCapaChart(chart_Capacity_Test, dataGridView_Capacity_Test);
        }
    }
}