using System;
using System.Collections.Generic;

namespace PomocDoRaprtow
{
    public class Lot
    {
        public Lot(string lotId, string planId, string rankA, string rankB, string mrm, Model model, WasteInfo wasteInfo, int testedQuantity,
            int orderedQuantity, int manufacturedGoodQuantity, int reworkQuantity, int scrapQuantity, DateTime printDate, 
            DateTime testStart, DateTime testEnd )
        {
            LotId = lotId;
            PlanId = planId;
            RankA = rankA;
            RankB = rankB;
            Mrm = mrm;
            Model = model;
            WasteInfo = wasteInfo;
            TestedQuantity = testedQuantity;
            OrderedQuantity = orderedQuantity;
            ManufacturedGoodQuantity = manufacturedGoodQuantity;
            ReworkQuantity = reworkQuantity;
            ScrapQuantity = scrapQuantity;
            PrintDate = printDate;
            TestStart = testStart;
            TestEnd = testEnd;
        }
        

        public String LotId { get; }
        public string PlanId { get; }
        public Model Model { get; set; }
        public String RankA { get; }
        public String RankB { get; }
        public String Mrm { get; }
        public WasteInfo WasteInfo { get; }
        public int TestedQuantity { get; set; }
        public int OrderedQuantity { get; }
        public int ManufacturedGoodQuantity { get; }
        public int ReworkQuantity { get; }
        public int ScrapQuantity { get; }
        public DateTime PrintDate { get; }
        public DateTime TestStart { get; set; }
        public DateTime TestEnd { get; set; }
        public List<Led> LedsInLot { get; } = new List<Led>();
    }
}