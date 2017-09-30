namespace PomocDoRaprtow
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Wymagana metoda obsługi projektanta — nie należy modyfikować 
        /// zawartość tej metody z edytorem kodu.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dateTimePicker_odpad_od = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_odpad_do = new System.Windows.Forms.DateTimePicker();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.Tab = new System.Windows.Forms.TabControl();
            this.tab_wydajnosc = new System.Windows.Forms.TabPage();
            this.tab_odpad = new System.Windows.Forms.TabPage();
            this.chart_odpad = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dataGridView_odpad = new System.Windows.Forms.DataGridView();
            this.comboBox_odpad_model = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.Tab.SuspendLayout();
            this.tab_odpad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_odpad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_odpad)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 123);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 37);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1 file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 165);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(452, 458);
            this.dataGridView1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(129, 123);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(109, 37);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(469, 165);
            this.chart1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(597, 458);
            this.chart1.TabIndex = 3;
            this.chart1.Text = "chart1";
            // 
            // dateTimePicker_odpad_od
            // 
            this.dateTimePicker_odpad_od.Location = new System.Drawing.Point(6, 5);
            this.dateTimePicker_odpad_od.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dateTimePicker_odpad_od.Name = "dateTimePicker_odpad_od";
            this.dateTimePicker_odpad_od.ShowCheckBox = true;
            this.dateTimePicker_odpad_od.Size = new System.Drawing.Size(299, 22);
            this.dateTimePicker_odpad_od.TabIndex = 4;
            this.dateTimePicker_odpad_od.Value = new System.DateTime(2017, 9, 29, 0, 0, 0, 0);
            this.dateTimePicker_odpad_od.ValueChanged += new System.EventHandler(this.dateTimePicker_odpad_od_ValueChanged);
            // 
            // dateTimePicker_odpad_do
            // 
            this.dateTimePicker_odpad_do.Location = new System.Drawing.Point(6, 31);
            this.dateTimePicker_odpad_do.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dateTimePicker_odpad_do.Name = "dateTimePicker_odpad_do";
            this.dateTimePicker_odpad_do.ShowCheckBox = true;
            this.dateTimePicker_odpad_do.Size = new System.Drawing.Size(299, 22);
            this.dateTimePicker_odpad_do.TabIndex = 5;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(749, 26);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 11);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(109, 37);
            this.button4.TabIndex = 7;
            this.button4.Text = "SQL";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(129, 12);
            this.button5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(109, 37);
            this.button5.TabIndex = 8;
            this.button5.Text = "CSV";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Tab
            // 
            this.Tab.Controls.Add(this.tab_wydajnosc);
            this.Tab.Controls.Add(this.tab_odpad);
            this.Tab.Location = new System.Drawing.Point(12, 54);
            this.Tab.Name = "Tab";
            this.Tab.SelectedIndex = 0;
            this.Tab.Size = new System.Drawing.Size(1051, 549);
            this.Tab.TabIndex = 9;
            this.Tab.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tab_wydajnosc
            // 
            this.tab_wydajnosc.Location = new System.Drawing.Point(4, 25);
            this.tab_wydajnosc.Name = "tab_wydajnosc";
            this.tab_wydajnosc.Padding = new System.Windows.Forms.Padding(3);
            this.tab_wydajnosc.Size = new System.Drawing.Size(1043, 520);
            this.tab_wydajnosc.TabIndex = 0;
            this.tab_wydajnosc.Text = "Wydajność";
            this.tab_wydajnosc.UseVisualStyleBackColor = true;
            // 
            // tab_odpad
            // 
            this.tab_odpad.Controls.Add(this.comboBox_odpad_model);
            this.tab_odpad.Controls.Add(this.dataGridView_odpad);
            this.tab_odpad.Controls.Add(this.chart_odpad);
            this.tab_odpad.Controls.Add(this.dateTimePicker_odpad_od);
            this.tab_odpad.Controls.Add(this.dateTimePicker_odpad_do);
            this.tab_odpad.Location = new System.Drawing.Point(4, 25);
            this.tab_odpad.Name = "tab_odpad";
            this.tab_odpad.Padding = new System.Windows.Forms.Padding(3);
            this.tab_odpad.Size = new System.Drawing.Size(1043, 520);
            this.tab_odpad.TabIndex = 1;
            this.tab_odpad.Text = "Odpad";
            this.tab_odpad.UseVisualStyleBackColor = true;
            // 
            // chart_odpad
            // 
            chartArea2.Name = "ChartArea1";
            this.chart_odpad.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart_odpad.Legends.Add(legend2);
            this.chart_odpad.Location = new System.Drawing.Point(6, 58);
            this.chart_odpad.Name = "chart_odpad";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart_odpad.Series.Add(series2);
            this.chart_odpad.Size = new System.Drawing.Size(646, 456);
            this.chart_odpad.TabIndex = 6;
            this.chart_odpad.Text = "chart2";
            // 
            // dataGridView_odpad
            // 
            this.dataGridView_odpad.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_odpad.Location = new System.Drawing.Point(638, 24);
            this.dataGridView_odpad.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView_odpad.Name = "dataGridView_odpad";
            this.dataGridView_odpad.RowTemplate.Height = 24;
            this.dataGridView_odpad.Size = new System.Drawing.Size(349, 458);
            this.dataGridView_odpad.TabIndex = 7;
            // 
            // comboBox_odpad_model
            // 
            this.comboBox_odpad_model.FormattingEnabled = true;
            this.comboBox_odpad_model.Location = new System.Drawing.Point(356, 7);
            this.comboBox_odpad_model.Name = "comboBox_odpad_model";
            this.comboBox_odpad_model.Size = new System.Drawing.Size(121, 24);
            this.comboBox_odpad_model.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1093, 636);
            this.Controls.Add(this.Tab);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.Tab.ResumeLayout(false);
            this.tab_odpad.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_odpad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_odpad)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DateTimePicker dateTimePicker_odpad_od;
        private System.Windows.Forms.DateTimePicker dateTimePicker_odpad_do;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TabControl Tab;
        private System.Windows.Forms.TabPage tab_wydajnosc;
        private System.Windows.Forms.TabPage tab_odpad;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_odpad;
        private System.Windows.Forms.DataGridView dataGridView_odpad;
        private System.Windows.Forms.ComboBox comboBox_odpad_model;
    }
}

