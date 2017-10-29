using System;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
using System.Globalization;
using PomocDoRaprtow.Tabs;
using PomocDoRaprtow.DataModels;
using System.Threading;
using System.Drawing;
using System.Reflection;
using System.IO;
using static PomocDoRaprtow.OccurenceCalculations;

namespace PomocDoRaprtow
{
    public partial class Form1 : Form
    {
        private LedStorage ledStorage;
        private WasteOperations wasteOperations;
        private LotInfoOperations lotInfoOperations;
        private LotsInUseOperations lotInUseOperations;
        private CapabilityOperation capabilityOperations;
        private ModelOperations modelOperations;


        public Form1()
        {
            InitializeComponent();
            dateTimePickerBegin.Value = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);
            wasteOperations = new WasteOperations(this, treeViewWaste, dataGridViewWaste, chart_odpad, radioModel, dataGridViewScrap);
            lotInfoOperations = new LotInfoOperations(this, treeViewLotInfo, textBoxFilterLotInfo, dataGridViewLotInfo);
            lotInUseOperations = new LotsInUseOperations(this, treeViewLotsinUse);
            capabilityOperations = new CapabilityOperation(this, treeViewTestCapa, richTextBoxCapaTest, chartCapaTest,  chartSplitting,  treeViewSplitting, treeViewCapaBoxing, chartCapaBoxing,radioModel);
            modelOperations = new ModelOperations(this, chartModel, dataGridViewModelInfo, treeViewModelInfo, comboBoxModels, dateTimePickerBegin, dateTimePickerEnd);

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            wasteOperations.RedrawWasteTab();
            capabilityOperations.DrawCapability();
        }

        private void button4_Click(object sender, EventArgs e)
        {
#if DEBUG
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

#endif


            SqlTableLoader.LoadTesterWorkCard("2017");
#if DEBUG
            Debug.WriteLine("SQL LoadTesterWorkCard: " + stopwatch.Elapsed.Seconds);

#endif
            SqlTableLoader.LoadWasteTable("2017-10");
#if DEBUG
            Debug.WriteLine("SQL LoadWasteTable: " + stopwatch.Elapsed.Seconds);


#endif
            SqlTableLoader.LoadBoxingTable("2017-10");

#if DEBUG
            Debug.WriteLine("SQL LoadBoxingTable: " + stopwatch.Elapsed.Seconds);
            stopwatch.Stop();

#endif
            SqlTableLoader.LoadLotTable("nic");
        }

        private void CheckBoxCheckAll()
        {
            for (int i = 0; i < CapaModelcheckedListBox.Items.Count; i++)
            {
                CapaModelcheckedListBox.SetItemChecked(i, true);
            }
        }

