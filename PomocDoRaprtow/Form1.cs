using System;
using System.Windows.Forms;


using System.Data;

namespace PomocDoRaprtow
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            dateTimePicker_odpad_do.Format = DateTimePickerFormat.Custom;
            dateTimePicker_odpad_od.Format = DateTimePickerFormat.Custom;
            dateTimePicker_odpad_do.CustomFormat = "dd-MM-yyyy hh:mm:ss";
            dateTimePicker_odpad_od.CustomFormat = "dd-MM-yyyy hh:mm:ss";
        }
            

        private void Form1_Load(object sender, EventArgs e)
        { 

        }
        DataTable Odpady_table = new DataTable();
        DataTable Tester_table = new DataTable();
        DataTable LOT_Module_Table = new DataTable(); //Nr_Zlecenia_Produkcyjnego NC12_wyrobu RankA RankB MRM

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable hist = TableOperations.HistogramTable(Odpady_table, new int[] { 3, 4, 5, 6, 7, 8, 9, 10 });
            dataGridView1.DataSource = hist;
            
            Charting.BarChart(chart1, hist, 0, 1);
        }

        public string LOT_to_Model(string LOT)
        {
            string result="";

            foreach (DataRow row in LOT_Module_Table.Rows)
            {
                if (row["Nr_Zlecenia_Produkcyjnego"].ToString() == LOT)
                {
                    result = row["NC12_wyrobu"].ToString();
                    break;//
                }
            }

            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTable shifts = TableOperations.Tester_IloscNaZmiane(Tester_table);
            dataGridView1.DataSource = shifts;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Charting.Shift_Line_chart(chart1, shifts, 0, 1, 2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Odpady_table = Load_SQL_tables.Load_odpad_table();
            Tester_table = Load_SQL_tables.Load_tester_karta_pracy();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Odpady_table = LodFiles.Odpad_Table();
            Tester_table = LodFiles.tester_Table();
            LOT_Module_Table = LodFiles.LOT_Module_Table();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Tab.SelectedTab.Name == "tab_odpad")
            {
                DataTable hist = TableOperations.HistogramTable(Odpady_table, new int[] { 3, 4, 5, 6, 7, 8, 9, 10 });
                dataGridView_odpad.DataSource = hist;
                Charting.BarChart(chart_odpad, hist, 0, 1);
                
            }
        }

        private void dateTimePicker_odpad_od_ValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}
