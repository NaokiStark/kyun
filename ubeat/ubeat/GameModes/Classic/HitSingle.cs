using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.Beatmap;
using ubeat.UIObjs;
using ubeat.Utils;

namespace ubeat.GameModes.Classic
{
    public class HitSingle : HitBase
    {
        internal IHitObj _hitButton;
        internal IBeatmap _beatmap;

        internal ApproachObj approachObj;

        internal ClassicModeScreen screenInstance;

        public long Time
        {
            get
            {
                return (long)_hitButton.StartTime;
            }
        }

        public long EndTime
        {
            get
            {
                return (long)_hitButton.EndTime;
            }
        }

        /// <summary>
        /// New Instance of HitSingle
        /// </summary>
        /// <param name="hitObject">HitObject</param>
        public HitSingle(IHitObj hitObject, IBeatmap beatmap, ClassicModeScreen Instance)
            : base((Screen.ScreenModeManager.GetActualMode().Height < 650)?
                  SpritesContent.Instance.ButtonDefault_0 :
                  SpritesContent.Instance.ButtonDefault)
        {
            
            _hitButton = hitObject;
            _beatmap = beatmap;
            screenInstance = Instance;
            approachObj = new ApproachObj(Position, _beatmap.ApproachRate, Time, Instance);

            Screen.ScreenMode scm = Screen.ScreenModeManager.GetActualMode();

            Vector2 cent = new Vector2(OsuUtils.OsuBeatMap.rnd.Next(Texture.Width / 2, scm.Width - Texture.Width), OsuUtils.OsuBeatMap.rnd.Next(Texture.Height / 2, scm.Height - Texture.Height));

            Position = cent;
            approachObj.Position = Position;
        }

        public override void Update()
        {
            //Note: this is for testing purposes only, redo this shit

            if (screenInstance.GamePosition > Time)
            {
                Audio.AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.HitHolder);
                Died = true;
                
            }

            approachObj.Update();

        }

        public override void Render()
        { 
            base.Render();
            approachObj.Render();     

        }



    }
}
