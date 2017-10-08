using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow.DataModels
{
    public class Boxing
    {
        public Boxing(DateTime boxingDate, DateTime palletisingDate)
        {
            BoxingDate = boxingDate;
            PalletisingDate = palletisingDate;
        }

        public DateTime BoxingDate { get; }
        public DateTime PalletisingDate { get; }
        //date a 
        //date b;

    }
}
