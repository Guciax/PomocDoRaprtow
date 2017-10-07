using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PomocDoRaprtow
{
    public class Model
    {
        public Model(String modelName)
        {
            ModelName = modelName;
        }

        public string ModelName { get; }
        public List<Lot> Lots { get; } = new List<Lot>();

        public List<int> WasteInModel;

        public void CalculateWaste()
        {
            WasteInModel = new List<int>();
            foreach (var ignored in WasteInfo.WasteFieldNames)
            {
                WasteInModel.Add(0);
            }
            foreach (var lot in Lots)
            {
                if (lot.WasteInfo != null)
                {
                    for (int i = 0; i < lot.WasteInfo.WasteCounts.Count; ++i)
                    {
                        WasteInModel[i] += lot.WasteInfo.WasteCounts[i];
                    }
                }
            }
        }
    }
}