using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System;
using kyun.game.Video;

namespace kyun.Video
{
    public class VideoPlayer
    {

        static VideoPlayer instance = null;

        public static VideoPlayer Instance
        {
            get
            {
                if(instance==null)
                    instance = new VideoPlayer();
                return instance;
            }
        }

        public Texture2D FrameVideo { get; set; }
        public int width = 0;
        public int height = 0;
        
        public Queue<byte[]> Buffer = new System.Collections.Generic.Queue<byte[]>();
        public Queue<Texture2D> BufferTx = new System.Collections.Generic.Queue<Texture2D>();
        public bool Stopped = true;
        public FFmpegDecoder vdc;

        

        public VideoPlayer()
        {
            
        }

        public void Play(string VideoPath)
        {
            FileInfo fi = new FileInfo(VideoPath);
            if (fi.Extension == "")
            {
                Stopped = true;
                return;
            }
            Stopped = false;

            if(FFmpegDecoder.Instance != null)
                FFmpegDecoder.Instance?.Dispose(); //CLEANUP SHIT BUFFER

            int ratio = (int)((480f / 853f) * (float)Math.Min(1280f, Screen.ScreenModeManager.GetActualMode().Width));

            //vdc = new FFmpegDecoder(VideoPath, Screen.ScreenModeManager.GetActualMode().Width, ratio);
            vdc = new FFmpegDecoder(VideoPath);
            vdc.Decode();
            width = vdc.VIDEOWIDTH;
            height = vdc.VIDEOHEIGHT;
            //vdc.WaitForDecoder();

           
        }

        public void Stop()
        {
            if (Stopped) return;
            Stopped = true;
            vdc.Dispose();
            
        }

        public byte[] GetFrame()
        {
            if (Stopped) return null;
            if (vdc == null) return null;

             byte[] frame = vdc.GetFrame((int)KyunGame.Instance.Player.Position);
            if (frame == null) return null;
            return frame;
        }

        internal byte[] GetFrame(long v)
        {
            if (Stopped) return null;
            if (vdc == null) return null;

            byte[] frame = vdc.GetFrame((int)v);
            if (frame == null) return null;
            return frame;
        }
    }    
}
