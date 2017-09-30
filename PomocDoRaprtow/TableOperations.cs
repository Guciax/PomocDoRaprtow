using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;

namespace PomocDoRaprtow
{
    class TableOperations
    {
        public static DataTable HistogramTable(DataTable inputTable, int[] valueColumn, OptionProvider optProv)
        {
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("Name");
            resultTable.Columns.Add("Sum", typeof (Int16));
            resultTable.Columns.Add("Date", typeof(DateTime));

            foreach (var col in valueColumn)
            {
                resultTable.Rows.Add(inputTable.Columns[col].ColumnName, 0);
            }

            foreach (DataRow row in inputTable.Rows)
            {
                for (int i=0;i<valueColumn.Length;i++) 
                {
                    int value = 0;
                    Int32.TryParse(row[valueColumn[i]].ToString(), out value);
                    DateTime czas = DateTime.ParseExact(row["DataCzas"].ToString(), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
                    if (czas > optProv.OdpadBegin && czas < optProv.OdpadEnd) 
                        resultTable.Rows[i][1] = (Int16)resultTable.Rows[i][1] + value;
                    
                }
            }
            DataView dv = resultTable.DefaultView;
            dv.Sort = "Sum desc";
            resultTable = dv.ToTable();


            return resultTable;
        }

        public static DataTable Tester_IloscNaZmiane(DataTable inputTable)
        {
            DataTable resultTable_1 = new DataTable();
            resultTable_1.Columns.Add("Date");
            resultTable_1.Columns.Add("Shift");
            resultTable_1.Columns.Add("Qauntity", typeof(int));

            DataTable resultTable_2 = new DataTable();
            resultTable_2.Columns.Add("Date");
            resultTable_2.Columns.Add("Shift");
            resultTable_2.Columns.Add("Qauntity", typeof(int));

            DataTable resultTable_3 = new DataTable();
            resultTable_3.Columns.Add("Date");
            resultTable_3.Columns.Add("Shift");
            resultTable_3.Columns.Add("Qauntity", typeof(int));

            List<string> ShiftList_1 = new List<string>();
            List<string> ShiftList_2 = new List<string>();
            List<string> ShiftList_3 = new List<string>();

            foreach (DataRow row in inputTable.Rows)
            {
                DateTime Czas = new DateTime();
                if (!DateTime.TryParseExact(row["inspection_time"].ToString(), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out Czas))
                {
                    Debug.WriteLine(row["inspection_time"].ToString() + " failed");
                    continue;
                }
                string date_shift = "";
                if (Czas.Hour >= 22)
                {
                    date_shift = Czas.AddDays(1).ToString("dd-MM-yyyy" + "_" + "3");
                }

                else if (Czas.Hour >= 14)
                {
                    date_shift = Czas.ToString("dd-MM-yyyy" + "_" + "2");
                }
                else if (Czas.Hour >= 6)
                {
                    date_shift = Czas.ToString("dd-MM-yyyy" + "_" + "1");
                }
                else if (Czas.Hour <6)
                {
                    date_shift = Czas.ToString("dd-MM-yyyy" + "_" + "3");
                }

                if (date_shift.Contains("_1"))
                if (!ShiftList_1.Contains(date_shift))
                {
                        ShiftList_1.Add(date_shift);
                    DateTime date_toInsert = DateTime.ParseExact(date_shift.Split('_')[0], "dd-MM-yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    resultTable_1.Rows.Add(date_shift.Split('_')[0], date_shift.Split('_')[1], 1);
                }
                else
                {
                    resultTable_1.Rows[ShiftList_1.IndexOf(date_shift)][2] = Int16.Parse(resultTable_1.Rows[ShiftList_1.IndexOf(date_shift)][2].ToString()) + 1;
                }

                if (date_shift.Contains("_2"))
                    if (!ShiftList_2.Contains(date_shift))
                    {
                        ShiftList_2.Add(date_shift);
                        DateTime date_toInsert = DateTime.ParseExact(date_shift.Split('_')[0], "dd-MM-yyyy", System.Globalization.CultureInfo.CurrentCulture);
                        resultTable_2.Rows.Add(date_shift.Split('_')[0], date_shift.Split('_')[1], 1);
                    }
                    else
                    {
                        resultTable_2.Rows[ShiftList_2.IndexOf(date_shift)][2] = Int16.Parse(resultTable_2.Rows[ShiftList_2.IndexOf(date_shift)][2].ToString()) + 1;
                    }

                if (date_shift.Contains("_3"))
                    if (!ShiftList_3.Contains(date_shift))
                    {
                        ShiftList_3.Add(date_shift);
                        DateTime date_toInsert = DateTime.ParseExact(date_shift.Split('_')[0], "dd-MM-yyyy", System.Globalization.CultureInfo.CurrentCulture);
                        resultTable_3.Rows.Add(date_shift.Split('_')[0], date_shift.Split('_')[1], 1);
                    }
                    else
                    {
                        resultTable_3.Rows[ShiftList_3.IndexOf(date_shift)][2] = Int16.Parse(resultTable_3.Rows[ShiftList_3.IndexOf(date_shift)][2].ToString()) + 1;
                    }
            }

            DataView dv = resultTable_1.DefaultView;
            dv.Sort = "Date asc";
            DataTable sorted_1 = dv.ToTable();

            DataView dv2 = resultTable_2.DefaultView;
            dv2.Sort = "Date asc";
            DataTable sorted_2 = dv2.ToTable();

            DataView dv3 = resultTable_3.DefaultView;
            dv3.Sort = "Date asc";
            DataTable sorted_3 = dv3.ToTable();

            for (int i1 = 0; i1 < sorted_1.Rows.Count; i1++)
            {
                DateTime data1 = DateTime.ParseExact(sorted_1.Rows[i1]["Date"].ToString(), "dd-MM-yyyy", System.Globalization.CultureInfo.CurrentCulture);
                for (int i2 = 0; i2 < sorted_2.Rows.Count; i2++) 
                {
                    DateTime data2 = DateTime.ParseExact(sorted_2.Rows[i2]["Date"].ToString(), "dd-MM-yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    if (data1 <= data2 )
                    {
                        DataRow nr = sorted_2.NewRow();
                        nr[0] = sorted_1.Rows[i1][0];
                        nr[1] = sorted_1.Rows[i1][1];
                        nr[2] = sorted_1.Rows[i1][2];
                        sorted_2.Rows.InsertAt(nr, i2);
                        break;
                    }
                    if(i2 == sorted_2.Rows.Count - 1)
                    {
                        DataRow nr = sorted_2.NewRow();
                        nr[0] = sorted_1.Rows[i1][0];
                        nr[1] = sorted_1.Rows[i1][1];
                        nr[2] = sorted_1.Rows[i1][2];
                        sorted_2.Rows.Add(nr);
                        break;
                    }
                }
            }

            for (int i3 = 0; i3 < sorted_3.Rows.Count; i3++)
            {
                DateTime data3 = DateTime.ParseExact(sorted_3.Rows[i3]["Date"].ToString(), "dd-MM-yyyy", System.Globalization.CultureInfo.CurrentCulture);
                for (int i2 = 0; i2 < sorted_2.Rows.Count; i2++)
                {
                    DateTime data2 = DateTime.ParseExact(sorted_2.Rows[i2]["Date"].ToString(), "dd-MM-yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    if (data3 <= data2)
                    {
                        DataRow nr = sorted_2.NewRow();
                        nr[0] = sorted_3.Rows[i3][0];
                        nr[1] = sorted_3.Rows[i3][1];
                        nr[2] = sorted_3.Rows[i3][2];
                        sorted_2.Rows.InsertAt(nr, i2);
                        break;
                    }
                    if (i2 == sorted_2.Rows.Count - 1)
                    {
                        DataRow nr = sorted_2.NewRow();
                        nr[0] = sorted_3.Rows[i3][0];
                        nr[1] = sorted_3.Rows[i3][1];
                        nr[2] = sorted_3.Rows[i3][2];
                        sorted_2.Rows.Add(nr);
                        break;
                    }
                }
            }

            return sorted_2;
        }

        
    }
}
