using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using kyun.Audio;
using kyun.Beatmap;
using kyun.GameModes.Classic;
using kyun.GameModes.OsuMode;
using kyun.Utils;
using kyun.game.GameModes;
using kyun.game.GameModes.CatchIt;
using kyun.game.GameScreen;
using kyun.game.GameModes.CatchItCollab;
using kyun.game.GameScreen.UI.Scoreboard;
using kyun.game.GameScreen.UI;

namespace kyun.GameScreen
{
    public partial class BeatmapScreen : ScreenBase
    {

        string lastStr = "";
        private bool changingDisplay;
        private int lastDiffIndex = 0;
        internal GameMode _gamemode = GameMode.CatchIt;

        private bool onlineSelector;

        public bool OnlineSelector
        {
            get
            {
                return onlineSelector;
            }
            set
            {
                onlineSelector = value;
                toggleSelector();
            }
        }

       

        public BeatmapScreen()
            : base("BeatmapScreen")
        {
            ScreenInstance = this;
            LoadInterface();
            KyunGame.Instance.KeyBoardManager.OnKeyPress += kbmgr_OnKeyPress;
            onKeyPress += BeatmapScreen_onKeyPress;
            AllowVideo = true;

            //OnlineSelector = true;
        }

        private void toggleSelector()
        {
            //
            if (onlineSelector)
            {
                btnStart.Caption = "Select";
                btnStart.Tooltip.Text = "Select this beatmap";
                autoBtn.Visible = false;
            }
            else
            {
                btnStart.Caption = "Play!";
                btnStart.Tooltip.Text = "Play now!";
                autoBtn.Visible = true;
                
            }
        }

        private void BeatmapScreen_onKeyPress(object sender, InputEvents.KeyPressEventArgs args)
        {
            switch (args.Key)
            {
                case Microsoft.Xna.Framework.Input.Keys.Up:
                    lbox.Select(true);
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Down:
                    lbox.Select(false);
                    break;
                case Keys.Left:
                    lBDff.Select(true);
                    break;
                case Keys.Right:
                    lBDff.Select(false);
                    break;
                case Microsoft.Xna.Framework.Input.Keys.F2:
                    Random();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Enter:
                    lBDff_MouseDoubleClick(new object(), new EventArgs());
                    break;
                case Keys.F5:
                    LoadScreen.Instance = null;
                    ScreenManager.ChangeTo(new LoadScreen(true));
                    break;
                case Keys.F3:
                    _scoreboard.Add(ScoreItem.BuildNew(UserBox.GetInstance().userAvatar.Texture, UserBox.GetInstance().nickLabel.Text, OsuUtils.OsuBeatMap.rnd.Next(10, 9999999), OsuUtils.OsuBeatMap.rnd.Next(10, 99999)));
                    break;
            }

        }

        void kbmgr_OnKeyPress()
        {
            lastStr = KyunGame.Instance.KeyBoardManager.Text;
            lblSearch.Text = lastStr;
            lbox.Items = BeatmapSearchEngine.SearchBeatmaps(lastStr);
            lbox.vertOffset = 0;
            int lastind = lbox.selectedIndex;
            lbox.selectedIndex = -1;
        }


        private void Random()
        {
            lastStr = "";

            int rnd = OsuUtils.OsuBeatMap.GetRnd(0, InstanceManager.AllBeatmaps.Count - 1, -1);

            lbox.selectedIndex = rnd;
            lbox.vertOffset = (rnd > 2) ? rnd - 1 : 0;

            //UbeatGame.Instance.SelectedBeatmap = InstanceManager.AllBeatmaps[rnd][0];
            ((MainScreen)MainScreen.Instance).ChangeMainDisplay(rnd);


            ChangeBeatmapDisplay(InstanceManager.AllBeatmaps[rnd].Beatmaps[0]);

        }

