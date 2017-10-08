using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow.DataModels
{
    public class Boxing
    {
        public Boxing(DateTime? boxingDate, DateTime? palletisingDate, string boxId, string palletId)
        {
            BoxingDate = boxingDate;
            PalletisingDate = palletisingDate;
            BoxId = boxId;
            PalletId = palletId;
        }

        public DateTime? BoxingDate { get; }
        public DateTime? PalletisingDate { get; }
        public string BoxId { get; }
        public string PalletId { get; }
        //date a 
        //date b;

    }
}
