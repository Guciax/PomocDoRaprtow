using PomocDoRaprtow.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static PomocDoRaprtow.OccurenceCalculations;

namespace PomocDoRaprtow.Tabs
{
    public class CapabilityOperation
    {
        public LedStorage LedStorage { get; set; }
        private readonly Form1 form1;
        private readonly TreeView treeViewTestCapa;
        private readonly Chart chartCapaTest;
        private readonly Chart chartSplitting;
        private readonly TreeView treeViewSplitting;
        private readonly TreeView treeViewCapaBoxing;
        private readonly Chart chartCapaBoxing;
        private readonly RadioButton radioModel;
        private readonly ListView listViewTestYield;

        public CapabilityOperation(Form1 form1, TreeView treeViewTestCapa ,
            Chart chartCapaTest, 
            Chart chartSplitting,  TreeView treeViewSplitting, TreeView treeViewCapaBoxing, 
             Chart chartCapaBoxing, RadioButton radioModel, ListView listViewTestYield)
        {
            this.form1 = form1;
            this.treeViewTestCapa = treeViewTestCapa;
            this.chartCapaTest = chartCapaTest;
            this.chartSplitting = chartSplitting;
            this.treeViewSplitting = treeViewSplitting;
            this.treeViewCapaBoxing = treeViewCapaBoxing;
            this.chartCapaBoxing = chartCapaBoxing;
            this.radioModel = radioModel;
            this.listViewTestYield = listViewTestYield;
        }

        private string FormatTreeViewNodeName(String mainName, int occurences)
        {
            return mainName + " " + occurences;
        }

        private void RebuildOccurenceTreeView(TreeView targetTreeView, OccurenceCalculations occurenceCalculations, bool includeProductionIdLevel)
        {
            targetTreeView.BeginUpdate();
            targetTreeView.Nodes.Clear();
            foreach (var weekTree in occurenceCalculations.Tree.WeekNoToTree.Values)
            {
                if (weekTree.Occurences == 0) continue;

                var weekTreeViewNode =
                    new TreeNode(FormatTreeViewNodeName(weekTree.Week.ToString(), weekTree.Occurences));
                targetTreeView.Nodes.Add(weekTreeViewNode);
                targetTreeView.Tag = weekTree;
                foreach (var dayTree in weekTree.DayToTree.Values)
                {
                    string dayMonth = dayTree.DateTime.Day.ToString("d2") + "-" + dayTree.DateTime.Month.ToString("d2")+ "-" + dayTree.DateTime.Year.ToString();
                    var dayTreeViewNode = weekTreeViewNode.Nodes.Add(FormatTreeViewNodeName(dayMonth, dayTree.Occurences));
                    dayTreeViewNode.Tag = dayTree;
                    foreach (var shiftTree in dayTree.ShiftToTree)
                    {
                        if (shiftTree.Occurences == 0) continue;
                        var shiftTreeViewNode = dayTreeViewNode.Nodes.Add(FormatTreeViewNodeName(shiftTree.ShiftNo.ToString(), shiftTree.Occurences));
                        shiftTreeViewNode.Tag = shiftTree;
                        if (includeProductionIdLevel)
                        {
                            var productionDetails = shiftTree.ModelToTree.Values.SelectMany(occModel => occModel.ProductionDetails).SelectMany(lotToProdDetail => lotToProdDetail.Value).ToList();
                            var prodIds = productionDetails.Select(pd => pd.ProductionLineId).Distinct();
                            foreach(var prodId in prodIds)
                            {
                                var productionDetailsForThisProdId = productionDetails.Where(pd => pd.ProductionLineId == prodId).ToList();
                                var prodIdTreeViewNode = shiftTreeViewNode.Nodes.Add(FormatTreeViewNodeName(prodId, productionDetailsForThisProdId.Sum(pd => pd.ProducedAmount)));
                                //tag?

                                prodIdTreeViewNode.Tag = BuildOccurenceModelFromProductionDetails(productionDetailsForThisProdId.ToList());

                                var productionDetailsGroupped = productionDetailsForThisProdId.GroupBy(pd => pd.ProducedIn.Model.ModelName);

                                foreach (var modelNameToDetails in productionDetailsGroupped)
                                {
                                    var modelTree = BuildOccurenceModelFromProductionDetails(modelNameToDetails.ToList());

                                    var modelTreeViewNode = prodIdTreeViewNode.Nodes.Add(FormatTreeViewNodeName(modelTree.Model, modelTree.Occurences));
                                    modelTreeViewNode.Tag = modelTree;
                                }
                            }
                        }
                        else
                        {
                            foreach (var modelTree in shiftTree.ModelToTree.Values)
                            {
                                if (modelTree.Occurences == 0) continue;
                                var modelTreeViewNode = shiftTreeViewNode.Nodes.Add(FormatTreeViewNodeName(modelTree.Model, modelTree.Occurences));
                                modelTreeViewNode.Tag = modelTree;
                            }
                        }
                    }
                }
            }
            targetTreeView.EndUpdate();
        }

