using System;
using System.Collections.Generic;

namespace PomocDoRaprtow
{
    public class WasteInfo
    {
        //consider loading this from file?
        public static readonly string[] WasteFieldNames =
        {
            "BrakLutowia", "BrakKomponentu", "ZabrudzonaDioda", "UszkodzonaDioda", "UszkodzonePCB",
            "PrzesuniecieDiody", "ZanieczyszczenieZpieca", "Inne"
        };

        //indicies correspond exactly to wasteifeld names
        public WasteInfo(List<int> wasteCounts)
        {
            WasteCounts = wasteCounts;
        }

        public List<int> WasteCounts { get; }
    }
}