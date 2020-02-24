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
using kyun.game.Video;
using System.IO;

namespace kyun.Video
{
    public class AudioVideoPlayer : UIObjectBase
    {

        public string Audio { get; private set; }
        public string Video { get; private set; }

        public bool hasGotFrame { get; set; }

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
        private byte[] lastAliveFrame;

        public Texture2D[] frameBuffer;
        public int frameIndex = 0;
        public int videoFramerate = 60;
        private int workingOn;
        private Texture2D lastAliveFrameTexture;

        Texture2D tx;
        private byte[] lastWorkingFrame;

        public AudioVideoPlayer()
        {

            //??
            videoplayer = VideoPlayer.Instance;
            audioplayer = KyunGame.Instance.Player;


        }

        public void SeekTime(int tm)
        {
            //videoplayer.FFmpegDecoder?.Seek(tm);
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
                VideoPlayer.Instance.vdc?.Dispose();

            }

            vBuffer.Clear();
            hasGotFrame = false;

            bool isDir = false;
            if (anotherfuckingvideovar != "")
            {
                if (File.Exists(anotherfuckingvideovar))
                {
                    isDir = IsFolder(anotherfuckingvideovar);
                }               
            }
                                  

            if (KyunGame.Instance.VideoEnabled && anotherfuckingvideovar != "" && !isDir)
            {

                videoplayer.Play(anotherfuckingvideovar);
                //updateBuffer(true);
            }
            frameIndex = 0;

            videoFramerate = 3;

            frameBuffer = new Texture2D[videoFramerate];

            audioplayer.Play(audio, 1, 1, true);

            Audio = audio;

        }

        public void Stop()
        {
            videoplayer.Stop();

        }

        /// <summary>
        /// Whether the <paramref name="path"/> is a folder (existing or not); 
        /// optionally assume that if it doesn't "look like" a file then it's a directory.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <param name="assumeDneLookAlike">If the <paramref name="path"/> doesn't exist, does it at least look like a directory name?  As in, it doesn't look like a file.</param>
        /// <returns><c>True</c> if a folder/directory, <c>false</c> if not.</returns>
        public static bool IsFolder(string path, bool assumeDneLookAlike = true)
        {
            // https://stackoverflow.com/questions/1395205/better-way-to-check-if-path-is-a-file-or-a-directory
            // turns out to be about the same as https://stackoverflow.com/a/19596821/1037948

            // check in order of verisimilitude

            // exists or ends with a directory separator -- files cannot end with directory separator, right?
            if (Directory.Exists(path)
                // use system values rather than assume slashes
                || path.EndsWith("" + Path.DirectorySeparatorChar)
                || path.EndsWith("" + Path.AltDirectorySeparatorChar))
                return true;

            // if we know for sure that it's an actual file...
            if (File.Exists(path))
                return false;

            // if it has an extension it should be a file, so vice versa
            // although technically directories can have extensions...
            if (!Path.HasExtension(path) && assumeDneLookAlike)
                return true;

            // only works for existing files, kinda redundant with `.Exists` above
            //if( File.GetAttributes(path).HasFlag(FileAttributes.Directory) ) ...; 

            // no idea -- could return an 'indeterminate' value (nullable bool)
            // or assume that if we don't know then it's not a folder
            return false;
        }

        public void updateBufferV2(bool callZero = false)
        {
            int framerate = 3;

            long pos = -framerate;
            if (frameIndex > framerate)
            {
                frameIndex = 0;
                long position = audioplayer.Position;

                for (int a = 0; a < frameBuffer.Length; a++)
                {
                    workingOn = a;
                    byte[] frame = videoplayer.GetFrame((long)((float)position + pos));

                    if (frame != null)
                    {
                        lastAliveFrame = frame;

                        if (frameBuffer[a] == null)
                            frameBuffer[a] = new Texture2D(KyunGame.Instance.GraphicsDevice, videoplayer.vdc.VIDEOWIDTH, videoplayer.vdc.VIDEOHEIGHT);

                        if (KyunGame.Instance.GraphicsDevice.Textures[0] == frameBuffer[a])
                        {
                            KyunGame.Instance.GraphicsDevice.Textures[0] = null;
                        }
                        frameBuffer[a].SetData(frame);

                    }
                    else
                    {

                        if (frameBuffer[a] == null)
                            frameBuffer[a] = new Texture2D(KyunGame.Instance.GraphicsDevice, videoplayer.vdc.VIDEOWIDTH, videoplayer.vdc.VIDEOHEIGHT);

                        if (KyunGame.Instance.GraphicsDevice.Textures[0] == frameBuffer[a])
                        {
                            KyunGame.Instance.GraphicsDevice.Textures[0] = null;
                        }
                        if (lastAliveFrame != null)
                            frameBuffer[a].SetData(lastAliveFrame);
                    }
                    pos += (int)((1000f * audioplayer.Velocity) / framerate);

                }

            }


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


            framRate = 29;


            if (audioplayer.PlayState != BassPlayState.Playing)
            {
                if (vBuffer.Count > 2)
                    vBuffer.Clear();

                framRate = 2;
            }

            if (callZero)
                framRate = 2;

            if (vBuffer.Count < KyunGame.Instance.VideoCounter.AverageFramesPerSecond / 2)
            {


                if (!callZero)
                    position = audioplayer.Position - 400;

                byte[] lastf = null;
                //int bcount = vBuffer.Count;
                for (int a = vBuffer.Count; a < framRate; a++)
                {
                    if (a == vBuffer.Count)
                    {
                        pos = 500;
                    }

                    byte[] frame = videoplayer.GetFrame((long)((float)position + pos));
                    hasGotFrame = true;


                    if (frame != null)
                    {
                        //lastf = frame;
                        for (int b = 0; b < (KyunGame.Instance.VideoCounter.AverageFramesPerSecond / framRate); b++)
                        {
                            vBuffer.Enqueue(frame);
                        }

                        lastAliveFrame = frame;
                    }
                    else
                    {
                        for (int b = 0; b < (KyunGame.Instance.VideoCounter.AverageFramesPerSecond / framRate); b++)
                        {
                            vBuffer.Enqueue(lastAliveFrame);
                        }
                    }
                    pos += 1000f / (float)(framRate);
                }

            }
        }