        private OccurenceModel BuildOccurenceModelFromProductionDetails(List<ProductionDetail> prodDetails)
        {
            var lot = prodDetails.First().ProducedIn;
            OccurenceModel modelTree = new OccurenceModel();
            modelTree.Model = lot.Model.ModelName;

            var lots = prodDetails.Select(pd => pd.ProducedIn).Distinct();

            foreach (var l in lots)
            {
                var prodDetailsForThisL = prodDetails.Where(pd => pd.ProducedIn == l).ToList();
                modelTree.ProductionDetails.Add(l, prodDetailsForThisL);
            }

            modelTree.Occurences = prodDetails.Sum(pd => pd.ProducedAmount);

            return modelTree;
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
            var occurences = new OccurenceCalculations(lots, LotToSplittingProdDetails);
            RebuildOccurenceTreeView(treeViewSplitting, occurences, false);
        }

        private void DisplayBoxingDataOccurences(List<Lot> lots)
        {

            var occurences = new OccurenceCalculations(lots, LotToBoxingProductionDetails);
            RebuildOccurenceTreeView(treeViewCapaBoxing, occurences, false);
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
            var occurencesCalculations = new OccurenceCalculations(lots, form1.LotToTesterProductionDetails);
            RebuildOccurenceTreeView(treeViewTestCapa, occurencesCalculations, true);
            listViewTestYield.Items.Clear();

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
                if (kvp.Value < 10) break;

                string[] itmArr = new string[3];
                itmArr[0]= kvp.Key.ToString();
                itmArr[1] = kvp.Value.ToString();
                itmArr[2] = MathUtilities.CalculatePercentage(occurencesSum, kvp.Value).ToString();

                var itm = new ListViewItem(itmArr);
                listViewTestYield.Items.Add(itm);
            }

            foreach (ColumnHeader col in listViewTestYield.Columns)
            {
                col.TextAlign = HorizontalAlignment.Center;
                col.Width = -2;
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

        private IEnumerable<ProductionDetail> LotToSplittingProdDetails(Lot lot)
        {
            if (!form1.WasteInfoBySplittingTime(lot.WasteInfo))
            {
                yield break;
            }

            yield return new ProductionDetail(DateUtilities.FixedShiftDate(lot.WasteInfo.SplittingDate), lot.WasteInfo.SplittingDate, WasteInfo.SplitterId, lot.LedsInLot.Count, lot);
        }

        private IEnumerable<ProductionDetail> LotToBoxingProductionDetails(Lot lot)
        {
            var dates = lot.LedsInLot
                .Select(l => l.Boxing.BoxingDate)
                .Where(bd => bd.HasValue)
                .Select(bdOpt => bdOpt.Value)
                .Where(form1.DateFilter);
            return dates.Select(d => new ProductionDetail(DateUtilities.FixedShiftDate(d), d, Boxing.BoxerId, 1, lot));
        }

    }
}
