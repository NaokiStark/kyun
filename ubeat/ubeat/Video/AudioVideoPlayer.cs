using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Audio;
using kyun.GameScreen;
using kyun.Screen;

namespace kyun.Video
{
    public class AudioVideoPlayer : UIObjectBase
    {

        public string Audio { get; private set; }
        public string Video { get; private set; }

        static Texture2D bgxd = null;

        internal Texture2D bg {
            get {
                if (bgxd == null)
                {
                    ScreenMode actualMode = ScreenModeManager.GetActualMode();

                    int screenWidth = actualMode.Width;
                    int screenHeight = actualMode.Height;

                    bgxd = Texture = new Texture2D(KyunGame.Instance.GraphicsDevice, screenWidth, screenHeight);

                    Color[] data = new Color[screenWidth * screenHeight];
                    for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                    bgxd.SetData(data);                    
                }
                return bgxd;
            }
        }

        VideoPlayer videoplayer;

        byte[] nextFrame;


        long nextTime = 0;

        bool FillBuffer = true;

        public BPlayer audioplayer;

        

        byte[][] bbuffer = new byte[1][];
        int nF = 0;
        private string anotherfuckingvideovar;

        public AudioVideoPlayer()
        {
            

            //??
            videoplayer = VideoPlayer.Instance;
            audioplayer = KyunGame.Instance.Player;

          
        }

        public void SeekTime(int tm)
        {
            videoplayer.vdc?.Seek(0);
        }

        public void Play(string audio, string video = "", bool fadein = false, float opacity = 0.75f)
        {
           

            
            this.Video = video;

            anotherfuckingvideovar = video;

            Opacity = opacity;

            audioplayer.Play(audio, 1, 1, true);

            //SHIT Cleanup
            if(Audio != audio)
            {
                videoplayer.vdc?.Dispose();
            }
            else
            {
                videoplayer.vdc?.Seek(0);
            }
            
            Audio = audio;
            if (KyunGame.Instance.VideoEnabled && anotherfuckingvideovar != "")
            {
            
                videoplayer.Play(anotherfuckingvideovar);
            }

        }

        public void Stop()
        {
            videoplayer.Stop();

        }

        public override void Update()
        {
            if (!videoplayer.Stopped)
            {            
               
                long position = audioplayer.Position;
            }
        }

        public override void Render()
        {
            RenderVideoFrame();
        }


        private void RenderVideoFrame()
        {
            if (!KyunGame.Instance.VideoEnabled)
                return;

            if (videoplayer.Stopped)
            {
                return;
            }

            if (audioplayer.PlayState == BassPlayState.Stopped)
                return;


            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            int screenWidth = actualMode.Width;
            int screenHeight = actualMode.Height;

            Rectangle screenVideoRectangle = new Rectangle();

            long position = audioplayer.Position;

            float pos = 0;
            nF++;
            if (nF >= bbuffer.Length)
            {

                nF = 0;

                lock (bbuffer)
                {
                    for (int a = 0; a < bbuffer.Length; a++)
                    {

                        byte[] frame = videoplayer.GetFrame((int)((float)position + pos));
                        if (frame != null)
                        {
                            bbuffer[a] = frame;
                        }
                        pos += 120f;
                    }
                }
            }


            lock (bbuffer)
            {
                if (bbuffer[nF] == null)
                {
                    KyunGame.Instance.SpriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);
                    return;
                }
                lock (bbuffer[nF])
                {
                    KyunGame.Instance.SpriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);

                    Texture2D texture = new Texture2D(KyunGame.Instance.GraphicsDevice, videoplayer.vdc.width, videoplayer.vdc.height, false, SurfaceFormat.Color);
                    screenVideoRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)texture.Width / (float)texture.Height) * (float)screenHeight), screenHeight);

                    try
                    {
                        texture.SetData(bbuffer[nF]);
                        KyunGame.Instance.SpriteBatch.Draw(texture, screenVideoRectangle, null, Color.White * ((ScreenBase)ScreenManager.ActualScreen).BackgroundDim, 0, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);
                    }
                    catch { }

                    texture.Dispose();
                }
            }
            

            
        }
    }
}
