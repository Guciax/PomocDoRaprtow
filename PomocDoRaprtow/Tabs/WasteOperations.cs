using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PomocDoRaprtow.Tabs
{
    public class WasteOperations
    {
        private readonly Form1 form;

        private readonly TreeView treeViewWaste;
        private readonly DataGridView wasteView;
        private readonly Chart wasteHistogramChart;
        public LedStorage LedStorage { get; set; }

        private List<Model> models;

        public WasteOperations(Form1 form, TreeView treeViewWaste, DataGridView wasteView, Chart wasteHistogramChart)
        {
            this.form = form;

            this.treeViewWaste = treeViewWaste;
            this.wasteView = wasteView;
            this.wasteHistogramChart = wasteHistogramChart;
        }

        public void RedrawWasteTab()
        {
            treeViewWaste.BeginUpdate();
            treeViewWaste.Nodes.Clear();
            models = LedStorage.Models.Values.Where(form.ModelSelected).ToList();
            var totalWaste = models.Sum(m => CalculateWaste(m, form.WasteInfoBySplittingTime).Sum());
            var totalProduced = models.SelectMany(m => m.Lots.Where(form.LotBySplittingTime))
                .Sum(l => l.TestedQuantity);

            TreeNode totalNode = new TreeNode("Total" + " " + totalWaste + " Total Produced " +
                                              MathUtilities.CalculatePercentage(totalProduced, totalWaste));
            totalNode.Name = "Total";
            treeViewWaste.Nodes.Add(totalNode);

            foreach (var model in models)
            {
                int totalWasteInModel = CalculateWaste(model, form.WasteInfoBySplittingTime).Sum();
                int totalProducedInModel = model.Lots.Where(form.LotBySplittingTime).Sum(l => l.TestedQuantity);
                if (totalProducedInModel == 0) continue;
                TreeNode modelNode =
                    new TreeNode(
                        $"{model.ModelName} {totalWasteInModel} - {MathUtilities.CalculatePercentage(totalProducedInModel, totalWasteInModel)}");
                modelNode.Name = model.ModelName;
                treeViewWaste.Nodes["Total"].Nodes.Add(modelNode);
            }

            treeViewWaste.ExpandAll();
            treeViewWaste.EndUpdate();
            treeViewWaste.SelectedNode = treeViewWaste.Nodes["Total"];
        }

        public void TreeViewWasteSelectionChanged()
        {
            DataTable hist = new DataTable();
            hist.Columns.Add("Name");
            hist.Columns.Add("Count", typeof(int));

            foreach (var wasteHeader in WasteInfo.WasteFieldNames)
            {
                hist.Rows.Add(wasteHeader, 0);
            }

            if (treeViewWaste.SelectedNode.Name == "Total")
            {
                for (int i = 0; i < WasteInfo.WasteFieldNames.Length; ++i)
                {
                    hist.Rows[i][1] = models.Sum(m => CalculateWaste(m, form.WasteInfoBySplittingTime)[i]);
                }
            }

            foreach (var model in models)
            {
                if (model.ModelName == treeViewWaste.SelectedNode.Name)
                {
                    var categorizedWaste = CalculateWaste(model, form.WasteInfoBySplittingTime);
                    for (int i = 0; i < WasteInfo.WasteFieldNames.Length; i++)
                    {
                        hist.Rows[i][1] = categorizedWaste[i];
                    }
                }
            }

            DataView dv = hist.DefaultView;
            dv.Sort = "Count desc";
            hist = dv.ToTable();

            wasteView.DataSource = hist;
            Charting.BarChart(wasteHistogramChart, hist, 0, 1);
        }

        private List<int> CalculateWaste(Model model, Predicate<WasteInfo> wasteInfoFilter)
        {
            var wasteInModel = new List<int>();
            foreach (var ignored in WasteInfo.WasteFieldNames)
            {
                wasteInModel.Add(0);
            }
            foreach (var lot in model.Lots)
            {
                if (lot.WasteInfo != null && wasteInfoFilter(lot.WasteInfo))
                {
                    for (int i = 0; i < lot.WasteInfo.WasteCounts.Count; ++i)
                    {
                        wasteInModel[i] += lot.WasteInfo.WasteCounts[i];
                    }
                }
            }

            return wasteInModel;
        }
    }
}