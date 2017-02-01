using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ubeat.GameScreen.UI;
using ubeat.Screen;
using ubeat.Utils;
using ubeat.Video;

namespace ubeat.GameScreen
{
    public class LoadScreen : ScreenBase
    {
        private Image Logo;
        private Image LoadingSpinner;
        private AudioVideoPlayer auplayer;
        private FilledRectangle rectanglexd;
        private float spinnerRotation;

        
        private bool loadDone;
        private Label labelEndLoad;
        private Label labelLoadingText;

        public LoadScreen()
        {
            
            auplayer = new AudioVideoPlayer();
            Background = SpritesContent.Instance.DefaultBackground;
            

            ScreenMode mode = ScreenModeManager.GetActualMode();

            Vector2 logoPosition = new Vector2((mode.Width / 2) - (SpritesContent.Instance.Logo.Width / 2), (mode.Height / 2) - (SpritesContent.Instance.Logo.Height / 2) - SpritesContent.Instance.Logo.Height /2);

            Logo = new Image(SpritesContent.Instance.Logo)
            {
                Position = logoPosition,
                BeatReact = false,
                Visible = false
                                
            };

            rectanglexd = new FilledRectangle(new Vector2(300, 150), Color.Black * .75f);

            rectanglexd.Position = new Vector2((mode.Width / 2) - (rectanglexd.Texture.Width / 2), mode.Height - rectanglexd.Texture.Height);


            labelLoadingText = new Label(0) {
                Text = "Loading",
                Position = new Vector2(rectanglexd.Position.X + (rectanglexd.Texture.Width / 2), rectanglexd.Position.Y + 10),
                Centered = true,
                Scale = 1,
                Font = SpritesContent.Instance.TitleFont,
                Visible = true
            };
            

            Vector2 spinnerPosition = new Vector2((mode.Width / 2), labelLoadingText.Position.Y + 20 + SpritesContent.Instance.LoadingSpinnerTx.Height);

            LoadingSpinner = new UI.Image(SpritesContent.Instance.LoadingSpinnerTx)
            {
                Position = spinnerPosition
            };

            

            labelEndLoad = new Label(0) {
                Text = "Press Space to skip",
                Position = new Vector2(LoadingSpinner.Position.X, LoadingSpinner.Position.Y - 20),
                Centered = true,
                Scale = 1,
                Font = SpritesContent.Instance.TitleFont,
                Visible = false
            };

            Controls.Add(auplayer);
            Controls.Add(rectanglexd);
            Controls.Add(Logo);
            Controls.Add(labelLoadingText);
            Controls.Add(labelEndLoad);
            Controls.Add(LoadingSpinner);

            onKeyPress += LoadScreen_onKeyPress;

            System.Windows.Forms.Application.DoEvents();
            Thread tr = new Thread(new ThreadStart(loadBeatmaps));
            tr.IsBackground = true;

            tr.Start();

            auplayer.Play(AppDomain.CurrentDomain.BaseDirectory + @"\Assets\miku.mp3",
                          AppDomain.CurrentDomain.BaseDirectory + @"\Assets\miku.avi");


            UbeatGame.Instance.IsMouseVisible = true;
        }

        private void LoadScreen_onKeyPress(object sender, InputEvents.KeyPressEventArgs args)
        {
            if(args.Key == Microsoft.Xna.Framework.Input.Keys.Space)
            {
                if (loadDone)
                {
                    auplayer.Stop();
                    ScreenManager.ChangeTo(MainScreen.Instance);
                }
            }
        }

        private void checkToEnd()
        {
            if (loadDone)
            {
                LoadingSpinner.Visible = false;
                labelEndLoad.Visible = true;
                labelLoadingText.Text = "Loaded!";

            }

            if(loadDone && auplayer.audioplayer.PlayState == NAudio.Wave.PlaybackState.Stopped)
            {
                auplayer.Stop();
                ScreenManager.ChangeTo(MainScreen.Instance);
            }
        }

        internal override void UpdateControls()
        {
            base.UpdateControls();
            checkToEnd();
           
        }

        public override void Render()
        {
            //RenderBg();
            float elapsed = (float)UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalSeconds*12;
            spinnerRotation += elapsed;

            float circle = MathHelper.Pi * 2;
            spinnerRotation = spinnerRotation % circle;


            foreach (UIObjectBase ctr in Controls)
            {
                if (ctr.Equals(LoadingSpinner))
                {
                    if (!ctr.Visible)
                        continue;

                    UbeatGame.Instance.SpriteBatch.Draw(LoadingSpinner.Texture, new Rectangle(
                        (int)LoadingSpinner.Position.X,
                        (int)LoadingSpinner.Position.Y,
                        LoadingSpinner.Texture.Width,
                        LoadingSpinner.Texture.Height
                        ), null, Color.White, spinnerRotation, new Vector2(LoadingSpinner.Texture.Width /2, LoadingSpinner.Texture.Height/2), SpriteEffects.None, 0);
                    continue;
                }

                ctr.Render();
            }
            RenderPeak();
        }


