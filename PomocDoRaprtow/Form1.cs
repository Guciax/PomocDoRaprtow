using System;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
using System.Globalization;

namespace PomocDoRaprtow
{
    public partial class Form1 : Form
    {
        private readonly OptionProvider optionProvider;
        private LedStorage leds;

        public Form1()
        {
            InitializeComponent();


            optionProvider = new OptionProvider(this);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
        }

        DataTable Odpady_table = new DataTable();
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
            Odpady_table = SqlTableLoader.LoadWasteTable();
            Tester_table = SqlTableLoader.LoadTesterWorkCard();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            leds = new LedStorageLoader().BuildStorage();
            CapaModelcheckedListBox.Items.Clear();
            foreach (var model in leds.Models)
            {
                CapaModelcheckedListBox.Items.Add(model);
                CapaModelcheckedListBox.SetItemChecked(CapaModelcheckedListBox.Items.Count - 1, true);
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

            var leds = FilterLeds().ToList();
            var occurencesCalculations = new OccurenceCalculations(leds);
            foreach (var led in leds)
            {
                foreach (var testerData in led.TesterData)
                {
                    if (testerData.TimeOfTest > dateTimePickerBegin.Value &&
                        testerData.TimeOfTest < dateTimePickerEnd.Value)
                    {
                        DateUtilities.ShiftInfo shiftInfo = DateUtilities.DateToShiftInfo(testerData.TimeOfTest);
                        string dayMonth = shiftInfo.DayOfTheMonth.ToString("d2") + "-" + shiftInfo.Month.ToString("d2");
                        if (!initializedDays.Contains(shiftInfo.DayOfTheMonth))
                        {
                            initializedDays.Add(shiftInfo.DayOfTheMonth);
                            GridSource.Rows.Add(dayMonth, 0, 0, 0);
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
            }
            RebuildOccurenceTreeView(occurencesCalculations);
            DataView dv = GridSource.DefaultView;
            dv.Sort = "Day asc";
            GridSource = dv.ToTable();

            richTextBox1.Text = "";
            foreach (KeyValuePair<int, int> kvp in occurencesCalculations.CountOccurences)
            {
                richTextBox1.AppendText($"Key = {kvp.Key}, Value = {kvp.Value}" + "\r");
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
                    .AddXY(row[0].ToString(), (int) row[1]);

                DestinationChart.Series[1].Points
                    .AddXY(row[0].ToString(), (int) row[2]);

                DestinationChart.Series[2].Points
                    .AddXY(row[0].ToString(), (int) row[3]);
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
                    foreach (var shiftTree in dayTree.ShiftToTree.Values)
                    {
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
            return enabledModels.Contains(led.Lot.Model);
        }
        private IEnumerable<Led> FilterLeds()
        {
            enabledModels.Clear();
            foreach(var model in CapaModelcheckedListBox.CheckedItems)
            {
                enabledModels.Add(model as String);
            }
            return leds.SerialNumbersToLed.Values.Where(PassesFilter);
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
        Dictionary<string, int[]> wastePerModel = new Dictionary<string, int[]>();
        private void buildWastePerModelDict()
        {
            wastePerModel.Clear();
            wastePerModel.Add("Total", new int[WasteInfo.WasteFieldNames.Length]);


            foreach (var lot in leds.Lots)
            {
                if (lot.Value.WasteInfo != null)
                {
                    if (lot.Value.WasteInfo.SplittingDate >= dateTimePickerBegin.Value && lot.Value.WasteInfo.SplittingDate <= dateTimePickerEnd.Value)
                    {
                        string model = lot.Value.Model;
                        if (!wastePerModel.ContainsKey(lot.Value.Model))
                        {
                            wastePerModel.Add(model, new int[WasteInfo.WasteFieldNames.Length]);
                        }

                        for (int i = 0; i < lot.Value.WasteInfo.WasteCounts.Count; i++)
                        {

                            int wasteCount = lot.Value.WasteInfo.WasteCounts[i];
                            wastePerModel[model][i] = wastePerModel[model][i] + wasteCount;
                            wastePerModel["Total"][i] = wastePerModel["Total"][i] + wasteCount;

                        }
                    }
                }
            }
            
        }

        private void refreshTreeViewWasteNodes()
        {
            treeViewWaste.BeginUpdate();
            List<TreeNode> wasteNodes = new List<TreeNode>();
            TreeNode totalNode = new TreeNode("Total" + " " + wastePerModel["Total"].Sum(x => Convert.ToInt32(x)));
            totalNode.Name = "Total";
            treeViewWaste.Nodes.Add(totalNode);
            
            foreach (var model in wastePerModel.Keys.Skip(1))
            {
                int total = wastePerModel[model].Sum(x => Convert.ToInt32(x));
                TreeNode modelNode = new TreeNode(model + " " + total);
                modelNode.Name = model;
                treeViewWaste.Nodes["Total"].Nodes.Add(modelNode);
            }

            treeViewWaste.ExpandAll();
            treeViewWaste.EndUpdate();
            treeViewWaste.SelectedNode = treeViewWaste.Nodes["Total"];
        }

        private void DrawWasteHistogram()
        {
            DataTable hist = new DataTable();
            hist.Columns.Add("Name");
            hist.Columns.Add("Count", typeof(int));

            foreach (var wasteHeader in WasteInfo.WasteFieldNames)
            {
                hist.Rows.Add(wasteHeader, 0);
            }


            foreach (var model in wastePerModel.Keys)
            {
                if (model == treeViewWaste.SelectedNode.Name)
                {
                    for (int i = 0; i < wastePerModel[model].Length; i++) 
                    {
                        hist.Rows[i][1] = (int)wastePerModel[model][i];
                    }
                }
            }
           
            DataView dv = hist.DefaultView;
            dv.Sort = "Count desc";
            hist = dv.ToTable();

            dataGridViewWaste.DataSource = hist;
            Charting.BarChart (chart_odpad, hist, 0, 1);
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

        private void ModelcheckedListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void dateTimePicker_wyd_od_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void dateTimePicker_wyd_do_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DrawCapaChart(chart_Capacity_Test, dataGridView_Capacity_Test);
            buildWastePerModelDict();
            refreshTreeViewWasteNodes();
        }

        private void treeViewWaste_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DrawWasteHistogram();
        }
    }
}