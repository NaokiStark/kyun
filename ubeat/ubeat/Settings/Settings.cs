using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ubeat.ElCosoQueSirveParaLasOpcionesDelJuegoYOtrasWeas
{
    public partial class Settings : Form
    {
        public static Settings Instance = null;
        List<Screen.ScreenMode> scrnm = new List<Screen.ScreenMode>();
        public Settings()
        {
            Instance = this;
            InitializeComponent();
            this.FormClosed += Settings_FormClosed;
        }

        void Settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Instance = null;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            scrnm = Screen.ScreenModeManager.GetSupportedModes();

            foreach (Screen.ScreenMode mode in scrnm)
            {
                comboDisplay.Items.Add(string.Format("({0}x{1}){2}", mode.Width, mode.Height, (mode.WindowMode == Screen.WindowDisposition.Windowed)?"[Windowed]":""));
            }

            comboDisplay.SelectedItem = Settings1.Default.ScreenMode;
            comboDisplay.Text = comboDisplay.Items[Settings1.Default.ScreenMode].ToString();

            comboDisplay.SelectedIndexChanged += comboDisplay_SelectedIndexChanged;
           
                       
            comboLang.Text = "English (default)";
            comboLang.Items.Add("English (default)");

            comboFrameRate.Text = Settings1.Default.FrameRate.ToString();
            comboFrameRate.Items.Add("25");
            comboFrameRate.Items.Add("30");
            comboFrameRate.Items.Add("60");
            comboFrameRate.Items.Add("100");
            comboFrameRate.Items.Add("250");
            comboFrameRate.Items.Add("1000");
            comboFrameRate.SelectedIndexChanged += comboFrameRate_SelectedIndexChanged;

            VSyncChk.Checked = Settings1.Default.VSync;
            comboFrameRate.Enabled = !VSyncChk.Checked;
            VSyncChk.CheckStateChanged += VSyncChk_CheckStateChanged;

            checkVideo.Checked = Settings1.Default.Video;
            checkVideo.CheckStateChanged += checkVideo_CheckStateChanged;
            textOsu.Text = Settings1.Default.osuBeatmaps;

            chkFullScreen.Checked = Settings1.Default.FullScreen;
            chkFullScreen.CheckStateChanged += chkFullScreen_CheckStateChanged;
        }

        void chkFullScreen_CheckStateChanged(object sender, EventArgs e)
        {
            UbeatGame.Instance.ToggleFullscreen(chkFullScreen.Checked);
            Settings1.Default.FullScreen = chkFullScreen.Checked;
            Settings1.Default.Save();
        }

        void VSyncChk_CheckStateChanged(object sender, EventArgs e)
        {
            comboFrameRate.Enabled = !((CheckBox)sender).Checked;
            UbeatGame.Instance.ToggleVSync(((CheckBox)sender).Checked);
            Logger.Instance.Info("Setting VSync: " + ((CheckBox)sender).Checked.ToString());
            Settings1.Default.VSync = ((CheckBox)sender).Checked;
            Settings1.Default.Save();
        }

        void checkVideo_CheckStateChanged(object sender, EventArgs e)
        {
            Logger.Instance.Info("Setting Video: " + ((CheckBox)sender).Checked.ToString());
            UbeatGame.Instance.VideoEnabled = ((CheckBox)sender).Checked;
            Settings1.Default.Video = ((CheckBox)sender).Checked;
            Settings1.Default.Save();
        }

        void comboDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.Instance.Info("Setting Display Mode: {0}", ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex]);
            Settings1.Default.ScreenMode = ((ComboBox)sender).SelectedIndex;
            Settings1.Default.Save();
            UbeatGame.Instance.ChangeResolution(scrnm[((ComboBox)sender).SelectedIndex]);
        }

        void comboFrameRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.Instance.Info("Setting Frame Rate: {0}", ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString());
            Settings1.Default.FrameRate = float.Parse(((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString());
            Settings1.Default.Save();
            UbeatGame.Instance.ChangeFrameRate(float.Parse(((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString()));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("{0}\n\r{1}","ubeat Alpha 0.1","Fabi Stark Corp."), "About ubeat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bOsuBM_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.ShowDialog();

            if (fbd.SelectedPath == "") return;

            DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);

            if (di.Name != "Songs")
            {
                MessageBox.Show("Sorry, this folder is invalid","ubeat",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            Settings1.Default.osuBeatmaps = di.FullName;
            Settings1.Default.Save();
            textOsu.Text = di.FullName;
            MessageBox.Show("ubeat needs to restart to reload maps.", "ubeat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lDefault_Click(object sender, EventArgs e)
        {
            DialogResult gg = MessageBox.Show("Warning!, this config will be reset!!!\n\rAre you sure?", "ubeat", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (gg == System.Windows.Forms.DialogResult.No)
                return;
            else
            {
                Settings1.Default.Reset();
                MessageBox.Show("Done.\n\rRestart ubeat to apply changes.", "ubeat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void sss_Click(object sender, EventArgs e)
        {
            new SuperSecretSettings().Show();
        }

        private void chkFullScreen_CheckedChanged(object sender, EventArgs e)
        {

        }

        
    }
}
