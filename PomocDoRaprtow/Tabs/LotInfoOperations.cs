using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PomocDoRaprtow.Tabs
{
    public class LotInfoOperations
    {
        public LedStorage LedStorage { get; set; }
        private readonly Form1 form;
        private readonly TextBox textBoxFilterLotInfo;
        private readonly TreeView treeViewLotInfo;
        private readonly DataGridView dataGridViewLotInfo;

        public LotInfoOperations(Form1 form, TreeView treeViewLotInfo, TextBox textBoxFilterLotInfo, DataGridView dataGridViewLotInfo)
        {
            this.form = form;
            this.treeViewLotInfo = treeViewLotInfo;
            this.textBoxFilterLotInfo = textBoxFilterLotInfo;
            this.dataGridViewLotInfo = dataGridViewLotInfo;
        }

        public void FilterLotInfoTreeView()
        {
            treeViewLotInfo.BeginUpdate();
            treeViewLotInfo.Nodes.Clear();
            foreach (var model in LedStorage.Models.Values)
            {
                var lots = FilterLots(model.Lots).ToList();
                if (lots.Count == 0) continue;

                TreeNode modelNode = new TreeNode(model.ModelName);
                modelNode.Name = model.ModelName;
                List<TreeNode> lotNodesList = new List<TreeNode>();

                foreach (var lot in lots)
                {
                    TreeNode lotNode = new TreeNode(lot.LotId);
                    lotNode.Name = lot.LotId;
                    lotNodesList.Add(lotNode);
                }

                treeViewLotInfo.Nodes.Add(modelNode);
                foreach (var lotNode in lotNodesList)
                {
                    treeViewLotInfo.Nodes[modelNode.Name].Nodes.Add(lotNode);
                }


            }
            if (textBoxFilterLotInfo.Text.Length > 0) treeViewLotInfo.ExpandAll();
            treeViewLotInfo.EndUpdate();
        }

        private IEnumerable<Lot> FilterLots(List<Lot> lots)
        {
            return lots.Where(l => l.LotId.Contains(textBoxFilterLotInfo.Text));
        }

        public void DisplayLotInfo(string lotID, DataGridView targetGrid)
        {
            var modelName = LedStorage.Lots[lotID].Model.ModelName;
            var MRM = LedStorage.Lots[lotID].Mrm;
            var RankA = LedStorage.Lots[lotID].RankA;
            var RangB = LedStorage.Lots[lotID].RankB;
            var orderedQty = LedStorage.Lots[lotID].OrderedQuantity;
            var goodQty = LedStorage.Lots[lotID].ManufacturedGoodQuantity;
            var reworkQty = LedStorage.Lots[lotID].ReworkQuantity;
            var scrapQty = LedStorage.Lots[lotID].ScrapQuantity;
            var planID = LedStorage.Lots[lotID].PlanId;
            var kittingDate = LedStorage.Lots[lotID].PrintDate.ToString();
            var testedQty = LedStorage.Lots[lotID].TestedQuantity;
            var boxedPercentage = BoxingUtilities.BoxingProgress(LedStorage.Lots[lotID]);
            var palletisedPercentage = BoxingUtilities.PalletizingProgress(LedStorage.Lots[lotID]);
            var boxingDate = BoxingUtilities.LotToBoxesDate(LedStorage.Lots[lotID]);
            var boxId = BoxingUtilities.LotToBoxesId(LedStorage.Lots[lotID]);
            var palletisingDate = BoxingUtilities.LotToPalletDate(LedStorage.Lots[lotID]);
            var palletisingId = BoxingUtilities.LotToPalletId(LedStorage.Lots[lotID]);
            var testDate = "";
            if (LedStorage.Lots[lotID].LedsInLot.Count > 0)
            {
                testDate = LedStorage.Lots[lotID].LedsInLot[0].TesterData[0].TimeOfTest.ToString();
            }

            string splittingDate = "";
            if (LedStorage.Lots[lotID].WasteInfo != null)
            {
                splittingDate = LedStorage.Lots[lotID].WasteInfo.SplittingDate.ToString();
            }

            DataTable sourceTable = new DataTable();
            sourceTable.Columns.Add("Name");
            sourceTable.Columns.Add("Value");

            sourceTable.Rows.Add("Plan ID", planID);
            sourceTable.Rows.Add("Model Name", modelName);
            sourceTable.Rows.Add("MRM", MRM);
            sourceTable.Rows.Add("Rank A", RankA);
            sourceTable.Rows.Add("Rank B", RangB);
            sourceTable.Rows.Add("Kitting date", kittingDate);
            sourceTable.Rows.Add("Ordered quantity", orderedQty);
            sourceTable.Rows.Add("Good quantity", goodQty);
            sourceTable.Rows.Add("Rework quantity", reworkQty);
            sourceTable.Rows.Add("Scrap quantity", scrapQty);
            sourceTable.Rows.Add("Tested quantity", testedQty);
            sourceTable.Rows.Add("Testing date (1st)", testDate);
            sourceTable.Rows.Add("Splitting Date", splittingDate);
            sourceTable.Rows.Add("Boxed", boxedPercentage);
            sourceTable.Rows.Add("Boxing date", String.Join(", ", boxingDate));
            sourceTable.Rows.Add("Box ID", String.Join(", ", boxId));
            sourceTable.Rows.Add("Palletised", palletisedPercentage);
            sourceTable.Rows.Add("Palletising date", String.Join(", ", palletisingDate));
            sourceTable.Rows.Add("Pallet ID", String.Join(", ", palletisingId));

            targetGrid.DataSource = sourceTable;
            foreach (DataGridViewColumn col in targetGrid.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        public void ShowLotInfo()
        {
            var selectedLot = treeViewLotInfo.SelectedNode.Name;

            var modelName = LedStorage.Lots[selectedLot].Model.ModelName;
            var MRM = LedStorage.Lots[selectedLot].Mrm;
            var RankA = LedStorage.Lots[selectedLot].RankA;
            var RangB = LedStorage.Lots[selectedLot].RankB;
            var orderedQty = LedStorage.Lots[selectedLot].OrderedQuantity;
            var goodQty = LedStorage.Lots[selectedLot].ManufacturedGoodQuantity;
            var reworkQty = LedStorage.Lots[selectedLot].ReworkQuantity;
            var scrapQty = LedStorage.Lots[selectedLot].ScrapQuantity;
            var planID = LedStorage.Lots[selectedLot].PlanId;
            var kittingDate = LedStorage.Lots[selectedLot].PrintDate.ToString();
            var testedQty = LedStorage.Lots[selectedLot].TestedQuantity;
            var boxedPercentage = BoxingUtilities.BoxingProgress(LedStorage.Lots[selectedLot]);
            var palletisedPercentage = BoxingUtilities.PalletizingProgress(LedStorage.Lots[selectedLot]);
            var boxingDate = BoxingUtilities.LotToBoxesDate(LedStorage.Lots[selectedLot]);
            var boxId = BoxingUtilities.LotToBoxesId(LedStorage.Lots[selectedLot]);
            var palletisingDate = BoxingUtilities.LotToPalletDate(LedStorage.Lots[selectedLot]);
            var palletisingId = BoxingUtilities.LotToPalletId(LedStorage.Lots[selectedLot]);
            var testDate = "";
            if (LedStorage.Lots[selectedLot].LedsInLot.Count > 0)
            {
                testDate = LedStorage.Lots[selectedLot].LedsInLot[0].TesterData[0].TimeOfTest.ToString();
            }

            string splittingDate = "";
            if (LedStorage.Lots[selectedLot].WasteInfo != null)
            {
                splittingDate = LedStorage.Lots[selectedLot].WasteInfo.SplittingDate.ToString();
            }

            DataTable gridTable = new DataTable();
            gridTable.Columns.Add("Name");
            gridTable.Columns.Add("Value");

            gridTable.Rows.Add("Plan ID", planID);
            gridTable.Rows.Add("Model Name", modelName);
            gridTable.Rows.Add("MRM", MRM);
            gridTable.Rows.Add("Rank A", RankA);
            gridTable.Rows.Add("Rank B", RangB);
            gridTable.Rows.Add("Kitting date", kittingDate);
            gridTable.Rows.Add("Ordered quantity", orderedQty);
            gridTable.Rows.Add("Good quantity", goodQty);
            gridTable.Rows.Add("Rework quantity", reworkQty);
            gridTable.Rows.Add("Scrap quantity", scrapQty);
            gridTable.Rows.Add("Tested quantity", testedQty);
            gridTable.Rows.Add("Testing date (1st)", testDate);
            gridTable.Rows.Add("Splitting Date", splittingDate);
            gridTable.Rows.Add("Boxed", boxedPercentage);
            gridTable.Rows.Add("Boxing date", String.Join(", ", boxingDate));
            gridTable.Rows.Add("Box ID", String.Join(", ", boxId));
            gridTable.Rows.Add("Palletised", palletisedPercentage);
            gridTable.Rows.Add("Palletising date", String.Join(", ", palletisingDate));
            gridTable.Rows.Add("Pallet ID", String.Join(", ", palletisingId));

            dataGridViewLotInfo.DataSource = gridTable;
            foreach (DataGridViewColumn col in dataGridViewLotInfo.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

        }

    }
}
