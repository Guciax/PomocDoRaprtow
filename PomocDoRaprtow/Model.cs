using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    public class Model
    {
        public Model(String modelName)
        {
            ModelName = modelName;
        }

        public 

        public string ModelName { get; }
        public List<Lot> Lot { get; } = new List<Lot>();
    }
}