        void BeatmapScreen_OnLoad(object sender, EventArgs e)
        {
            KyunGame.Instance.KeyBoardManager.Enabled = true;
            if (InstanceManager.AllBeatmaps.Count < 1)
            {
                var mpset = new Mapset();
                lbox.Items.Add(mpset);

            }
            else
            {
                /*
                foreach (Mapset mps in InstanceManager.AllBeatmaps)
                    lbox.Items.Add(mps);*/

                lbox.Items = InstanceManager.AllBeatmaps;
            }

            lbox.IndexChanged += lbox_IndexChanged;
            lBDff.IndexChanged += lBDff_IndexChanged;
            lBDff.MouseDoubleClick += lBDff_MouseDoubleClick;

            ChangeBeatmapDisplay(KyunGame.Instance.SelectedBeatmap);
            lbox.selectedIndex = 0;

            //var sItem = ScoreItem.BuildNew(
            //    UserBox.GetInstance().userAvatar.Texture,
            //    UserBox.GetInstance().nickLabel.Text,
            //    999999,
            //    99999);           
            

            //_scoreboard.Add(sItem);
        }

        void lBDff_MouseDoubleClick(object sender, EventArgs e)
        {
            if (lBDff.selectedIndex < 0 || lBDff.selectedIndex > lBDff.Items.Count - 1) return;

            KyunGame.Instance.KeyBoardManager.Enabled = false;

        
            GameModes.GameMod modes = GameModes.GameMod.None;

            if (AMode)
            {
                modes |= GameModes.GameMod.Auto;
            }

            if (doubleTime)
            {
                modes |= GameModes.GameMod.DoubleTime;
            }

            switch (_gamemode)
            {
                case GameMode.Classic:

                    //ClassicModeScreen.GetInstance().Play(lBDff.Items[lBDff.selectedIndex], modes);

                    GameLoader.GetInstance().LoadBeatmapAndRun(lBDff.Items.Beatmaps[lBDff.selectedIndex], ClassicModeScreen.GetInstance(), modes);
                    break;
                case GameMode.Osu:

                    GameLoader.GetInstance().LoadBeatmapAndRun(lBDff.Items.Beatmaps[lBDff.selectedIndex], OsuMode.GetInstance(), modes);

                    break;
                case GameMode.CatchIt:

                    GameLoader.GetInstance().LoadBeatmapAndRun(lBDff.Items.Beatmaps[lBDff.selectedIndex], CatchItMode.GetInstance(), modes);

                    break;
            }



        }

        void lBDff_IndexChanged(object sender, EventArgs e)
        {

            if (lBDff.selectedIndex < 0 || lBDff.selectedIndex > lBDff.Items.Count - 1) return;
            //AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.SelectorHit);
            EffectsPlayer.PlayEffect(SpritesContent.Instance.SelectorHit);

            ubeatBeatMap selectd = lBDff.Items.Beatmaps[lBDff.selectedIndex];

            lblTitleDesc.Text = selectd.Artist + " - " + selectd.Title;
            lbldesc.Text = $"Mapped by " + selectd.Creator;
            lblDiffDesc.Text = $"Diff: {selectd.Version} | OD: {selectd.OverallDifficulty} | AP: {selectd.ApproachRate} | CS: {selectd.CircleSize} | HD: {selectd.HPDrainRate}";

            if (lastDiffIndex != lBDff.selectedIndex)
            {


                if (lastDiffIndex >= lBDff.Items.Count)
                    lastDiffIndex = 0;

                if (lBDff.Items.Beatmaps[lBDff.selectedIndex].Background != lBDff.Items.Beatmaps[lastDiffIndex].Background)
                {
                    //ChangeBackground(lBDff.Items[lBDff.selectedIndex].Background);
                    ChangeBeatmapDisplay(lBDff.Items.Beatmaps[lBDff.selectedIndex]);
                }

                lastDiffIndex = lBDff.selectedIndex;

                
            }
            _scoreboard.Items.Clear();
            _scoreboard.AddList(ScoreItem.GetFromDb(lBDff.Items.Beatmaps[lBDff.selectedIndex]));
        }

