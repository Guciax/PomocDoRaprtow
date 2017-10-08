using System;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
using System.Globalization;
using PomocDoRaprtow.Tabs;

namespace PomocDoRaprtow
{
    public partial class Form1 : Form
    {
        private LedStorage ledStorage;
        private WasteOperations wasteOperations;
        private LotInfoOperations lotInfoOperations;
        private LotsInUseOperations lotInUseOperations;

        public Form1()
        {
            InitializeComponent();
            
            wasteOperations = new WasteOperations(this, treeViewWaste, dataGridViewWaste, chart_odpad);
            lotInfoOperations = new LotInfoOperations(this, treeViewLotInfo, textBoxFilterLotInfo, dataGridViewLotInfo);
            lotInUseOperations = new LotsInUseOperations(this, treeViewLotsinUse);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
        }

        DataTable Tester_table = new DataTable();
        static DataTable LOT_Module_Short = new DataTable();

        private void button1_Click(object sender, EventArgs e)
        {

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
            Tester_table = SqlTableLoader.LoadTesterWorkCard();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ledStorage = new LedStorageLoader().BuildStorage();
            wasteOperations.LedStorage = ledStorage;
            lotInfoOperations.LedStorage = ledStorage;
            lotInUseOperations.LedStorage = ledStorage;
            CapaModelcheckedListBox.Items.Clear();
            foreach (var model in ledStorage.Models.Values)
            {
                CapaModelcheckedListBox.Items.Add(model.ModelName);
                CapaModelcheckedListBox.SetItemChecked(CapaModelcheckedListBox.Items.Count - 1, true);
            }
            
            RebuildEnabledModelsSet();
        }


        private void DrawCapaChart(Chart DestinationChart, DataGridView DestinationGrid)
        {
            DataTable GridSource = new DataTable();
            GridSource.Columns.Add("Day");
            GridSource.Columns.Add("Shift III", typeof(int));
            GridSource.Columns.Add("Shift I", typeof(int));
            GridSource.Columns.Add("Shift II", typeof(int));

            List<int> initializedDays = new List<int>();

            var leds = FilterLeds().ToList();
            var occurencesCalculations = new OccurenceCalculations(leds,TesterDataFilter);
            foreach (var led in leds)
            {
                foreach (var testerData in TesterDataFilter(led.TesterData))
                {
                    DateUtilities.ShiftInfo shiftInfo = DateUtilities.DateToShiftInfo(testerData.TimeOfTest);
                    string dayMonth = shiftInfo.DayOfTheMonth.ToString("d2") + "-" + shiftInfo.Month.ToString("d2");
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
            RebuildOccurenceTreeView(occurencesCalculations);
            DataView dv = GridSource.DefaultView;
            dv.Sort = "Day asc";
            GridSource = dv.ToTable();

            richTextBox1.Text = "";
            foreach (KeyValuePair<int, int> kvp in occurencesCalculations.CountOccurences)
            {
                richTextBox1.AppendText($"{kvp.Key} test: {kvp.Value} modułów - " + "\r");
            }

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

            foreach (DataRow row in GridSource.Rows)
            {
                DestinationChart.Series[0].Points
                    .AddXY(row[0].ToString(), (int)row[1]);

                DestinationChart.Series[1].Points
                    .AddXY(row[0].ToString(), (int)row[2]);

                DestinationChart.Series[2].Points
                    .AddXY(row[0].ToString(), (int)row[3]);
            }
        }

        private string FormatTreeViewNodeName(String mainName, int occurences)
        {
            return mainName + " " + occurences;
        }

        private void RebuildOccurenceTreeView(OccurenceCalculations occurenceCalculations)
        {
            treeViewCapa.BeginUpdate();
            treeViewCapa.Nodes.Clear();
            foreach (var weekTree in occurenceCalculations.Tree.WeekNoToTree.Values)
            {
                var weekTreeViewNode =
                    new TreeNode(FormatTreeViewNodeName(weekTree.Week.ToString(), weekTree.Occurences));
                treeViewCapa.Nodes.Add(weekTreeViewNode);
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
            treeViewCapa.EndUpdate();
        }

        private HashSet<String> enabledModels = new HashSet<string>();

        private bool PassesFilter(Led led)
        {
            return ModelSelected(led.Lot.Model);
        }

        public bool WasteInfoBySplittingTime(WasteInfo wasteInfo)
        {
            return wasteInfo != null && wasteInfo.SplittingDate > dateTimePickerBegin.Value &&
            wasteInfo.SplittingDate < dateTimePickerEnd.Value;
        }

        public bool LotBySplittingTime(Lot lot)
        {
            return WasteInfoBySplittingTime(lot.WasteInfo);
        }
        public bool ModelSelected(Model model)
        {
            return enabledModels.Contains(model.ModelName);
        }
        public IEnumerable<Led> FilterLeds()
        {
            return ledStorage.SerialNumbersToLed.Values.Where(PassesFilter);
        }

        public IEnumerable<TesterData> TesterDataFilter(List<TesterData> testerData)
        {
            return testerData.Where(t =>
            t.TimeOfTest > dateTimePickerBegin.Value &&
            t.TimeOfTest < dateTimePickerEnd.Value);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*if (Tab.SelectedTab.Name == "tab_Waste")
             {
                 DrawWasteHistogram();
             }
             if (Tab.SelectedTab.Name == "tab_Capacity")
             {
                 DrawCapaChart(chart_Capacity_Test, dataGridView_Capacity_Test);
             }*/
        }

        private void RefreshTreeViewWasteNodes()
        {
        } 

        private void dateTimePicker_odpad_od_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker_odpad_do_ValueChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_MouseEnter(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 200;
        }

        private void checkedListBox1_MouseLeave(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 50;
        }
        private void CapaModelcheckedListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            RebuildEnabledModelsSet();
        }


        private void RebuildEnabledModelsSet()
        {
            enabledModels.Clear();
            foreach (var model in CapaModelcheckedListBox.CheckedItems)
            {
                enabledModels.Add(model as String);
            }   
        }

        private void dateTimePicker_wyd_od_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker_wyd_do_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            wasteOperations.RedrawWasteTab();
            DrawCapaChart(chart_Capacity_Test, dataGridView_Capacity_Test);
        }

        private void treeViewWaste_AfterSelect(object sender, TreeViewEventArgs e)
        {
            wasteOperations.TreeViewWasteSelectionChanged();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //lotInfoOperations.BuildModelLotInfoDictionary();
            lotInfoOperations.FilterLotInfoTreeView();
        }

        private void textBoxFilterLotInfo_TextChanged(object sender, EventArgs e)
        {
            lotInfoOperations.FilterLotInfoTreeView();
        }

        private void treeViewLotInfo_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewLotInfo.SelectedNode.Level > 0) lotInfoOperations.ShowLotInfo();
        }

        private void buttonLotsinUse_Click(object sender, EventArgs e)
        {
            lotInUseOperations.GenerateLotsInUseToTree();
        }
    }
}