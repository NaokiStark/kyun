using kyun.Audio;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.GameScreen.UI.Particles;
using kyun.OsuUtils;
using kyun.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameScreen
{
    public class Jukebox : AnimatedScreenBase
    {
        static Jukebox instance = null;
        private ParticleEngine particleEngine;
        private Image coverimg;
        private bool switchParticle;

        public int coverSize = 75;

        public int[] EnphasisColor { get; private set; }

        List<int[]> sColors = new List<int[]>();
        private bool squareYesNo;

        public static IScreen Instance
        {
            get
            {
                if (instance == null)
                    instance = new Jukebox();

                return instance;
            }
        }
        public Jukebox()
        {
            particleEngine = new ParticleEngine();

            ChangeCoverDispl();
            

            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);

            changeEmphasis();
            Controls.Add(particleEngine);
            Controls.Add(coverimg);

            onKeyPress += Jukebox_onKeyPress;
            KyunGame.Instance.OnPeak += Instance_OnPeak;
        }

        private void ChangeCoverDispl()
        {
            System.Drawing.Image cimg = System.Drawing.Image.FromFile(KyunGame.Instance.SelectedMapset.Beatmaps[0].Background);

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

            if(coverimg == null)
            {
                coverimg = new Image(ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, (Stream)istream))
                {
                    BeatReact = false,
                    Position = new Vector2(0, 0),
                };
            }
            else
            {
                coverimg.Texture = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, (Stream)istream);
            }

        }

        public void ChangeSongDisplay()
        {

            ChangeCoverDispl();

            changeEmphasis();
            ChangeBackground(KyunGame.Instance.SelectedMapset.Beatmaps[0].Background);
        }

        private void Jukebox_onKeyPress(object sender, kyun.GameScreen.InputEvents.KeyPressEventArgs args)
        {
            AllowVideo = true;
                        
            switch (args.Key)
            {
                case Keys.Escape:
                    ScreenManager.ChangeTo(MainScreen.instance);
                    break;
            }
        }

        private void changeEmphasis()
        {
            Color[] colors = new Color[coverimg.Texture.Width * coverimg.Texture.Height];
            coverimg.Texture.GetData(colors);

            int ncolor = OsuBeatMap.rnd.Next(15, colors.Length - 1);

            sColors.Clear();
            int lcc = 0;
            for (int a = 0; a < coverimg.Texture.Width; a++)
            {
                int[] lc = new int[3];
                lc[0] = colors[a * lcc].R;
                lc[1] = colors[a * lcc].G;
                lc[2] = colors[a * lcc].B;
                sColors.Add(lc);
                lcc++;
            }

            for(int a = 0; a < particleEngine.particles.Count; a++)
            {
                if(particleEngine.particles[a] is SquareParticle)
                {
                    int[] selectedEnph = new int[3];
                    int magicColor = OsuBeatMap.rnd.Next(0, sColors.Count - 1);

                    selectedEnph = sColors[magicColor];
                    Color ccolor = Color.FromNonPremultiplied(selectedEnph[0], selectedEnph[1], selectedEnph[2], 255)/*Color.FromNonPremultiplied(black_rand, black_rand, black_rand, 255)Color.Lerp(Color.FromNonPremultiplied(selectedEnph[0], selectedEnph[1], selectedEnph[2], 255), Color.Black,.7f)*/;

                    (particleEngine.particles[a] as SquareParticle).squareColor = ccolor;
                }
            }            

        }

        private void Instance_OnPeak(object sender, EventArgs e)
        {
            if (!Visible) return;
            if (KyunGame.Instance.Player.PlayState != BassPlayState.Playing) return;

            if (particleEngine.ParticleCount > 50) return;

            if (!AVPlayer.videoplayer.Stopped) return;

            kyun.Screen.ScreenMode actualMode = kyun.Screen.ScreenModeManager.GetActualMode();

            int randomNumber = OsuUtils.OsuBeatMap.GetRnd(1, 10, -1);

            for (int a = 0; a < randomNumber; a++)
            {
                switchParticle = OsuBeatMap.rnd.NextBoolean();
                int startLeft = 0;
                int startTop = 0;
                if (switchParticle)
                {
                    startTop = OsuUtils.OsuBeatMap.GetRnd(25, actualMode.Height - 25, -1);
                    startLeft = OsuUtils.OsuBeatMap.GetRnd(25, actualMode.Width + 500, -1);

                    Particle particle = particleEngine.AddNewParticle(SpritesContent.Instance.MenuSnow,
                        new Microsoft.Xna.Framework.Vector2((5f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2 - 1)) / 10f, Math.Abs(5f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2 - 1)) / 10f),
                        new Microsoft.Xna.Framework.Vector2(startLeft, 0),
                        (30 + OsuUtils.OsuBeatMap.rnd.Next(40)) * 100,
                        0.01f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2f - 1)
                        );

                    particle.Opacity = 0.6f;
                    particle.Scale = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.1, 0.6);

                    particle.StopAtBottom = true;
                }
                else
                {
                    //int startTop = OsuUtils.OsuBeatMap.GetRnd(25, actualMode.Height - 25, -1);
                    startLeft = OsuUtils.OsuBeatMap.GetRnd(-50, actualMode.Width + 500, -1);

                    float vel = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.2, 1);

                    int black_rand = 20;

                    if (KyunGame.xmas)
                    {
                        black_rand = OsuBeatMap.rnd.Next(250, 255);
                    }
                    else
                    {
                        black_rand = OsuBeatMap.rnd.Next(20, 40);
                    }

                    int[] selectedEnph = new int[3];

                    int magicColor = OsuBeatMap.rnd.Next(0, sColors.Count - 1);

                    selectedEnph = sColors[magicColor];

                    Color ccolor = Color.FromNonPremultiplied(selectedEnph[0], selectedEnph[1], selectedEnph[2], 255)/*Color.FromNonPremultiplied(black_rand, black_rand, black_rand, 255)Color.Lerp(Color.FromNonPremultiplied(selectedEnph[0], selectedEnph[1], selectedEnph[2], 255), Color.Black,.7f)*/;
                                       

                    Particle particle = particleEngine.AddNewSquareParticle(SpritesContent.Instance.SquareParticle,
                        new Vector2(0, vel),
                        new Vector2(startLeft, actualMode.Height),
                        (30 + OsuUtils.OsuBeatMap.rnd.Next(40)) * 100,
                        0.01f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2f - 1),
                        ccolor
                        );
                    particle.Scale = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.35, 0.7);
                    particle.Opacity = /*(float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.4, 0.9)*/0.8f;
                    squareYesNo = !squareYesNo;
                }

            }
        }
    }
}
