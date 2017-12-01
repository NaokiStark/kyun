using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Beatmap;
using kyun.UIObjs;
using kyun.Utils;

namespace kyun.GameModes.OsuMode
{
    public class HitSingle : HitBase
    {
        internal IHitObj _hitButton;
        internal IBeatmap _beatmap;

        internal ApproachObj approachObj;

        internal OsuMode screenInstance;
        private bool startShow;

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
        public HitSingle(IHitObj hitObject, IBeatmap beatmap, OsuMode Instance)
            : base((Screen.ScreenModeManager.GetActualMode().Height < 650)?
                  SpritesContent.Instance.ButtonDefault_0 :
                  SpritesContent.Instance.ButtonDefault)
        {
            
            _hitButton = hitObject;
            _beatmap = beatmap;
            screenInstance = Instance;
            approachObj = new ApproachObj(Position, _beatmap.ApproachRate, Time, Instance);

            Screen.ScreenMode scm = Screen.ScreenModeManager.GetActualMode();

            //Vector2 cent = new Vector2(OsuUtils.OsuBeatMap.rnd.Next(Texture.Width / 2, scm.Width - Texture.Width), OsuUtils.OsuBeatMap.rnd.Next(Texture.Height / 2, scm.Height - Texture.Height));

            Position = CalculatePosition(hitObject.OsuLocation);
            approachObj.Position = Position;
            approachObj.Opacity = 0;
        }

        private Vector2 CalculatePosition(Vector2 pos)
        {
            
            Screen.ScreenMode actualMode = Screen.ScreenModeManager.GetActualMode();

            int heightNoScaled = actualMode.Height - Texture.Height;

            int widthScaled = (int)((512f / 384f) * (float)heightNoScaled);
            int widthScaledForPosition = (int)((512f / 384f) * (float)actualMode.Height);

            int heigthScaled = heightNoScaled;

            float porcx = 100f / 512f * pos.X;
            float porcy = 100f / 384f * pos.Y;

            int posX = (int)(porcx * (float)widthScaled / 100f);
            int posY = (int)(porcy * (float)heigthScaled / 100f);
            posX += ((actualMode.Width / 2) - (widthScaledForPosition / 2));

            return new Vector2(posX, posY);
        }

        public void Show()
        {
            startShow = true;
        }

        internal void updateOpacity()
        {
            if (!startShow) return;

            GameTime gt = KyunGame.Instance.GameTimeP;

            float toAdd = 0.009f * (float)gt.ElapsedGameTime.Milliseconds;

            if(Opacity + toAdd < 1)
            {
                Opacity += toAdd;
                approachObj.Opacity = Opacity;
            }

        }

        public override void Update()
        {
            UpdateTime();
            updateOpacity();            

            //Note: this is for testing purposes only, redo this shit
            
            if (screenInstance.GamePosition > Time)
            {
                //Audio.AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.OsuHit);
                Died = true;
                
            }

            approachObj.Update();
            
        }

        internal void UpdateTime()
        {
            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - screenInstance.Beatmap.ApproachRate * 150f);
            long nextObjStart = (long)Time - approachStart;
            if (screenInstance.GamePosition > nextObjStart)
            {
                Show();
            }
        }

        public override void Render()
        {

            base.Render();
            approachObj.Render();     

        }



    }
}