        private void CheckBoxCheckNone()
        {
            for (int i = 0; i < CapaModelcheckedListBox.Items.Count; i++)
            {
                CapaModelcheckedListBox.SetItemChecked(i, false);
            }
        }
        bool CsvLoadinDone = false;
        private void button5_Click(object sender, EventArgs e)
        {
            CsvLoadinDone = false;
            timerThreadCsv.Enabled = true;
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                ledStorage = new LedStorageLoader().BuildStorage();
                wasteOperations.LedStorage = ledStorage;
                lotInfoOperations.LedStorage = ledStorage;
                lotInUseOperations.LedStorage = ledStorage;
                capabilityOperations.LedStorage = ledStorage;
                modelOperations.ledStorage = ledStorage;
                CsvLoadinDone = true;
                
                
            }).Start();

            pictureBox1.Visible = true;
            pictureBox1.Image = PomocDoRaprtow.Properties.Resources.spinner2;
            pictureBox1.Size = PomocDoRaprtow.Properties.Resources.spinner2.Size;
        }
        

        public System.Drawing.Image GetImageFromManifest(string sPath)
        {
            // Ready the return
            System.Drawing.Image oImage = null;

            try
            {
                // Get the assembly
                Assembly oAssembly = Assembly.GetAssembly(this.GetType());

                string[] names = oAssembly.GetManifestResourceNames();

                // Get the stream
                Stream oStream = oAssembly.GetManifestResourceStream(sPath);

                // Read from the stream
                oImage = System.Drawing.Image.FromStream(oStream);

            }
            catch (Exception)
            {
                // Missing image?           
            }

            //Return the image
            return oImage;
        }

        private HashSet<String> enabledModels = new HashSet<string>();

        public bool LedModelIsSelected(Led led)
        {
            return ModelSelected(led.Lot.Model);
        }

        public bool WasteInfoBySplittingTime(WasteInfo wasteInfo)
        {
            return wasteInfo != null && DateFilter(wasteInfo.SplittingDate);
        }

        public bool LotBySplittingTime(Lot lot)
        {
            return WasteInfoBySplittingTime(lot.WasteInfo);
        }

        public bool LotByModel(Lot lot)
        {
            return ModelSelected(lot.Model);
        }

        public bool ModelSelected(Model model)
        {
            return enabledModels.Contains(model.ModelName);
        }

        public IEnumerable<Led> FilterLedsBySelectedModels()
        {
            return ledStorage.SerialNumbersToLed.Values.Where(LedModelIsSelected);
        }

        public IEnumerable<Lot> FilterLotsBySelecteModels()
        {
            return ledStorage.Lots.Values.Where(LotByModel);
        }

        public bool DateFilter(DateTime dt)
        {
            return dt > dateTimePickerBegin.Value &&
            dt < dateTimePickerEnd.Value;
        }

        public IEnumerable<TesterData> TesterDataFilter(IEnumerable<TesterData> testerData)
        {
            return testerData.Where(t => DateFilter(t.TimeOfTest));
        }

        public bool BoxingDateFilter(Boxing boxingData)
        {
            return boxingData.BoxingDate.HasValue && DateFilter(boxingData.BoxingDate.Value); 
        }

        public IEnumerable<ProductionDetail> LotToTesterProductionDetails(Lot lot)
        {
            var testerDatas = lot.LedsInLot
                .SelectMany(l => l.TesterData)
                .Where(td => DateFilter(td.FixedDateTime));

            return testerDatas.Select(td => new ProductionDetail(td.FixedDateTime,td.TimeOfTest, td.TesterId, 1, lot));
        }

        private void dateTimePicker_odpad_od_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker_odpad_do_ValueChanged(object sender, EventArgs e)
        {

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

        private void treeViewWaste_AfterSelect(object sender, TreeViewEventArgs e)
        {
            wasteOperations.TreeViewWasteSelectionChanged();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            lotInfoOperations.FilterLotInfoTreeView();
        }

        private void textBoxFilterLotInfo_TextChanged(object sender, EventArgs e)
        {
            lotInfoOperations.FilterLotInfoTreeView();
        }

        private void treeViewLotInfo_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewLotInfo.SelectedNode.Level > 0) lotInfoOperations.DisplayLotInfo(treeViewLotInfo.SelectedNode.Name, dataGridViewLotInfo);
        }

        private void buttonLotsinUse_Click(object sender, EventArgs e)
        {
            lotInUseOperations.GenerateLotsInUseToTree();
        }

        private void treeViewLotsinUse_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewLotsinUse.SelectedNode.Level > 1) lotInfoOperations.DisplayLotInfo(treeViewLotsinUse.SelectedNode.Name, dataGridViewLotsInUse);
        }

        private string modelFamily(string model)
        {
            if (model.Length < 10) return "";
            return model.Substring(0, 6) + model[7] + model[9];

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lots = ledStorage.Lots.Values.Where(l => l.Model.ModelName.Substring(0,3)== comboBoxModels.Text).ToList();
            //modelOperations.GenerateCycleTimeChart(ledStorage.Models[comboBoxModels.Text]);
            modelOperations.GenerateLotEfficiencyChart(lots);
            modelOperations.DisplayTesterDataOccurences(ledStorage.Leds, lots);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckBoxCheckAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CheckBoxCheckNone();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CapaModelcheckedListBox.Items.Count; i++)
            {
                string itText = CapaModelcheckedListBox.GetItemText(CapaModelcheckedListBox.Items[i]);
                if (itText.Contains("22-") || itText.Contains("32-") || itText.Contains("33-") || itText.Contains("53-"))
                CapaModelcheckedListBox.SetItemChecked(i, true);
                else CapaModelcheckedListBox.SetItemChecked(i, false);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CapaModelcheckedListBox.Items.Count; i++)
            {
                string itText = CapaModelcheckedListBox.GetItemText(CapaModelcheckedListBox.Items[i]);
                if (itText.Contains("K1-") || itText.Contains("K2-") || itText.Contains("61-") || itText.Contains("41-"))
                    CapaModelcheckedListBox.SetItemChecked(i, true);
                else CapaModelcheckedListBox.SetItemChecked(i, false);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CapaModelcheckedListBox.Items.Count; i++)
            {
                string itText = CapaModelcheckedListBox.GetItemText(CapaModelcheckedListBox.Items[i]);
                if (itText.Contains("G1-") || itText.Contains("G2-") || itText.Contains("31-"))
                    CapaModelcheckedListBox.SetItemChecked(i, true);
                else CapaModelcheckedListBox.SetItemChecked(i, false);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CapaModelcheckedListBox.Items.Count; i++)
            {
                string itText = CapaModelcheckedListBox.GetItemText(CapaModelcheckedListBox.Items[i]);
                if (itText.Contains("E1-") || itText.Contains("E2-") || itText.Contains("D1-") || itText.Contains("D2-"))
                    CapaModelcheckedListBox.SetItemChecked(i, true);
                else CapaModelcheckedListBox.SetItemChecked(i, false);
            }
        }

        private void checkedListBox1_MouseEnter(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 450;
        }

        private void checkedListBox1_MouseLeave(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 50;
        }

        private void button7_MouseEnter(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 450;
        }

        private void button7_MouseLeave(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 50;
        }

        private void button8_MouseEnter(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 450;
        }

        private void button9_MouseEnter(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 450;
        }

        private void button10_MouseEnter(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 450;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 450;
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 450;
        }

        private void button8_MouseLeave(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 50;
        }

        private void button9_MouseLeave(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 50;
        }

        private void button10_MouseLeave(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 50;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 50;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            CapaModelcheckedListBox.Height = 50;
        }

        private void timerThreadCsv_Tick(object sender, EventArgs e)
        {
            if (CsvLoadinDone)
            {
                pictureBox1.Visible = false;
                timerThreadCsv.Enabled = false;
                CapaModelcheckedListBox.Items.Clear();
                CapaModelcheckedListBox.Sorted = true;
                var modelList = ledStorage.Models.Select(m => m.Value.ModelName).ToList().OrderBy(o => o);
                foreach (var model in modelList)
                {
                    CapaModelcheckedListBox.Items.Add(model);
                    CapaModelcheckedListBox.SetItemChecked(CapaModelcheckedListBox.Items.Count - 1, true);
                    string familyName = model.Substring(0, 3);
                    if (!comboBoxModels.Items.Contains(familyName)) comboBoxModels.Items.Add(familyName);
                }

                CheckBoxCheckAll();
                RebuildEnabledModelsSet();
                button3.Enabled = true;
                button3.PerformClick();
                
            }
        }

        private void Tab_SizeChanged(object sender, EventArgs e)
        {
            //tabMain.Size = new Size(tabMain.Width, this.Height - 95);
        }

        private void radioModel_CheckedChanged(object sender, EventArgs e)
        {
            wasteOperations.RedrawWasteTab();
        }

        private void CapaModelcheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            RebuildEnabledModelsSet();
        }



        private void treeViewTestCapa_AfterSelect(object sender, TreeViewEventArgs e)
        {
            richTextBox1.Clear();
            Dictionary<Lot, List<ProductionDetail>> lots = new Dictionary<Lot, List<ProductionDetail>>();
            List<OccurenceModel> models = new List<OccurenceModel>();
            if (e.Node.Level > 0)
            {
                var tag = e.Node.Tag;

                if (tag as OccurenceDay != null)
                {
                    OccurenceDay occurence = (OccurenceDay)e.Node.Tag;
                    models = occurence.ShiftToTree.SelectMany(st => st.ModelToTree.Values).ToList();
                }
                else if (tag as OccurenceShift != null)
                {
                    OccurenceShift occurence = (OccurenceShift)e.Node.Tag;
                    models = occurence.ModelToTree.Values.ToList();
                }
                else if (tag as OccurenceModel != null)
                {
                    OccurenceModel occurence = (OccurenceModel)e.Node.Tag;
                    models.Add(occurence);
                }

                foreach (var model in models)
                {
                    foreach (var lotDetailsEntry in model.ProductionDetails)
                    {
                        if (!lots.ContainsKey(lotDetailsEntry.Key))
                        {
                            lots.Add(lotDetailsEntry.Key, new List<ProductionDetail>());
                        }
                        lots[lotDetailsEntry.Key].AddRange(lotDetailsEntry.Value);
                    }
                }

                List<Tuple<DateTime, String>> lotsDateInfo = new List<Tuple<DateTime, string>>();
                foreach (var entry in lots)
                {
                    String lotInfo = "";
                    entry.Value.Sort((lhs, rhs) => lhs.ProductionRealDate.CompareTo(rhs.ProductionRealDate));
                    var productionIds = entry.Value.Select(pd => pd.ProductionLineId).Distinct();
                    String productionIdText = String.Join(", ", productionIds);
                   

                    lotInfo += entry.Key.LotId + " - " + entry.Value.Count + " by: " + productionIdText +  '\n';

                    var startTime = entry.Value.First().ProductionRealDate;
                    var endTime = entry.Value.Last().ProductionRealDate;

                    lotInfo +=
                        "  " + startTime.ToShortTimeString() + " - " + endTime.ToShortTimeString() + ' ' + entry.Key.Model.ModelName + '\n';
                    lotsDateInfo.Add(Tuple.Create(startTime, lotInfo));
                }

                lotsDateInfo.Sort((lhs, rhs) => lhs.Item1.CompareTo(rhs.Item1));

                foreach (var entry in lotsDateInfo)
                {
                    richTextBox1.Text += entry.Item2;
                    listView1.Items.Add(entry.Item2);
                }
            }
        }

        string allNodes = "";
        public void PrintNodesRecursive(TreeNode oParentNode)
        {
            
            string result = "";
            //Console.WriteLine(oParentNode.Text);
            if (oParentNode.Level == 4)
            {
                string week = oParentNode.Parent.Parent.Parent.Parent.Text.Split(' ')[0];
                string day = oParentNode.Parent.Parent.Parent.Text.Split(' ')[0];
                string shift = oParentNode.Parent.Parent.Text.Split(' ')[0];
                string model = oParentNode.Parent.Text.Split(' ')[0];
                string testerID = oParentNode.Text.Split(' ')[0];
                string qty = oParentNode.Parent.Text.Split(' ')[1];

                result = week + "\t" + day + "\t" + shift + "\t" + model + "\t" + testerID + "\t" + qty;
                allNodes += result + "\n";
            }
            

            foreach (TreeNode oSubNode in oParentNode.Nodes)
            {
                PrintNodesRecursive(oSubNode);
            }
        }
        private void treeViewTestCapa_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button== MouseButtons.Right)
            {
                allNodes = "week" + "\t" + "day" + "\t" + "shift" + "\t" + "model" + "\t" + "testerID" + "\t" + "qty" + "\n";
                foreach (TreeNode node in treeViewTestCapa.Nodes)
                {
                    PrintNodesRecursive(node);
                }

                Clipboard.Clear();    //Clear if any old value is there in Clipboard        
                Clipboard.SetText(allNodes); //Copy text to Clipboard
        
            }
        }
    }
}