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

        public const String LotPath = @"DB\Zlecenia_produkcyjne.txt";
        public const String WastePath = @"DB\Odpady.csv";
        public const String TesterPath = @"DB\tester.csv";


        public LedStorage BuildStorage(String lotPath = LotPath, String wastePath = WastePath,
            String testerPath = TesterPath)
        {
            LoadWasteTable(wastePath);
            LoadLotTable(lotPath);
            LoadLedTable(testerPath);
            return new LedStorage(Lots, LotIdToWasteInfo, SerialNumbersToLed, models);
        }

        private void LoadLotTable(String path = LotPath)
        {
            Lots = new Dictionary<string, Lot>();
            models = new Dictionary<string, Model>();
            string[] fileLines = System.IO.File.ReadAllLines(path);
            string[] header = fileLines[0].Split(';');
            int indexLotId = Array.IndexOf(header, "Nr_Zlecenia_Produkcyjnego");
            int indexModel = Array.IndexOf(header, "NC12_wyrobu");
            int indexRankA = Array.IndexOf(header, "RankA");
            int indexRankB = Array.IndexOf(header, "RankB");
            int indexMrm = Array.IndexOf(header, "MRM");


            foreach (var line in fileLines.Skip(1))
            {
                var splitLine = line.Split(';');
                var lotId = splitLine[indexLotId];
                var modelName = splitLine[indexModel].Replace("LLFML","");
                WasteInfo info;
                LotIdToWasteInfo.TryGetValue(lotId, out info);

                var lot = new Lot(splitLine[indexLotId],
                    splitLine[indexRankA],
                    splitLine[indexRankB],
                    splitLine[indexMrm], info, 0);


                Lots.Add(lot.LotId, lot);

                Model model;
                models.TryGetValue(modelName, out model);
                if(model == null)
                {
                    model = new Model(modelName, lot);
                }

                models.Add(modelName, model);
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
                LotIdToWasteInfo.Add(lotId, new WasteInfo(counts,splittingTime));
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

                if(lot == null)
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
                var testerData = new TesterData(splitLine[indexTesterId],timeOfTest, fixedDateTime, shiftNo, wasTestSuccesful, splitLine[indexFailReason]);
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

            foreach (var lot in serialsInLot.Keys)
            {
                if (!Lots.Keys.Contains(lot)) continue;
                int count = lot.Length;
                Lots[lot].TestedQuantity = count;
            }
        }
    }
}