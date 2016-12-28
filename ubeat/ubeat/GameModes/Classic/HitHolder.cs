using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.Beatmap;
using ubeat.Utils;

namespace ubeat.GameModes.Classic
{
    public class HitHolder : HitSingle
    {

        bool holding;
            
        public HitHolder(IHitObj hitObject, IBeatmap beatmap, ClassicModeScreen Instance)
            :base(hitObject, beatmap, Instance)
        {
            Texture = (Screen.ScreenModeManager.GetActualMode().Height < 650) ?
                  SpritesContent.Instance.ButtonHolder_0 :
                  SpritesContent.Instance.ButtonHolder;



        }

        public override void Update()
        {

            //Note: this is for testing purposes only, redo this shit


            if (screenInstance.GamePosition > Time && !holding)
            {
                holding = true;
                Audio.AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.HitHolder);
            }

            if(holding && screenInstance.GamePosition > EndTime)
            {
                Audio.AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.HitHolder);
                Died = true;
            }


            approachObj.Update();
        }
    }
}
