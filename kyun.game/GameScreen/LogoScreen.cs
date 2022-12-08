using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.GameScreen.UI;
using kyun.Screen;
using kyun.Utils;
using kyun.Audio;

namespace kyun.GameScreen
{
    public class LogoScreen : ScreenBase
    {
        private Image Logo;

        private double initTime;

        //Test only

        private bool DONT_UPDATE;
        private Label lComp;

        public Image EWarnImage { get; private set; }

        public LogoScreen()
        {
            DONT_UPDATE = true;

            ScreenMode mode = ScreenModeManager.GetActualMode();
            Background = new Texture2D(KyunGame.Instance.GraphicsDevice, mode.Width, mode.Height);

            Color[] bgcolor = new Color[mode.Width * mode.Height];
            for(int a = 0; a < bgcolor.Length; a++)
            {
                bgcolor[a] = Color.White;
            }

            Background.SetData(bgcolor);

            var logoSize = new Vector2(200);

            Vector2 logoPosition = new Vector2((mode.Width / 2) - (logoSize.X / 2), (mode.Height / 2) - (logoSize.Y / 2) - logoSize.Y / 2);

            Logo = new Image(SpritesContent.Instance.FabiCorpLogo)
            {
                Position = logoPosition,
                BeatReact = false,
                Size = logoSize,
            };

            lComp = new Label(0) {
                Text = KyunGame.CompilationVersion,
                Position = new Vector2(Logo.Position.X + Logo.Size.X / 2, Logo.Position.Y + 20 + Logo.Size.Y),
                ForegroundColor = Color.Black,
                Shadow = false,
                Centered = true          
            };

            EWarnImage = new Image(SpritesContent.Instance.EpWarn)
            {
                Position = new Vector2((mode.Width / 2) - (SpritesContent.Instance.EpWarn.Width / 2), lComp.Position.Y + 40),
                BeatReact = false,
                Scale = .5f
            };

            Logo.Position = new Vector2(logoPosition.X, 0);

            Controls.Add(Logo);
            Controls.Add(lComp);
            Controls.Add(EWarnImage);
            initTime = KyunGame.Instance.GameTimeP.TotalGameTime.TotalSeconds;
            DONT_UPDATE = false;

            EffectsPlayer.PlayEffect(SpritesContent.Instance.WelcomeToOsuXd);

            
            Logo.MoveTo(AnimationEffect.bounceOut, 1000, logoPosition);
            lComp.FadeIn(AnimationEffect.Ease, 1000);
            EWarnImage.FadeIn(AnimationEffect.Ease, 1000);
            KyunGame.Instance.Notifications.ShowDialog("Volume control: PageUP | PageDown keys", 2000, Notifications.NotificationType.Critical);

            onKeyPress += (obj, args) =>
            {
                if (args.Key == Microsoft.Xna.Framework.Input.Keys.P)
                {
                    DONT_UPDATE = !DONT_UPDATE;
                    KyunGame.Instance.Notifications.ShowDialog(DONT_UPDATE ? "GAME LOADING PAUSED" : "GAME LOADING UNPAUSED", 5000, Notifications.NotificationType.Critical);
                }
            };
        }

        public override void Update(GameTime tm)
        {
            if (DONT_UPDATE)
                return;

            base.Update(tm);

            //return;
           
            double diff = KyunGame.Instance.GameTimeP.TotalGameTime.TotalSeconds - initTime;            

            if(diff > 2)
            {
                ScreenManager.ChangeTo(LoadScreen.Instance);
                //ScreenManager.ChangeTo(game.GameModes.Test.TestScreen.GetInstance());
                DONT_UPDATE = true;
            }
        }
    }
}
