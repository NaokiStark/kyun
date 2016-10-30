using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ubeat.Beatmap;
using ubeat.UIObjs;
using ubeat.Score;
using ubeat.GameScreen.UI;
using ubeat.Screen;

namespace ubeat.GameScreen
{

    public class Grid : IScreen
    {

        #region PublicVars

        public List<ScreenUIObject> Controls { get; set; } //Dummy
        public IScreen ScreenInstance { get; set; }

        public bool autoMode = false;
        public bool NoFailMode = false;
        public bool inGame;
        public bool Paused { get; set; }

        public static Grid Instance = null;

        public List<IUIObject> objs = new List<IUIObject>();
        public ubeatBeatMap bemap;
        public Texture2D bg;
        public HealthBar Health { get; set; }
        public ScoreDisplay ScoreDispl;
        public ComboDisplay ComboDspl;
        public int FailsCount = 0;
        public long GameTimeTotal = 0;
        public bool Visible { get; set; }
        bool cooldown { get; set; }


        // TODO: TEST ONLY
        private int offset = 0;

        #endregion

        #region PrivateVars

        bool nomoreobjectsplsm8;
        bool failed;
        bool started { get; set; }
        int actualIndex = 0;

        List<IHitObj> hitObjects = new List<IHitObj>();
        List<List<IHitObj>> grid = new List<List<IHitObj>>();
        public Texture2D Background { get; set; }
        Video.VideoPlayer videoplayer;
        Combo combo;

        Texture2D lastFrameOfVid;

        public GameTime songGameTime { get; set; }
        public TimeSpan timePosition { get; set; }
        public DateTime? lastUpdate { get; set; }
        Label FPSMetter;

        public delegate void onend();
        public event onend endedd;
        #endregion

        #region Constructor
        public Grid(Beatmap.ubeatBeatMap beatmap)
        {
            Instance = this;
            ScreenInstance = this;

            bemap = beatmap;

            for (int a = 0; a < 9; a++)
            {
                grid.Add(new List<IHitObj>());
            }
            songGameTime = new GameTime();
            Health = new HealthBar();
            Health.OnFail += Health_OnFail;
            ScoreDispl = new ScoreDisplay();
            ComboDspl = new ComboDisplay();
            videoplayer = Video.VideoPlayer.Instance;
            combo = new Combo();

            List<ScreenMode> scmL = ScreenModeManager.GetSupportedModes();

            ScreenMode ActualMode = scmL[Settings1.Default.ScreenMode];

            Vector2 meas = UbeatGame.Instance.defaultFont.MeasureString("ubeat") * .85f;

            FPSMetter = new Label();
            FPSMetter.Text = "fps";
            FPSMetter.Scale = .75f;
            FPSMetter.Position = new Vector2(0, ActualMode.Height - meas.Y);
        }
        #endregion  

        #region PrivateMethods
        void UpdateSongGameTime()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan elapsed = now - lastUpdate ?? TimeSpan.Zero;

            timePosition += elapsed;
            songGameTime = new GameTime(timePosition, elapsed);
            lastUpdate = now;
        }

        void ResetSongGameTime()
        {
            lastUpdate = null;
            timePosition = new TimeSpan();

            UpdateSongGameTime();
        }

        void cleanObjects()
        {
            for (int a = 0; a < grid.Count; a++)
                grid[a].Clear();
            objs.Clear();
        }

        void Health_OnFail()
        {

            Logger.Instance.Info("Game Failed");
            failed = true;
            ScoreDispl.IsActive = false;
            ComboDspl.IsActive = false;
            Pause();
        }

        void addTextureG()
        {
            int wid = (UbeatGame.Instance.buttonDefault.Bounds.Width + 20) * 3;
            int hei = (UbeatGame.Instance.buttonDefault.Bounds.Height + 20) * 3;
            bg = new Texture2D(UbeatGame.Instance.GraphicsDevice, wid, hei);

            Color[] data = new Color[wid * hei];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            bg.SetData(data);

        }

        #endregion

