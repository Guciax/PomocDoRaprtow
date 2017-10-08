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
            var boxedPercentage = BoxingUtilities.PercentBoxed(LedStorage.Lots[selectedLot]);
            var palletisedPercentage = BoxingUtilities.PercentPalletised(LedStorage.Lots[selectedLot]);
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
