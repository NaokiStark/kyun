using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.Beatmap;
using ubeat.Utils;

namespace ubeat.GameModes.OsuMode
{
    public class HitHolder : HitSingle
    {

        bool holding;
            
        public HitHolder(IHitObj hitObject, IBeatmap beatmap, OsuMode Instance)
            :base(hitObject, beatmap, Instance)
        {
            Texture = (Screen.ScreenModeManager.GetActualMode().Height < 650) ?
                  SpritesContent.Instance.ButtonHolder_0 :
                  SpritesContent.Instance.ButtonHolder;

        }

        public override void Update()
        {

            //Note: this is for testing purposes only, redo this shit
            UpdateTime();
            updateOpacity();
            

            if (screenInstance.GamePosition > Time && !holding)
            {
                holding = true;
                Audio.AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.OsuHit);
            }

            if(holding && screenInstance.GamePosition > EndTime)
            {
                Audio.AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.OsuHit);
                Died = true;
            }

            approachObj.Update();
        }
    }
}
