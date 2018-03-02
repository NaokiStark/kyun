using kyun.game.GameScreen.UI;
using kyun.game.NikuClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kyun.game.Winforms
{
    public partial class LoginForm : Form
    {

        public bool assign = false;

        public LoginForm()
        {
            InitializeComponent();
        }


        private void loginbtn_Click(object sender, EventArgs e)
        {
            if (usertx.Text == "" || passtx.Text == "")
                return;

            if (!assign)
            {
                NikuClientApi.LoginFail += NikuClientApi_LoginFail;
                NikuClientApi.LoginSuccess += NikuClientApi_LoginSuccess;
                assign = true;
            }
           
            KyunGame.Instance.server = new NikuClientApi(usertx.Text, passtx.Text, false);

        }

        private void NikuClientApi_LoginSuccess(object sender, EventArgs e)
        {
            UserBox.GetInstance().ReloadAvatar();
            Settings1.Default.User = NikuClientApi.User.Username;
            Settings1.Default.Token = NikuClientApi.User.Token;
            Settings1.Default.Save();

            var isFullScreen = KyunGame.Instance.Graphics.IsFullScreen;
            if (isFullScreen)
                KyunGame.Instance.ToggleFullscreen(true);
            Close();
        }

        private void NikuClientApi_LoginFail(object sender, EventArgs e)
        {
            MessageBox.Show("Login incorrect!\r\n\r\nCheck your user and password, or your internet connection.", "kyun!Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void regbtn_Click(object sender, EventArgs e)
        {
            Process.Start("https://onics.club/register");
        }

        private void logoutbtn_Click(object sender, EventArgs e)
        {
            KyunGame.Instance.server.Logout();
            logoutbtn.Visible = false;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if(NikuClientApi.User == null)
                logoutbtn.Visible = false;
            else
                logoutbtn.Visible = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
