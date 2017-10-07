using System;

namespace PomocDoRaprtow
{
    public class Lot
    {
        public Lot(string lotId, string rankA, string rankB, string mrm, Model model, WasteInfo wasteInfo, int testedQuantity)
        {
            LotId = lotId;
            RankA = rankA;
            RankB = rankB;
            Mrm = mrm;
            Model = model;
            WasteInfo = wasteInfo;
            TestedQuantity = testedQuantity;
        }

        public String LotId { get; }
        public Model Model { get; set; }
        public String RankA { get; }
        public String RankB { get; }
        public String Mrm { get; }
        public WasteInfo WasteInfo { get; }
        public int TestedQuantity { get; set; }
    }
}