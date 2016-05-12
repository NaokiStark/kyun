using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ubeat.Utils;

namespace ubeat
{
    public partial class BeatmapSelector : Form
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public static int GWL_STYLE = -16;
        public static int WS_CHILD = 0x40000000; 

        public static BeatmapSelector Instance = null;
        List<Beatmap.Mapset> beatmapList;
        public BeatmapSelector()
        {
            Instance = this;
            InitializeComponent();

            beatmapList = Game1.Instance.AllBeatmaps.OrderBy(x => x.Artist).ToList<Beatmap.Mapset>();
            treeView1.BeforeSelect+=treeView1_BeforeSelect;
        }

        private void BeatmapSelector_Load(object sender, EventArgs e)
        {

            
            for (int a = 0; a < beatmapList.Count; a++)
            {
                TreeNode lvi = new TreeNode() { Text = string.Format("{0} - {1}", beatmapList[a].Artist, beatmapList[a].Title) };
                foreach (Beatmap.ubeatBeatMap bmp in beatmapList[a])
                {
                    Utils.ListBeatmapItem ublmi = new ListBeatmapItem() { 
                        Beatmap = bmp,
                        Text = string.Format("{0} - {1} [{2}]", bmp.Artist, bmp.Title, bmp.Version)                        
                    };

                    lvi.Nodes.Add(ublmi);
                }
                lvi.EnsureVisible();
                EnsureVisibleWithoutRightScrolling(lvi);
                treeView1.Nodes.Add(lvi);
                
            }
            //treeView1.Sort();

            this.Height = 601;
            treeView1.MouseDoubleClick += treeView1_DoubleClick;
            treeView1.KeyPress += treeView1_DoubleClick;

            if (this.Width > Game1.Instance.wSize.X)
            {
                this.Width = (int)Game1.Instance.wSize.X;
                
            }
            if (Game1.Instance.wSize.X < 801)
            {
                this.Left = 0;
            }
            else
            {
                this.Left = (int)(Game1.Instance.wSize.X/2) - (this.Width / 2);
            }

            if (this.Height > Game1.Instance.wSize.Y)
            {
                this.Height = (int)Game1.Instance.wSize.Y;
                
            }
            if (Game1.Instance.wSize.Y < 601)
            {
                this.Top = 0;
            }


                IntPtr hostHandle = MainWindow.Instance.Handle;
                IntPtr guestHandle = this.Handle;

                SetWindowLong(guestHandle, GWL_STYLE, GetWindowLong(guestHandle, GWL_STYLE) | WS_CHILD);
                SetParent(guestHandle, hostHandle);
                this.Show();

               

        }

        void treeView1_DoubleClick(object sender, EventArgs e)
        {
           
            if (e is KeyPressEventArgs)
            {
                KeyPressEventArgs ev = (KeyPressEventArgs)e;

                if (!this.Visible)
                {
                    ev.Handled = true;
                    return;
                }

                if (ev.KeyChar == 13)
                {
                    ev.Handled = true;
                    
                    play();
                }
            }
            else if(e is MouseEventArgs)
            {
                if (!this.Visible)
                {
                   return;
                }
                play();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        public string lastSelected = null;

        void treeView1_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            
            try
            {
                if (e.Node.Parent == null)
                {
                    //Padre
                    ((TreeView)sender).CollapseAll();
                    e.Node.Expand();
                    if (lastSelected != ((ListBeatmapItem)e.Node.Nodes[0]).Beatmap.SongPath)
                    {
                        
                        ((TreeView)sender).SelectedNode = e.Node.Nodes[0];
                    }
                }
                else
                {
                    this.lArtista.Text = ((ListBeatmapItem)e.Node).Beatmap.Artist + " - " + ((ListBeatmapItem)e.Node).Beatmap.Title;
                    this.lCreador.Text = ((ListBeatmapItem)e.Node).Beatmap.Creator + " // [" + ((ListBeatmapItem)e.Node).Beatmap.Version + "]";
                    //Hijo
                    if (lastSelected != ((ListBeatmapItem)e.Node).Beatmap.SongPath)
                    {
                        showData(((ListBeatmapItem)e.Node).Beatmap);
                        lastSelected = ((ListBeatmapItem)e.Node).Beatmap.SongPath;

                    }
                    
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Severe(ex.Message);
            }
            
        }
        void showData(Beatmap.ubeatBeatMap bm)
        {
            try
            {
                //bg
                FileStream filestream = new FileStream(bm.Background, FileMode.Open, FileAccess.Read);
                var img = Bitmap.FromStream(filestream);

                pictureBox1.Image = img;

                filestream.Close();
                filestream.Dispose();
            }
            catch
            {
                Logger.Instance.Warn("Nopenope");
            }

            MainWindow.Instance.ChangeBeatmap(bm);
            
        }

        private void label2_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.ShowControls();
            this.Visible = false;
        }

        private const int WM_HSCROLL = 276;
        private const int SB_LEFT = 6;
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg,
                                          int wParam, int lParam);

        private void EnsureVisibleWithoutRightScrolling(TreeNode node)
        {
            // we do the standard call.. 
            node.EnsureVisible();

            // ..and afterwards we scroll to the left again!
            SendMessage(treeView1.Handle, WM_HSCROLL, SB_LEFT, 0);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void play() {
            if (treeView1.SelectedNode.Parent == null)
                return;

            MainWindow.Instance.Visible = false;
            this.Visible = false;
            this.Enabled = false;
            SetForegroundWindow(Game1.Instance.Window.Handle);
            Game1.Instance.GameStart(((ListBeatmapItem)((TreeView)treeView1).SelectedNode).Beatmap);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            play();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.ShowControls();
            this.Visible = false;
        }
    }
}
