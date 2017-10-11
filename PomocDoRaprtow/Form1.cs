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
        private CapabilityOperation capabilityOperations;

        public Form1()
        {
            InitializeComponent();
            wasteOperations = new WasteOperations(this, treeViewWaste, dataGridViewWaste, chart_odpad);
            lotInfoOperations = new LotInfoOperations(this, treeViewLotInfo, textBoxFilterLotInfo, dataGridViewLotInfo);
            lotInUseOperations = new LotsInUseOperations(this, treeViewLotsinUse);
            capabilityOperations = new CapabilityOperation(this, treeViewTestCapa, richTextBoxCapaTest, chartCapaTest, dataGridViewCapaTest, chartSplitting, dataGridViewSplitting);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            wasteOperations.RedrawWasteTab();

            capabilityOperations.DrawCapability(FilterLeds().ToList(), FilterLots().ToList());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Tester_table = SqlTableLoader.LoadTesterWorkCard();
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

        public bool LotByModel(Lot lot)
        {
            return ModelSelected(lot.Model);
        }

        public bool ModelSelected(Model model)
        {
            return enabledModels.Contains(model.ModelName);
        }

        public IEnumerable<Led> FilterLeds()
        {
            return ledStorage.SerialNumbersToLed.Values.Where(PassesFilter);
        }

        public IEnumerable<Lot> FilterLots()
        {
            return ledStorage.Lots.Values.Where(LotByModel).Where(LotBySplittingTime);
        }

        public IEnumerable<TesterData> TesterDataFilter(List<TesterData> testerData)
        {
            return testerData.Where(t =>
            t.TimeOfTest > dateTimePickerBegin.Value &&
            t.TimeOfTest < dateTimePickerEnd.Value);
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
            if (treeViewLotInfo.SelectedNode.Level > 0) lotInfoOperations.ShowLotInfo();
        }

        private void buttonLotsinUse_Click(object sender, EventArgs e)
        {
            lotInUseOperations.GenerateLotsInUseToTree();
        }
    }
}