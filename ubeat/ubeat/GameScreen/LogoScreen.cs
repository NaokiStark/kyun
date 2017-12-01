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

        private DateTime initTime;

        //Test only

        private bool DONT_UPDATE;
        private Label lComp;

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

            Vector2 logoPosition = new Vector2((mode.Width / 2) - (SpritesContent.Instance.Logo.Width / 2), (mode.Height / 2) - (SpritesContent.Instance.Logo.Height / 2) - SpritesContent.Instance.Logo.Height / 2);

            Logo = new Image(SpritesContent.Instance.IsoLogo)
            {
                Position = logoPosition,
                BeatReact = false,
            };

            lComp = new Label(0) {
                Text = KyunGame.CompilationVersion,
                Position = new Vector2(Logo.Position.X + Logo.Texture.Width / 2, Logo.Position.X + 20),
                ForegroundColor = Color.Black,
                Shadow = false,
                Centered = true          
            };

            Controls.Add(Logo);
            Controls.Add(lComp);
            initTime = DateTime.Now;
            DONT_UPDATE = false;

            EffectsPlayer.PlayEffect(SpritesContent.Instance.WelcomeToOsuXd);
        }

        public override void Update(GameTime tm)
        {
            if (DONT_UPDATE)
                return;

            base.Update(tm);

           
            TimeSpan diff = (DateTime.Now - initTime);            

            if(diff.TotalSeconds > 2)
            {
                ScreenManager.ChangeTo(LoadScreen.Instance);
                DONT_UPDATE = true;
            }
        }


    }
}
