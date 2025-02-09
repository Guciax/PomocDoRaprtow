﻿using System;
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
        private readonly Button buttonShowTables;

        public SqlTableLoader(Form1 form, RichTextBox richConsole, Button buttonShowTables)
        {
            this.form = form;
            this.richConsole = richConsole;
            this.buttonShowTables = buttonShowTables;
        }

        public static void LaunchDbLoadSequence(RichTextBox console, int daysAgo, Button buttonToToggle)
        {
            ToggleButton(buttonToToggle, false);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                LoadLotTable(daysAgo, console);
                new Thread(() =>
                {
                    LoadTesterWorkCard(daysAgo, console);
                    new Thread(() =>
                    {
                        LoadWasteTable(daysAgo, console);
                        new Thread(() =>
                        {
                            LoadBoxingTable(daysAgo, console);
                            writeToConsole(console, "----------------------------- Synchronization completed ----------------------------- \n");
                            ToggleButton(buttonToToggle, true);

                        }).Start();
                    }).Start();
                }).Start();
            }).Start();
        }

        private static void ToggleButton(Button btn, bool state)
        {
            btn.Invoke((MethodInvoker)delegate
            {
                btn.Enabled = state;
            });
        }
      
        public static void writeToConsole(RichTextBox console, string text)
        {
            console.Invoke((MethodInvoker)delegate
            {
                console.AppendText(text);
            });
        }

        public static DataTable LoadMeasurements(RichTextBox sourceRichBox)
        {

            string[] serials = sourceRichBox.Lines;

            DataTable result = new DataTable();
            DateTime dateSince = DateTime.Now;
            
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTMS010;Initial Catalog=MES;User Id=mes;Password=mes;";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;

            string serialsToCommand = " WHERE serial_no = ";

            for (int i=0;i< serials.Length;i++)
            {
                serialsToCommand += "'"+ serials[i]+"'";
                if (i < serials.Length - 1) serialsToCommand += " OR serial_no = ";
            }
            command.CommandText =
                @"SELECT serial_no, inspection_time, tester_id, wip_entity_name, result, ng_type,lm,lm_w,sdcm,cri,cct,v,i,w,x,y,r9,bin,lx,retest,module_num,lm1_gain,x1_offset,y1_offset,vf1_offset,cri1_offset,cct1_offset,lm1_master,x1_master,y1_master,vf1_master,cri1_master,cct1_master,hi_pot,light_on,optical,result_int FROM dbo.tb_tester_measurements "+ serialsToCommand+"; ";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);
            return result;

        }

        public static void LoadLotTable(int daysAgo, RichTextBox console)
        {
            writeToConsole(console, "Synchronizing with dbo.tb_Zlecenia_produkcyjne...");

            daysAgo = daysAgo + 7;
            string stratDate = DateTime.Now.AddDays(-daysAgo).ToString("yyyy-MM-dd HH:mm:ss");
            DataTable result = new DataTable();

                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = @"Data Source=MSTMS010;Initial Catalog=MES;User Id=mes;Password=mes;";

                SqlCommand command = new SqlCommand();
                command.Connection = conn;
            command.CommandText =
                @"SELECT Nr_Planu_Produkcji, Nr_Zlecenia_Produkcyjnego, NC12_wyrobu, Data_Poczatku_Zlecenia,Ilosc_wyrobu_zlecona, RankA, RankB, MRM, STATUS, Ilosc_wyr_dobrego, Ilosc_wyr_do_poprawy, Ilosc_wyr_na_zlom, DataCzasWydruku FROM dbo.tb_Zlecenia_produkcyjne ; ";

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(result);

                SaveDataTableToCsv(result, @"DB\tb_Zlecenia_produkcyjne.csv", console);

            writeToConsole(console, "Done. \n");
    }

        public static void LoadTesterWorkCard(int daysAgo, RichTextBox console)
        {
            writeToConsole(console, "Synchronizing with dbo.tb_tester_measurements...");

            

            DataTable result = new DataTable();
            DateTime dateSince = DateTime.Now;
            string stratDate = DateTime.Now.AddDays(-daysAgo).ToString("yyyy-MM-dd HH:mm:ss");


            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTMS010;Initial Catalog=MES;User Id=mes;Password=mes;";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT serial_no, inspection_time, tester_id, wip_entity_name, result, ng_type FROM dbo.tb_tester_measurements WHERE ISNUMERIC(wip_entity_name)= 1 AND (inspection_time > '"+stratDate+"' ) ; ";

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

        public static void LoadWasteTable(int daysAgo, RichTextBox console)
        {
            writeToConsole(console, "Synchronizing with dbo.tb_Zlecenia_produkcyjne_Karta_Pracy...");
            string stratDate = DateTime.Now.AddDays(-daysAgo).ToString("yyyy-MM-dd HH:mm:ss");
            DataTable result = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTMS010;Initial Catalog=MES;User Id=mes;Password=mes;";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT Nr_Zlecenia_Produkcyjnego,BrakLutowia,BrakKomponentu,ZabrudzonaDioda,UszkodzonaDioda,UszkodzonePCB,PrzesuniecieDiody,ZanieczyszczenieZpieca,Inne,DataCzas FROM MES.dbo.tb_Zlecenia_produkcyjne_Karta_Pracy;";// WHERE DataCzas > '" + stratDate + "';";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            SaveDataTableToCsv(result, @"DB\tb_Zlecenia_produkcyjne_Karta_Pracy.csv", console);
            writeToConsole(console, "Done. \n");
        }

        public static void LoadBoxingTable(int daysAgo, RichTextBox console)
        {
            writeToConsole(console, "Synchronizing with dbo.tb_WyrobLG_opakowanie...");
            string stratDate = DateTime.Now.AddDays(-daysAgo).ToString("yyyy-MM-dd");

            DataTable result = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTMS010;Initial Catalog=MES;User Id=mes;Password=mes;";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT serial_no,Box_LOT_NO,Palet_LOT_NO,Boxing_Date,Palletising_Date FROM dbo.tb_WyrobLG_opakowanie;"; //WHERE Boxing_Date  > '"+ stratDate+"';";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(result);

            SaveDataTableToCsv(result, @"DB\tb_WyrobLG_opakowanie.csv", console);
            writeToConsole(console, "Done. \n");
        }
    }
}