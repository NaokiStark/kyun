using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ubeat
{
    public partial class LoadingWindow : Form
    {
        public LoadingWindow()
        {
            InitializeComponent();
        }

        private void LoadingWindow_Load(object sender, EventArgs e)
        {
            ((Control)elementHost1).Click += LoadingWindow_Click;
            label1.Click += LoadingWindow_Click;
        }

        private void LoadingWindow_Click(object sender, EventArgs e)
        {
            label1.Text = "POWERED BY PEPPY xdd";
            UbeatGame.Instance.ppyMode = true;
        }
    }
}
