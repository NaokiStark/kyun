namespace FreqData
{
    partial class Mapper
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.opnBtn = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tLengthlbl = new System.Windows.Forms.Label();
            this.actPoslbl = new System.Windows.Forms.Label();
            this.statusLbl = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tEff = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tMusic = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.tMetronome = new System.Windows.Forms.TrackBar();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.button4 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.tEff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tMusic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tMetronome)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // opnBtn
            // 
            this.opnBtn.Location = new System.Drawing.Point(12, 507);
            this.opnBtn.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.opnBtn.Name = "opnBtn";
            this.opnBtn.Size = new System.Drawing.Size(86, 86);
            this.opnBtn.TabIndex = 1;
            this.opnBtn.Text = "Open MP3";
            this.opnBtn.UseVisualStyleBackColor = true;
            this.opnBtn.Click += new System.EventHandler(this.opnBtn_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1/1",
            "1/2",
            "1/4",
            "1/8",
            "1/16",
            "1/3",
            "1/6",
            "1/12",
            "1/24",
            "1/32"});
            this.comboBox1.Location = new System.Drawing.Point(316, 13);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(86, 25);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(207, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 22);
            this.label1.TabIndex = 3;
            this.label1.Text = "Divider";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(207, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 78);
            this.label2.TabIndex = 4;
            this.label2.Text = "Recommended: \r\nEasy-Normal: 1/2- 1/4\r\nHard: 1/8-1/16\r\nInsane: 1/16";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 113);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(184, 86);
            this.button1.TabIndex = 5;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(14, 209);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(184, 86);
            this.button2.TabIndex = 6;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(14, 305);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(184, 86);
            this.button3.TabIndex = 7;
            this.button3.Text = "Save osu!Beatmap";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tLengthlbl
            // 
            this.tLengthlbl.AutoSize = true;
            this.tLengthlbl.Location = new System.Drawing.Point(276, 148);
            this.tLengthlbl.Name = "tLengthlbl";
            this.tLengthlbl.Size = new System.Drawing.Size(63, 17);
            this.tLengthlbl.TabIndex = 8;
            this.tLengthlbl.Text = "00:00,000";
            // 
            // actPoslbl
            // 
            this.actPoslbl.AutoSize = true;
            this.actPoslbl.Location = new System.Drawing.Point(207, 148);
            this.actPoslbl.Name = "actPoslbl";
            this.actPoslbl.Size = new System.Drawing.Size(63, 17);
            this.actPoslbl.TabIndex = 9;
            this.actPoslbl.Text = "00:00,000";
            // 
            // statusLbl
            // 
            this.statusLbl.AutoSize = true;
            this.statusLbl.Location = new System.Drawing.Point(207, 125);
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Size = new System.Drawing.Size(43, 17);
            this.statusLbl.TabIndex = 10;
            this.statusLbl.Text = "Status";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tEff
            // 
            this.tEff.Location = new System.Drawing.Point(223, 277);
            this.tEff.Maximum = 100;
            this.tEff.Name = "tEff";
            this.tEff.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tEff.Size = new System.Drawing.Size(45, 122);
            this.tEff.TabIndex = 11;
            this.tEff.Value = 25;
            this.tEff.Scroll += new System.EventHandler(this.tEff_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(220, 257);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Effects";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(301, 257);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 17);
            this.label4.TabIndex = 14;
            this.label4.Text = "Music";
            // 
            // tMusic
            // 
            this.tMusic.Location = new System.Drawing.Point(304, 277);
            this.tMusic.Maximum = 100;
            this.tMusic.Name = "tMusic";
            this.tMusic.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tMusic.Size = new System.Drawing.Size(45, 122);
            this.tMusic.TabIndex = 13;
            this.tMusic.Value = 50;
            this.tMusic.Scroll += new System.EventHandler(this.tMusic_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(354, 257);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 17);
            this.label5.TabIndex = 16;
            this.label5.Text = "Metronome";
            // 
            // tMetronome
            // 
            this.tMetronome.Location = new System.Drawing.Point(372, 277);
            this.tMetronome.Maximum = 100;
            this.tMetronome.Name = "tMetronome";
            this.tMetronome.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tMetronome.Size = new System.Drawing.Size(45, 122);
            this.tMetronome.TabIndex = 15;
            this.tMetronome.Value = 25;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(210, 178);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(180, 21);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Text = "Use Divider in metronome";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 1;
            this.trackBar1.Location = new System.Drawing.Point(236, 209);
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(166, 45);
            this.trackBar1.TabIndex = 18;
            this.trackBar1.Value = 5;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(14, 17);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(184, 86);
            this.button4.TabIndex = 19;
            this.button4.Text = "Open Beatmap";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Firebrick;
            this.panel1.Location = new System.Drawing.Point(14, 415);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(50, 50);
            this.panel1.TabIndex = 20;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Green;
            this.panel2.Location = new System.Drawing.Point(70, 415);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(50, 50);
            this.panel2.TabIndex = 21;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.CornflowerBlue;
            this.panel3.Location = new System.Drawing.Point(126, 415);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(50, 50);
            this.panel3.TabIndex = 21;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.DarkMagenta;
            this.panel4.Location = new System.Drawing.Point(182, 415);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(50, 50);
            this.panel4.TabIndex = 21;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.MidnightBlue;
            this.panel5.Location = new System.Drawing.Point(357, 415);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(65, 50);
            this.panel5.TabIndex = 22;
            this.panel5.Visible = false;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(238, 440);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(101, 25);
            this.comboBox2.TabIndex = 23;
            // 
            // Mapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 477);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tMetronome);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tMusic);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tEff);
            this.Controls.Add(this.statusLbl);
            this.Controls.Add(this.actPoslbl);
            this.Controls.Add(this.tLengthlbl);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.opnBtn);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Mapper";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mapper";
            this.Load += new System.EventHandler(this.Mapper_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tEff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tMusic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tMetronome)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button opnBtn;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label tLengthlbl;
        private System.Windows.Forms.Label actPoslbl;
        private System.Windows.Forms.Label statusLbl;
        private System.Windows.Forms.Timer timer1;
        internal System.Windows.Forms.TrackBar tEff;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.TrackBar tMusic;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TrackBar tMetronome;
        internal System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ComboBox comboBox2;
    }
}