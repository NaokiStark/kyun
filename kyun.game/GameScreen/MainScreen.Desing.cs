using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using kyun.GameScreen.UI.Buttons;
using kyun.Screen;
using kyun.Notifications;
using kyun.Utils;
using kyun.GameScreen.UI.Particles;
using System.IO;
using kyun.GameScreen.UI;
using Microsoft.Xna.Framework.Graphics;
using kyun.Beatmap;
using kyun.game.GameScreen.UI;
using System.Diagnostics;

namespace kyun.GameScreen
{
    public partial class MainScreen : ScreenBase
    {
        public void LoadInterface()
        {
            /*
            if (KyunGame.Instance.ppyMode)
                Audio.AudioPlaybackEngine.Instance.PlaySound(KyunGame.Instance.WelcomeToOsuXd);*/

            //Controls = new HashSet<UIObjectBase>();

            ScreenMode ActualMode = ScreenModeManager.GetActualMode();

            Vector2 center = new Vector2(ActualMode.Width / 2, ActualMode.Height / 2);

            Logo = new UI.Image(SpritesContent.Instance.IsoLogo) { BeatReact = true };
            Logo.Position = new Vector2(center.X - (Logo.Texture.Width / 2),
                center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);

            Logo.Click += Logo_Click;

            Logo.Tooltip = new Tooltip
            {
                Text = "Click me!"
            };

            /*
            Logo.Effect = SpritesContent.Instance.RGBShiftEffect;
            Logo.EffectParameters.Add(new KeyValuePair<string, object>("DisplacementDist", 1f));
            Logo.EffectParameters.Add(new KeyValuePair<string, object>("DisplacementScroll", .1f));
            */


            objectsInitialPosition[0] = Logo.Position;

            CnfBtn = new ConfigButton();
            CnfBtn.Position = new Vector2(center.X - (CnfBtn.Texture.Width / 2),
                center.Y - (CnfBtn.Texture.Height / 2) + Logo.Texture.Height / 2);
            objectsInitialPosition[2] = CnfBtn.Position;

            StrBtn = new StartButton();
            StrBtn.Position = new Vector2(center.X - (StrBtn.Texture.Width / 2),
                (center.Y - (StrBtn.Texture.Height / 2) - StrBtn.Texture.Height - 2) + Logo.Texture.Height / 2);
            objectsInitialPosition[1] = StrBtn.Position;

            ExtBtn = new ExitButton();
            ExtBtn.Position = new Vector2(center.X - (ExtBtn.Texture.Width / 2),
                center.Y - (ExtBtn.Texture.Height / 2) + ExtBtn.Texture.Height + 2 + Logo.Texture.Height / 2);
            objectsInitialPosition[3] = ExtBtn.Position;

            coverSize = 75;

            Vector2 meas = SpritesContent.Instance.DefaultFont.MeasureString("ubeat") * .85f;

            Label1 = new UI.Label();
            Label1.Text = "Hello world";
            Label1.Scale = .85f;
            Label1.Position = new Vector2(0);
            Label1.Visible = false;

            FPSMetter = new UI.Label();
            FPSMetter.Text = "0";
            FPSMetter.Scale = .75f;
            FPSMetter.Position = new Vector2(0, ActualMode.Height - meas.Y);

            ntfr = KyunGame.Instance.Notifications;

            int btnSize = SpritesContent.Instance.SquareButton.Width + 5;

            btnPrev = new SquareButton(Color.FromNonPremultiplied(89, 53, 122, 255))
            {
                ForegroundColor = Color.White,
                Caption = "<<",
                Position = new Vector2(0, coverSize + 2)
            };

            btnPlay = new SquareButton(Color.FromNonPremultiplied(89, 53, 122, 255))
            {
                ForegroundColor = Color.White,
                Caption = ">",
                Position = new Vector2(btnPrev.Position.X + btnSize, btnPrev.Position.Y)
            };
            btnPause = new SquareButton(Color.FromNonPremultiplied(89, 53, 122, 255))
            {
                ForegroundColor = Color.White,
                Caption = "||",
                Position = new Vector2(btnPrev.Position.X + btnSize * 2, btnPrev.Position.Y)
            };

            btnStop = new SquareButton(Color.FromNonPremultiplied(89, 53, 122, 255))
            {
                ForegroundColor = Color.White,
                Caption = "[-]",
                Position = new Vector2(btnPrev.Position.X + btnSize * 3, btnPrev.Position.Y)
            };

            btnNext = new SquareButton(Color.FromNonPremultiplied(89, 53, 122, 255))
            {
                ForegroundColor = Color.White,
                Caption = ">>",
                Position = new Vector2(btnPrev.Position.X + btnSize * 4, btnPrev.Position.Y)
            };
            

            filledRect1 = new UI.FilledRectangle(
               new Vector2(ActualMode.Width, ActualMode.Height),
               Color.FromNonPremultiplied(43, 39, 48, (int)(255 * 0.5f)));

            filledRect1.Position = Vector2.Zero;


            particleEngine = new ParticleEngine();

            DlbmLbl = new Label(.7f)
            {
                Text = "Download Beatmaps",
                Font = SpritesContent.Instance.StandardButtonsFont,
                Position = new Vector2(ActualMode.Width - SpritesContent.Instance.StandardButtonsFont.MeasureString("Download Beatmaps").X - 15,
                                       ActualMode.Height - SpritesContent.Instance.StandardButtonsFont.LineSpacing - 10),
                RoundCorners = true,
                Opacity = .8f
            };


            string[] songs = { "Junk - enchanted.mp3" };

            string[] bgs = { "bg.jpg", "" };
            if (KyunGame.xmas)
            {
                bgs[0] = "xmas2.jpg";
            }
            float[] mspb = { 483.90999274135f, 428f };

            int song = /*getRndNotRepeated(0, songs.Length - 1)*/0;

            mainBm = new ubeatBeatMap()
            {
                Artist = "Junk",
                BPM = mspb[song],
                SongPath = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\" + songs[song],
                ApproachRate = 10,
                Background = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\" + bgs[song],
                Creator = "Fabi",
                OverallDifficulty = 10,
                Video = "",
                VideoStartUp = 0,
                Version = "",
                SleepTime = 0,
                Title = "enchanted",

            };



            System.Drawing.Image cimg = System.Drawing.Image.FromFile(mainBm.Background);

            System.Drawing.Bitmap cbimg = SpritesContent.ResizeImage(cimg, (int)(((float)cimg.Width / (float)cimg.Height) * coverSize), (int)coverSize);

            System.Drawing.Bitmap ccbimg;
            MemoryStream istream;
            if (cbimg.Width != cbimg.Height)
            {
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)((cbimg.Width - coverSize) / 2), 0, (int)coverSize, (int)coverSize));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }


            //coverimg = new Image(Texture2D.FromStream(KyunGame.Instance.GraphicsDevice, (Stream)istream))
            coverimg = new Image(ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, (Stream)istream))
            {
                BeatReact = false,
                Position = new Vector2(0, 0),
            };

            coverBox = new FilledRectangle(new Vector2((SpritesContent.Instance.SettingsFont.MeasureString(mainBm.Title).X * .8f) + 20, coverSize), Color.Black * .8f)
            {
                Position = new Vector2(coverSize, coverimg.Position.Y)
            };

            coverLabel = new Label(0)
            {
                Text = mainBm.Title,
                Font = SpritesContent.Instance.SettingsFont,
                Position = new Vector2(coverSize + 5, coverimg.Position.Y),
                Scale = .8f
            };

            coverLabelArt = new Label(0)
            {
                Text = mainBm.Artist,
                Font = SpritesContent.Instance.SettingsFont,
                Position = new Vector2(coverSize + 15, coverimg.Position.Y + 20),
                Scale = .6f
            };

            userBox = UserBox.GetInstance();
            userBox.Position = new Vector2(ActualMode.Width - 275, 0);

            Controls.Add(particleEngine);
            Controls.Add(filledRect1);
            Controls.Add(CnfBtn);
            Controls.Add(StrBtn);
            Controls.Add(ExtBtn);
            Controls.Add(Logo);
            Controls.Add(Label1);
            //Controls.Add(FPSMetter);
            //Controls.Add(ntfr);

            //Music controls
            Controls.Add(btnNext);
            Controls.Add(btnPause);
            Controls.Add(btnPlay);
            Controls.Add(btnPrev);
            Controls.Add(btnStop);

            Controls.Add(coverBox);
            Controls.Add(coverimg);
            Controls.Add(coverLabel);
            Controls.Add(coverLabelArt);
            //Controls.Add(userBox);
            Controls.Add(DlbmLbl);

            OnLoad += MainScreen_OnLoad;
            DlbmLbl.Click += DlbmLbl_Click;

            OnLoadScreen();
        }

        private void DlbmLbl_Click(object sender, EventArgs e)
        {
            ntfr.ShowDialog("Drop downloaded \".osz\" file in this window to add your new beatmaps", 30000, NotificationType.Critical);
            Process.Start("https://bloodcat.com/osu/");
            
        }

        private void Logo_Click(object sender, EventArgs e)
        {
            if (StateHidden)
            {
                actualElapsed = 0;
                hidding = false;
                showing = true;
            }
        }

        public void changeCoverDisplay(string image)
        {
            EnphasisColor = ecolors[OsuUtils.OsuBeatMap.rnd.Next(0, ecolors.Count - 1)];
            float coverSize = 75;


            System.Drawing.Image cimg = null;


            if (!File.Exists(image))
            {

                if (SpritesContent.Instance.CroppedBg == null)
                {
                    using (FileStream ff = File.Open(SpritesContent.Instance.defaultbg, FileMode.Open))
                    {
                        cimg = System.Drawing.Image.FromStream(ff);
                        SpritesContent.Instance.CroppedBg = cimg;
                    }

                }

            }
            else if (File.GetAttributes(image) == FileAttributes.Directory)
            {
                if (SpritesContent.Instance.CroppedBg == null)
                {
                    using (FileStream ff = File.Open(SpritesContent.Instance.defaultbg, FileMode.Open))
                    {
                        cimg = System.Drawing.Image.FromStream(ff);
                        SpritesContent.Instance.CroppedBg = cimg;
                    }

                }
            }
            else
            {
                cimg = System.Drawing.Image.FromFile(image);
            }

            if (cimg == null)
            {
                cimg = SpritesContent.Instance.CroppedBg;
            }


            System.Drawing.Bitmap cbimg = SpritesContent.ResizeImage(cimg, (int)(((float)cimg.Width / (float)cimg.Height) * coverSize), (int)coverSize);

            System.Drawing.Bitmap ccbimg;
            MemoryStream istream;
            if (cbimg.Width != cbimg.Height)
            {
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)((cbimg.Width - coverSize) / 2), 0, (int)coverSize, (int)coverSize));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }

            coverimg.Texture = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, istream);
        }

        public override void Update(GameTime tm)
        {
            if(KyunGame.Instance.SelectedBeatmap != null)
            {
                if (NextTimingPoint == null)
                {
                    ActualTimingPoint = KyunGame.Instance.SelectedBeatmap.TimingPoints[0];
                    NextTimingPoint = KyunGame.Instance.SelectedBeatmap.GetNextTimingPointFor(ActualTimingPoint.Offset + 50);
                }
                else
                {
                    if (AVPlayer.audioplayer.PositionV2 >= NextTimingPoint.Offset - 50)
                    {
                        ActualTimingPoint = NextTimingPoint;/*Beatmap.GetTimingPointForV2(AVPlayer.audioplayer.PositionV2 + 50);*/
                        NextTimingPoint = KyunGame.Instance.SelectedBeatmap.GetNextTimingPointFor(ActualTimingPoint.Offset + 50);
                    }
                }
            }
            else
            {
                ActualTimingPoint = new osuBMParser.TimingPoint { Offset = 0, KiaiMode = true };
            }
            renderBeat = ActualTimingPoint.KiaiMode;
            //FPSMetter.Text = Game1.Instance.frameCounter.AverageFramesPerSecond.ToString("0", CultureInfo.InvariantCulture) +" FPS";

            int fps = (int)Math.Round(KyunGame.Instance.frameCounter.AverageFramesPerSecond, 0);

            if (countToHide < 7000)
                countToHide += tm.ElapsedGameTime.Milliseconds;
            

            if (countToHide >= 7000)
                foreach (UIObjectBase ctl in Controls)
                    ctl.Opacity = Math.Max(ctl.Opacity - tm.ElapsedGameTime.Milliseconds * .0001f, 0f);



            FPSMetter.Text = fps.ToString("0", CultureInfo.InvariantCulture) + " FPS";

            base.Update(tm);

            if (!Visible) return;

            if (!StateHidden)
            {
                actualElapsed += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
            }


            if (actualElapsed > maxElapsedToHide && !StateHidden)
            {
                StateHidden = true;
                HideControls();
            }

            if (hidding)
            {

                
                filledRect1.Visible = false;
                bool[] doneButtons = new bool[] { false, false, false };

                int elapsed = KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;

                if (!logoMoving)
                {
                    Logo.MoveTo(AnimationEffect.bounceOut, 500, new Vector2(Logo.Position.X, (ActualScreenMode.Height / 2) ),
                        () => {
                            doneLogo = true;
                            Logo.Texture = SpritesContent.Instance.LogoAtTwo;
                            Logo.Position = new Vector2((ActualScreenMode.Width / 2) - (Logo.Texture.Width / 2), (ActualScreenMode.Height / 2) - (Logo.Texture.Height / 2));
                        });
                    logoMoving = true;
                }

                /*
                if (Logo.Position.Y < (ActualScreenMode.Height / 2) - (Logo.Texture.Height / 2))
                {
                    Logo.Position = new Vector2(Logo.Position.X, Logo.Position.Y + ((float)elapsed * 0.5f));
                }
                else
                {
                    doneLogo = true;
                }
                */
                if (StrBtn.Position.Y > (ActualScreenMode.Height / 2) - (StrBtn.Texture.Height / 2))
                {
                    StrBtn.Position = new Vector2(StrBtn.Position.X, StrBtn.Position.Y - ((float)elapsed * 0.5f));



                }
                else
                {
                    doneButtons[0] = true;
                }
                if (StrBtn.Opacity - ((float)elapsed * 0.005f) > 0)
                {
                    StrBtn.Opacity -= ((float)elapsed * 0.005f);
                }
                else
                {
                    StrBtn.Opacity = 0;
                }
                if (CnfBtn.Position.Y > (ActualScreenMode.Height / 2) - (CnfBtn.Texture.Height / 2))
                {
                    CnfBtn.Position = new Vector2(CnfBtn.Position.X, CnfBtn.Position.Y - ((float)elapsed * 0.5f));
                    if (CnfBtn.Opacity - ((float)elapsed * 0.005f) > 0)
                    {
                        CnfBtn.Opacity -= ((float)elapsed * 0.005f);
                    }
                    else
                    {
                        CnfBtn.Opacity = 0;
                    }
                }
                else
                {
                    doneButtons[1] = true;
                }

                if (ExtBtn.Position.Y > (ActualScreenMode.Height / 2) - (ExtBtn.Texture.Height / 2))
                {
                    ExtBtn.Position = new Vector2(ExtBtn.Position.X, ExtBtn.Position.Y - ((float)elapsed * 0.5f));
                    if (ExtBtn.Opacity - ((float)elapsed * 0.005f) > 0)
                    {
                        ExtBtn.Opacity -= ((float)elapsed * 0.005f);
                    }
                    else
                    {
                        ExtBtn.Opacity = 0;
                    }
                }
                else
                {
                    doneButtons[2] = true;
                }

                if (doneLogo && doneButtons[0] && doneButtons[1] && doneButtons[2])
                {
                    ExtBtn.Visible = CnfBtn.Visible = StrBtn.Visible = false;
                    Logo.ChangeScale(.7f);
                    //Logo.Texture = SpritesContent.Instance.LogoAtTwo;
                    //Logo.Position = new Vector2((ActualScreenMode.Width / 2) - (SpritesContent.Instance.LogoAtTwo.Width / 2), (ActualScreenMode.Height / 2) - (SpritesContent.Instance.LogoAtTwo.Height / 2));
                    hidding = false;
                }
            }

            if (showing)
            {
                filledRect1.Visible = true;
                logoMoving = false;
                Logo.ChangeScale(1f);
                Logo.Texture = SpritesContent.Instance.IsoLogo;
                Logo.Position = new Vector2((ActualScreenMode.Width / 2) - (Logo.Texture.Width / 2), Logo.Position.Y);
                bool doneLogo = false;
                bool[] doneButtons = new bool[] { false, false, false };

                int elapsed = KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
                if (Logo.Position.Y > objectsInitialPosition[0].Y)
                {
                    Logo.Position = new Vector2(Logo.Position.X, Logo.Position.Y - ((float)elapsed * 0.5f));
                }
                else
                {
                    doneLogo = true;
                }

                if (StrBtn.Position.Y < objectsInitialPosition[1].Y)
                {
                    StrBtn.Position = new Vector2(StrBtn.Position.X, StrBtn.Position.Y + ((float)elapsed * 0.5f));


                }
                else
                {
                    doneButtons[0] = true;
                }

                if (StrBtn.Opacity + ((float)elapsed * 0.005f) < 1)
                {
                    StrBtn.Opacity += ((float)elapsed * 0.005f);
                }
                else
                {
                    StrBtn.Opacity = 1;
                }

                if (CnfBtn.Position.Y < objectsInitialPosition[2].Y)
                {
                    CnfBtn.Position = new Vector2(CnfBtn.Position.X, CnfBtn.Position.Y + ((float)elapsed * 0.5f));
                    if (CnfBtn.Opacity + ((float)elapsed * 0.005f) < 1)
                    {
                        CnfBtn.Opacity += ((float)elapsed * 0.005f);
                    }
                    else
                    {
                        CnfBtn.Opacity = 1;
                    }
                }
                else
                {
                    doneButtons[1] = true;
                }

                if (ExtBtn.Position.Y < objectsInitialPosition[3].Y)
                {
                    ExtBtn.Position = new Vector2(ExtBtn.Position.X, ExtBtn.Position.Y + ((float)elapsed * 0.5f));
                    if (ExtBtn.Opacity + ((float)elapsed * 0.005f) < 1)
                    {
                        ExtBtn.Opacity += ((float)elapsed * 0.005f);
                    }
                    else
                    {
                        ExtBtn.Opacity = 1;
                    }
                }
                else
                {
                    doneButtons[2] = true;
                }
                ExtBtn.Visible = CnfBtn.Visible = StrBtn.Visible = true;
                if (doneLogo && doneButtons[0] && doneButtons[1] && doneButtons[2])
                {
                    showing = false;
                    StateHidden = false;
                }
            }

            if (jukebox)
            {

                coverBox.Opacity = coverimg.Opacity = coverLabel.Opacity = coverLabelArt.Opacity = 1;

                Logo.Visible =
                    userBox.Visible =
                    btnNext.Visible = 
                    btnPause.Visible = 
                    btnPlay.Visible =
                    btnPrev.Visible =
                    btnStop.Visible =
                    false;

            }
            else
            {
                Logo.Visible =
                    userBox.Visible =
                    btnNext.Visible =
                    btnPause.Visible =
                    btnPlay.Visible =
                    btnPrev.Visible =
                    btnStop.Visible =
                    true;
            }
        }

        #region UI

        public ConfigButton CnfBtn;
        public StartButton StrBtn;
        public ExitButton ExtBtn;
        public UI.Image Logo;
        public UI.Label Label1;
        public UI.Label FPSMetter;
        private ButtonStandard btnNext;
        private ButtonStandard btnPrev;
        private ButtonStandard btnPlay;
        private ButtonStandard btnStop;
        private ButtonStandard btnPause;
        public ParticleEngine particleEngine;

        public Label DlbmLbl { get; private set; }

        private bool showing;

        Vector2[] objectsInitialPosition = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        private UI.FilledRectangle filledRect1;
        private Image coverimg;
        public FilledRectangle coverBox;
        public Label coverLabel;
        public Label coverLabelArt;
        public int coverSize;
        private UserBox userBox;
        private Notifier ntfr;
        private bool logoMoving;
        bool doneLogo = false;
        public ubeatBeatMap mainBm { get; private set; }
        public osuBMParser.TimingPoint ActualTimingPoint { get; set; }
        public osuBMParser.TimingPoint NextTimingPoint { get; set; }
        #endregion

    }
}
