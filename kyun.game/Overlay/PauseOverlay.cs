using kyun.GameModes;
using kyun.GameModes.Classic;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.GameScreen.UI.Buttons;
using kyun.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.Overlay
{
    public class PauseOverlay : OverlayScreen
    {
        private static PauseOverlay instance;
        private Label ltitle;
        private Label ladditional;
        private ButtonStandard btnYes;
        private ButtonStandard btnNo;
        private ButtonStandard btnRestart;
        private static List<EventHandler> deleg = new List<EventHandler>();
        static GameModeScreenBase i;

        public static PauseOverlay Instance
        {
            get
            {
                if (instance == null)
                    instance = new PauseOverlay();

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public PauseOverlay() : base(OverlayType.Pause)
        {

            Visible = false;
            ltitle = new Label(0)
            {
                Text = "",
                Position = new Vector2(ActualScreenMode.Width / 2, ActualScreenMode.Height * 0.2f),
                Centered = true,
                Font = SpritesContent.Instance.ScoreBig
            };

            ladditional = new Label(0)
            {
                Text = "",
                Position = new Vector2(ActualScreenMode.Width / 2, ltitle.Position.Y + 70),
                Centered = true,
                Font = SpritesContent.Instance.ListboxFont
            };

            btnYes = new ButtonStandard(Color.ForestGreen)
            {
                Caption = "Continue",
                Position = new Vector2(ActualScreenMode.Width / 2 - SpritesContent.Instance.ButtonStandard.Width / 2, ladditional.Position.Y + 100),

            };

            btnNo = new ButtonStandard(Color.Violet)
            {
                Caption = "Restart",
                Position = new Vector2(ActualScreenMode.Width / 2 - SpritesContent.Instance.ButtonStandard.Width / 2, btnYes.Position.Y + SpritesContent.Instance.ButtonStandard.Height + 20),
            };

            btnRestart = new ButtonStandard(Color.Red)
            {
                Caption = "Quit",
                Position = new Vector2(ActualScreenMode.Width / 2 - SpritesContent.Instance.ButtonStandard.Width / 2, btnNo.Position.Y + SpritesContent.Instance.ButtonStandard.Height + 20),

            };

            btnNo.Click += (obj, args) => {
                i.Play(i.Beatmap, i.gameMod); //restart game

                Visible = false;
                ScreenManager.RemoveOverlay();
            };

            btnRestart.Click += (obj, args) => {                
                Visible = false;

                ScreenManager.RemoveOverlay();
                ScreenManager.ChangeTo(BeatmapScreen.Instance);
                i.InGame = false;
                i.Visible = false;
                AVPlayer.audioplayer.Play();

               // GameModes.GameMod gm = ((ClassicModeScreen)ClassicModeScreen.Instance).gameMod;

                //float vel = ((gm & GameModes.GameMod.DoubleTime) == GameModes.GameMod.DoubleTime) ? 1.5f : 1f;
                AVPlayer.audioplayer.SetVelocity(1);
            };

            btnYes.Click += (obj, args) => {
                Visible = false;
                ScreenManager.RemoveOverlay();
                AVPlayer.audioplayer.Play();

                GameModes.GameMod gm = i.gameMod;

                float vel = ((gm & GameModes.GameMod.DoubleTime) == GameModes.GameMod.DoubleTime) ? 1.5f : 1f;
                AVPlayer.audioplayer.SetVelocity(vel);
            };

            Controls.Add(ltitle);
            Controls.Add(ladditional);
            Controls.Add(btnYes);
            Controls.Add(btnNo);
            Controls.Add(btnRestart);
        }

        public static PauseOverlay ShowAlert(GameModeScreenBase instance)
        {
            i = instance;
            Instance.ltitle.Text = "Paused";
            Instance.ladditional.Text = "A little break";
            Instance.btnYes.Visible = true;
            Instance.Visible = true;
            return Instance;
        }

        public static PauseOverlay ShowFailed(GameModeScreenBase instance)
        {
            i = instance;
            Instance.btnYes.Visible = false;
            Instance.ltitle.Text = "You Failed";
            Instance.ladditional.Text = "Why :c";

            Instance.Visible = true;
            return Instance;
        }
    }
}
