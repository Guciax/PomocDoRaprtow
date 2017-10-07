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


        private bool TakeAllWaste(WasteInfo w) => true;

        public List<int> CalculateWaste(Predicate<WasteInfo> wasteInfoFilter)
        {
            var wasteInModel = new List<int>();
            foreach (var ignored in WasteInfo.WasteFieldNames)
            {
                wasteInModel.Add(0);
            }
            foreach (var lot in Lots)
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