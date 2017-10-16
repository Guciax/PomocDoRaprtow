using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PomocDoRaprtow.Tabs
{
    //printDate, [SMT date], testDate, splittingDate, boxingDate, palletisingDate
    class LotsInUseOperations
    {
        public LotsInUseOperations(Form1 form, TreeView treeViewLotsInUse)
        {
            Form = form;
            TreeViewLotsInUse = treeViewLotsInUse;
        }



        public LedStorage LedStorage { get; set; }
        public Form1 Form { get; }
        public TreeView TreeViewLotsInUse { get; }
        
        public void GenerateLotsInUseToTree()
        {

            List<Lot> lotWaitingForSMT = new List<Lot>();
            List<Lot> lotWaitingForTest = new List<Lot>();
            List<Lot> lotWaitingForSplitting = new List<Lot>();
            List<Lot> lotWaitingForBoxing = new List<Lot>();
            List<Lot> lotWaitingForPalletising = new List<Lot>();
            List<Lot> lotOnMatching = new List<Lot>();

            foreach (var lot in LedStorage.Lots.Values)
            {


                if (BoxingUtilities.IsFullyPalletised(lot))
                    continue;

                if (BoxingUtilities.BoxedQuantity(lot) > 0)
                {
                    lotWaitingForPalletising.Add(lot);
                    if (BoxingUtilities.BoxedQuantity(lot)<lot.ManufacturedGoodQuantity) lotOnMatching.Add(lot);
                }
                else if (lot.WasteInfo != null)
                {
                    lotWaitingForBoxing.Add(lot);
                }
                else if (lot.TestedQuantity > 0)
                {
                    lotWaitingForSplitting.Add(lot);
                }
                else
                {
                    lotWaitingForTest.Add(lot);
                }
                //TreeViewLotsInUse.Nodes.Add($"Palletised {percentPaletised} Boxed {percentBoxed}");
            }

            lotWaitingForSMT = lotWaitingForSMT.OrderBy(l => l.Model.ModelName).ToList();
            lotWaitingForTest=lotWaitingForTest.OrderBy(l => l.Model.ModelName).ToList();
            lotWaitingForSplitting=lotWaitingForSplitting.OrderBy(l => l.Model.ModelName).ToList();
            lotWaitingForBoxing=lotWaitingForBoxing.OrderBy(l => l.Model.ModelName).ToList();
            lotWaitingForPalletising=lotWaitingForPalletising.OrderBy(l => l.Model.ModelName).ToList();
            lotOnMatching=lotOnMatching.OrderBy(l => l.Model.ModelName).ToList();

            TreeViewLotsInUse.BeginUpdate();
            TreeNode waitinfForSmtNode = new TreeNode("Waiting for SMT");
            waitinfForSmtNode.Name = "Waiting for SMT";
            TreeNode waitinfForTestNode = new TreeNode("Waiting for Test");
            waitinfForTestNode.Name = "Waiting for Test";
            TreeNode waitinfForSplittingNode = new TreeNode("Waiting for Splitting");
            waitinfForSplittingNode.Name = "Waiting for Splitting";
            TreeNode onMatchingNode = new TreeNode("On Matching");
            onMatchingNode.Name = "On Matching";
            TreeNode waitinfForBoxingNode = new TreeNode("Waiting for Boxing");
            waitinfForBoxingNode.Name = "Waiting for Boxing";
            TreeNode waitinfForPalletisingNode = new TreeNode("Waiting for Palletising");
            waitinfForPalletisingNode.Name = "Waiting for Palletising";
            TreeViewLotsInUse.Nodes.Add(waitinfForSmtNode);
            TreeViewLotsInUse.Nodes.Add(waitinfForTestNode);
            TreeViewLotsInUse.Nodes.Add(waitinfForSplittingNode);
            TreeViewLotsInUse.Nodes.Add(onMatchingNode);
            TreeViewLotsInUse.Nodes.Add(waitinfForBoxingNode);
            TreeViewLotsInUse.Nodes.Add(waitinfForPalletisingNode);

            int totalForTest = 0;
            Dictionary<string, int> totalForTestPerModel = new Dictionary<string, int>();

            int totalForSplitting = 0;
            Dictionary<string, int> totalForSplittingPerModel = new Dictionary<string, int>();

            int totalOnMatching = 0;
            Dictionary<string, int> totalonMatchingPerModel = new Dictionary<string, int>();

            int totalForBoxing = 0;
            Dictionary<string, int> totalForBoxingPerModel = new Dictionary<string, int>();

            int totalForPalletising = 0;
            Dictionary<string, int> totalForPalletisingPerModel = new Dictionary<string, int>();
            
            foreach (Lot testLot in lotWaitingForTest)
            {
                TreeNode modelNode = new TreeNode(testLot.Model.ModelName);
                modelNode.Name = testLot.Model.ModelName;

                if (!TreeViewLotsInUse.Nodes["Waiting for Test"].Nodes.ContainsKey(modelNode.Name))
                {
                    TreeViewLotsInUse.Nodes["Waiting for Test"].Nodes.Add(modelNode);
                    totalForTestPerModel.Add(modelNode.Name, 0);
                }

                totalForTestPerModel[modelNode.Name] += testLot.OrderedQuantity;

                TreeNode lotNode = new TreeNode(testLot.LotId + " - " + testLot.OrderedQuantity);
                lotNode.Name = testLot.LotId;
                TreeViewLotsInUse.Nodes["Waiting for Test"].Nodes[modelNode.Name].Nodes.Add(lotNode);
            }

            foreach (TreeNode modelNode in TreeViewLotsInUse.Nodes["Waiting for Test"].Nodes)
            {
                modelNode.Text = modelNode.Name + " - " + totalForTestPerModel[modelNode.Name];
                totalForTest += totalForTestPerModel[modelNode.Name];
            }
            TreeViewLotsInUse.Nodes["Waiting for Test"].Text = "Waiting for Test - " + totalForTest;
            //----Splitting----
            foreach (Lot splittingLot in lotWaitingForSplitting)
            {

                TreeNode modelNode = new TreeNode(splittingLot.Model.ModelName);
                modelNode.Name = splittingLot.Model.ModelName;

                if (!TreeViewLotsInUse.Nodes["Waiting for Splitting"].Nodes.ContainsKey(modelNode.Name))
                {
                    TreeViewLotsInUse.Nodes["Waiting for Splitting"].Nodes.Add(modelNode);
                    totalForSplittingPerModel.Add(modelNode.Name, 0);
                }

                totalForSplittingPerModel[modelNode.Name] += splittingLot.OrderedQuantity;

                TreeNode lotNode = new TreeNode(splittingLot.LotId + " - " + splittingLot.TestedQuantity);
                lotNode.Name = splittingLot.LotId;
                TreeViewLotsInUse.Nodes["Waiting for Splitting"].Nodes[modelNode.Name].Nodes.Add(lotNode);
            }
            foreach (TreeNode modelNode in TreeViewLotsInUse.Nodes["Waiting for Splitting"].Nodes)
            {
                modelNode.Text = modelNode.Name + " - " + totalForSplittingPerModel[modelNode.Name];
                totalForSplitting += totalForSplittingPerModel[modelNode.Name];
            }
            TreeViewLotsInUse.Nodes["Waiting for Splitting"].Text = "Waiting for Splitting - " + totalForSplitting;
            //------Matching-----
            foreach (Lot matchingLot in lotOnMatching)
            {

                TreeNode modelNode = new TreeNode(matchingLot.Model.ModelName);
                modelNode.Name = matchingLot.Model.ModelName;

                if (!TreeViewLotsInUse.Nodes["On Matching"].Nodes.ContainsKey(modelNode.Name))
                {
                    TreeViewLotsInUse.Nodes["On Matching"].Nodes.Add(modelNode);
                    totalonMatchingPerModel.Add(modelNode.Name, 0);
                }

                totalonMatchingPerModel[modelNode.Name] += matchingLot.ManufacturedGoodQuantity - BoxingUtilities.BoxedQuantity(matchingLot);

                TreeNode lotNode = new TreeNode(matchingLot.LotId + " - " + (matchingLot.ManufacturedGoodQuantity - BoxingUtilities.BoxedQuantity(matchingLot)));
                lotNode.Name = matchingLot.LotId;
                TreeViewLotsInUse.Nodes["On Matching"].Nodes[modelNode.Name].Nodes.Add(lotNode);
            }
            foreach (TreeNode modelNode in TreeViewLotsInUse.Nodes["On Matching"].Nodes)
            {
                modelNode.Text = modelNode.Name + " - " + totalonMatchingPerModel[modelNode.Name];
                totalOnMatching += totalonMatchingPerModel[modelNode.Name];
            }
            TreeViewLotsInUse.Nodes["On Matching"].Text = "On Matching - " + totalOnMatching;


            //-----Boxing-----

            foreach (Lot boxLot in lotWaitingForBoxing)
            {

                TreeNode modelNode = new TreeNode(boxLot.Model.ModelName);
                modelNode.Name = boxLot.Model.ModelName;

                if (!TreeViewLotsInUse.Nodes["Waiting for Boxing"].Nodes.ContainsKey(modelNode.Name))
                {
                    TreeViewLotsInUse.Nodes["Waiting for Boxing"].Nodes.Add(modelNode);
                    totalForBoxingPerModel.Add(modelNode.Name, 0);
                }

                totalForBoxingPerModel[modelNode.Name] += boxLot.OrderedQuantity;

                TreeNode lotNode = new TreeNode(boxLot.LotId + " - " + boxLot.ManufacturedGoodQuantity);
                lotNode.Name = boxLot.LotId;
                TreeViewLotsInUse.Nodes["Waiting for Boxing"].Nodes[modelNode.Name].Nodes.Add(lotNode);
            }
            foreach (TreeNode modelNode in TreeViewLotsInUse.Nodes["Waiting for Boxing"].Nodes)
            {
                modelNode.Text = modelNode.Name + " - " + totalForBoxingPerModel[modelNode.Name];
                totalForBoxing += totalForBoxingPerModel[modelNode.Name];
            }
            TreeViewLotsInUse.Nodes["Waiting for Boxing"].Text = "Waiting for Boxing - " + totalForBoxing;


            foreach (Lot palletLot in lotWaitingForPalletising)
            {

                TreeNode modelNode = new TreeNode(palletLot.Model.ModelName);
                modelNode.Name = palletLot.Model.ModelName;

                if (!TreeViewLotsInUse.Nodes["Waiting for Palletising"].Nodes.ContainsKey(modelNode.Name))
                {
                    TreeViewLotsInUse.Nodes["Waiting for Palletising"].Nodes.Add(modelNode);
                    totalForPalletisingPerModel.Add(modelNode.Name, 0);
                }

                totalForPalletisingPerModel[modelNode.Name] += palletLot.OrderedQuantity;

                TreeNode lotNode = new TreeNode(palletLot.LotId + " - " + BoxingUtilities.PalletizingProgress(palletLot));
                lotNode.Name = palletLot.LotId;
                TreeViewLotsInUse.Nodes["Waiting for Palletising"].Nodes[modelNode.Name].Nodes.Add(lotNode);
            }
            foreach (TreeNode modelNode in TreeViewLotsInUse.Nodes["Waiting for Palletising"].Nodes)
            {
                modelNode.Text = modelNode.Name + " - " + totalForPalletisingPerModel[modelNode.Name];
                totalForPalletising += totalForPalletisingPerModel[modelNode.Name];
            }
            TreeViewLotsInUse.Nodes["Waiting for Palletising"].Text = "Waiting for Palletising - " + totalForPalletising;

            TreeViewLotsInUse.EndUpdate();
        }
    }
}
