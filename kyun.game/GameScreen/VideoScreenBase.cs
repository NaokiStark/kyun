using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/*
 * 
 * ABORTED!!!!!!!
 * 
 */

namespace kyun.GameScreen
{
    class VideoScreenBase : ScreenBase
    {
        Video.VideoPlayer vPlayer { get; set; }

        public VideoScreenBase(string videoPath)
        {
            // TO DO: TO DO
        }


        public override void Render()
        {
            if (!Visible || isDisposing) return;
            RenderBg();
            RenderVideoFrame();
            RenderPeak();
            RenderObjects();


        }


        private void RenderVideoFrame()
        {

        }

    }
}
