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
            wasteOperations = new WasteOperations(this, treeViewWaste, dataGridViewWaste, chart_odpad);
            lotInfoOperations = new LotInfoOperations(this, treeViewLotInfo, textBoxFilterLotInfo, dataGridViewLotInfo);
            lotInUseOperations = new LotsInUseOperations(this, treeViewLotsinUse);
            capabilityOperations = new CapabilityOperation(this, treeViewTestCapa, richTextBoxCapaTest, chartCapaTest, dataGridViewCapaTest, chartSplitting, dataGridViewSplitting, treeViewSplitting, treeViewCapaBoxing,dataGridViewCapaBoxing, chartCapaBoxing);
            modelOperations = new ModelOperations(this, chartModel, dataGridViewModelInfo);
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
            //Tester_table = SqlTableLoader.LoadTesterWorkCard();
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
            /*
            PictureBox loadingBox = new PictureBox();
            loadingBox.Parent = this;
            loadingBox.BringToFront();
            loadingBox.Location = new System.Drawing.Point(150, 150);
            Image pacman = Image.FromFile("Pacman.gif");
            loadingBox.Size = pacman.Size;
            loadingBox.Image = pacman;*/


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
            catch (Exception ex)
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            modelOperations.GenerateCycleTimeChart(ledStorage.Models[comboBoxModels.Text]);
            modelOperations.GenerateLotEfficiencyChart(ledStorage.Models[comboBoxModels.Text]);
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
                timerThreadCsv.Enabled = false;
                CapaModelcheckedListBox.Items.Clear();
                CapaModelcheckedListBox.Sorted = true;
                var modelList = ledStorage.Models.Select(m => m.Value.ModelName).ToList().OrderBy(o => o);
                foreach (var model in modelList)
                {
                    CapaModelcheckedListBox.Items.Add(model);
                    CapaModelcheckedListBox.SetItemChecked(CapaModelcheckedListBox.Items.Count - 1, true);
                    comboBoxModels.Items.Add(model);
                }

                CheckBoxCheckAll();
                RebuildEnabledModelsSet();
                button3.Enabled = true;
                button3.PerformClick();
                
            }
        }
    }
}