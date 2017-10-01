using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PomocDoRaprtow
{
    public class LedStorageLoader
    {
        private Dictionary<String, Lot> Lots { get; set; }
        private Dictionary<String, WasteInfo> LotIdToWasteInfo { get; set; }
        private Dictionary<String, Led> SerialNumbersToLed { get; set; }

        public const String LotPath = @"DB\Zlecenia_produkcyjne.txt";
        public const String WastePath = @"DB\Odpad new.txt";
        public const String TesterPath = @"DB\tester.csv";


        public LedStorage BuildStorage(String lotPath = LotPath, String wastePath = WastePath,
            String testerPath = TesterPath)
        {
            LoadWasteTable(wastePath);
            LoadLotTable(lotPath);
            LoadLedTable(testerPath);
            return new LedStorage(Lots, LotIdToWasteInfo, SerialNumbersToLed);
        }

        private void LoadLotTable(String path = LotPath)
        {
            Lots = new Dictionary<string, Lot>();
            string[] fileLines = System.IO.File.ReadAllLines(path);
            string[] header = fileLines[0].Split(';');
            int indexLotId = Array.IndexOf(header, "Nr_Zlecenia_Produkcyjnego");
            int indexNc12 = Array.IndexOf(header, "NC12_wyrobu");
            int indexRankA = Array.IndexOf(header, "RankA");
            int indexRankB = Array.IndexOf(header, "RankB");
            int indexMrm = Array.IndexOf(header, "MRM");


            foreach (var line in fileLines.Skip(1))
            {
                var splitLine = line.Split(';');
                var lotId = splitLine[indexLotId];

                WasteInfo info;
                LotIdToWasteInfo.TryGetValue(lotId, out info);

                var lot = new Lot(splitLine[indexLotId],
                    splitLine[indexNc12],
                    splitLine[indexRankA],
                    splitLine[indexRankB],
                    splitLine[indexMrm], info);

                Lots.Add(lot.LotId, lot);
            }
        }

        private void LoadWasteTable(String path = WastePath)
        {
            LotIdToWasteInfo = new Dictionary<string, WasteInfo>();
            string[] fileLines = System.IO.File.ReadAllLines(path);
            string[] header = fileLines[0].Split(';');

            var indices = WasteInfo.WasteFieldNames.Select(wasteName => Array.IndexOf(header, wasteName)).ToList();
            var lotIdIndex = Array.IndexOf(header, "Nr_Zlecenia_Produkcyjnego");

            foreach (var line in fileLines.Skip(1))
            {
                List<int> counts = new List<int>();
                var splitLine = line.Split(';');
                String lotId = splitLine[lotIdIndex];
                foreach (var index in indices)
                {
                    counts.Add(int.Parse(splitLine[index]));
                }
                LotIdToWasteInfo.Add(lotId, new WasteInfo(counts));
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

            foreach (var line in fileLines.Skip(1))
            {
                var splitLine = line.Split(';');
                var lotId = splitLine[indexLotId];

                Lot lot;
                Lots.TryGetValue(lotId, out lot);

                var timeOfTest = DateTime.ParseExact(splitLine[indexTestTime],"yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
                var testResult = splitLine[indexResult] == "OK";
                var testerData = new TesterData(splitLine[indexTesterId],timeOfTest, testResult, splitLine[indexFailReason]);
                var led = new Led(splitLine[indexSerialNr], lot, testerData);

                //TODO WHY there can are duplicates here?
                if (!SerialNumbersToLed.ContainsKey(led.SerialNumber))
                {
                    SerialNumbersToLed.Add(led.SerialNumber, led);
                }
            }
        }
    }
}