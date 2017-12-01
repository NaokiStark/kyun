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

namespace kyun.GameScreen
{
    public partial class MainScreen : ScreenBase
    {
        public void LoadInterface()
        {
            if (KyunGame.Instance.ppyMode)
                Audio.AudioPlaybackEngine.Instance.PlaySound(KyunGame.Instance.WelcomeToOsuXd);

            Controls = new List<UIObjectBase>();

            ScreenMode ActualMode = ScreenModeManager.GetActualMode();

            Vector2 center = new Vector2(ActualMode.Width / 2, ActualMode.Height / 2);

            Logo = new UI.Image(SpritesContent.Instance.IsoLogo) { BeatReact = true };
            Logo.Position = new Vector2(center.X - (Logo.Texture.Width / 2),
                center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);

            Logo.Click += Logo_Click;

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

            ntfr = new Notifier();

            int btnSize = 100;

            btnNext = new ButtonStandard(Color.FromNonPremultiplied(89, 53, 122, 255))
            {
                ForegroundColor = Color.White,
                Caption = ">>",
                Position = new Vector2(ActualMode.Width - btnSize, 0)
            };

            btnPrev = new ButtonStandard(Color.FromNonPremultiplied(89, 53, 122, 255))
            {
                ForegroundColor = Color.White,
                Caption = "<<",                
                Position = new Vector2(ActualMode.Width - (btnSize * 5), 0)
            };                
            btnPlay = new ButtonStandard(Color.FromNonPremultiplied(89, 53, 122, 255))
            {
                ForegroundColor = Color.White,
                Caption = ">",
                Position = new Vector2(ActualMode.Width - (btnSize * 4), 0)
            };
            btnStop = new ButtonStandard(Color.FromNonPremultiplied(89, 53, 122, 255))
            {
                ForegroundColor = Color.White,
                Caption = "[-]",
                Position = new Vector2(ActualMode.Width - (btnSize * 2), 0)
            };
            btnPause = new ButtonStandard(Color.FromNonPremultiplied(89, 53, 122, 255))
            {
                ForegroundColor = Color.White,
                Caption = "||",
                Position = new Vector2(ActualMode.Width - (btnSize * 3), 0)
            };

            filledRect1 = new UI.FilledRectangle(
               new Vector2(ActualMode.Width, ActualMode.Height),
               Color.FromNonPremultiplied(43, 39, 48, (int)(255 * 0.5f)));

            filledRect1.Position = Vector2.Zero;


            particleEngine = new ParticleEngine();

            


            string[] songs = { "Junk - enchanted.mp3" };
            
            string[] bgs = { "bg.jpg", "" };
            float[] mspb = { 483.90999274135f, 428f };

            if (KyunGame.Instance.ppyMode)
            {
                songs[2] = "CirclesClick_xddd.mp3";
                bgs[1] = "ppy.jpg";
                mspb[1] = 326;
            }

            int song = /*getRndNotRepeated(0, songs.Length - 1)*/0;

            mainBm = new ubeatBeatMap()
            {
                Artist = (!KyunGame.Instance.ppyMode) ? "Junk" : "Nekodex",
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
                Title = (!KyunGame.Instance.ppyMode) ? "enchanted" : "Circles",

            };

            coverSize = 75;

            System.Drawing.Image cimg = System.Drawing.Image.FromFile(mainBm.Background);

            System.Drawing.Bitmap cbimg = SpritesContent.ResizeImage(cimg, (int)(((float)cimg.Width / (float)cimg.Height) * coverSize), (int)coverSize);

            System.Drawing.Bitmap ccbimg;
            MemoryStream istream;
            if (cbimg.Width != cbimg.Height)
            {
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)((cbimg.Width - coverSize) /2) , 0, (int)coverSize, (int)coverSize));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }

            coverimg = new Image(Texture2D.FromStream(KyunGame.Instance.GraphicsDevice, (Stream)istream))
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


            Controls.Add(particleEngine);
            Controls.Add(filledRect1);
            Controls.Add(CnfBtn);
            Controls.Add(StrBtn);
            Controls.Add(ExtBtn);
            Controls.Add(Logo);
            Controls.Add(Label1);
            //Controls.Add(FPSMetter);
            Controls.Add(ntfr);

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


            KyunGame.Instance.IsMouseVisible = true;
            OnLoad += MainScreen_OnLoad;

            OnLoadScreen();
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

            System.Drawing.Image cimg;

            if (!File.Exists(image))
            {
                MemoryStream mms = new MemoryStream();
                SpritesContent.Instance.DefaultBackground.SaveAsPng(mms, SpritesContent.Instance.DefaultBackground.Width, SpritesContent.Instance.DefaultBackground.Height);
                cimg = System.Drawing.Image.FromStream(mms);
            }
            else if(File.GetAttributes(image) == FileAttributes.Directory)
            {
                MemoryStream mms = new MemoryStream();
                SpritesContent.Instance.DefaultBackground.SaveAsPng(mms, SpritesContent.Instance.DefaultBackground.Width, SpritesContent.Instance.DefaultBackground.Height);
                cimg = System.Drawing.Image.FromStream(mms);
            }
            else
            {
                cimg = System.Drawing.Image.FromFile(image);
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

            coverimg.Texture = Texture2D.FromStream(KyunGame.Instance.GraphicsDevice, istream);
        }

        public override void Update(GameTime tm)
        {

            //FPSMetter.Text = Game1.Instance.frameCounter.AverageFramesPerSecond.ToString("0", CultureInfo.InvariantCulture) +" FPS";

            int fps = (int)Math.Round(KyunGame.Instance.frameCounter.AverageFramesPerSecond, 0);



            FPSMetter.Text = fps.ToString("0", CultureInfo.InvariantCulture) + " FPS";

            base.Update(tm);

            if (!Visible) return;

            if (!StateHidden)
            {
                actualElapsed += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
            }
            

            if(actualElapsed > maxElapsedToHide && !StateHidden)
            {
                StateHidden = true;
                HideControls();
            }

            if (hidding)
            {

                bool doneLogo = false;
                filledRect1.Visible = false;
                bool[] doneButtons = new bool[]{false,false,false};

                int elapsed = KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
                if(Logo.Position.Y < (ActualScreenMode.Height / 2 ) - (Logo.Texture.Height / 2 ))
                {
                    Logo.Position = new Vector2(Logo.Position.X, Logo.Position.Y + ((float)elapsed * 0.5f));
                }
                else
                {
                    doneLogo = true;
                }

                if (StrBtn.Position.Y > (ActualScreenMode.Height / 2) - (StrBtn.Texture.Height / 2))
                {
                    StrBtn.Position = new Vector2(StrBtn.Position.X, StrBtn.Position.Y + ((float)elapsed * 0.5f));
                    
                 
                    
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
                
                if(doneLogo && doneButtons[0] && doneButtons[1] && doneButtons[2])
                {
                    ExtBtn.Visible = CnfBtn.Visible = StrBtn.Visible = false;
                    Logo.ChangeScale(.7f);
                    Logo.Texture = SpritesContent.Instance.LogoAtTwo;
                    Logo.Position = new Vector2((ActualScreenMode.Width / 2) - (Logo.Texture.Width / 2), (ActualScreenMode.Height / 2) - (Logo.Texture.Height /2));
                    hidding = false;
                }
            }

            if (showing)
            {
                filledRect1.Visible = true;
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
        }
        
        #region UI

        public ConfigButton CnfBtn;
        public StartButton StrBtn;
        public ExitButton ExtBtn;
        public UI.Image Logo;
        public UI.Label Label1;
        public UI.Label FPSMetter;
        private Notifier ntfr;
        private ButtonStandard btnNext;
        private ButtonStandard btnPrev;
        private ButtonStandard btnPlay;
        private ButtonStandard btnStop;
        private ButtonStandard btnPause;
        public ParticleEngine particleEngine;
        private bool showing;

        Vector2[] objectsInitialPosition = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        private UI.FilledRectangle filledRect1;
        private Image coverimg;
        public FilledRectangle coverBox;
        public Label coverLabel;
        public Label coverLabelArt;
        public int coverSize;

        public ubeatBeatMap mainBm { get; private set; }
        #endregion

    }
}
