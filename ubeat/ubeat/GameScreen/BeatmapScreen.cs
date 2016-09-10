using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using ubeat.Audio;
using ubeat.Beatmap;
using ubeat.Utils;

namespace ubeat.GameScreen
{
    public partial class BeatmapScreen : IScreen
    {
        string lastStr = "";
        public BeatmapScreen()
        {
            LoadInterface();
            Game1.Instance.kbmgr.OnKeyPress += kbmgr_OnKeyPress;
        }

        void kbmgr_OnKeyPress()
        {
            string ActualText = Game1.Instance.kbmgr.Text;
            if (ActualText != lastStr)
            {
                lastStr = ActualText;
                lblSearch.Text = ActualText;
                lbox.Items = BeatmapSearchEngine.SearchBeatmaps(ActualText);
                lbox.vertOffset = 0;
                int lastind = lbox.selectedIndex;
                /*
                if (lastind == 0)
                {
                    lbox.LaunchEvent();//Helps
                }*/
            }

        }

        void BeatmapScreen_OnLoad(object sender, EventArgs e)
        {
            Game1.Instance.kbmgr.Enabled = true;
            if (Game1.Instance.AllBeatmaps.Count < 1)
            {
                var mpset = new Mapset();
                lbox.Items.Add(mpset);

            }
            else
            {
                foreach (Mapset mps in Game1.Instance.AllBeatmaps)
                    lbox.Items.Add(mps);
            }
            
            lbox.IndexChanged+=lbox_IndexChanged;
            lBDff.IndexChanged += lBDff_IndexChanged;
            lBDff.MouseDoubleClick += lBDff_MouseDoubleClick;
            ChangeBeatmapDisplay(Game1.Instance.SelectedBeatmap);
        }

        void lBDff_MouseDoubleClick(object sender, EventArgs e)
        {
            if (lBDff.selectedIndex < 0 || lBDff.selectedIndex > lBDff.Items.Count - 1) return;
            
            Game1.Instance.kbmgr.Enabled = false;

            Game1.Instance.GameStart(lBDff.Items[lBDff.selectedIndex],this.AMode);

        }

        void lBDff_IndexChanged(object sender, EventArgs e)
        {
            if (lBDff.selectedIndex < 0 || lBDff.selectedIndex > lBDff.Items.Count-1) return;
            AudioPlaybackEngine.Instance.PlaySound(Game1.Instance.SelectorHit);
            lblTitleDesc.Text = lBDff.Items[lBDff.selectedIndex].Artist + " - " + lBDff.Items[lBDff.selectedIndex].Title;
            ChangeBeatmapDisplay(lBDff.Items[lBDff.selectedIndex]);
        }

        void lbox_IndexChanged(object sender, EventArgs e)
        {
            if (lbox.selectedIndex < 0 || lbox.selectedIndex > lbox.Items.Count - 1) return;
            AudioPlaybackEngine.Instance.PlaySound(Game1.Instance.SelectorHit);
            lblTitleDesc.Text = lbox.Items[lbox.selectedIndex][0].Artist + " - " + lbox.Items[lbox.selectedIndex][0].Title;

            ChangeBeatmapDisplay(lbox.Items[lbox.selectedIndex][0]);
            lBDff.Items = Mapset.OrderByDiff(lbox.Items[lbox.selectedIndex]);
        }

        void ChangeBeatmapDisplay(ubeatBeatMap bm)
        {
            if (Game1.Instance.SelectedBeatmap.SongPath != bm.SongPath)
            {
                Game1.Instance.player.Play(bm.SongPath);
                Game1.Instance.player.soundOut.Volume = Game1.Instance.GeneralVolume;
            }

            Game1.Instance.SelectedBeatmap = bm;

            try
            {
                FileStream bgFstr = new FileStream(bm.Background, FileMode.Open);
                Background = Texture2D.FromStream(Game1.Instance.GraphicsDevice, bgFstr);
                bgFstr.Close();
            }
            catch
            {
                Logger.Instance.Warn("BACKGROUND NOT FOUND!!");
            }
        }
    }
}
