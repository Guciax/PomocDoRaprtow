using System;

namespace PomocDoRaprtow
{
    public class Lot
    {
        public Lot(string lotId, string nc12, string rankA, string rankB, string mrm, WasteInfo wasteInfo)
        {
            LotId = lotId;
            Nc12 = nc12;
            RankA = rankA;
            RankB = rankB;
            Mrm = mrm;
            WasteInfo = wasteInfo;
        }

        public String LotId { get; }
        public String Nc12 { get; }
        public String RankA { get; }
        public String RankB { get; }
        public String Mrm { get; }
        public WasteInfo WasteInfo { get; }
    }
}