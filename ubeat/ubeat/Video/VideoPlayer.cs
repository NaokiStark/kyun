using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System;

namespace ubeat.Video
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
        int width = 0;
        int height = 0;
        
        public Queue<byte[]> Buffer = new System.Collections.Generic.Queue<byte[]>();
        public Queue<Texture2D> BufferTx = new System.Collections.Generic.Queue<Texture2D>();
        public bool Stopped = true;
        public VideoDecoder vdc;

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
            vdc = VideoDecoder.Instance;
            bool oppenned = vdc.Open(VideoPath);
            //vdc.Seek(64); //TEST
            if (oppenned == false)
            {
                Stopped = true;
                return;
            }
            this.width = vdc.width;
            this.height = vdc.height;
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
            if (Buffer.Count > 2) return null;

            byte[] frame = vdc.GetFrame((int)UbeatGame.Instance.Player.Position);
            if (frame == null) return null;
            return frame;
        }

        internal byte[] GetFrame(long v)
        {
            if (Stopped) return null;
            if (vdc == null) return null;
            if (Buffer.Count > 2) return null;

            byte[] frame = vdc.GetFrame((int)v);
            if (frame == null) return null;
            return frame;
        }
    }    
}
