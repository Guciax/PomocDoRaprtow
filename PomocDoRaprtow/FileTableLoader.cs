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
        
        public static List<Led> LoadTesterWorkCard2()
        {
            return null;
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
    }
}
