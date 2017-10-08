using System;
using System.Collections.Generic;
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
        //Dictionary<string, List<string>> ModelLotDictionary = new Dictionary<string, List<string>>();

        public LotInfoOperations(Form1 form, TreeView treeViewLotInfo, TextBox textBoxFilterLotInfo)
        {
            this.form = form;
            this.treeViewLotInfo = treeViewLotInfo;
            this.textBoxFilterLotInfo = textBoxFilterLotInfo;
        }

        //public void BuildModelLotInfoDictionary()
        //{
        //    foreach (var lotId in LedStorage.Lots)
        //    {
        //        if (!ModelLotDictionary.ContainsKey(lotId.Value.Model.ModelName))
        //        {
        //            ModelLotDictionary.Add(lotId.Value.Model.ModelName, new List<string>());
        //        }
        //        ModelLotDictionary[lotId.Value.Model.ModelName].Add(lotId.Key);
        //    }
        //}

        public void FilterLotInfoTreeView()
        {
            treeViewLotInfo.BeginUpdate();
            treeViewLotInfo.Nodes.Clear();
            foreach (var model in LedStorage.Models.Values)
            {
                var lots = FilterLots(model.Lots).ToList();
                if (lots.Count == 0) continue;

                TreeNode modelNode = new TreeNode( model.ModelName);
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

        }
    }
}
