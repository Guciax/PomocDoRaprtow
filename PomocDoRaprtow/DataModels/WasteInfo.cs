using System;
using System.Collections.Generic;

namespace PomocDoRaprtow
{
    public class WasteInfo
    {
        public static String SplitterId = "Splitter";
        //consider loading this from file?
        public static readonly string[] WasteFieldNames =
        {
            "BrakLutowia", "BrakKomponentu", "ZabrudzonaDioda", "UszkodzonaDioda", "UszkodzonePCB",
            "PrzesuniecieDiody", "ZanieczyszczenieZpieca", "Inne"
        };

        //indicies correspond exactly to wasteifeld names
        public WasteInfo(List<int> wasteCounts, DateTime splittingDate)
        {
            WasteCounts = wasteCounts;
            SplittingDate = splittingDate;
        }

        public List<int> WasteCounts { get; }
        public DateTime SplittingDate { get; }
    }
}