using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    class ShiftUtilities
    {
        public static int ShiftNoToIndex(int shiftNo)
        {
            switch (shiftNo)
            {
                case 3: return 1;
                case 1: return 2;
                case 2: return 3;
            }

            throw new Exception("UNKNOWN SHIFT");
        }
    }
}
