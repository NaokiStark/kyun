using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Audio;
using kyun.GameScreen;
using kyun.Screen;
using kyun.game;

namespace kyun.Video
{
    public class AudioVideoPlayer : UIObjectBase
    {

        public string Audio { get; private set; }
        public string Video { get; private set; }

        static Texture2D bgxd = null;

        internal Texture2D bg
        {
            get
            {
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

        public VideoPlayer videoplayer;

        byte[] nextFrame;


        long nextTime = 0;

        bool FillBuffer = true;

        public BPlayer audioplayer;

        private double cEl = 0;



        byte[][] bbuffer = new byte[60][];
        int nF = 0;
        private string anotherfuckingvideovar;

        public Queue<byte[]> vBuffer = new Queue<byte[]>();

        public AudioVideoPlayer()
        {


            //??
            videoplayer = VideoPlayer.Instance;
            audioplayer = KyunGame.Instance.Player;


        }

        public void SeekTime(int tm)
        {
            videoplayer.vdc?.Seek(tm);
        }

        public void Play(string audio, string video = "", bool fadein = false, float opacity = 0.75f)
        {

            this.Video = video;

            anotherfuckingvideovar = video;

            Opacity = opacity;

            //SHIT Cleanup
            if (Audio != audio)
            {
                videoplayer.vdc?.Dispose();

            }
            else
            {
                if (!videoplayer.Stopped)
                    videoplayer.vdc?.Seek(0);
            }

            vBuffer.Clear();

            if (KyunGame.Instance.VideoEnabled && anotherfuckingvideovar != "")
            {

                videoplayer.Play(anotherfuckingvideovar);
                //updateBuffer(true);
            }

            audioplayer.Play(audio, 1, 1, true);

            Audio = audio;

        }

        public void Stop()
        {
            videoplayer.Stop();

        }

        private void updateBuffer(bool callZero = false)
        {
            float pos = 0;
            long position = 0;

            int framRate = Math.Min((int)Settings1.Default.FrameRate, 144);

            if (!callZero)
                position = audioplayer.Position - 400;

            if (videoplayer.Stopped)
                return;

            if (Settings1.Default.VSync)
                framRate = 60;

            if (System.Windows.Forms.Form.ActiveForm != KyunGame.WinForm)
            {
                
                framRate = 35; //Fix 
            }
                

            if (audioplayer.PlayState != BassPlayState.Playing)
            {
                if (vBuffer.Count > 2)
                    vBuffer.Clear();

                framRate = 2;
            }

            if (callZero)
                framRate = 2;

            if (vBuffer.Count < framRate / 2)
            {
                byte[] lastf = null;
                //int bcount = vBuffer.Count;
                for (int a = vBuffer.Count; a < framRate; a++)
                {
                    byte[] frame = videoplayer.GetFrame((long)((float)position + pos));
                    if (frame != null)
                    {
                        //lastf = frame;
                        vBuffer.Enqueue(frame);
                    }
                    else
                    {
                        if(lastf != null)
                            vBuffer.Enqueue(lastf);
                    }
                    pos += 1000f / (float)framRate;
                }

            }
        }

        public override void Update()
        {
            if (videoplayer.Stopped)
                return;

            long position = audioplayer.Position;
            updateBuffer();
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

            //There are no buffer
            if (vBuffer.Count < 1)
                return;

            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            int screenWidth = actualMode.Width;
            int screenHeight = actualMode.Height;

            Rectangle screenVideoRectangle = new Rectangle();


            byte[] deFrame = vBuffer.Dequeue();
            if (deFrame == null)
            {
                KyunGame.Instance.SpriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);

                return;
            }

            KyunGame.Instance.SpriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);

            Texture2D texture = new Texture2D(KyunGame.Instance.GraphicsDevice, videoplayer.vdc.width, videoplayer.vdc.height, false, SurfaceFormat.Color);
            //Texture2D texture = Utils.ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, new System.IO.MemoryStream(deFrame), true);
            screenVideoRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)texture.Width / (float)texture.Height) * (float)screenHeight), screenHeight);

            try
            {
                texture.SetData(deFrame);

                KyunGame.Instance.SpriteBatch.Draw(texture, screenVideoRectangle, null, Color.White * ((ScreenBase)ScreenManager.ActualScreen).BackgroundDim, 0, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);

            }
            catch { }

            texture.Dispose();


        }
    }
}
