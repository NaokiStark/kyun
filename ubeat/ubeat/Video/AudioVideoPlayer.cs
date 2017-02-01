using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.Audio;
using ubeat.GameScreen;
using ubeat.Screen;

namespace ubeat.Video
{
    public class AudioVideoPlayer : UIObjectBase
    {

        public string Audio { get; private set; }
        public string Video { get; private set; }

        VideoPlayer videoplayer;

        byte[] nextFrame;
        long nextTime = 0;


        public NPlayer audioplayer;

        Texture2D bg;

        public AudioVideoPlayer()
        {
            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            int screenWidth = actualMode.Width;
            int screenHeight = actualMode.Height;

            //??
            videoplayer = VideoPlayer.Instance;
            audioplayer = UbeatGame.Instance.Player;

            bg = Texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, screenWidth, screenHeight);

            Color[] data = new Color[screenWidth * screenHeight];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            bg.SetData(data);
        }

        public void Play(string audio, string video)
        {
            Audio = audio;
            Video = video;

            videoplayer.Play(video);
            audioplayer.Play(audio);

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
                if (nextTime <  position)
                {
                    byte[] frame = videoplayer.GetFrame(position);             
                    nextFrame = frame;
                    nextTime = position + 1;
                }
            }
        }

        public override void Render()
        {
            RenderVideoFrame();
        }


        private void RenderVideoFrame()
        {

            if (!UbeatGame.Instance.VideoEnabled)
                return;

            if (videoplayer.Stopped)
                return;


            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            int screenWidth = actualMode.Width;
            int screenHeight = actualMode.Height;

            Rectangle screenVideoRectangle = new Rectangle();

            
            if (nextFrame != null)
            {

                UbeatGame.Instance.SpriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);

                Texture2D texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, videoplayer.vdc.width, videoplayer.vdc.height, false,SurfaceFormat.Color);
                screenVideoRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)texture.Width / (float)texture.Height) * (float)screenHeight), screenHeight);

                texture.SetData(nextFrame);

                UbeatGame.Instance.SpriteBatch.Draw(texture, screenVideoRectangle, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);

                texture.Dispose();

            }
            else
            {
                UbeatGame.Instance.SpriteBatch.Draw(bg, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);
            }
        }
    }
}
