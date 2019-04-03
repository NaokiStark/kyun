using kyun.game.GameModes;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.GameScreen.UI.Buttons;
using kyun.Overlay;
using kyun.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.Overlay
{
    public class ModeOverlay : OverlayScreen
    {
        private static ModeOverlay instance;

        public static ModeOverlay Instance
        {
            get
            {
                if (instance == null)
                    instance = new ModeOverlay();

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        private ButtonStandard osuModeBtn;
        private ButtonStandard osuModeBtn2;
        private ButtonStandard osuModeBtn3;
        private Label ltitle;
        private Label ladditional;

        public ModeOverlay() : base(OverlayType.Normal)
        {
 
            rPeak = false;
            ltitle = new Label(0)
            {
                Text = "Game Modes",
                Position = new Vector2(ActualScreenMode.Width / 2, ActualScreenMode.Height * 0.2f),
                Centered = true,
                Font = SpritesContent.Instance.ScoreBig
            };

            ladditional = new Label(0)
            {
                Text = "Choose your flavor",
                Position = new Vector2(ActualScreenMode.Width / 2, ltitle.Position.Y + 70),
                Centered = true,
                Font = SpritesContent.Instance.ListboxFont
            };



            osuModeBtn3 = new ButtonStandard(Color.Green)
            {
                Position = new Vector2(ActualScreenMode.Width / 2 - SpritesContent.Instance.ButtonStandard.Width / 2, ladditional.Position.Y + 100),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "Catch!",
                Tooltip = new GameScreen.UI.Tooltip { Text = "Catch all the circles!" }
            };

            osuModeBtn2 = new ButtonStandard(Color.PaleVioletRed)
            {
                Position = new Vector2(ActualScreenMode.Width / 2 - SpritesContent.Instance.ButtonStandard.Width / 2, osuModeBtn3.Position.Y + SpritesContent.Instance.ButtonStandard.Height + 20),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "Circles!",
                Tooltip = new GameScreen.UI.Tooltip { Text = "Click the circles... To the beat!" }
            };


            osuModeBtn = new ButtonStandard(Color.DodgerBlue)
            {
                Position = new Vector2(ActualScreenMode.Width / 2 - SpritesContent.Instance.ButtonStandard.Width / 2, osuModeBtn2.Position.Y + SpritesContent.Instance.ButtonStandard.Height + 20),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "ubeat",
                Tooltip = new GameScreen.UI.Tooltip { Text = "Use your numpad to press hexagons, just like j-ubeat... but with numpad :3" }
            };

            osuModeBtn.Click += OsuModeBtn_Click;

            osuModeBtn2.Click += OsuModeBtn2_Click;

            osuModeBtn3.Click += OsuModeBtn3_Click;

            Controls.Add(ltitle);
            Controls.Add(ladditional);
            Controls.Add(osuModeBtn);
            Controls.Add(osuModeBtn2);
            Controls.Add(osuModeBtn3);

        }

        private void OsuModeBtn3_Click(object sender, EventArgs e)
        {
            BeatmapScreen i = (BeatmapScreen.Instance as BeatmapScreen);
            i._gamemode = GameMode.CatchIt;

            ScreenManager.RemoveOverlay();
        }

        private void OsuModeBtn2_Click(object sender, EventArgs e)
        {
            BeatmapScreen i = (BeatmapScreen.Instance as BeatmapScreen);

            i._gamemode = GameMode.Osu;

            ScreenManager.RemoveOverlay();
            //if (i._gamemode == GameMode.Osu)
            //{

            //    i._gamemode = GameMode.CatchIt;

            //    //autoBtn.Texture = SpritesContent.Instance.AutoModeButton;
            //    osuModeBtn2.TextureColor = Color.DimGray;
            //}
            //else
            //{
            //    if (i._gamemode == GameMode.Classic)
            //    {
            //        osuModeBtn.TextureColor = Color.DimGray;
            //    }

            //    i._gamemode = GameMode.Osu;

            //    //autoBtn.Texture = SpritesContent.Instance.AutoModeButtonSel;
            //    osuModeBtn2.TextureColor = Color.DarkRed;
            //}
        }

        public override void RenderDim()
        {
            BackgroundDim = .4f;
            RenderBg();
            base.RenderDim();
        }

        private void OsuModeBtn_Click(object sender, EventArgs e)
        {
            BeatmapScreen i = (BeatmapScreen.Instance as BeatmapScreen);
            i._gamemode = GameMode.Classic;

            ScreenManager.RemoveOverlay();
            //if (i._gamemode == GameMode.CatchIt || i._gamemode == GameMode.Osu)
            //{
            //    if (i._gamemode == GameMode.Osu)
            //    {
            //        osuModeBtn2.TextureColor = Color.DimGray;
            //    }
            //    i._gamemode = GameMode.Classic;

            //    //autoBtn.Texture = SpritesContent.Instance.AutoModeButton;
            //    osuModeBtn.TextureColor = Color.DarkRed;

            //}
            //else
            //{
            //    _gamemode = GameMode.CatchIt;

            //    //autoBtn.Texture = SpritesContent.Instance.AutoModeButtonSel;
            //    osuModeBtn.TextureColor = Color.DimGray;
            //}
        }

        public void Show()
        {
            BeatmapScreen i = (BeatmapScreen.Instance as BeatmapScreen);
            string bgstr = i.lBDff.Items.Beatmaps[i.lBDff.selectedIndex].Background;
            if (File.Exists(bgstr))
                Background = SpritesContent.ToGaussianBlur(System.Drawing.Bitmap.FromFile(bgstr) as System.Drawing.Bitmap, 10);
            else
                Background = SpritesContent.Instance.DefaultBackground;

        }
    }
}
