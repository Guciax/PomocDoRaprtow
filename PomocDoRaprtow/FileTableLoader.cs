using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    class FileTableLoader
    {
        public static DataTable LoadWasteTable()
        {
            return LoadCsvIntoDataTable(@"DB\Odpad new.txt");
        }
        
        public static DataTable LoadTesterWorkCard()
        {
            return LoadCsvIntoDataTable(@"DB\tester.csv");
        }
        
        public static List<LedModules> LoadTesterWorkCard2()
        {
            return null;
        }

        public static List<LedModules> LoadTesterCsvToList()
        {
            return CsvToLedModules(@"DB\tester.csv");
        }


        public static DataTable LOT_Module_Table()
        {
            DataTable result = new DataTable();
            string[] plikArray = System.IO.File.ReadAllLines(@"DB\Zlecenia_produkcyjne.txt");
            result.Columns.Add("Nr_Zlecenia_Produkcyjnego");
            result.Columns.Add("NC12_wyrobu");
            result.Columns.Add("RankA");
            result.Columns.Add("RankB");
            result.Columns.Add("MRM");

            int indexNrZleceniaProdukcyjnego = Array.IndexOf(plikArray[0].Split(';'), "Nr_Zlecenia_Produkcyjnego");
            int indexNc12Wyrobu = Array.IndexOf(plikArray[0].Split(';'), "NC12_wyrobu");
            int indexRankA = Array.IndexOf(plikArray[0].Split(';'), "RankA");
            int indexRankB = Array.IndexOf(plikArray[0].Split(';'), "RankB");
            int indexMrm = Array.IndexOf(plikArray[0].Split(';'), "MRM");

            foreach (var row in plikArray)
            {
                result.Rows.Add(row.Split(';')[indexNrZleceniaProdukcyjnego],
                                                row.Split(';')[indexNc12Wyrobu],
                                                row.Split(';')[indexRankA],
                                                row.Split(';')[indexRankB],
                                                row.Split(';')[indexMrm]);
            }

            return result;
        }

        private static DataTable LoadCsvIntoDataTable(string pathToCsv)
        {
            DataTable resultTable = new DataTable();
            string[] fileLines = System.IO.File.ReadAllLines(pathToCsv);
            foreach (var item in fileLines[0].Split(';'))
            {
                resultTable.Columns.Add(item);
            }

            for (int i = 1; i < fileLines.Length; i++)
            {
                resultTable.Rows.Add(fileLines[i].Split(';'));
            }

            return resultTable;
        }
        private static List<LedModules> CsvToLedModules(string FilePath)
        {
            List<LedModules> result = new List<LedModules>();

            string[] FileArray = System.IO.File.ReadAllLines(FilePath);
            List<string> HeaderList = new List<string>();

            foreach (var header in FileArray[0].Split(';'))
            {
                HeaderList.Add(header);
            }

            for (int i = 1; i < FileArray.Length; i++) 
            {
                LedModules LedToAdd = new LedModules();
                for (int j = 0; j < HeaderList.Count; j++) 
                {
                    if (HeaderList[j] == "serial_no") { LedToAdd.SerialNumber = FileArray[i].Split(';')[j]; continue; }
                    if (HeaderList[j] == "wip_entity_name") { LedToAdd.ProductionOrderId = FileArray[i].Split(';')[j]; LedToAdd.ModelName = Form1.LOT_to_Model(FileArray[i].Split(';')[j]); continue; }
                    if (HeaderList[j] == "DataCzasWydruku") { LedToAdd.KittingDateTime = FileArray[i].Split(';')[j]; continue; }
                    if (HeaderList[j] == "Ilosc_wyrobu_zlecona") { LedToAdd.KittingOrderQuantity = Int32.Parse(FileArray[i].Split(';')[j]); continue; }
                    if (HeaderList[j] == "LiniaProdukcyjna") { LedToAdd.KittingLineNumber = FileArray[i].Split(';')[j]; continue; }
                    if (HeaderList[j] == "tester_id") { LedToAdd.TesterId = FileArray[i].Split(';')[j]; continue; }
                    if (HeaderList[j] == "inspection_time") { LedToAdd.TesterTimeOfTest = DateTime.ParseExact(FileArray[i].Split(';')[j], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);  continue; }
                    if (HeaderList[j] == "result") { if (FileArray[i].Split(';')[j] == "OK") LedToAdd.TestResult = true; else LedToAdd.TestResult = false; ; continue; }
                    if (HeaderList[j] == "ng_type") { LedToAdd.TesterFailureReason = FileArray[i].Split(';')[j]; continue; }
                    //...
                }
                result.Add(LedToAdd);
            }
                return result;
        }

        private static List<TesterData> CsvTesterFileToTesterData(string FilePath)
        {
            List<TesterData> LedModulesList = new List<TesterData>();
            string[] FileArray = System.IO.File.ReadAllLines(FilePath);
            foreach (var item in FileArray)
            {
                TesterData LedToAdd = new TesterData();
                LedToAdd.TimeOfTest = DateTime.ParseExact(item.Split(';')[1],"yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
                LedToAdd.TesterId = item.Split(';')[2];
                if (item.Split(';')[6] == "OK")
                    LedToAdd.TestResult = true;
                else
                {
                    LedToAdd.TestResult = true;
                    LedToAdd.FailureReason = item.Split(';')[7];
                }

                LedModulesList.Add(LedToAdd);
            }

            return LedModulesList;
        }
    }
}
