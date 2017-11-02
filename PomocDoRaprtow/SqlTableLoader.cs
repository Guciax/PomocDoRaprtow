using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PomocDoRaprtow
{
    class SqlTableLoader
    {
        private readonly Form1 form;
        private readonly RichTextBox richConsole;

        public SqlTableLoader(Form1 form, RichTextBox richConsole)
        {
            this.form = form;
            this.richConsole = richConsole;
        }

        public static void launchDbLoadSequence(RichTextBox console, string date)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                LoadLotTable(date, console);
                new Thread(() =>
                {
                    LoadTesterWorkCard(30, console);
                    new Thread(() =>
                    {
                        LoadWasteTable(date, console);
                        new Thread(() =>
                        {
                            LoadBoxingTable(date, console);
                            
                        }).Start();
                    }).Start();
                }).Start();
            }).Start();
        }

      
        public static void writeToConsole(RichTextBox console, string text)
        {
            console.Invoke((MethodInvoker)delegate
            {
                console.AppendText(text);
            });
        }


        public static void LoadLotTable(string date, RichTextBox console)
        {
            writeToConsole(console, "Synchronizing with dbo.tb_Zlecenia_produkcyjne...");
                

                DataTable result = new DataTable();

                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = @"Data Source=MSTDB\SQLEXPRESS;Initial Catalog=Sparing2;Integrated Security=True";

                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.CommandText =
                    @"SELECT Nr_Planu_Produkcji, Nr_Zlecenia_Produkcyjnego, NC12_wyrobu, Ilosc_wyrobu_zlecona, RankA, RankB, MRM, STATUS, Ilosc_wyr_dobrego, Ilosc_wyr_do_poprawy, Ilosc_wyr_na_zlom, DataCzasWydruku FROM dbo.tb_Zlecenia_produkcyjne ;";

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(result);

                SaveDataTableToCsv(result, @"DB\tb_Zlecenia_produkcyjne.csv", console);

            writeToConsole(console, "Done. \n");
    }

        public static void LoadTesterWorkCard(int fromMonth, RichTextBox console)
        {
            writeToConsole(console, "Synchronizing with dbo.tb_tester_measurements...");

            

            DataTable result = new DataTable();
            DateTime dateSince = DateTime.Now;
            string dateCriteriaVer1 = "'%2017-10%'";//"'%" + dateSince.Year.ToString() + "-" + fromMonth.ToString() + "%'";
            string dateCriteriaVer2 = "'%10____2017%'"; //"'%" + fromMonth.ToString() + "____" + dateSince.Year.ToString() + "%'";

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTDB\SQLEXPRESS;Initial Catalog=Sparing2;Integrated Security=True";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT serial_no, inspection_time, tester_id, wip_entity_name, result, ng_type FROM dbo.tb_tester_measurements WHERE ISNUMERIC(wip_entity_name)= 1 AND (inspection_time LIKE " + dateCriteriaVer1 + " OR inspection_time LIKE " + dateCriteriaVer2 + ") ; ";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            SaveDataTableToCsv(result, @"DB\tb_tester_measurements.csv", console);

            writeToConsole(console, "Done. \n");

        }

        public static void SaveDataTableToCsv(DataTable dt, string filename, RichTextBox console)
        {
            writeToConsole(console, "Saving file...");
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

            File.WriteAllText(filename, "Last synchro: " + DateTime.Now.ToString()+Environment.NewLine);
            File.AppendAllText(filename, sb.ToString());
        }

        public static void LoadWasteTable(string date, RichTextBox console)
        {
            writeToConsole(console, "Synchronizing with dbo.tb_Zlecenia_produkcyjne_Karta_Pracy...");

                DataTable result = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTDB\SQLEXPRESS;Initial Catalog=Sparing2;Integrated Security=True";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT Nr_Zlecenia_Produkcyjnego,BrakLutowia,BrakKomponentu,ZabrudzonaDioda,UszkodzonaDioda,UszkodzonePCB,PrzesuniecieDiody,ZanieczyszczenieZpieca,Inne,DataCzas FROM dbo.tb_Zlecenia_produkcyjne_Karta_Pracy WHERE DataCzas >= '20171001 00:00:00.000'";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            SaveDataTableToCsv(result, @"DB\tb_Zlecenia_produkcyjne_Karta_Pracy.csv", console);
            writeToConsole(console, "Done. \n");
        }

        public static void LoadBoxingTable(string date, RichTextBox console)
        {
            writeToConsole(console, "Synchronizing with dbo.tb_WyrobLG_opakowanie...");


                DataTable result = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTDB\SQLEXPRESS;Initial Catalog=Sparing2;Integrated Security=True";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT serial_no,Box_LOT_NO,Palet_LOT_NO,Boxing_Date,Palletising_Date FROM dbo.tb_WyrobLG_opakowanie WHERE Boxing_Date  >= '20171001 00:00:00.000'";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            SaveDataTableToCsv(result, @"DB\tb_WyrobLG_opakowanie.csv", console);
            writeToConsole(console, "Done. \n");
        }
    }
}