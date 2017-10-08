using System;
using System.Collections.Generic;
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
            foreach (var lot in LedStorage.Lots.Values)
            {
                //var okLeds = lot.LedsInLot.Where(l => l.te)
                if (lot.LedsInLot.Count == 0 && BoxingUtilities.IsFullyPalletised(lot)) continue;
                string percentPaletised =  BoxingUtilities.PercentPalletised(lot);
                string percentBoxed = BoxingUtilities.PercentBoxed(lot);
            }
        }
    }
}
