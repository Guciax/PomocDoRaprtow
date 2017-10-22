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
       private readonly Chart chartSplitting;
        private readonly TreeView treeViewSplitting;
        private readonly TreeView treeViewCapaBoxing;
        private readonly Chart chartCapaBoxing;
        private readonly RadioButton radioModel;

        public CapabilityOperation(Form1 form1, TreeView treeViewTestCapa, RichTextBox richTextBoxCapaTest,
            Chart chartCapaTest, 
            Chart chartSplitting,  TreeView treeViewSplitting, TreeView treeViewCapaBoxing, 
             Chart chartCapaBoxing, RadioButton radioModel)
        {
            this.form1 = form1;
            this.treeViewTestCapa = treeViewTestCapa;
            this.chartCapaTest = chartCapaTest;
            this.richTextBoxCapaTest = richTextBoxCapaTest;
            this.chartSplitting = chartSplitting;
            this.treeViewSplitting = treeViewSplitting;
            this.treeViewCapaBoxing = treeViewCapaBoxing;
            this.chartCapaBoxing = chartCapaBoxing;
            this.radioModel = radioModel;
        }

        private string FormatTreeViewNodeName(String mainName, int occurences)
        {
            return mainName + " " + occurences;
        }

        private void RebuildOccurenceTreeView(TreeView targetTreeView, OccurenceCalculations occurenceCalculations)
        {
            targetTreeView.BeginUpdate();
            targetTreeView.Nodes.Clear();
            foreach (var weekTree in occurenceCalculations.Tree.WeekNoToTree.Values)
            {
                var weekTreeViewNode =
                    new TreeNode(FormatTreeViewNodeName(weekTree.Week.ToString(), weekTree.Occurences));
                targetTreeView.Nodes.Add(weekTreeViewNode);
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
            targetTreeView.EndUpdate();
        }

        public void DrawCapability()
        {
            var leds = LedStorage.Leds.Where(form1.LedModelIsSelected).ToList();
            var lots = LedStorage.Lots.Values.Where(form1.LotByModel).ToList();

            DrawCapaChart(PrepareTesterDataForCharting(leds), chartCapaTest);
            DrawCapaChart(PrepareSplittingDataForCharting(lots), chartSplitting);
            DrawCapaChart(PrepareBoxingDataForCharting(leds), chartCapaBoxing);
            DisplayTesterDataOccurences(leds, lots);
            DisplaySplittingDataOccurences(lots);
            DisplayBoxingDataOccurences(lots);

        }



        private void DisplaySplittingDataOccurences(List<Lot> lots)
        {

            var occurences = new OccurenceCalculations(lots, LotToSplittingDates);
            RebuildOccurenceTreeView(treeViewSplitting, occurences);
        }

        private void DisplayBoxingDataOccurences(List<Lot> lots)
        {

            var occurences = new OccurenceCalculations(lots, LotToBoxingDates);
            RebuildOccurenceTreeView(treeViewCapaBoxing, occurences);
        }

        private DataTable PrepareSplittingDataForCharting(List<Lot> lots)
        {
            DataTable GridSource = new DataTable();
            GridSource.Columns.Add("Day", typeof(DateTime));
            GridSource.Columns.Add("Shift III", typeof(int));
            GridSource.Columns.Add("Shift I", typeof(int));
            GridSource.Columns.Add("Shift II", typeof(int));

            List<DateTime> initializedDays = new List<DateTime>();

            foreach (var lot in lots)
            {
                if (!form1.WasteInfoBySplittingTime(lot.WasteInfo)) continue;

                var splittingDate = lot.WasteInfo.SplittingDate;
                var shiftInfo = DateUtilities.DateToShiftInfo(splittingDate);
                string dayMonth = ShiftInfoToDayMonth(shiftInfo);
                var date = shiftInfo.Date.Date;

                if (!initializedDays.Contains(date))
                {
                    initializedDays.Add(date);
                    GridSource.Rows.Add(date, 0, 0, 0);
                }

                int gridColumn = ShiftUtilities.ShiftNoToIndex(shiftInfo.ShiftNo);

                var indexInInitializedDays = initializedDays.IndexOf(date);
                GridSource.Rows[indexInInitializedDays][gridColumn] =
                    (int)GridSource.Rows[indexInInitializedDays][gridColumn] + lot.LedsInLot.Count;
            }

            DataView dv = GridSource.DefaultView;
            dv.Sort = "Day asc";
            GridSource = dv.ToTable();


            return GridSource;
        }

        private DataTable PrepareBoxingDataForCharting(List<Led> leds)
        {
            DataTable GridSource = new DataTable();
            GridSource.Columns.Add("Day", typeof (DateTime));
            GridSource.Columns.Add("Shift III", typeof(int));
            GridSource.Columns.Add("Shift I", typeof(int));
            GridSource.Columns.Add("Shift II", typeof(int));

            List<DateTime> initializedDays = new List<DateTime>();
            foreach (var boxing in leds.Select(l => l.Boxing).Where(form1.BoxingDateFilter))
            {
                DateUtilities.ShiftInfo shiftInfo = DateUtilities.DateToShiftInfo(boxing.BoxingDate.Value);
                var date = shiftInfo.Date.Date;
                if (!initializedDays.Contains(date))
                {
                    initializedDays.Add(date);
                    GridSource.Rows.Add(date, 0, 0, 0);
                }

                int gridColumn = ShiftUtilities.ShiftNoToIndex(shiftInfo.ShiftNo);

                var indexInInitializedDays = initializedDays.IndexOf(date);
                GridSource.Rows[indexInInitializedDays][gridColumn] =
                    (int)GridSource.Rows[indexInInitializedDays][gridColumn] + 1;
            }

            DataView dv = GridSource.DefaultView;
            dv.Sort = "Day asc";
            GridSource = dv.ToTable();


            return GridSource;
        }


        private DataTable PrepareTesterDataForCharting(List<Led> leds)
        {
            DataTable GridSource = new DataTable();
            GridSource.Columns.Add("Day",typeof(DateTime));
            GridSource.Columns.Add("Shift III", typeof(int));
            GridSource.Columns.Add("Shift I", typeof(int));
            GridSource.Columns.Add("Shift II", typeof(int));

            List<DateTime> initializedDays = new List<DateTime>();

            foreach (var led in leds)
            {
                foreach (var testerData in form1.TesterDataFilter(led.TesterData))
                {
                    DateUtilities.ShiftInfo shiftInfo = DateUtilities.DateToShiftInfo(testerData.TimeOfTest);
                    var date = shiftInfo.Date.Date;
                    if (!initializedDays.Contains(date))
                    {
                        initializedDays.Add(date);
                        GridSource.Rows.Add(date, 0, 0, 0);
                    }

                    int gridColumn = ShiftUtilities.ShiftNoToIndex(shiftInfo.ShiftNo);

                    var indexInInitializedDays = initializedDays.IndexOf(date);
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

        private void DisplayTesterDataOccurences(IEnumerable<Led> leds, List<Lot> lots)
        {
            var occurencesCalculations = new OccurenceCalculations(lots, LotToTesterDates);
            RebuildOccurenceTreeView(treeViewTestCapa, occurencesCalculations);

            richTextBoxCapaTest.Text = "";
            int occurencesSum = 0;
            var countOccurences = new SortedDictionary<int, int>();

            foreach (var led in leds)
            {
                int val = 0;
                var count = led.TesterData.Where(td => form1.DateFilter(td.TimeOfTest)).Count();
                if (count == 0) continue;
                countOccurences.TryGetValue(count, out val);
                countOccurences[count] = val + 1;
            }

            foreach (KeyValuePair<int, int> kvp in countOccurences)
            {
                occurencesSum += kvp.Value;
            }

            foreach (KeyValuePair<int, int> kvp in countOccurences)
            {
                richTextBoxCapaTest.AppendText($"{kvp.Key} test: {kvp.Value} - {MathUtilities.CalculatePercentage(occurencesSum, kvp.Value)}" + "\r");
            }
        }

        private void DrawCapaChart(DataTable gridSource, Chart DestinationChart)
        {
            //DestinationGrid.DataSource = gridSource;
            //foreach (DataGridViewColumn col in DestinationGrid.Columns)
            {
                //col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }


            DestinationChart.Series.Clear();
            DestinationChart.ChartAreas.Clear();
           // DestinationChart.Legends.Clear();

            Series ser1 = new Series();
            ser1.IsVisibleInLegend = true;
            ser1.IsValueShownAsLabel = false;
            ser1.ChartType = SeriesChartType.StackedColumn;
            ser1.Color = System.Drawing.Color.LightSalmon;
            ser1.BorderWidth = 2;
            ser1.Name = "3rd Shift";

            Series ser2 = new Series();
            ser2.IsVisibleInLegend = true;
            ser2.IsValueShownAsLabel = false;
            ser2.ChartType = SeriesChartType.StackedColumn;
            ser2.Color = System.Drawing.Color.CornflowerBlue;
            ser2.Name = "1st Shift";
            ser2.BorderWidth = 2;

            Series ser3 = new Series();
            ser3.IsVisibleInLegend = true;
            ser3.IsValueShownAsLabel = false;
            ser3.ChartType = SeriesChartType.StackedColumn;
            ser3.Color = System.Drawing.Color.Tomato;
            ser3.Name = "2nd Shift";
            ser3.BorderWidth = 2;
            double xRange = 0;
            if (gridSource.Rows.Count>0)
                 xRange = ((DateTime)gridSource.Rows[gridSource.Rows.Count - 1][0]-(DateTime)gridSource.Rows[0][0]  ).TotalDays;
            decimal numberOfPoints = gridSource.Rows.Count;
            ChartArea area = new ChartArea("area");
            area.AxisX.IsLabelAutoFit = true;
            area.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep45;
            area.AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Days;
            area.AxisX.LabelStyle.Enabled = true;
            area.AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 12);

            area.AxisX.LabelStyle.Interval = (double)Math.Ceiling(numberOfPoints / 20);

            area.AxisX.MajorGrid.Interval = xRange / 10;
            //area.AxisY.Interval = 500;
            area.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            area.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            //area.AxisX.LabelStyle.Interval = (area.AxisX.Maximum - area.AxisX.Minimum) / 50;


            //area.AxisX.LabelStyle.Interval = range / 40;

            


            //DestinationChart.Legends.Add(leg);
            DestinationChart.Series.Add(ser1);
            DestinationChart.Series.Add(ser2);
            DestinationChart.Series.Add(ser3);
            DestinationChart.ChartAreas.Add(area);
            DestinationChart.Legends[0].Docking = Docking.Top;
            DestinationChart.Legends[0].IsDockedInsideChartArea = true;
            DestinationChart.Legends[0].DockedToChartArea = "area";


            foreach (DataRow row in gridSource.Rows)
            {
                DestinationChart.Series[0].Points
                    .AddXY(((DateTime)row[0]).ToString("dd-MM"), (int)row[1]);

                DestinationChart.Series[1].Points
                    .AddXY(((DateTime)row[0]).ToString("dd-MM"), (int)row[2]);

                DestinationChart.Series[2].Points
                    .AddXY(((DateTime)row[0]).ToString("dd-MM"), (int)row[3]);

                /*DestinationChart.Series[1].Points
                    .AddXY(((DateTime)row[0]).ToString("dd-MM"), (int)row[2]);

                DestinationChart.Series[2].Points
                    .AddXY(((DateTime)row[0]).ToString("dd-MM"), (int)row[3]);*/
            }
        }



        private IEnumerable<Tuple<DateTime, int>> LotToTesterDates(Lot lot)
        {
            var dates = lot.LedsInLot.SelectMany(l => l.TesterData.Select(testerData => testerData.FixedDateTime)).Where(form1.DateFilter);
            return dates.Select(d => new Tuple<DateTime, int>(d, 1));
        }

        private IEnumerable<Tuple<DateTime, int>> LotToSplittingDates(Lot lot)
        {
            if (!form1.WasteInfoBySplittingTime(lot.WasteInfo))
            {
                yield break;
            }

            yield return Tuple.Create(DateUtilities.FixedShiftDate(lot.WasteInfo.SplittingDate), lot.LedsInLot.Count);
        }

        private IEnumerable<Tuple<DateTime, int>> LotToBoxingDates(Lot lot)
        {
            var dates = lot.LedsInLot
                .Select(l => l.Boxing.BoxingDate)
                .Where(bd => bd.HasValue)
                .Select(bdOpt => bdOpt.Value)
                .Where(form1.DateFilter)
                .Select(DateUtilities.FixedShiftDate);
            return dates.Select(d => new Tuple<DateTime, int>(d, 1));
        }

    }
}
