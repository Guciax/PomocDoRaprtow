﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    class SqlTableLoader
    {
        public static DataTable LoadTesterWorkCard()
        {
            DataTable result = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTDB\SQLEXPRESS;Initial Catalog=Sparing2;Integrated Security=True";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT serial_no, inspection_time, tester_id, wip_entity_name, result, ng_type, STATUS FROM dbo.v_tester_measurements_ZlecenieGlowne";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            return result;
        }

        public static DataTable LoadWasteTable()
        {
            DataTable result = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTDB\SQLEXPRESS;Initial Catalog=Sparing2;Integrated Security=True";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT id,Nr_Planu_Produkcji,Nr_Zlecenia_Produkcyjnego,BrakLutowia,BrakKomponentu,ZabrudzonaDioda,UszkodzonaDioda,UszkodzonePCB,PrzesuniecieDiody,ZanieczyszczenieZpieca,Inne FROM dbo.tb_Zlecenia_produkcyjne_Karta_Pracy";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            return result;
        }
    }
}