        #region PublicMethods
        public static Vector2 GetPositionFor(int index)
        {

            int posYY = 0;
            int posXX = index;

            int sWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int sHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (index > 6)
            {
                posYY = 1;
                if (index == 7)
                    posXX = 1;
                else if (index == 8)
                    posXX = 2;
                else if (index == 9)
                    posXX = 3;
            }
            else if (index > 3)
            {
                posYY = 2;
                if (index == 4)
                    posXX = 1;
                else if (index == 5)
                    posXX = 2;
                else if (index == 6)
                    posXX = 3;
            }
            else if (index > 0)
            {
                posYY = 3;
                posXX = index;
            }



            int x = (sWidth / 2) + (UbeatGame.Instance.buttonDefault.Bounds.Width + 20) * posXX;
            int y = (sHeight / 2) + (UbeatGame.Instance.buttonDefault.Bounds.Height + 20) * posYY;


            x = x - (UbeatGame.Instance.buttonDefault.Bounds.Width + 20) * 2 - (UbeatGame.Instance.buttonDefault.Bounds.Width / 2);
            y = y - (UbeatGame.Instance.buttonDefault.Bounds.Height + 20) * 2 - (UbeatGame.Instance.buttonDefault.Bounds.Height / 2);
            return new Vector2(x, y);
        }

        public void Pause()
        {
            if (!Paused)
            {
                Logger.Instance.Info("Game Paused");
                UbeatGame.Instance.Player.Paused = true;
                Paused = !Paused;
            }
            else
            {
                Logger.Instance.Info("Game unpaused");
                UbeatGame.Instance.Player.Paused = false;
                Paused = !Paused;
            }
        }