        void loadBeatmaps()
        {
            Logger.Instance.Info("");
            Logger.Instance.Info("Loading beatmaps.");
            Logger.Instance.Info("");


            if (!InstanceManager.Instance.IntancedBeatmaps)
            {
                InstanceManager.AllBeatmaps = new List<Beatmap.Mapset>();
                if (Settings1.Default.osuBeatmaps != "")
                {

                    DirectoryInfo osuDirPath = new DirectoryInfo(Settings1.Default.osuBeatmaps);
                    if (osuDirPath.Exists)
                    {
                        DirectoryInfo[] osuMapsDirs = osuDirPath.GetDirectories();
                        int flieCnt = 0;


                        int fCount = osuMapsDirs.Length;
                        int dCount = 0;

                        foreach (DirectoryInfo odir in osuMapsDirs)
                        {
                            System.Windows.Forms.Application.DoEvents();

                            dCount++;
                            FileInfo[] fils = odir.GetFiles();
                            // Mapset
                            Beatmap.Mapset bmms = null;
                            foreach (FileInfo fff in fils)
                            {

                                if (fff.Extension.ToLower() == ".osu")
                                {

                                    flieCnt++;
                                    OsuUtils.OsuBeatMap bmp = OsuUtils.OsuBeatMap.FromFile(fff.FullName);
                                    if (bmp != null)
                                    {

                                        //Beatmaps.Add(bmp);
                                        if (bmms == null)
                                            bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator, bmp.Tags);
                                        bmms.Add(bmp);


                                    }

                                    // Debug.WriteLine("File: {0}s", flieCnt);
                                }

                            }
                            if (bmms != null)
                            {
                                Beatmap.Mapset mapst = Beatmap.Mapset.OrderByDiff(bmms);

                                InstanceManager.AllBeatmaps.Add(mapst);
                            }
                            float pctg = (float)dCount / (float)fCount * 100f;
                            if (pctg % 20 == 0)
                                Logger.Instance.Info("-> {0}%", pctg);
                        }
                    }
                    else
                    {
                        Logger.Instance.Warn("Could not find Osu! beatmaps folder, please, make sure that if exist.");
                    }
                }
                else
                {
                    Logger.Instance.Warn("osu! beatmaps is not setted, if you have osu beatmaps, set folder in config and restart ubeat.");
                }
            }
            loadLocalMaps();
            Logger.Instance.Info("");
            Logger.Instance.Info("Done.");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");
            Logger.Instance.Info("");

            loadDone = true;
            
        }


        void loadLocalMaps()
        {
            if (!InstanceManager.Instance.IntancedBeatmaps)
            {

                Logger.Instance.Info("Loading local");

                DirectoryInfo osuDirPath = new DirectoryInfo(Path.Combine(System.Windows.Forms.Application.StartupPath, "Maps"));
                if (!osuDirPath.Exists)
                    osuDirPath.Create();

                DirectoryInfo[] osuMapsDirs = osuDirPath.GetDirectories();
                int flieCnt = 0;


                int fCount = osuMapsDirs.Length;
                int dCount = 0;

                foreach (DirectoryInfo odir in osuMapsDirs)
                {
                    System.Windows.Forms.Application.DoEvents();

                    dCount++;
                    FileInfo[] fils = odir.GetFiles();
                    // Mapset
                    Beatmap.Mapset bmms = null;
                    foreach (FileInfo fff in fils)
                    {

                        if (fff.Extension.ToLower() == ".osu")
                        {

                            flieCnt++;
                            OsuUtils.OsuBeatMap bmp = OsuUtils.OsuBeatMap.FromFile(fff.FullName);
                            if (bmp != null)
                            {

                                //Beatmaps.Add(bmp);
                                if (bmms == null)
                                    bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator, bmp.Tags);
                                bmms.Add(bmp);


                            }

                            // Debug.WriteLine("File: {0}s", flieCnt);
                        }

                    }
                    if (bmms != null)
                    {
                        Beatmap.Mapset mapst = Beatmap.Mapset.OrderByDiff(bmms);

                        InstanceManager.AllBeatmaps.Add(mapst);
                    }
                    float pctg = (float)dCount / (float)fCount * 100f;
                    if (pctg % 20 == 0)
                        Logger.Instance.Info("-> {0}%", pctg);
                }

                InstanceManager.AllBeatmaps = InstanceManager.AllBeatmaps.OrderBy(x => x.Artist).ToList<Beatmap.Mapset>();
                InstanceManager.Instance.IntancedBeatmaps = true;
            }
        }
    }
}
