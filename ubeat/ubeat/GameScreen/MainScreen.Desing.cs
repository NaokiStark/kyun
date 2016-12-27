﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using ubeat.GameScreen.UI.Buttons;
using ubeat.Screen;
using ubeat.Notifications;
using ubeat.Utils;

namespace ubeat.GameScreen
{
    public partial class MainScreen : ScreenBase
    {
        public void LoadInterface()
        {
            if (UbeatGame.Instance.ppyMode)
                Audio.AudioPlaybackEngine.Instance.PlaySound(UbeatGame.Instance.WelcomeToOsuXd);

            Controls = new List<UIObjectBase>();

            ScreenMode ActualMode = ScreenModeManager.GetActualMode();

            Vector2 center = new Vector2(ActualMode.Width / 2, ActualMode.Height / 2);

            Logo = new UI.Image(SpritesContent.Instance.Logo) { BeatReact = true };
            Logo.Position = new Vector2(center.X - (Logo.Texture.Width / 2),
                center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);

            CnfBtn = new ConfigButton();
            CnfBtn.Position = new Vector2(center.X - (CnfBtn.Texture.Width / 2),
                center.Y - (CnfBtn.Texture.Height / 2) + Logo.Texture.Height / 2);

            StrBtn = new StartButton();
            StrBtn.Position = new Vector2(center.X - (StrBtn.Texture.Width / 2),
                (center.Y - (StrBtn.Texture.Height / 2) - StrBtn.Texture.Height - 2) + Logo.Texture.Height / 2);

            ExtBtn = new ExitButton();
            ExtBtn.Position = new Vector2(center.X - (ExtBtn.Texture.Width / 2),
                center.Y - (ExtBtn.Texture.Height / 2) + ExtBtn.Texture.Height + 2 + Logo.Texture.Height / 2);

            Vector2 meas = SpritesContent.Instance.DefaultFont.MeasureString("ubeat") * .85f;

            Label1 = new UI.Label();
            Label1.Text = "Hello world";
            Label1.Scale = .85f;
            Label1.Position = new Vector2(0);

            FPSMetter = new UI.Label();
            FPSMetter.Text = "0";
            FPSMetter.Scale = .75f;
            FPSMetter.Position = new Vector2(0, ActualMode.Height - meas.Y);

            ntfr = new Notifier();

            Controls.Add(CnfBtn);
            Controls.Add(StrBtn);
            Controls.Add(ExtBtn);
            Controls.Add(Logo);
            Controls.Add(Label1);
            Controls.Add(FPSMetter);
            Controls.Add(ntfr);

            UbeatGame.Instance.IsMouseVisible = true;
            OnLoad += MainScreen_OnLoad;

            OnLoadScreen();
        }


        public override void Update(GameTime tm)
        {

            //FPSMetter.Text = Game1.Instance.frameCounter.AverageFramesPerSecond.ToString("0", CultureInfo.InvariantCulture) +" FPS";

            int fps = (int)Math.Round(UbeatGame.Instance.frameCounter.AverageFramesPerSecond, 0);

            FPSMetter.Text = fps.ToString("0", CultureInfo.InvariantCulture) + " FPS";

            base.Update(tm);
        }

        public override void Redraw()
        {
            ScreenMode ActualMode = ScreenModeManager.GetActualMode();

            Vector2 center = new Vector2(ActualMode.Width / 2, ActualMode.Height / 2);

            Logo.Position = new Vector2(center.X - (Logo.Texture.Width / 2),
               center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);

            CnfBtn.Position = new Vector2(center.X - (CnfBtn.Texture.Width / 2),
                center.Y - (CnfBtn.Texture.Height / 2) + Logo.Texture.Height / 2);

            StrBtn.Position = new Vector2(center.X - (StrBtn.Texture.Width / 2),
                (center.Y - (StrBtn.Texture.Height / 2) - StrBtn.Texture.Height - 2) + Logo.Texture.Height / 2);

            ExtBtn.Position = new Vector2(center.X - (ExtBtn.Texture.Width / 2),
                center.Y - (ExtBtn.Texture.Height / 2) + ExtBtn.Texture.Height + 2 + Logo.Texture.Height / 2);
        }


        #region UI

        public ConfigButton CnfBtn;
        public StartButton StrBtn;
        public ExitButton ExtBtn;
        public UI.Image Logo;
        public UI.Label Label1;
        public UI.Label FPSMetter;
        private Notifier ntfr;

        #endregion

    }
}
