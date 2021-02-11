using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kyun.game.Winforms
{
    public partial class Experiments : Form
    {
        public Experiments()
        {
            InitializeComponent();
        }

        private void Experiments_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = Settings1.Default.DoubleRender;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.DoubleRender = checkBox1.Checked;
            Settings1.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            KyunGame.Instance.AutoMode = checkBox2.Checked;
        }
    }
}
