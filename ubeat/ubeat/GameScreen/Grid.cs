using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ubeat.Beatmap;
using ubeat.UIObjs;
using ubeat.Score;
namespace ubeat.GameScreen
{
    
    public class Grid
    {

        #region PublicVars

        public bool autoMode = false;
        public bool inGame;
        public bool Paused { get; set; }

        public static Grid Instance = null;

        public List<IUIObject> objs = new List<IUIObject>();
        public Beatmap.ubeatBeatMap bemap;
        public Texture2D bg;
        public HealthBar Health { get; set; }
        public ScoreDisplay ScoreDispl;
        public ComboDisplay ComboDspl;
        public int FailsCount = 0;

        #endregion

        #region PrivateVars

        bool nomoreobjectsplsm8;
        bool failed;
        bool started { get; set; }
        int actualIndex = 0;

        List<IHitObj> hitObjects = new List<IHitObj>();
        List<List<IHitObj>> grid = new List<List<IHitObj>>();        
        Texture2D Background;       
        Video.VideoPlayer videoplayer;
        Combo combo;

        Texture2D lastFrameOfVid;

        public GameTime songGameTime { get; set; }
        public TimeSpan timePosition { get; set; }
        public DateTime? lastUpdate { get; set; }
        
        #endregion

        #region Constructor 
        public Grid(Beatmap.ubeatBeatMap beatmap)
        {
            Instance = this;
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
            videoplayer = new Video.VideoPlayer();
            combo = new Combo();
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
            ScoreDispl.isActive = false;
            ComboDspl.isActive = false;
            Pause();
        }

        void addTextureG()
        {
            int wid = (Game1.Instance.buttonDefault.Bounds.Width+40)*3;
            int hei = (Game1.Instance.buttonDefault.Bounds.Height + 40) * 3;
            bg = new Texture2D(Game1.Instance.GraphicsDevice, wid, hei);

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

            int sWidth = Game1.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int sHeight = Game1.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

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



            int x = (sWidth / 2) + (Game1.Instance.buttonDefault.Bounds.Width + 20) * posXX;
            int y = (sHeight / 2) + (Game1.Instance.buttonDefault.Bounds.Height + 20) * posYY;


            x = x - (Game1.Instance.buttonDefault.Bounds.Width + 20) * 2 - (Game1.Instance.buttonDefault.Bounds.Width / 2);
            y = y - (Game1.Instance.buttonDefault.Bounds.Height + 20) * 2 - (Game1.Instance.buttonDefault.Bounds.Height / 2);
            return new Vector2(x, y);
        }

        public void Pause()
        {
            if (!Paused)
            {
                Logger.Instance.Info("Game Paused");
                Game1.Instance.player.Paused = true;
                Paused = !Paused;
            }
            else
            {
                Logger.Instance.Info("Game unpaused");
                Game1.Instance.player.Paused = false;
                Paused = !Paused;
            }
        }

        public void Play(Beatmap.ubeatBeatMap beatmap = null)
        {
            ScoreDispl.Reset();
            ScoreDispl.isActive = true;
            ComboDspl.isActive = true;
            combo.ResetAll();
            actualIndex = 0;
            failed = false;
            autoMode = BeatmapSelector.Instance.checkBox1.Checked;
            addTextureG();
            Game1.Instance.player.Stop();
            if(beatmap!=null)
                bemap = beatmap;
            //Start
            try
            {
                FileStream filestream = new FileStream(bemap.Background, FileMode.Open, FileAccess.Read);
                Background = Texture2D.FromStream(Game1.Instance.GraphicsDevice, filestream);
                filestream.Close();
            }
            catch
            {
                Logger.Instance.Warn("this beatmap has not image");
            }
            
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(()=>
            {
                
                if (bemap.SleepTime > 0)
                {
                    System.Threading.Thread.Sleep(bemap.SleepTime);


                    Logger.Instance.Info("Audio LeadIn: " + bemap.SleepTime);
                }
                else
                {
                    var sttime = bemap.HitObjects[0].StartTime;
                    var differ = sttime - (long)(1950 - bemap.ApproachRate * 150);
                    if (differ < 500)
                        System.Threading.Thread.Sleep(3000);
                }

                Game1.Instance.player.Play(bemap.SongPath/*, bemap.SleepTime*/);
                Game1.Instance.player.soundOut.Volume =Game1.Instance.GeneralVolume;
                
                if(bemap.Video!=null)
                    if(bemap.Video!="")
                        videoplayer.Play(bemap.Video);

                ResetSongGameTime();
                inGame = true;
            }));
            Logger.Instance.Info("Game Started: {0} - {1} [{2}]", bemap.Artist, bemap.Title, bemap.Version);
            th.Start();
        }
        #endregion

