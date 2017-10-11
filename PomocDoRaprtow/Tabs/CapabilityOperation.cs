using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PomocDoRaprtow.Tabs
{
    public class CapabilityOperation
    {
        public LedStorage LedStorage { get; set; }
        private readonly Form1 form1;
        private readonly TreeView treeViewTestCapa;
        private readonly Chart chartCapaTest;
        private readonly RichTextBox richTextBoxCapaTest;
        private readonly DataGridView dataGridViewCapaTest;
        private readonly Chart chartSplitting;
        private readonly DataGridView dataGridViewSplitting;

        public CapabilityOperation(Form1 form1, TreeView treeViewTestCapa, RichTextBox richTextBoxCapaTest, 
            Chart chartCapaTest, DataGridView dataGridViewCapaTest, 
            Chart chartSplitting, DataGridView dataGridViewSplitting)
        {
            this.form1 = form1;
            this.treeViewTestCapa = treeViewTestCapa;
            this.chartCapaTest = chartCapaTest;
            this.richTextBoxCapaTest = richTextBoxCapaTest;
            this.dataGridViewCapaTest = dataGridViewCapaTest;
            this.chartSplitting = chartSplitting;
            this.dataGridViewSplitting = dataGridViewSplitting;
        }

        private string FormatTreeViewNodeName(String mainName, int occurences)
        {
            return mainName + " " + occurences;
        }

        private void RebuildOccurenceTreeView(OccurenceCalculations occurenceCalculations)
        {
            treeViewTestCapa.BeginUpdate();
            treeViewTestCapa.Nodes.Clear();
            foreach (var weekTree in occurenceCalculations.Tree.WeekNoToTree.Values)
            {
                var weekTreeViewNode =
                    new TreeNode(FormatTreeViewNodeName(weekTree.Week.ToString(), weekTree.Occurences));
                treeViewTestCapa.Nodes.Add(weekTreeViewNode);
                foreach (var dayTree in weekTree.DayToTree.Values)
                {
                    string dayMonth = dayTree.DateTime.Day.ToString("d2") + "-" + dayTree.DateTime.Month.ToString("d2");
                    var dayTreeViewNode = weekTreeViewNode.Nodes.Add(FormatTreeViewNodeName(dayMonth, dayTree.Occurences));
                    foreach (var shiftTree in dayTree.ShiftToTree)
                    {
                        if (shiftTree.Occurences == 0) continue;
                        var shiftTreeViewNode = dayTreeViewNode.Nodes.Add(FormatTreeViewNodeName(shiftTree.ShiftNo.ToString(), shiftTree.Occurences));
                        foreach (var modelTree in shiftTree.ModelToTree.Values)
                        {
                            shiftTreeViewNode.Nodes.Add(FormatTreeViewNodeName(modelTree.Model, modelTree.Occurences));
                        }
                    }
                }
            }
            treeViewTestCapa.EndUpdate();
        }

        public void DrawCapability(List<Led> leds, List<Lot> lots)
        {
            DrawCapaChart(PrepareTesterDataForCharting(leds), chartCapaTest, dataGridViewCapaTest);
            DrawCapaChart(PrepareSplittingDataForCharting(lots), chartSplitting, dataGridViewSplitting);
            DisplayOccurences(leds);
        }

        private DataTable PrepareSplittingDataForCharting(List<Lot> lots)
        {
            DataTable GridSource = new DataTable();
            GridSource.Columns.Add("Day");
            GridSource.Columns.Add("Shift III", typeof(int));
            GridSource.Columns.Add("Shift I", typeof(int));
            GridSource.Columns.Add("Shift II", typeof(int));

            List<int> initializedDays = new List<int>();

            foreach (var lot in lots)
            {
                if (lot.WasteInfo == null) continue;

                var splittingDate = lot.WasteInfo.SplittingDate;
                var shiftInfo = DateUtilities.DateToShiftInfo(splittingDate);
                string dayMonth = ShiftInfoToDayMonth(shiftInfo);
                if (!initializedDays.Contains(shiftInfo.DayOfTheMonth))
                {
                    initializedDays.Add(shiftInfo.DayOfTheMonth);
                    GridSource.Rows.Add(dayMonth, 0, 0, 0);
                }

                int gridColumn = ShiftUtilities.ShiftNoToIndex(shiftInfo.ShiftNo);

                var indexInInitializedDays = initializedDays.IndexOf(shiftInfo.DayOfTheMonth);
                GridSource.Rows[indexInInitializedDays][gridColumn] =
                    (int)GridSource.Rows[indexInInitializedDays][gridColumn] + lot.LedsInLot.Count;
            }

            DataView dv = GridSource.DefaultView;
            dv.Sort = "Day asc";
            GridSource = dv.ToTable();


            return GridSource;
        }

        private DataTable PrepareTesterDataForCharting(List<Led> leds)
        {
            DataTable GridSource = new DataTable();
            GridSource.Columns.Add("Day");
            GridSource.Columns.Add("Shift III", typeof(int));
            GridSource.Columns.Add("Shift I", typeof(int));
            GridSource.Columns.Add("Shift II", typeof(int));

            List<int> initializedDays = new List<int>();

            foreach (var led in leds)
            {
                foreach (var testerData in form1.TesterDataFilter(led.TesterData))
                {
                    DateUtilities.ShiftInfo shiftInfo = DateUtilities.DateToShiftInfo(testerData.TimeOfTest);
                    string dayMonth = ShiftInfoToDayMonth(shiftInfo);
                    if (!initializedDays.Contains(shiftInfo.DayOfTheMonth))
                    {
                        initializedDays.Add(shiftInfo.DayOfTheMonth);
                        GridSource.Rows.Add(dayMonth, 0, 0, 0);
                    }

                    int gridColumn = ShiftUtilities.ShiftNoToIndex(shiftInfo.ShiftNo);

                    var indexInInitializedDays = initializedDays.IndexOf(shiftInfo.DayOfTheMonth);
                    GridSource.Rows[indexInInitializedDays][gridColumn] =
                        (int)GridSource.Rows[indexInInitializedDays][gridColumn] + 1;

                }
            }

            DataView dv = GridSource.DefaultView;
            dv.Sort = "Day asc";
            GridSource = dv.ToTable();


            return GridSource;
        }

        private String ShiftInfoToDayMonth(DateUtilities.ShiftInfo shiftInfo)
        {
            return shiftInfo.DayOfTheMonth.ToString("d2") + "-" + shiftInfo.Month.ToString("d2");
        }

        private void DisplayOccurences(List<Led> leds)
        {
            var occurencesCalculations = new OccurenceCalculations(leds, form1.TesterDataFilter);
            RebuildOccurenceTreeView(occurencesCalculations);

            richTextBoxCapaTest.Text = "";
            int occurencesSum = 0;
            foreach (KeyValuePair<int, int> kvp in occurencesCalculations.CountOccurences)
            {
                occurencesSum += kvp.Value;
            }

            foreach (KeyValuePair<int, int> kvp in occurencesCalculations.CountOccurences)
            {
                richTextBoxCapaTest.AppendText($"{kvp.Key} test: {kvp.Value} - {MathUtilities.CalculatePercentage(occurencesSum, kvp.Value)}" + "\r");
            }
        }

        private void DrawCapaChart(DataTable gridSource, Chart DestinationChart, DataGridView DestinationGrid)
        {
            DestinationGrid.DataSource = gridSource;
            foreach (DataGridViewColumn col in DestinationGrid.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }


            DestinationChart.Series.Clear();
            DestinationChart.ChartAreas.Clear();

            Series ser1 = new Series();
            ser1.IsVisibleInLegend = false;
            ser1.IsValueShownAsLabel = false;
            ser1.ChartType = SeriesChartType.Column;
            ser1.Color = System.Drawing.Color.BlueViolet;
            ser1.LegendText = "1";
            ser1.BorderWidth = 2;

            Series ser2 = new Series();
            ser2.IsVisibleInLegend = false;
            ser2.IsValueShownAsLabel = false;
            ser2.ChartType = SeriesChartType.Column;
            ser2.Color = System.Drawing.Color.Chocolate;
            ser2.LegendText = "2";
            ser2.BorderWidth = 2;

            Series ser3 = new Series();
            ser3.IsVisibleInLegend = false;
            ser3.IsValueShownAsLabel = false;
            ser3.ChartType = SeriesChartType.Column;
            ser3.Color = System.Drawing.Color.Lime;
            ser3.LegendText = "3";
            ser3.BorderWidth = 2;

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

            foreach (DataRow row in gridSource.Rows)
            {
                DestinationChart.Series[0].Points
                    .AddXY(row[0].ToString(), (int)row[1]);

                DestinationChart.Series[1].Points
                    .AddXY(row[0].ToString(), (int)row[2]);

                DestinationChart.Series[2].Points
                    .AddXY(row[0].ToString(), (int)row[3]);
            }
        }

    }
}
