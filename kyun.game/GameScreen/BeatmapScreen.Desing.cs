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
using kyun.game.GameScreen;
using kyun.game.GameModes.CatchItCollab;
using kyun.game.GameScreen.UI.Scoreboard;
using kyun.game.Overlay;

namespace kyun.GameScreen
{
    public partial class BeatmapScreen : ScreenBase
    {

        static IScreen instance = null;
        public static IScreen Instance
        {
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
        internal ListboxDiff lBDff;
        FilledRectangle filledRect1;
        Label lblTitleDesc = new Label();
        private Image topBarBase;
        private Image topBarTail;
        ButtonStandard autoBtn;

        bool AMode = false;

        public void LoadInterface()
        {
            //KyunGame.Instance.IsMouseVisible = true;
            //Controls = new HashSet<UIObjectBase>(); Alredy in Base

            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            // === FIX THIS SHAPE ===
            
            lblTitleDesc = new Label(0)
            {
                Scale = 1f,
                Text = "",
                Position = new Vector2(SpritesContent.Instance.BeatmapBarBase.Height + 5, 0),
                //Size = new Vector2(actualMode.Width, 50),
                Font = SpritesContent.Instance.StandardButtonsFont
            };

            topBarBase = new Image(SpritesContent.Instance.BeatmapBarBase)
            {
                Position = Vector2.Zero,
                Size = new Vector2(actualMode.Width - 512, SpritesContent.Instance.BeatmapBarBase.Height),
                BeatReact = false
            };

            topBarTail = new Image(SpritesContent.Instance.BeatmapBarTail)
            {
                Position = new Vector2(actualMode.Width - 512, 0),
                BeatReact = false
            };


            // ========= DELETE THIS ============

            filledRect1 = new FilledRectangle(new Vector2(actualMode.Width, 4), Color.FromNonPremultiplied(34, 92, 173, 255));
            filledRect1.Position = new Vector2(0, lblTitleDesc.Size.Y);

            filledRectBottom = new FilledRectangle(new Vector2(actualMode.Width, 75), Color.Black * .8f);
            filledRectBottom.Position = new Vector2(0, actualMode.Height - 75);

            filledRectBottomClr = new FilledRectangle(new Vector2(actualMode.Width, 4), Color.FromNonPremultiplied(34, 92, 173, 255));
            filledRectBottomClr.Position = filledRectBottom.Position;

            // ========= DELETE THIS ============

            // cover image

            coverImage = new Image(SpritesContent.Instance.DefaultBackground)
            {
                Position = new Vector2(2,2),
                BeatReact = false
            };

            Vector2 lPos = new Vector2(0, topBarBase.Texture.Height);

            lbox = new Listbox(lPos, 400, actualMode.Height - topBarBase.Texture.Height - 80, SpritesContent.Instance.SettingsFont);

            lbox.IndexChanged += lbox_IndexChanged;

            lbox.autoAdjust = false;

            songDescImg = new Image(SpritesContent.Instance.SongDescBox)
            {
                Position = new Vector2(actualMode.Width - SpritesContent.Instance.SongDescBox.Width, lblTitleDesc.Size.Y + filledRect1.Texture.Height),
                BeatReact = false
            };

            songListDiffImg = new Image(SpritesContent.Instance.DiffSelector)
            {
                Position = new Vector2(SpritesContent.Instance.ScrollListBeatmap_alt.Width + 20, actualMode.Height / 2 - (SpritesContent.Instance.DiffSelector.Height / 2)),
                BeatReact = false
            };
            


            lBDff = new ListboxDiff(
                new Vector2(songListDiffImg.Position.X + 15, songListDiffImg.Position.Y + 15),
                songListDiffImg.Texture.Width - 5,
                songListDiffImg.Texture.Height,
                SpritesContent.Instance.SettingsFont);


            lblSearch = new Label(0f)
            {
                Scale = 1,
                Text = "",
                Position = new Vector2(actualMode.Width / 2, 48),
                Centered = true,
                Font = SpritesContent.Instance.SettingsFont
            };

            int buttonSpace = 20;

            btnStart = new ButtonStandard(Color.DarkOliveGreen)
            {
                Position = new Vector2(actualMode.Width - SpritesContent.Instance.ButtonStandard.Width - 20, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),
                Caption = "Play!",
                ForegroundColor = Color.White
            };

            autoBtn = new ButtonStandard(Color.DimGray)
            {
                Position = new Vector2(btnStart.Position.X - SpritesContent.Instance.ButtonStandard.Width - buttonSpace, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "Auto"

            };

            doubleBtn = new ButtonStandard(Color.DimGray)
            {
                Position = new Vector2(autoBtn.Position.X - SpritesContent.Instance.ButtonStandard.Width - buttonSpace, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "Double"

            };

            randomBtn = new ButtonStandard(Color.Aquamarine)
            {
                Position = new Vector2(doubleBtn.Position.X - SpritesContent.Instance.ButtonStandard.Width - buttonSpace, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "Random"

            };

            osuModeBtn = new ButtonStandard(Color.DimGray)
            {
                Position = new Vector2(randomBtn.Position.X - SpritesContent.Instance.ButtonStandard.Width - buttonSpace, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "Modes"

            };

            osuModeBtn2 = new ButtonStandard(Color.DimGray)
            {
                Position = new Vector2(osuModeBtn.Position.X - SpritesContent.Instance.ButtonStandard.Width - buttonSpace, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),

                //Scale=.85f,
                ForegroundColor = Color.White,
                Caption = "osu!"

            };


            _scoreboard = Scoreboard.Instance;

            _scoreboard.Position = new Vector2(actualMode.Width - _scoreboard.Size.X, 100);

            backButton = new ButtonStandard(Color.DarkRed)
            {
                ForegroundColor = Color.White,
                Caption = "Back",
                Position = new Vector2(15, (filledRectBottom.Position.Y + 75 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),
            };


            // Label SortBy
            lblSortBy = new Label()
            {
                Position = new Vector2(actualMode.Width - 192, 40),
                Text = "Sort by:",
                Font = SpritesContent.Instance.GeneralBig,
                RoundCorners = true
            };

            // button by title

            btnByTitle = new Label()
            {
                Position = new Vector2(lblSortBy.Position.X + 60, 40),
                Text = "Title",
                Font = SpritesContent.Instance.GeneralBig,
                RoundCorners = true
            };

            // button by artist

            btnByArtist = new Label()
            {
                Position = new Vector2(lblSortBy.Position.X + 60 + 40, 40),
                Text = "Artist",
                Font = SpritesContent.Instance.GeneralBig,
                RoundCorners = true
            };

            backButton.Click += BackButton_Click;

            autoBtn.Click += autoBtn_Click;

            btnStart.Click += BtnStart_Click;

            randomBtn.Click += RandomBtn_Click;

            doubleBtn.Click += DoubleBtn_Click;

            osuModeBtn.Click += OsuModeBtn_Click;
            
            osuModeBtn2.Click += OsuModeBtn2_Click;

            btnByTitle.Click += BtnByTitle_Click;

            btnByArtist.Click += BtnByArtist_Click;

            //Main Listbox
            Controls.Add(lbox);

            // == Delete this ==
            Controls.Add(filledRectBottom);
            //Controls.Add(filledRectBottomClr);
            //

            // Top bar
            Controls.Add(topBarBase);
            Controls.Add(topBarTail);

            // Cover in top

            Controls.Add(coverImage);

            // Label "Sort By"

            Controls.Add(lblSortBy);

            // Label by title

            Controls.Add(btnByTitle);

            // Label by artist

            Controls.Add(btnByArtist);

            // delete dis
            //Controls.Add(songDescImg);

            Controls.Add(songListDiffImg);
            Controls.Add(lBDff);

            //Controls.Add(filledRect1);

            Controls.Add(lblTitleDesc);
            Controls.Add(autoBtn);
            Controls.Add(randomBtn);
            Controls.Add(doubleBtn);

            Controls.Add(osuModeBtn);
            //Controls.Add(osuModeBtn2);

            Controls.Add(btnStart);
            Controls.Add(lblSearch);
            
            Controls.Add(backButton);
            
            Controls.Add(randomBtn);
            Controls.Add(_scoreboard);


            OnLoad += BeatmapScreen_OnLoad;
            OnBackSpacePress += BeatmapScreen_OnBackSpacePress;
            

            addTextureG();

            OnLoadScreen();
        }

        private void BtnByArtist_Click(object sender, EventArgs e)
        {
            orderByArtist();
            lbox.Items = InstanceManager.AllBeatmaps;
        }

        private void BtnByTitle_Click(object sender, EventArgs e)
        {
            orderByTitle();
            lbox.Items = InstanceManager.AllBeatmaps;
        }

        private void OsuModeBtn2_Click(object sender, EventArgs e)
        {
            if (_gamemode == GameMode.Osu)
            {

                _gamemode = GameMode.CatchIt;

                //autoBtn.Texture = SpritesContent.Instance.AutoModeButton;
                osuModeBtn2.TextureColor = Color.DimGray;
            }
            else
            {
                if (_gamemode == GameMode.Classic)
                {
                    osuModeBtn.TextureColor = Color.DimGray;
                }

                _gamemode = GameMode.Osu;

                //autoBtn.Texture = SpritesContent.Instance.AutoModeButtonSel;
                osuModeBtn2.TextureColor = Color.DarkRed;
            }
        }

        private void OsuModeBtn_Click(object sender, EventArgs e)
        {
            ModeOverlay.Instance.Show();
            ScreenManager.ShowOverlay(ModeOverlay.Instance);
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
                    GameLoader.GetInstance().LoadBeatmapAndRun(lBDff.Items.Beatmaps[lBDff.selectedIndex], ClassicModeScreen.GetInstance(), modes);
                    break;
                case GameMode.Osu:
                    GameLoader.GetInstance().LoadBeatmapAndRun(lBDff.Items.Beatmaps[lBDff.selectedIndex], OsuMode.GetInstance(), modes);
                    break;
                case GameMode.CatchIt:
                    GameLoader.GetInstance().LoadBeatmapAndRun(lBDff.Items.Beatmaps[lBDff.selectedIndex], CatchItMode.GetInstance(), modes);
                    break;
            }

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

            coverSize = SpritesContent.Instance.BeatmapBarBase.Height - 10;

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

            coverImage.Texture = SpritesContent.RoundCorners(ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, istream), 5);

            ((MainScreen)MainScreen.Instance).changeCoverDisplay(image);


        }

        private Texture2D bg;
        private FilledRectangle filledRectBottom;
        private FilledRectangle filledRectBottomClr;
        private ButtonStandard backButton;
        private Label lblSortBy;
        private Label btnByTitle;
        private Label btnByArtist;
        public Image songDescImg;
        private Image songListDiffImg;

        public int coverSize;
        private ButtonStandard osuModeBtn;
        private ButtonStandard osuModeBtn2;
        private Scoreboard _scoreboard;
        private Image coverImage;

        public override void Update(GameTime tm)
        {
            if (isDisposing) return;

            //if (!KyunGame.Instance.IsMouseVisible) KyunGame.Instance.IsMouseVisible = true;

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