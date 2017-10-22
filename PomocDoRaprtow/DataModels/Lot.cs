using PomocDoRaprtow.DataModels;
using System;
using System.Collections.Generic;

namespace PomocDoRaprtow
{
    public class Lot
    {
        public Lot(string lotId, string planId, string rankA, string rankB, string mrm, Model model, WasteInfo wasteInfo, LedTest ledTest,
            int orderedQuantity, int manufacturedGoodQuantity, int reworkQuantity, int scrapQuantity, DateTime printDate, 
             Status lotStatus, PerformanceTest performance  )
        {
            LotId = lotId;
            PlanId = planId;
            RankA = rankA;
            RankB = rankB;
            Mrm = mrm;
            Model = model;
            WasteInfo = wasteInfo;
            LedTest = ledTest;
            OrderedQuantity = orderedQuantity;
            ManufacturedGoodQuantity = manufacturedGoodQuantity;
            ReworkQuantity = reworkQuantity;
            ScrapQuantity = scrapQuantity;
            PrintDate = printDate;
            LotStatus = lotStatus;
            Performance = performance;
        }
        

        public String LotId { get; }
        public string PlanId { get; }
        public Model Model { get; set; }
        public String RankA { get; }
        public String RankB { get; }
        public String Mrm { get; }
        public WasteInfo WasteInfo { get; }
        public LedTest LedTest { get; }
        public int OrderedQuantity { get; }
        public int ManufacturedGoodQuantity { get; }
        public int ReworkQuantity { get; }
        public int ScrapQuantity { get; }
        public DateTime PrintDate { get; }
        public Status LotStatus { get; set; }
        public PerformanceTest Performance { get; set; }
        public List<Led> LedsInLot { get; } = new List<Led>();
    }

    public class Status
    {
        public Status(bool smtDone, bool testDone, bool lotFinished)
        {
            SmtDone = smtDone;
            TestDone = testDone;
            LotFinished = lotFinished;
        }

        public bool SmtDone { get; set; }
        public bool TestDone { get; set; }
        public bool LotFinished { get; set; }
    }

    public class LedTest
    {
        public LedTest(int testedUniqueQuantity, DateTime testStart, DateTime testEnd, string testerId)
        {
            TestedUniqueQuantity = testedUniqueQuantity;
            TestStart = testStart;
            TestEnd = testEnd;
            TesterId = testerId;
        }

        public int TestedUniqueQuantity { get; set; }
        public DateTime TestStart { get; set; }
        public DateTime TestEnd { get; set; }
        public string TesterId { get; set; }
    }


}