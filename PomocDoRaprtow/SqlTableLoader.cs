using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    class SqlTableLoader
    {
        public static void LoadLotTable(string date)
        {
            DataTable result = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTDB\SQLEXPRESS;Initial Catalog=Sparing2;Integrated Security=True";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT Nr_Planu_Produkcji, Nr_Zlecenia_Produkcyjnego, NC12_wyrobu, Ilosc_wyrobu_zlecona, RankA, RankB, MRM, STATUS, Ilosc_wyr_dobrego, Ilosc_wyr_do_poprawy, Ilosc_wyr_na_zlom, DataCzasWydruku FROM dbo.tb_Zlecenia_produkcyjne ;";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            SaveDataTableToCsv(result, @"DB\Zlecenia_produkcyjne.csv");

        }

        public static void LoadTesterWorkCard(string date)
        {
            DataTable result = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTDB\SQLEXPRESS;Initial Catalog=Sparing2;Integrated Security=True";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT serial_no, inspection_time, tester_id, wip_entity_name, result, ng_type, STATUS FROM dbo.v_tester_measurements_ZlecenieGlowne WHERE ISNUMERIC(wip_entity_name)= 1 AND (inspection_time LIKE '%2017-10%' OR inspection_time LIKE '%10____2017%') ;";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            SaveDataTableToCsv(result, @"DB\tester.csv");

        }

        public static void SaveDataTableToCsv(DataTable dt, string filename)
        {
            StringBuilder sb = new StringBuilder();

            string[] columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();
            sb.AppendLine(string.Join(";", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                List<string> rowToSave = new List<string>();
                foreach (var cell in row.ItemArray)
                {
                    if (cell.ToString()=="") rowToSave.Add("NULL");
                    else rowToSave.Add(cell.ToString());
                }
                sb.AppendLine(string.Join(";", rowToSave.ToArray()));
            }

            File.WriteAllText(filename, sb.ToString());
        }

        public static void LoadWasteTable(string date)
        {
            DataTable result = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTDB\SQLEXPRESS;Initial Catalog=Sparing2;Integrated Security=True";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT Nr_Zlecenia_Produkcyjnego,BrakLutowia,BrakKomponentu,ZabrudzonaDioda,UszkodzonaDioda,UszkodzonePCB,PrzesuniecieDiody,ZanieczyszczenieZpieca,Inne,DataCzas FROM dbo.tb_Zlecenia_produkcyjne_Karta_Pracy WHERE DataCzas >= '20171001 00:00:00.000'";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            SaveDataTableToCsv(result, @"DB\odpady.csv");
        }

        public static void LoadBoxingTable(string date)
        {
            DataTable result = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTDB\SQLEXPRESS;Initial Catalog=Sparing2;Integrated Security=True";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT serial_no,Box_LOT_NO,Palet_LOT_NO,Boxing_Date,Palletising_Date FROM dbo.tb_WyrobLG_opakowanie WHERE Boxing_Date  >= '20171001 00:00:00.000'";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            SaveDataTableToCsv(result, @"DB\WyrobLG_opakowanie.csv");
        }
    }
}