        void lbox_IndexChanged(object sender, EventArgs e)
        {
            if (!Visible) return;

            if (lbox.selectedIndex < 0 || lbox.selectedIndex > lbox.Items.Count - 1) return;
            //AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.SelectorHit);
            EffectsPlayer.PlayEffect(SpritesContent.Instance.SelectorHit);

            ubeatBeatMap selectd = lbox.Items[lbox.selectedIndex].Beatmaps[0];

            if(selectd != null)
            {
                lblTitleDesc.Text = selectd.Artist + " - " + selectd.Title;
                lbldesc.Text = $"Mapped by " + selectd.Creator;
                lblDiffDesc.Text = $"Diff: {selectd.Version} | OD: {selectd.OverallDifficulty} | AP: {selectd.ApproachRate} | CS: {selectd.CircleSize} | HD: {selectd.HPDrainRate}";
            }

           
            
            
            //lblTitleDesc.Text = lbox.Items[lbox.selectedIndex].Beatmaps[0].Artist + " - " + lbox.Items[lbox.selectedIndex].Beatmaps[0].Title;

            ChangeBeatmapDisplay(selectd);

            ((MainScreen)MainScreen.Instance).ChangeMainDisplay(lbox.selectedIndex);

            lBDff.Items = Mapset.OrderByDiff(lbox.Items[lbox.selectedIndex]);
            lBDff.selectedIndex = 0;
            lBDff.vertOffset = 0;

            KyunGame.Instance.discordHandler.SetState("Picking beatmap", $"This or this?");
        }

        public void changeBeatmapAndReorderDisplay()
        {
            lbox.selectedIndex = lbox.Items.Count - 1;
        }

        void orderByTitle()
        {

            InstanceManager.AllBeatmaps = InstanceManager.AllBeatmaps.OrderBy(x => x.Title).ToList<Beatmap.Mapset>();
        }

        void orderByArtist()
        {
            InstanceManager.AllBeatmaps = InstanceManager.AllBeatmaps.OrderBy(x => x.Artist).ToList<Beatmap.Mapset>();

        }
        void orderByCreator()
        {
            InstanceManager.AllBeatmaps = InstanceManager.AllBeatmaps.OrderBy(x => x.Creator).ToList<Beatmap.Mapset>();
        }

        public override void ChangeBeatmapDisplay(ubeatBeatMap beatMap, bool overrideBg = true)
        {

            if (beatMap == null || KyunGame.Instance.SelectedBeatmap == beatMap) return;

            KyunGame.Instance.SelectedBeatmap = beatMap;
            base.ChangeBeatmapDisplay(beatMap);
            ((MainScreen)MainScreen.Instance).ChangeBackground(beatMap.Background);

            float titleSize = SpritesContent.Instance.SettingsFont.MeasureString(beatMap.Title).X;
            float artSize = SpritesContent.Instance.SettingsFont.MeasureString(beatMap.Artist).X;

            float maxSize = Math.Max(titleSize, artSize);


            ((MainScreen)MainScreen.Instance).coverBox.Resize(new Microsoft.Xna.Framework.Vector2((maxSize * .8f) + 20, ((MainScreen)MainScreen.Instance).coverSize));
            ((MainScreen)MainScreen.Instance).coverLabel.Text = beatMap.Title;
            ((MainScreen)MainScreen.Instance).coverLabelArt.Text = beatMap.Artist;


            changeCoverDisplay(beatMap.Background);
            changingDisplay = false;

            //if (UbeatGame.Instance.SelectedBeatmap.Video != beatMap.Video)
            //    if (UbeatGame.Instance.VideoEnabled)
            //        if (beatMap.Video != null)
            //            videoPlayer.Play(beatMap.Video);

        }
    }
}
