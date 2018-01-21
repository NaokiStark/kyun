using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using kyun.Screen;
using kyun.GameScreen.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using kyun.GameScreen.UI.Buttons;
using kyun.Utils;
using System.IO;
using kyun.GameModes.Classic;
using kyun.game.GameModes;
using kyun.GameModes.OsuMode;
using kyun.game.GameModes.CatchIt;

namespace kyun.GameScreen
{
    public partial class BeatmapScreen : ScreenBase
    {

        static IScreen instance = null;
        public static IScreen Instance {
            get
            {
                if (instance == null)
                {
                    instance = new BeatmapScreen();
                    ((BeatmapScreen)instance).Random();
                }
                    
                KyunGame.Instance.KeyBoardManager.Enabled = true;
               
                return instance;
            }
            set
            {
                instance = value;
            }
        }

       
        public Listbox lbox;
        ListboxDiff lBDff;
        FilledRectangle filledRect1;
        Label lblTitleDesc = new Label();
        ButtonStandard autoBtn;

        bool AMode = false;

        public void LoadInterface()
        {
            KyunGame.Instance.IsMouseVisible = true;
            Controls = new List<UIObjectBase>();

            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            lblTitleDesc = new Label(.8f)
            {
                Scale = 1f,
                Text = "",
                Position = new Vector2(0, 0),
                Size = new Vector2(actualMode.Width, 50),
                Font = SpritesContent.Instance.StandardButtonsFont
            };

            filledRect1 = new FilledRectangle(new Vector2(actualMode.Width, 4), Color.FromNonPremultiplied(34, 92, 173, 255));
            filledRect1.Position = new Vector2(0, lblTitleDesc.Size.Y);

            filledRectBottom = new FilledRectangle(new Vector2(actualMode.Width, 75), Color.Black * .8f);
            filledRectBottom.Position = new Vector2(0, actualMode.Height - 75);

            filledRectBottomClr = new FilledRectangle(new Vector2(actualMode.Width, 4), Color.FromNonPremultiplied(34, 92, 173, 255));
            filledRectBottomClr.Position = filledRectBottom.Position;

            Vector2 lPos = new Vector2(0, lblTitleDesc.Size.Y + filledRect1.Texture.Height - 5);

            lbox = new Listbox(lPos, 400, actualMode.Height - filledRectBottom.Texture.Height - 80, SpritesContent.Instance.SettingsFont);
            
            lbox.IndexChanged += lbox_IndexChanged;

            lbox.autoAdjust = false;

            songDescImg = new Image(SpritesContent.Instance.SongDescBox) {
                Position = new Vector2(actualMode.Width - SpritesContent.Instance.SongDescBox.Width, lblTitleDesc.Size.Y + filledRect1.Texture.Height),
                BeatReact = false
            };


            coverSize = songDescImg.Texture.Width - 50;

            System.Drawing.Image cimg = null;

            if (SpritesContent.Instance.CroppedBg == null)
            {
                using (FileStream ff = File.Open(SpritesContent.Instance.defaultbg, FileMode.Open))
                {
                    cimg = System.Drawing.Image.FromStream(ff);
                    SpritesContent.Instance.CroppedBg = cimg;
                }

            }

            if (cimg == null)
            {
                cimg = SpritesContent.Instance.CroppedBg;
            }

            System.Drawing.Bitmap cbimg = SpritesContent.ResizeImage(cimg, (int)(((float)cimg.Width / (float)cimg.Height) * coverSize), (int)coverSize);

            System.Drawing.Bitmap ccbimg;
            MemoryStream istream;
            if (true)
            {
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)((cbimg.Width - coverSize) / 2), (int)((cbimg.Height - coverSize / 2.2f) / 2f), (int)coverSize, (int)(coverSize / 2.2f)));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }

            coverimg = new Image(SpritesContent.RoundCorners(ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, (Stream)istream), 5))
            {
                BeatReact = false,
                Position = new Vector2(songDescImg.Position.X + (songDescImg.Texture.Width / 2 - coverSize/2) , songDescImg.Position.Y + 20),
            };


            lBDff = new ListboxDiff(new Vector2(songDescImg.Position.X + 15, songDescImg.Position.Y + 30 + coverimg.Texture.Height), songDescImg.Texture.Width - 5 , songDescImg.Texture.Height - coverimg.Texture.Height - 30, SpritesContent.Instance.SettingsFont);


            lblSearch = new Label(0f)
            {
                Scale = 1,
                Text = "",
                Position = new Vector2(actualMode.Width / 2, 48),
                Centered = true,
                Font = SpritesContent.Instance.SettingsFont
            };

            btnStart = new ButtonStandard(Color.DarkOliveGreen)
            {
                Position = new Vector2(actualMode.Width - SpritesContent.Instance.ButtonStandard.Width - 20, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),
                Caption = "Play!",
                ForegroundColor = Color.White
            };

            autoBtn = new ButtonStandard(Color.DimGray) {
                Position = new Vector2(btnStart.Position.X - SpritesContent.Instance.ButtonStandard.Width - 50, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "Auto"

            };

            doubleBtn = new ButtonStandard(Color.DimGray)
            {
                Position = new Vector2(autoBtn.Position.X - SpritesContent.Instance.ButtonStandard.Width - 50, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "Double"

            };

            randomBtn = new ButtonStandard(Color.Aquamarine)
            {
                Position = new Vector2(doubleBtn.Position.X - SpritesContent.Instance.ButtonStandard.Width - 50, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "Random"

            };

            osuModeBtn = new ButtonStandard(Color.DimGray)
            {
                Position = new Vector2(randomBtn.Position.X - SpritesContent.Instance.ButtonStandard.Width - 50, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "CatchIt!"

            };


            backButton = new ButtonStandard(Color.DarkRed)
            {
                ForegroundColor = Color.White,
                Caption = "Back",
                Position = new Vector2(15, (filledRectBottom.Position.Y + 75/2) - (SpritesContent.Instance.ButtonStandard.Height/2)),
            };

            backButton.Click += BackButton_Click;

            autoBtn.Click += autoBtn_Click;

            btnStart.Click += BtnStart_Click;

            randomBtn.Click += RandomBtn_Click;

            doubleBtn.Click += DoubleBtn_Click;

            osuModeBtn.Click += OsuModeBtn_Click;

            Controls.Add(lbox);
            Controls.Add(filledRectBottom);
            Controls.Add(filledRectBottomClr);            
            Controls.Add(songDescImg);
            Controls.Add(lBDff);
            Controls.Add(filledRect1);
            Controls.Add(lblTitleDesc);
            Controls.Add(autoBtn);
            Controls.Add(randomBtn);
            Controls.Add(doubleBtn);
            Controls.Add(osuModeBtn);
            Controls.Add(btnStart);
            Controls.Add(lblSearch);
            Controls.Add(backButton);
            Controls.Add(coverimg);
            Controls.Add(randomBtn);
            

            OnLoad += BeatmapScreen_OnLoad;
            OnBackSpacePress += BeatmapScreen_OnBackSpacePress;


            addTextureG();

            OnLoadScreen();
        }

        private void OsuModeBtn_Click(object sender, EventArgs e)
        {
            if (_gamemode == GameMode.CatchIt)
            {
                _gamemode = GameMode.Classic;

                //autoBtn.Texture = SpritesContent.Instance.AutoModeButton;
                osuModeBtn.TextureColor = Color.DimGray;
            }
            else
            {
                _gamemode = GameMode.CatchIt;

                //autoBtn.Texture = SpritesContent.Instance.AutoModeButtonSel;
                osuModeBtn.TextureColor = Color.DarkRed;
            }
        }

        private void DoubleBtn_Click(object sender, EventArgs e)
        {
            if (doubleTime)
            {
                doubleTime = false;

                //autoBtn.Texture = SpritesContent.Instance.AutoModeButton;
                doubleBtn.TextureColor = Color.DimGray;
            }
            else
            {
                doubleTime = true;

                //autoBtn.Texture = SpritesContent.Instance.AutoModeButtonSel;
                doubleBtn.TextureColor = Color.DarkRed;
            }
        }

        private void RandomBtn_Click(object sender, EventArgs e)
        {
            Random();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (lBDff.selectedIndex < 0 || lBDff.selectedIndex > lBDff.Items.Count - 1) return;

            KyunGame.Instance.KeyBoardManager.Enabled = false;

            //UbeatGame.Instance.GameStart(lBDff.Items[lBDff.selectedIndex], this.AMode);

            //if (ClassicModeScreen.Instance == null)
            //    ClassicModeScreen.Instance = new ClassicModeScreen();

            //ClassicModeScreen.GetInstance().Play(lBDff.Items[lBDff.selectedIndex], (AMode) ? GameModes.GameMod.Auto : GameModes.GameMod.None);
            //ScreenManager.ChangeTo(ClassicModeScreen.GetInstance());

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
                    
                    ClassicModeScreen.GetInstance().Play(lBDff.Items[lBDff.selectedIndex], modes);

                    ScreenManager.ChangeTo(ClassicModeScreen.GetInstance());
                    break;
                case GameMode.Osu:
                  
                    OsuMode.GetInstance().Play(lBDff.Items[lBDff.selectedIndex], modes);

                    ScreenManager.ChangeTo(OsuMode.GetInstance());
                    break;
                case GameMode.CatchIt:
                    CatchItMode.GetInstance().Play(lBDff.Items[lBDff.selectedIndex], modes);

                    ScreenManager.ChangeTo(CatchItMode.GetInstance());
                    break;
            }
            /*

            if (ClassicModeScreen.Instance == null)
                ClassicModeScreen.Instance = new ClassicModeScreen();

           


            ClassicModeScreen.GetInstance().Play(lBDff.Items[lBDff.selectedIndex], modes);

            ScreenManager.ChangeTo(ClassicModeScreen.GetInstance());*/
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            BackPressed(MainScreen.Instance);
        }

        private void BeatmapScreen_OnBackSpacePress(object sender, EventArgs e)
        {
            //BackPressed(new MainScreen(false));
            BackPressed(MainScreen.Instance);
        }

        void autoBtn_Click(object sender, EventArgs e)
        {
            if (AMode)
            {
                AMode = false;
                //autoBtn.Texture = SpritesContent.Instance.AutoModeButton;
                autoBtn.TextureColor = Color.DimGray;
            }
            else
            {
                AMode = true;
                //autoBtn.Texture = SpritesContent.Instance.AutoModeButtonSel;
                autoBtn.TextureColor = Color.DarkRed;
            }
        }

        void addTextureG()
        {
            int wid = (SpritesContent.Instance.ButtonDefault.Bounds.Width + 20) * 3;
            int hei = (SpritesContent.Instance.ButtonDefault.Bounds.Height + 20) * 3;
            bg = new Texture2D(KyunGame.Instance.GraphicsDevice, wid, hei);

            Color[] data = new Color[wid * hei];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            bg.SetData(data);

        }

        private void changeCoverDisplay(string image)
        {


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

            if(cimg == null)
            {
                cimg = SpritesContent.Instance.CroppedBg;
            }


            System.Drawing.Bitmap cbimg = SpritesContent.ResizeImage(cimg, (int)(((float)cimg.Width / (float)cimg.Height) * coverSize), (int)coverSize);

            System.Drawing.Bitmap ccbimg;
            MemoryStream istream;
            if (true)
            {
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)((cbimg.Width - coverSize) / 2), (int)((cbimg.Height - coverSize / 2.2f) / 2f), (int)coverSize, (int)(coverSize/2.2f)));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }

            coverimg.Texture = SpritesContent.RoundCorners(ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, istream), 5);

            ((MainScreen)MainScreen.Instance).changeCoverDisplay(image);

            
        }

        private Texture2D bg;
        private FilledRectangle filledRectBottom;
        private FilledRectangle filledRectBottomClr;
        private ButtonStandard backButton;
        private Image songDescImg;
        private Image coverimg;
        private int coverSize;
        private ButtonStandard osuModeBtn;

        public override void Update(GameTime tm)
        {
            if (isDisposing) return;
                        
            if (!KyunGame.Instance.IsMouseVisible) KyunGame.Instance.IsMouseVisible = true;

            base.Update(tm);

        }
        
        public override void Render()
        {
            if (!Visible || isDisposing) return;
                     

            RenderBg();


            //foreach (UIObjectBase ctr in Controls)
            //    ctr.Render();

            RenderObjects();

        }

        public Label lblSearch { get; set; }
        public int VidFrame { get; private set; }
        public ButtonStandard btnStart { get; private set; }
        public ButtonStandard doubleBtn { get; private set; }
        public ButtonStandard randomBtn { get; private set; }
        public bool doubleTime { get; private set; }
    }
}