        public void Play(Beatmap.ubeatBeatMap beatmap = null, bool automode = false)
        {
            endedd += Grid_endedd;
            UbeatGame.Instance.IsMouseVisible = false;
            ScoreDispl.Reset();
            ScoreDispl.IsActive = true;
            ComboDspl.IsActive = true;
            combo.ResetAll();
            actualIndex = 0;
            GameTimeTotal = 0;
            failed = false;
            cooldown = true;
            autoMode = automode;
            addTextureG();
            UbeatGame.Instance.Player.Stop();
            if (beatmap != null)
                bemap = beatmap;
            //Start
            try
            {
                FileStream filestream = new FileStream(bemap.Background, FileMode.Open, FileAccess.Read);
                Background = Texture2D.FromStream(UbeatGame.Instance.GraphicsDevice, filestream);
                filestream.Close();
            }
            catch
            {
                Logger.Instance.Warn("this beatmap has not image");
            }

            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                ResetSongGameTime();
                inGame = true;
            }));
            Logger.Instance.Info("Game Started: {0} - {1} [{2}]", bemap.Artist, bemap.Title, bemap.Version);

            if (UbeatGame.Instance.VideoEnabled)
                if (bemap.Video != null)
                    if (bemap.Video != "")
                        videoplayer.Play(bemap.Video);

            th.Start();
        }

        void Grid_endedd()
        {
            UbeatGame.Instance.IsMouseVisible = false;

        }
        #endregion

        void skip()
        {
            endedd();
            if (Waiting)
            {
                Waiting = false;
                Background = null;
                ScreenManager.ChangeTo(new ScoreScreen());
                videoplayer.Stop();
            }
        }

        //TEST ONLY

        void changeOffset(bool ter)
        {
            if (ter)
                offset--;
            else
                offset++;
            Console.WriteLine("ACTUAL OFFSET: " + offset);
            long pos = UbeatGame.Instance.Player.Position;
            Console.WriteLine("pos - OFFSETTED/PLAYER: " + (pos + offset) + "/" + pos);
            Console.WriteLine("DIFF: " + (pos + offset - pos));
        }

        #region GameEvents
        public void Update(GameTime tm)
        {
            if (!Visible)
                return;

            int fpsm = (int)Math.Round((double)UbeatGame.Instance.frameCounter.AverageFramesPerSecond, 0);
            FPSMetter.Text = fpsm.ToString("0", CultureInfo.InvariantCulture) + " FPS";
            FPSMetter.Update();

            if (Waiting)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    SpaceAlredyPressed = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.Space))
                {
                    if (SpaceAlredyPressed)
                    {
                        SpaceAlredyPressed = false;
                        skip();
                    }
                }
            }


            if (!Paused)
            {
                //IUIObject
                for (int b = 0; b < objs.Count; b++)
                {
                    if (objs[b] is ApproachObj)
                        continue;

                    objs[b].Update();
                }
            }

            if (!inGame)
                return;



            if (inGame && GameTimeTotal > bemap.SleepTime && UbeatGame.Instance.Player.PlayState == NAudio.Wave.PlaybackState.Stopped)
            {
                UbeatGame.Instance.Player.Play(bemap.SongPath);
                UbeatGame.Instance.Player.soundOut.Volume = UbeatGame.Instance.GeneralVolume;
                
            }

            if (started)
            {
                Health.Start(bemap.OverallDifficulty);
                started = false;
            }

            Health.Update();
            ScoreDispl.Update();
            ComboDspl.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                EscapeAlredyPressed = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Escape))
            {
                if (EscapeAlredyPressed)
                {
                    EscapeAlredyPressed = false;
                    backPressed();
                }
            }


            // TEST
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                UpAlredyPressed = true;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Up))
            {
                if (UpAlredyPressed)
                {
                    UpAlredyPressed = false;
                    changeOffset(true);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                DownAlredyPressed = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Down))
            {
                if (DownAlredyPressed)
                {
                    DownAlredyPressed = false;
                    changeOffset(false);
                }
            }

            //long pos = (long)Game1.Instance.player.Position;
            if (!Paused)
            {
                if (UbeatGame.Instance.Player.PlayState == NAudio.Wave.PlaybackState.Stopped)
                    GameTimeTotal += (long)tm.ElapsedGameTime.TotalMilliseconds;
                else
                    GameTimeTotal = UbeatGame.Instance.Player.Position + bemap.SleepTime;
            }


            long pos = GameTimeTotal + offset;
            if (!Paused)
            {
                pos = GameTimeTotal;
                if (actualIndex <= bemap.HitObjects.Count - 1)
                {
                    long startTime = (long)bemap.HitObjects[actualIndex].StartTime - (long)(1950 - bemap.ApproachRate * 150);

                    if (pos > startTime)
                    {
                        if (bemap.HitObjects[actualIndex] is HitHolder)
                            bemap.HitObjects[actualIndex].AddTexture(UbeatGame.Instance.buttonHolder);
                        else
                            bemap.HitObjects[actualIndex].AddTexture(UbeatGame.Instance.buttonDefault);

                        bemap.HitObjects[actualIndex].Start(pos);

                        grid[bemap.HitObjects[actualIndex].Location - 97].Add(bemap.HitObjects[actualIndex]);

                        if (actualIndex == 0)
                            started = true;

                        actualIndex++;

                    }
                }
                else
                {
                    //end
                    if (!nomoreobjectsplsm8)
                    {

                        nomoreobjectsplsm8 = true;
                    }
                }
            }


            //IHitObj

            for (int a = 0; a < grid.Count; a++)
            {
                for (int c = 0; c < grid[a].Count; c++)
                {

                    //if (c == 0)
                    // {
                    Vector2 poss = GetPositionFor(a + 1);
                    if (grid[a][c].apo != null)
                        grid[a][c].apo.Apdeit(poss);

                    grid[a][c].Update(pos, poss);

                    // }
                    if (grid[a][c].Died)
                    {
                        grid[a].Remove(grid[a][c]);
                    }
                }
            }




            if (Paused)
            {
                if (failed)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.F2))
                    {
                        for (int a = 0; a < bemap.HitObjects.Count; a++)
                        {
                            bemap.HitObjects[a].Reset();
                        }
                        videoplayer.Stop();
                        UbeatGame.Instance.GameStart(this.bemap);
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.F2))
                    {
                        Logger.Instance.Info("Exit Game");
                        Health.Stop();
                        nomoreobjectsplsm8 = true;
                        inGame = false;
                        Background = null;
                        UbeatGame.Instance.Player.Paused = false;
                        Paused = false;
                        ScreenManager.ChangeTo(new BeatmapScreen());

                        videoplayer.Stop();
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.F3))
                    {
                        System.Threading.Thread.Sleep(100);
                        if (ElCosoQueSirveParaLasOpcionesDelJuegoYOtrasWeas.Settings.Instance == null)
                        {
                            new ElCosoQueSirveParaLasOpcionesDelJuegoYOtrasWeas.Settings().Show();
                        }
                    }
                }
            }

        }

        private void backPressed()
        {
            if (!Paused)
                Pause();
            else if (failed)
            {
                Logger.Instance.Info("Exit Game");
                Health.Stop();
                nomoreobjectsplsm8 = true;
                inGame = false;
                Background = null;
                UbeatGame.Instance.Player.Paused = false;
                Paused = false;
                ScreenManager.ChangeTo(new BeatmapScreen());

                videoplayer.Stop();
            }
            else
            {
                Pause();
            }
        }

        int VidFrame = 0;
        bool Waiting;


        public void Render()
        {


            if (!Visible)
                return;

            if (Waiting)
            {
                RenderWaiting();
                return;
            }

            bool onbreak = onBreak();
            int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;



            if (Background != null)
            {
                Rectangle screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)Background.Width / (float)Background.Height) * (float)screenHeight), screenHeight);
                UbeatGame.Instance.spriteBatch.Draw(Background, screenRectangle, null, Color.White, 0, new Vector2(Background.Width / 2, Background.Height / 2), SpriteEffects.None, 0);
            }
            else if (!inGame)
            {
                return;
            }

            RenderVideoFrame();

            //IN GAME
            FPSMetter.Render();
            if (!onbreak)
                Health.Render();

            if (Health.Enabled)
            {
                ScoreDispl.Render();
                ComboDspl.Render();
            }

            long pos = GameTimeTotal + offset;

            //draw square
            int sWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int sHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;
            int xi = (sWidth / 2) + (UbeatGame.Instance.buttonDefault.Bounds.Width + 10) * 1;
            int yi = (sHeight / 2) + (UbeatGame.Instance.buttonDefault.Bounds.Height + 10) * 1;


            xi = xi - (UbeatGame.Instance.buttonDefault.Bounds.Width + 20) * 2 - (UbeatGame.Instance.buttonDefault.Bounds.Width / 2);
            yi = yi - (UbeatGame.Instance.buttonDefault.Bounds.Height + 20) * 2 - (UbeatGame.Instance.buttonDefault.Bounds.Height / 2);

            if (Health.Enabled || cooldown)
                UbeatGame.Instance.spriteBatch.Draw(bg, new Vector2(xi, yi), Color.White * ((onbreak) ? 0f : .75f));

            int objectsCount = 0;
            for (int a = 0; a < grid.Count; a++)
            {
                for (int c = grid[a].Count - 1; c > -1; c--)
                {
                    objectsCount++;
                    Vector2 poss = GetPositionFor(a + 1);
                    poss = new Vector2(poss.X, poss.Y + (c * 5));

                    grid[a][c].Render(pos, poss);
                    if (grid[a][c].apo != null)
                        grid[a][c].apo.Render();

                    if (grid[a][c].Died)
                    {
                        grid[a].Remove(grid[a][c]);
                    }
                    else
                    {
                        objectsCount++;
                    }
                }
            }

            for (int b = 0; b < objs.Count; b++)
            {
                if (objs[b] is ApproachObj) continue;
                objs[b].Render();
            }

            if (nomoreobjectsplsm8 && inGame)
            {
                if (objectsCount < 1)
                {
                    inGame = false;
                    ScoreDispl.IsActive = false;
                    Health.Stop();
                    cooldown = false;
                    Health.Enabled = false;
                    Waiting = true;



                    Logger.Instance.Info("Game End: {0} - {1} [{2}]", bemap.Artist, bemap.Title, bemap.Version);

                }
            }

            if (Paused)
            {
                int splashW = UbeatGame.Instance.PauseSplash.Bounds.Width;
                int splashH = UbeatGame.Instance.PauseSplash.Bounds.Height;
                if (!failed)
                    UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.PauseSplash, new Rectangle(sWidth / 2 - splashW / 2, sHeight / 2 - splashH / 2, splashW, splashH), Color.White);
                else
                    UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.FailSplash, new Rectangle(sWidth / 2 - splashW / 2, sHeight / 2 - splashH / 2, splashW, splashH), Color.White);
            }
        }

        void RenderVideoFrame()
        {

            if (GameTimeTotal < bemap.VideoStartUp)
                return;

            if (nomoreobjectsplsm8)
            {
                GameTimeTotal = UbeatGame.Instance.Player.Position + bemap.SleepTime;
            }
            int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (UbeatGame.Instance.VideoEnabled)
            {
                Rectangle screenVideoRectangle = new Rectangle();
                if (!videoplayer.Stopped)
                {
                    if (VidFrame % Settings1.Default.VideoFrameSkip != 0 || Settings1.Default.VideoMode == 0 && !Paused)
                    {
                        byte[] frame = videoplayer.GetFrame(GameTimeTotal - bemap.SleepTime - bemap.VideoStartUp);
                        if (frame != null)
                        {

                            UbeatGame.Instance.spriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);

                            Texture2D texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, videoplayer.vdc.width, videoplayer.vdc.height);
                            screenVideoRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)texture.Width / (float)texture.Height) * (float)screenHeight), screenHeight);
                            texture.SetData(frame);
                            lastFrameOfVid = texture;
                            UbeatGame.Instance.spriteBatch.Draw(texture, screenVideoRectangle, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);
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
                                UbeatGame.Instance.spriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);
                                screenVideoRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)lastFrameOfVid.Width / (float)lastFrameOfVid.Height) * (float)screenHeight), screenHeight);
                                UbeatGame.Instance.spriteBatch.Draw(lastFrameOfVid, screenVideoRectangle, null, Color.White, 0, new Vector2(lastFrameOfVid.Width / 2, lastFrameOfVid.Height / 2), SpriteEffects.None, 0);
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

        int wait4End = 0;
        bool gEn;
        void RenderWaiting()
        {

            if (wait4End >= 0)
            {
                wait4End -= (int)UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;


                int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
                int screenHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

                if (Background != null)
                {
                    Rectangle screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)Background.Width / (float)Background.Height) * (float)screenHeight), screenHeight);
                    UbeatGame.Instance.spriteBatch.Draw(Background, screenRectangle, null, Color.White, 0, new Vector2(Background.Width / 2, Background.Height / 2), SpriteEffects.None, 0);
                }

                RenderVideoFrame();
                FPSMetter.Render();

                UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.SpaceSkip,
                    new Rectangle(screenWidth - (UbeatGame.Instance.SpaceSkip.Width / 2),
                        screenHeight - (UbeatGame.Instance.SpaceSkip.Height / 2),
                        UbeatGame.Instance.SpaceSkip.Width / 2,
                        UbeatGame.Instance.SpaceSkip.Height / 2),
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);


                for (int b = 0; b < objs.Count; b++)
                {
                    if (objs[b] is ApproachObj) continue;
                    objs[b].Render();
                }
                if (!gEn)
                {
                    int diff = (int)(UbeatGame.Instance.Player.soundOut.TotalTime.TotalMilliseconds - UbeatGame.Instance.Player.Position);
                    if (diff > 2000 && diff < 60000)
                    {
                        gEn = true;
                        wait4End = diff;
                    }
                    else
                    {
                        gEn = true;
                        wait4End = 2000;
                    }
                }
            }
            else if (wait4End < 0)
            {
                gEn = false;
                endedd();
                Waiting = false;
                Background = null;
                ScreenManager.ChangeTo(new ScoreScreen());
                videoplayer.Stop();
            }
        }

        public void Redraw()
        {
            //Magic things
        }

        public bool onBreak()
        {
            long actualTime = GameTimeTotal - bemap.SleepTime;
            foreach (Beatmap.Break brk in bemap.Breaks)
            {
                if (brk.Start < actualTime && brk.End > actualTime)
                    return true;
            }

            return false;
        }
        #endregion

        public bool SpaceAlredyPressed { get; set; }

        public bool EscapeAlredyPressed { get; set; }

        public bool UpAlredyPressed { get; set; }

        public bool DownAlredyPressed { get; set; }
    }
}
