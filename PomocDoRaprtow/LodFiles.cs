using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    class LodFiles
    {
        public static DataTable Odpad_Table()
        {
            DataTable resultTable = new DataTable();
            string[] plikArray = System.IO.File.ReadAllLines(@"D:\Google Drive\Praca\DB\Odpad new.txt");
            foreach (var item in plikArray[0].Split(';'))
            {
                resultTable.Columns.Add(item);
            }

            for (int i=1;i<plikArray.Length;i++)
            {
                resultTable.Rows.Add(plikArray[i].Split(';'));
            }

            return resultTable;
        }

        public static DataTable tester_Table()
        {
            DataTable resultTable = new DataTable();
            string[] plikArray = System.IO.File.ReadAllLines(@"D:\Google Drive\Praca\DB\tester.csv");
            foreach (var item in plikArray[0].Split(';'))
            {
                resultTable.Columns.Add(item);
            }

            for (int i = 1; i < plikArray.Length; i++)
            {
                resultTable.Rows.Add(plikArray[i].Split(';'));
            }

            return resultTable;
        }

        public static DataTable LOT_Module_Table()
        {
            DataTable result = new DataTable();
            string[] plikArray = System.IO.File.ReadAllLines(@"D:\Google Drive\Praca\DB\Zlecenia_produkcyjne.txt");
            result.Columns.Add("Nr_Zlecenia_Produkcyjnego");
            result.Columns.Add("NC12_wyrobu");
            result.Columns.Add("RankA");
            result.Columns.Add("RankB");
            result.Columns.Add("MRM");

            int index_Nr_Zlecenia_Produkcyjnego = Array.IndexOf(plikArray[0].Split(';'), "Nr_Zlecenia_Produkcyjnego");
            int index_NC12_wyrobu = Array.IndexOf(plikArray[0].Split(';'), "NC12_wyrobu");
            int index_RankA = Array.IndexOf(plikArray[0].Split(';'), "RankA");
            int index_RankB = Array.IndexOf(plikArray[0].Split(';'), "RankB");
            int index_MRM = Array.IndexOf(plikArray[0].Split(';'), "MRM");

            foreach (var row in plikArray)
            {
                result.Rows.Add(row.Split(';')[index_Nr_Zlecenia_Produkcyjnego],
                                                row.Split(';')[index_NC12_wyrobu],
                                                row.Split(';')[index_RankA],
                                                row.Split(';')[index_RankB],
                                                row.Split(';')[index_MRM]);
            }

            return result;
        }
    }
}
