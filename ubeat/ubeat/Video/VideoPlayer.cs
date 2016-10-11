﻿using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace ubeat.Video
{
	public class VideoPlayer
    {

        public Microsoft.Xna.Framework.Graphics.Texture2D FrameVideo { get; set; }
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
            vdc = new VideoDecoder(32);
            bool oppenned = vdc.Open(VideoPath);
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

        
    }    
}
