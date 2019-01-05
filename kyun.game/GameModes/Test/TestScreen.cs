using kyun.GameModes;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using kyun.Utils;
using Microsoft.Xna.Framework.Input;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using Microsoft.Xna.Framework;
using kyun.OsuUtils;

//This is a test of platformer

namespace kyun.game.GameModes.Test
{
    public class TestScreen : GameModeScreenBase
    {
        public TestPlayer pl;

        private static TestScreen instance;
        public Label screenExp;

        public int score = 0;

        long mltoadd = 0;

        public static new TestScreen GetInstance()
        {
            if (instance == null)
                instance = new TestScreen();

            return instance;
        }

        public TestScreen() : base("Test Game")
        {
            KyunGame.Instance.discordHandler.SetState("Catching eggs", "Has found an easter egg!");

            Background = ContentLoader.LoadTextureFromAssets("test.png");
            BackgroundDim = 1;

            string song = "";

            if (string.IsNullOrWhiteSpace(AVPlayer.audioplayer.ActualSong))
                song = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Effects", "tristan-lohengrin_8bit-introduction.wav");
            else
                song = AVPlayer.audioplayer.ActualSong;

            pl = new TestPlayer();

            screenExp = new Label(1)
            {
                Position = Vector2.Zero,
                Font = SpritesContent.Instance.TitleFont,
                Text = "THIS IS ONLY A TEST, press ESC to continue with kyun!"
            };


            Controls.Add(pl);
            Controls.Add(screenExp);

            AVPlayer.Play(song);
            AVPlayer.audioplayer.OnStopped += () => {
                if (Visible)
                    ScreenManager.ChangeTo(LoadScreen.Instance);
                //AVPlayer.Play(song);
            };

            onKeyPress += (e, args) => {
                if(args.Key == Keys.Escape)
                {
                    ScreenManager.ChangeTo(LoadScreen.Instance);
                }
            };

           
        }

        public override void Update(GameTime tm)
        {
            Controls.RemoveAll(isDed);

            base.Update(tm);

            if(score > 0)
            {
                screenExp.Text = score.ToString();
            }


            checkEggs();  
        }

        private void checkEggs()
        {
            if(KyunGame.Instance.maxPeak > .9)
            {
                int cCount = Controls.Count(x => x is Egg);

                if (cCount >= 6)
                    return;
                int magic = OsuBeatMap.rnd.Next(6, 12);
                for (int a = cCount; a < magic; a++)
                {
                    Controls.Add(new Egg {
                        Position = new Vector2(ActualScreenMode.Width + OsuBeatMap.rnd.Next(100, ActualScreenMode.Width), OsuBeatMap.rnd.Next(300, ActualScreenMode.Height - 100)),                        
                    });
                }

                mltoadd = 0;
            }
        }
    }
}
