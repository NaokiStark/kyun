using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ubeat.Utils;

namespace ubeat
{
    public partial class BeatmapSelector : Form
    {
        public static BeatmapSelector Instance = null;
        List<Beatmap.Mapset> beatmapList;
        public BeatmapSelector()
        {
            Instance = this;
            InitializeComponent();
            beatmapList = Game1.Instance.AllBeatmaps;
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
            treeView1.Sort();
            
            
            treeView1.MouseDoubleClick += treeView1_DoubleClick;
            treeView1.KeyPress += treeView1_DoubleClick;
        }

        void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (e is KeyPressEventArgs)
            {
                KeyPressEventArgs ev = (KeyPressEventArgs)e;

                if (ev.KeyChar == 13)
                {
                    
                    MainWindow.Instance.Visible = false;
                    this.Visible = false;
                    Game1.Instance.GameStart(((ListBeatmapItem)((TreeView)sender).SelectedNode).Beatmap);
                }
            }
            else if(e is MouseEventArgs)
            {
                MainWindow.Instance.Visible = false;
                this.Visible = false;
                Game1.Instance.GameStart(((ListBeatmapItem)((TreeView)sender).SelectedNode).Beatmap);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        public string lastSelected = null;
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (((TreeView)sender).SelectedNode.Parent != null)
            {
                if (lastSelected == null)
                {
                    showData(((ListBeatmapItem)((TreeView)sender).SelectedNode).Beatmap);
                    lastSelected = ((ListBeatmapItem)((TreeView)sender).SelectedNode).Beatmap.SongPath;
                }
                else if (lastSelected != ((ListBeatmapItem)((TreeView)sender).SelectedNode).Beatmap.SongPath)
                {
                    showData(((ListBeatmapItem)((TreeView)sender).SelectedNode).Beatmap);
                    lastSelected = ((ListBeatmapItem)((TreeView)sender).SelectedNode).Beatmap.SongPath;
                }
                return;
            }

            TreeNode selected = ((TreeView)sender).SelectedNode;
            ((TreeView)sender).CollapseAll();
            selected.Expand();
            ((TreeView)sender).SelectedNode = selected.FirstNode;
        }
        void showData(Beatmap.ubeatBeatMap bm)
        {
            MainWindow.Instance.ChangeBeatmap(bm);
            this.lArtista.Text = bm.Artist+"-"+bm.Title;
            this.lCreador.Text = bm.Creator;
            this.lDiff.Text = "[" + bm.Version + "]";
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
    }
}
