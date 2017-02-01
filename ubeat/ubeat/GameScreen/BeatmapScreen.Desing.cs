using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using ubeat.Screen;
using ubeat.GameScreen.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ubeat.GameScreen.UI.Buttons;
using ubeat.Utils;

namespace ubeat.GameScreen
{
    public partial class BeatmapScreen : ScreenBase
    {

        static IScreen instance = null;
        public static IScreen Instance {
            get
            {
                if (instance == null)
                    instance = new BeatmapScreen();
                UbeatGame.Instance.KeyBoardManager.Enabled = true;
                return instance;
            }
            set
            {
                instance = value;
            }
        }

       
        Listbox lbox;
        ListboxDiff lBDff;
        FilledRectangle filledRect1;
        Label lblTitleDesc = new Label();
        AutoModeButton autoBtn;

        bool AMode = false;

        public void LoadInterface()
        {
            UbeatGame.Instance.IsMouseVisible = true;
            Controls = new List<UIObjectBase>();

            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            Vector2 lPos = new Vector2(0, 100);

            lbox = new Listbox(lPos, actualMode.Width / 2, actualMode.Height - 100, SpritesContent.Instance.ListboxFont);
            
            lbox.IndexChanged += lbox_IndexChanged;
            
            lBDff = new ListboxDiff(new Vector2(lbox.width, lbox.Position.Y), 200, 500, SpritesContent.Instance.ListboxFont);

            filledRect1 = new FilledRectangle(new Vector2(actualMode.Width, 4), Color.SpringGreen);
            filledRect1.Position = new Vector2(0, 96);

            lblTitleDesc = new Label(.98f) {
                Scale = 1f,
                Text = "",
                Position = new Vector2(0, 0),
                Size = new Vector2(actualMode.Width, 96),
                Font = SpritesContent.Instance.TitleFont
            };

            lblSearch = new Label(0f)
            {
                Scale = 1,
                Text = "",
                Position = new Vector2(actualMode.Width / 2, 48),
                Centered = true,
                Font = SpritesContent.Instance.SettingsFont
            };

            autoBtn = new AutoModeButton() {
                Position = new Vector2(actualMode.Width - SpritesContent.Instance.AutoModeButton.Width-20, actualMode.Height - SpritesContent.Instance.AutoModeButton.Height-20),
                Scale=.85f,
                PlayHit=true
            };

            autoBtn.Click += autoBtn_Click;

            Controls.Add(lbox);
            Controls.Add(lBDff);
            Controls.Add(filledRect1);
            Controls.Add(lblTitleDesc);
            Controls.Add(autoBtn);
            Controls.Add(lblSearch);

            OnLoad += BeatmapScreen_OnLoad;
            OnBackSpacePress += BeatmapScreen_OnBackSpacePress;


            addTextureG();

            OnLoadScreen();
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
                autoBtn.Texture = SpritesContent.Instance.AutoModeButton;
            }
            else
            {
                AMode = true;
                autoBtn.Texture = SpritesContent.Instance.AutoModeButtonSel;
            }
        }

        void addTextureG()
        {
            int wid = (SpritesContent.Instance.ButtonDefault.Bounds.Width + 20) * 3;
            int hei = (SpritesContent.Instance.ButtonDefault.Bounds.Height + 20) * 3;
            bg = new Texture2D(UbeatGame.Instance.GraphicsDevice, wid, hei);

            Color[] data = new Color[wid * hei];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            bg.SetData(data);

        }

        private Texture2D lastFrameOfVid;
        private Texture2D bg;

        public override void Update(GameTime tm)
        {
            if (isDisposing) return;
                        

            if(UbeatGame.Instance.Player.PlayState == NAudio.Wave.PlaybackState.Stopped)
            {
                videoPlayer?.Stop();
            }
            
            if (!UbeatGame.Instance.IsMouseVisible) UbeatGame.Instance.IsMouseVisible = true;

            base.Update(tm);

        }
        
        public override void Render()
        {
            if (!Visible || isDisposing) return;

            if (Background != null)
            {
                int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
                int screenHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

                Rectangle screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)Background.Width / (float)Background.Height) * (float)screenHeight), screenHeight);

                UbeatGame.Instance.SpriteBatch.Draw(Background, screenRectangle, null, Color.White, 0, new Vector2(Background.Width / 2, Background.Height / 2), SpriteEffects.None, 0);
            }

            //RenderVideoFrame();

            foreach (UIObjectBase ctr in Controls)
                ctr.Render();

        }

        void RenderVideoFrame()
        {

            int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (UbeatGame.Instance.VideoEnabled)
            {
                Rectangle screenVideoRectangle = new Rectangle();
                if (!videoPlayer.Stopped)
                {
                    if (VidFrame % Settings1.Default.VideoFrameSkip != 0 || Settings1.Default.VideoMode == 0)
                    {
                        byte[] frame = videoPlayer.GetFrame(UbeatGame.Instance.Player.Position + UbeatGame.Instance.SelectedBeatmap.SleepTime);
                        if (frame != null)
                        {

                            UbeatGame.Instance.SpriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);

                            Texture2D texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, videoPlayer.vdc.width, videoPlayer.vdc.height);
                            screenVideoRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)texture.Width / (float)texture.Height) * (float)screenHeight), screenHeight);
                            texture.SetData(frame);
                            lastFrameOfVid = texture;
                            UbeatGame.Instance.SpriteBatch.Draw(texture, screenVideoRectangle, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);
                            if (Settings1.Default.VideoMode > 0)
                                VidFrame++;
                            if (Settings1.Default.VideoMode == 0)
                                texture.Dispose();
                        }

                    }
                    else
                    {
                        VidFrame = 1;
                        if (lastFrameOfVid != null)
                        {
                            try
                            {
                                UbeatGame.Instance.SpriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);
                                screenVideoRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)lastFrameOfVid.Width / (float)lastFrameOfVid.Height) * (float)screenHeight), screenHeight);
                                UbeatGame.Instance.SpriteBatch.Draw(lastFrameOfVid, screenVideoRectangle, null, Color.White, 0, new Vector2(lastFrameOfVid.Width / 2, lastFrameOfVid.Height / 2), SpriteEffects.None, 0);
                                lastFrameOfVid.Dispose();
                            }
                            catch
                            {
                                ///???
                            }
                        }
                    }
                }
            }
        }

        public Label lblSearch { get; set; }
        public int VidFrame { get; private set; }
    }
}