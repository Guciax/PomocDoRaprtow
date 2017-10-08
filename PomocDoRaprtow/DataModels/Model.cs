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
    }
}