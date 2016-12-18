using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ubeat.ElCosoQueSirveParaLasOpcionesDelJuegoYOtrasWeas
{
    public partial class SuperSecretSettings : Form
    {
        public SuperSecretSettings()
        {
            InitializeComponent();
        }

        private void SuperSecretSettings_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Direct Frame Render");
            comboBox1.Items.Add("Render and skip");
            comboBox1.Text = comboBox1.Items[Settings1.Default.VideoMode].ToString();

            if(Settings1.Default.VideoMode>0)
                numericUpDown1.Enabled = true;
            else
                numericUpDown1.Enabled = false;

            numericUpDown1.Value = Settings1.Default.VideoFrameSkip;

            comboBox1.SelectedValueChanged += comboBox1_SelectedValueChanged;
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
        }

        void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Settings1.Default.VideoFrameSkip = (int)numericUpDown1.Value;
            Settings1.Default.Save();
        }

        void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            Settings1.Default.VideoMode = comboBox1.SelectedIndex;
            if (comboBox1.SelectedIndex > 0)
            {
                numericUpDown1.Enabled = true;
                Settings1.Default.VideoFrameSkip = (int)numericUpDown1.Value;
            }
            else
            {
                numericUpDown1.Enabled = false;
            }

            Settings1.Default.Save();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
