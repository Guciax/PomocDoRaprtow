using PomocDoRaprtow.DataModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace PomocDoRaprtow
{
    public class LedStorageLoader
    {
        private Dictionary<String, Lot> Lots { get; set; }
        private Dictionary<String, WasteInfo> LotIdToWasteInfo { get; set; }
        private Dictionary<String, Led> SerialNumbersToLed { get; set; }
        private Dictionary<string, Model> models { get; set; }
        private Dictionary<String, Boxing> serialToBoxing { get; set; }

        public const String LotPath = @"DB\Zlecenia_produkcyjne.txt";
        public const String WastePath = @"DB\Odpady.csv";
        public const String TesterPath = @"DB\tester.csv";
        public const string BoxingPath = @"DB\WyrobLG_opakowanie.txt";



        public LedStorage BuildStorage(String lotPath = LotPath, String wastePath = WastePath,
            String testerPath = TesterPath, string boxingPathi = BoxingPath)
        {
            LoadWasteTable(wastePath);
            LoadLotTable(lotPath);
            LoadLedTable(testerPath);
            LoadBoxingTable(boxingPathi);
            return new LedStorage(Lots, LotIdToWasteInfo, SerialNumbersToLed, models, serialToBoxing);
        }

        private void LoadBoxingTable(string boxingPath)
        {
            serialToBoxing = new Dictionary<string, Boxing>();
            string[] boxLines = System.IO.File.ReadAllLines(boxingPath);
            string[] header = boxLines[0].Split(';');

            int indexSerial = Array.IndexOf(header, "serial_no");
            int indexBoxingDate = Array.IndexOf(header, "Boxing_Date");
            int indexPalletisingDate = Array.IndexOf(header, "Palletising_Date");

            foreach (var line in boxLines.Skip(1))
            {
                var splitLine = line.Split(';');
                var serial = splitLine[indexSerial];
                DateTime boxingDate = new DateTime();
                DateTime palletisingDate = new DateTime();

                if (splitLine[indexBoxingDate]!="NULL")
                {
                    boxingDate = DateUtilities.ParseExact(splitLine[indexBoxingDate]);
                }

                if (splitLine[indexPalletisingDate] != "NULL")
                {
                    palletisingDate = DateUtilities.ParseExact(splitLine[indexPalletisingDate]);
                }

                if (!serialToBoxing.ContainsKey(serial))
                {
                    serialToBoxing.Add(serial, new Boxing(boxingDate, palletisingDate));
                }
            }
        }

        private void LoadLotTable(String path = LotPath)
        {
            Lots = new Dictionary<string, Lot>();
            models = new Dictionary<string, Model>();
            string[] fileLines = System.IO.File.ReadAllLines(path);
            string[] header = fileLines[0].Split(';');

            int indexLotId = Array.IndexOf(header, "Nr_Zlecenia_Produkcyjnego");
            int indexPlanId = Array.IndexOf(header, "Nr_Planu_Produkcji");
            int indexModel = Array.IndexOf(header, "NC12_wyrobu");
            int indexRankA = Array.IndexOf(header, "RankA");
            int indexRankB = Array.IndexOf(header, "RankB");
            int indexMrm = Array.IndexOf(header, "MRM");
            int indexOrderedQty = Array.IndexOf(header, "Ilosc_wyrobu_zlecona");
            int indexManufacturedGoodQty = Array.IndexOf(header, "Ilosc_wyr_dobrego");
            int indexReworkQty = Array.IndexOf(header, "Ilosc_wyr_do_poprawy");
            int indexmScrapQty = Array.IndexOf(header, "Ilosc_wyr_na_zlom");
            int indexPrintDate = Array.IndexOf(header, "DataCzasWydruku");

            
            foreach (var line in fileLines.Skip(1))
            {
                var splitLine = line.Split(';');
                var lotId = splitLine[indexLotId];
                var modelName = splitLine[indexModel].Replace("LLFML", "");
                WasteInfo info;
                LotIdToWasteInfo.TryGetValue(lotId, out info);

                Model model;
                models.TryGetValue(modelName, out model);
                if (model == null)
                {
                    model = new Model(modelName);
                    models.Add(modelName, model);
                }
                int orderedQty = 0;
                int manufacturedQty = 0;
                int reworkQty = 0;
                int scrapQty = 0;
                DateTime printDate = new DateTime();

                Int32.TryParse(splitLine[indexOrderedQty], out orderedQty);
                Int32.TryParse(splitLine[indexManufacturedGoodQty], out manufacturedQty);
                Int32.TryParse(splitLine[indexReworkQty], out reworkQty);
                Int32.TryParse(splitLine[indexmScrapQty], out scrapQty);
                if (splitLine[indexPrintDate]!="NULL") DateUtilities.ParseExactWithFraction(splitLine[indexPrintDate]);


                var lot = new Lot(splitLine[indexLotId],
                    splitLine[indexPlanId],
                    splitLine[indexRankA],
                    splitLine[indexRankB],
                    splitLine[indexMrm], 
                    model, 
                    info, 
                    0,//testted quantity to fiilup later
                    orderedQty,
                    manufacturedQty,
                    reworkQty,
                    scrapQty,
                    printDate);


                Lots.Add(lot.LotId, lot);

                model.Lots.Add(lot);
            }
        }

        private void LoadWasteTable(String path = WastePath)
        {
            LotIdToWasteInfo = new Dictionary<string, WasteInfo>();
            string[] fileLines = System.IO.File.ReadAllLines(path);
            string[] header = fileLines[0].Split(';');

            var indices = WasteInfo.WasteFieldNames.Select(wasteName => Array.IndexOf(header, wasteName)).ToList();
            var lotIdIndex = Array.IndexOf(header, "Nr_Zlecenia_Produkcyjnego");
            var splittingTimeIndex = Array.IndexOf(header, "DataCzas");

            foreach (var line in fileLines.Skip(1))
            {
                List<int> counts = new List<int>();
                var splitLine = line.Split(';');
                String lotId = splitLine[lotIdIndex];
                var splittingTime = DateUtilities.ParseExact(splitLine[splittingTimeIndex]);
                foreach (var index in indices)
                {
                    counts.Add(int.Parse(splitLine[index]));
                }
                LotIdToWasteInfo.Add(lotId, new WasteInfo(counts, splittingTime));
            }
        }

        private void LoadLedTable(String path = TesterPath)
        {
            SerialNumbersToLed = new Dictionary<string, Led>();
            string[] fileLines = System.IO.File.ReadAllLines(path);
            string[] header = fileLines[0].Split(';');
            int indexSerialNr = Array.IndexOf(header, "serial_no");
            int indexLotId = Array.IndexOf(header, "wip_entity_name");
            int indexTesterId = Array.IndexOf(header, "tester_id");
            int indexTestTime = Array.IndexOf(header, "inspection_time");
            int indexResult = Array.IndexOf(header, "result");
            int indexFailReason = Array.IndexOf(header, "ng_type");
            Dictionary<string, HashSet<string>> serialsInLot = new Dictionary<string, HashSet<string>>();

            foreach (var line in fileLines.Skip(1))
            {
                var splitLine = line.Split(';');
                var lotId = splitLine[indexLotId];
                var ledId = splitLine[indexSerialNr];

                if (!serialsInLot.ContainsKey(lotId))
                {
                    serialsInLot.Add(lotId, new HashSet<string>());
                }
                serialsInLot[lotId].Add(ledId);

                Lot lot;
                Lots.TryGetValue(lotId, out lot);

                if (lot == null)
                {
                    continue;
                }

                string testResult = splitLine[indexResult];
                if (testResult != "OK" && testResult != "NG")
                {
                    continue;
                }

                var timeOfTest = DateUtilities.ParseExact(splitLine[indexTestTime]);
                var wasTestSuccesful = splitLine[indexResult] == "OK";
                var fixedDateTime = DateUtilities.FixedShiftDate(timeOfTest);
                var shiftNo = DateUtilities.DateToShiftInfo(timeOfTest).ShiftNo;
                var testerData = new TesterData(splitLine[indexTesterId], timeOfTest, fixedDateTime, shiftNo,
                    wasTestSuccesful, splitLine[indexFailReason]);
                var led = new Led(splitLine[indexSerialNr], lot, testerData);

                if (!SerialNumbersToLed.ContainsKey(led.SerialNumber))
                {
                    SerialNumbersToLed.Add(led.SerialNumber, led);
                }
                else
                {
                    Led previousLed = SerialNumbersToLed[led.SerialNumber];
                    previousLed.AddTesterData(testerData);
                }
            }

            foreach (var entry in serialsInLot)
            {
                if (!Lots.Keys.Contains(entry.Key)) continue;
                int count = entry.Value.Count;
                Lots[entry.Key].TestedQuantity = count;
            }
        }
    }
}