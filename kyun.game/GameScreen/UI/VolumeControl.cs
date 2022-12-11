﻿using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.Screen;
using kyun.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameScreen.UI
{
    public class VolumeControl : UIObjectBase
    {

        static VolumeControl _instance;

        public static VolumeControl Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VolumeControl();

                return _instance;
            }
        }

        ProgressBar _progressbar;
        Label textLabel;

        static int timeToHide = 2 * 1000;

        int counter = 0;

        int barWidth = 500;
        int barHeight = 20;
        Color barColor = Color.CornflowerBlue;

        bool showed = false;

        public VolumeControl()
        {
            _progressbar = new ProgressBar(barWidth, barHeight);

            _progressbar.BarColor = barColor;


            textLabel = new Label(.5f)
            {
                Text = $"Volume {(KyunGame.Instance.GeneralVolume * 100).ToString("0")}%",
                Font = SpritesContent.Instance.DefaultFont,
                Scale = 0.5f
            };

            _progressbar.Visible = textLabel.Visible = false;

        }

        public void Show()
        {
            _progressbar.Value = KyunGame.Instance.GeneralVolume * 100;
            textLabel.Text = $"Volume {(KyunGame.Instance.GeneralVolume * 100).ToString("0")}%";



            var screenMode = ScreenModeManager.GetActualMode();
            _progressbar.Position = new Vector2(screenMode.Width / 2 - (barWidth / 2), screenMode.Height - barHeight);
            textLabel.Position = _progressbar.Position;

            _progressbar.Visible = textLabel.Visible = true;
            if (!showed)
            {
                _progressbar.FadeIn(AnimationEffect.Linear, 200);
                textLabel.FadeIn(AnimationEffect.Linear, 200);
            }
            showed = true;
        }

        public override void Update()
        {
            //base.Update();

            updateTick();

            _progressbar.Update();
            textLabel.Update();
        }

        private void updateTick()
        {

            if (!showed)
            {
                return;
            }

            counter += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;

            if (counter >= timeToHide)
            {
                if (!_progressbar.animating && _progressbar.Opacity == 1f)
                {
                    _progressbar.FadeOut(AnimationEffect.Linear, 200);
                }
                if (!textLabel.animating && textLabel.Opacity == 1f)
                {
                    textLabel.FadeOut(AnimationEffect.Linear, 200);

                }
                counter = 0;
                showed = false;
            }

        }

        public override void Render()
        {
            //base.Render();

            _progressbar.Render();
            textLabel.Render();
        }


    }
}
