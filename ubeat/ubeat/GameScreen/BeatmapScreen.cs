using System;
using ubeat.Audio;
using ubeat.Beatmap;
using ubeat.GameModes.Classic;
using ubeat.Utils;

namespace ubeat.GameScreen
{
    public partial class BeatmapScreen : ScreenBase
    { 
        Video.VideoPlayer videoPlayer;
        string lastStr = "";

        public BeatmapScreen()
            : base("BeatmapScreen")
        {
            ScreenInstance = this;
            LoadInterface();
            UbeatGame.Instance.KeyBoardManager.OnKeyPress += kbmgr_OnKeyPress;
            //videoPlayer = Video.VideoPlayer.Instance;
        }

        void kbmgr_OnKeyPress()
        {
            
            string ActualText = UbeatGame.Instance.KeyBoardManager.Text;
            if (ActualText != lastStr)
            {
                lastStr = ActualText;
                lblSearch.Text = ActualText;
                lbox.Items = BeatmapSearchEngine.SearchBeatmaps(ActualText);
                lbox.vertOffset = 0;
                int lastind = lbox.selectedIndex;
                
            }

        }

        void BeatmapScreen_OnLoad(object sender, EventArgs e)
        {
            UbeatGame.Instance.KeyBoardManager.Enabled = true;
            if (InstanceManager.AllBeatmaps.Count < 1)
            {
                var mpset = new Mapset();
                lbox.Items.Add(mpset);

            }
            else
            {
                foreach (Mapset mps in InstanceManager.AllBeatmaps)
                    lbox.Items.Add(mps);
            }
            
            lbox.IndexChanged+=lbox_IndexChanged;
            lBDff.IndexChanged += lBDff_IndexChanged;
            lBDff.MouseDoubleClick += lBDff_MouseDoubleClick;

            ChangeBeatmapDisplay(UbeatGame.Instance.SelectedBeatmap);
            lbox.selectedIndex = 0;
        }

        void lBDff_MouseDoubleClick(object sender, EventArgs e)
        {
            if (lBDff.selectedIndex < 0 || lBDff.selectedIndex > lBDff.Items.Count - 1) return;
            
            UbeatGame.Instance.KeyBoardManager.Enabled = false;

            UbeatGame.Instance.GameStart(lBDff.Items[lBDff.selectedIndex], this.AMode);
            /*
            if (ClassicModeScreen.Instance == null)
                ClassicModeScreen.Instance = new ClassicModeScreen();

            ClassicModeScreen.GetInstance().Play(lBDff.Items[lBDff.selectedIndex], (AMode) ? GameModes.GameMod.Auto : GameModes.GameMod.None);
            ScreenManager.ChangeTo(ClassicModeScreen.GetInstance());
            */

        }

        void lBDff_IndexChanged(object sender, EventArgs e)
        {
            if (lBDff.selectedIndex < 0 || lBDff.selectedIndex > lBDff.Items.Count-1) return;
            AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.SelectorHit);
            lblTitleDesc.Text = lBDff.Items[lBDff.selectedIndex].Artist + " - " + lBDff.Items[lBDff.selectedIndex].Title;
        }

        void lbox_IndexChanged(object sender, EventArgs e)
        {
            if (lbox.selectedIndex < 0 || lbox.selectedIndex > lbox.Items.Count - 1) return;
            AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.SelectorHit);
            lblTitleDesc.Text = lbox.Items[lbox.selectedIndex][0].Artist + " - " + lbox.Items[lbox.selectedIndex][0].Title;

            ChangeBeatmapDisplay(lbox.Items[lbox.selectedIndex][0]);
            lBDff.Items = Mapset.OrderByDiff(lbox.Items[lbox.selectedIndex]);
            lBDff.selectedIndex = 0;
        }

        public override void ChangeBeatmapDisplay(ubeatBeatMap beatMap)
        {
            base.ChangeBeatmapDisplay(beatMap);

            videoPlayer?.Stop();
            

            //if (UbeatGame.Instance.SelectedBeatmap.Video != beatMap.Video)
            //    if (UbeatGame.Instance.VideoEnabled)
            //        if (beatMap.Video != null)
            //            videoPlayer.Play(beatMap.Video);

        }
    }
}