        public override void Update()
        {
            if (videoplayer.Stopped)
                return;

            long position = audioplayer.Position;

        }

        public override void Render()
        {
            //updateBufferV2(); //Fuck main thread
            RenderVideoFrame(true);
        }


        private void RenderVideoFrame(bool newRender)
        {
            if (!KyunGame.Instance.VideoEnabled)
                return;


            if (audioplayer.PlayState == BassPlayState.Stopped)
                return;

            if (videoplayer.vdc == null)
            {
                return;
            }

            if (videoplayer.vdc.GetBufferCount() < 1)
                return;

            if(tx == null)
            {
                tx = new Texture2D(KyunGame.Instance.GraphicsDevice, videoplayer.vdc.VIDEOWIDTH, videoplayer.vdc.VIDEOHEIGHT);

            }

            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            int screenWidth = actualMode.Width;
            int screenHeight = actualMode.Height;

            Rectangle screenVideoRectangle = new Rectangle();

            screenVideoRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)tx.Width / (float)tx.Height) * (float)screenHeight), screenHeight);
            if (screenVideoRectangle.Width < screenWidth)
            {
                screenVideoRectangle = new Rectangle(screenVideoRectangle.X, screenVideoRectangle.Y, (int)screenWidth, (int)(((float)tx.Height / (float)tx.Width) * (float)screenWidth));
            }

            long position = audioplayer.Position;

            byte[] frame = videoplayer.GetFrame(position);

            if (frame != null)
            {
                tx.SetData(frame);

                KyunGame.Instance.SpriteBatch.Draw(tx, screenVideoRectangle, null, Color.White * .8f, 0, new Vector2(tx.Width / 2, tx.Height / 2), SpriteEffects.None, 0);

                lastWorkingFrame = frame;
            }

            if (lastWorkingFrame != null && frame == null)
            {
                tx.SetData(lastWorkingFrame);
                KyunGame.Instance.SpriteBatch.Draw(tx, screenVideoRectangle, null, Color.White * .8f, 0, new Vector2(tx.Width / 2, tx.Height / 2), SpriteEffects.None, 0);

            }
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

            //updateBufferV2();

            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            int screenWidth = actualMode.Width;
            int screenHeight = actualMode.Height;

            Rectangle screenVideoRectangle = new Rectangle();


            KyunGame.Instance.SpriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);
            Texture2D texture;
            if (frameBuffer == null)
                frameBuffer = new Texture2D[videoFramerate];

            if (frameIndex >= frameBuffer.Length)
                texture = frameBuffer[frameBuffer.Length - 1];
            else
                texture = frameBuffer[frameIndex];

            if (workingOn == frameIndex)
            {
                texture = lastAliveFrameTexture;
            }


            frameIndex++;

            if (texture == null)
                return;

            if (texture.IsDisposed)
                return;

            screenVideoRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)texture.Width / (float)texture.Height) * (float)screenHeight), screenHeight);
            if (screenVideoRectangle.Width < screenWidth)
            {
                screenVideoRectangle = new Rectangle(screenVideoRectangle.X, screenVideoRectangle.Y, (int)screenWidth, (int)(((float)texture.Height / (float)texture.Width) * (float)screenWidth));
            }

            if (texture.IsDisposed)
            {
                return;
            }

            try
            {
                //KyunGame.Instance.SpriteBatch.Draw(texture, screenVideoRectangle, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);
                KyunGame.Instance.SpriteBatch.Draw(texture, screenVideoRectangle, null, Color.White * .8f, 0, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);

                lastAliveFrameTexture = new Texture2D(KyunGame.Instance.GraphicsDevice, texture.Width, texture.Height);
                Color[] dcolor = new Color[texture.Width * texture.Height];
                texture.GetData(dcolor);
                lastAliveFrameTexture.SetData(dcolor);


            }
            catch { }

        }
    }
}
