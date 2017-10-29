using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    public class ProductionDetail
    {
        public ProductionDetail(DateTime productionFixedDate, DateTime productionRealDate, String productionLineId, int producedAmount, Lot producedIn)
        {
            ProductionFixedDate = productionFixedDate;
            ProductionRealDate = productionRealDate;
            ProducedAmount = producedAmount;
            ProductionLineId = productionLineId;
            ProducedIn = producedIn;
        }

        public DateTime ProductionFixedDate { get; }
        public DateTime ProductionRealDate { get; }
        public String ProductionLineId { get; }
        public int ProducedAmount { get; }
        public Lot ProducedIn { get; }
    }
}