        #region GameEvents
        public void Update(GameTime tm)
        {
           
            if (!inGame)
                return;

            if (started)
            {
                Health.Start(bemap.OverallDifficulty);
                started = false;
            }

            Health.Update();
            ScoreDispl.Update();
            ComboDspl.Update();
            long pos = (long)Game1.Instance.player.Position;
            if(!Paused){
                
                if (actualIndex <= bemap.HitObjects.Count - 1)
                {
                    long startTime = (long)bemap.HitObjects[actualIndex].StartTime - (long)(1950 - bemap.ApproachRate * 150);

                    if (pos > startTime )
                    {
                        if (bemap.HitObjects[actualIndex] is HitHolder)
                            bemap.HitObjects[actualIndex].AddTexture(Game1.Instance.buttonHolder);
                        else
                            bemap.HitObjects[actualIndex].AddTexture(Game1.Instance.buttonDefault);

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
                    if (c == 0)
                    {
                        Vector2 poss = GetPositionFor(a + 1);
                        grid[a][c].Update(pos, poss);

                    }
                    if (grid[a][c].Died)
                    {
                        grid[a].Remove(grid[a][c]);
                    }
                }
            }

            if (!Paused)
            {
                //IUIObject
                for (int b = 0; b < objs.Count; b++)
                    objs[b].Update();
            }   


            if (Paused)
            {
                if (failed)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        Logger.Instance.Info("Exit Game");
                        Health.Stop();
                        nomoreobjectsplsm8 = true;
                        inGame = false;
                        Background = null;
                        Game1.Instance.player.Paused = false;
                        Paused = false;
                        MainWindow.Instance.Show();
                        BeatmapSelector.Instance.Show();
                        videoplayer.Stop();
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.F2))
                    {
                        for (int a = 0; a < bemap.HitObjects.Count; a++)
                        {
                            bemap.HitObjects[a].Reset();
                        }
                        videoplayer.Stop();
                        Game1.Instance.GameStart(this.bemap);
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        System.Threading.Thread.Sleep(200);
                        Pause();
                        
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.F2))
                    {
                        Logger.Instance.Info("Exit Game");
                        Health.Stop();
                        nomoreobjectsplsm8 = true;
                        inGame = false;
                        Background = null;
                        Game1.Instance.player.Paused = false;
                        Paused = false;
                        MainWindow.Instance.Show();
                        BeatmapSelector.Instance.Show();
                        videoplayer.Stop();
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !Paused)
            {
                Pause();
                System.Threading.Thread.Sleep(200);
            }           
        }

        public void Render()
        {
            int screenWidth = Game1.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = Game1.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            if (Background != null)
            {              
                Game1.Instance.spriteBatch.Draw(Background, screenRectangle, Color.White);
            }
            else if(!inGame)
            {
                return;
            }
            
            

            if (!videoplayer.Stopped)
            {
                byte[] frame = videoplayer.GetFrame();
                if (frame != null)
                {

                    Texture2D texture = new Texture2D(Game1.Instance.GraphicsDevice, videoplayer.vdc.width, videoplayer.vdc.height);

                    texture.SetData(frame);
                    lastFrameOfVid = texture;
                    Game1.Instance.spriteBatch.Draw(texture, screenRectangle, Color.White);

                }
                else
                {
                    if (lastFrameOfVid != null)
                    {
                        Game1.Instance.spriteBatch.Draw(lastFrameOfVid, screenRectangle, Color.White);
                    }
                }
            }


            //IN GAME

            Health.Render();
            ScoreDispl.Render();
            ComboDspl.Render();
            long pos = (long)Game1.Instance.player.Position;            
            //draw square
            int sWidth = Game1.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int sHeight = Game1.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;
            int xi = (sWidth / 2) + (Game1.Instance.buttonDefault.Bounds.Width + 20) * 1;
            int yi = (sHeight / 2) + (Game1.Instance.buttonDefault.Bounds.Height + 20) * 1;


            xi = xi - (Game1.Instance.buttonDefault.Bounds.Width + 40) * 2 - (Game1.Instance.buttonDefault.Bounds.Width / 2);
            yi = yi - (Game1.Instance.buttonDefault.Bounds.Height + 40) * 2 - (Game1.Instance.buttonDefault.Bounds.Height / 2);
            Game1.Instance.spriteBatch.Draw(bg, new Vector2(xi,yi), Color.White*.75f);
           
            int objectsCount = 0;
            for (int a = 0; a < grid.Count; a++)
            {
                for (int c = grid[a].Count-1; c > -1; c--)
                {
                    objectsCount++;
                    Vector2 poss = GetPositionFor(a+1);
                    grid[a][c].Render(pos,poss);
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
                objs[b].Render();

            if (nomoreobjectsplsm8 && inGame)
            {
                if (objectsCount < 1)
                {
                    inGame = false;
                    ScoreDispl.isActive = false;
                    Health.Stop();
                    System.Threading.Thread thr = new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                        System.Threading.Thread.Sleep(1200);
                        Background = null;
                        MainWindow.Instance.ShowAsync();
                        videoplayer.Stop();
                    }));
                    Logger.Instance.Info("Game End: {0} - {1} [{2}]", bemap.Artist, bemap.Title, bemap.Version);

                    thr.Start();                 
                }
            }

            if (Paused)
            {
                int splashW=Game1.Instance.PauseSplash.Bounds.Width;
                int splashH=Game1.Instance.PauseSplash.Bounds.Height;
                if(!failed)
                    Game1.Instance.spriteBatch.Draw(Game1.Instance.PauseSplash, new Rectangle(sWidth / 2 - splashW/2, sHeight / 2-splashH/2, splashW, splashH), Color.White);
                else
                    Game1.Instance.spriteBatch.Draw(Game1.Instance.FailSplash, new Rectangle(sWidth / 2 - splashW / 2, sHeight / 2 - splashH / 2, splashW, splashH), Color.White);
            }
        }

        #endregion
    }
}
