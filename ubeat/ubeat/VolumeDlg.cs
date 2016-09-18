using System;
using System.Windows.Forms;

namespace ubeat
{
    public partial class VolumeDlg : Form
    {
        public VolumeDlg()
        {
            InitializeComponent();
            tm = new Timer() { Interval = 2500 };
            tm.Tick += tm_Tick;
            this.Show();
            this.Hide();
           
        }
        Timer tm;
        private void VolumeDlg_Load(object sender, EventArgs e)
        {
            
        }

        void tm_Tick(object sender, EventArgs e)
        {
            this.Visible = false;
            tm.Stop();
        }
        public void VolShow()
        {
            int vol = (int)(UbeatGame.Instance.GeneralVolume * 100f);

            label1.Text = vol.ToString();
            progressBar1.Value = vol;
            this.Width = 55;
            this.Top = ((Form)FromHandle(UbeatGame.Instance.Window.Handle)).Top + 300;
            this.Left = ((Form)FromHandle(UbeatGame.Instance.Window.Handle)).Left + 5;
            this.TopMost = true;
            this.Visible = true;
            this.Width = 55;
            tm.Start();
        }
        
    }
}
