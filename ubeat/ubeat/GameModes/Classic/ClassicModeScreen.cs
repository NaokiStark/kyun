using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ubeat.Beatmap;

namespace ubeat.GameModes.Classic
{
    public class ClassicModeScreen : GameModeScreenBase
    {

        int lastIndex = 0;
        public long GamePosition;
        
        public static ClassicModeScreen GetInstance()
        {
            return (ClassicModeScreen)Instance;
        }

        public ClassicModeScreen()
            : base("ClassicModeScreen")
        {
        }

        /// <summary>
        /// Start game
        /// </summary>
        /// <param name="beatmap"></param>
        public override void Play(IBeatmap beatmap, GameMod GameMods = GameMod.None)
        {
            base.gameMod = GameMods;
            UbeatGame.Instance.Player.Stop();
            Beatmap = beatmap;
            GamePosition = 0;
            InGame = true;
            
        }


        private void checkObjectsInTime()
        {

            if (UbeatGame.Instance.Player.PlayState == NAudio.Wave.PlaybackState.Stopped)
                GamePosition += (long)UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
            else
                GamePosition = UbeatGame.Instance.Player.Position + Beatmap.SleepTime;

            if (!InGame) return;

            if (InGame && GamePosition > Beatmap.SleepTime && UbeatGame.Instance.Player.PlayState == NAudio.Wave.PlaybackState.Stopped)
            {
                UbeatGame.Instance.Player.Play(Beatmap.SongPath);
                UbeatGame.Instance.Player.soundOut.Volume = UbeatGame.Instance.GeneralVolume;
            }
            

            if (lastIndex >= Beatmap.HitObjects.Count)
            {
                InGame = false;
                return;
            }

            

            long actualTime = GamePosition;


            IHitObj lastObject = Beatmap.HitObjects[lastIndex];

            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - Beatmap.ApproachRate * 150f);

            long nextObjStart = (long)lastObject.StartTime - approachStart;

            if(actualTime > nextObjStart)
            {
                if(lastObject is HitButton)
                {
                    var obj = new HitSingle(lastObject, Beatmap, this);
                    Controls.Add(obj);
                }
                else
                {
                    var obj = new HitHolder(lastObject, Beatmap, this);
                    Controls.Add(obj);
                }
                lastIndex++;
            }


        }

        public override void Update(GameTime tm)
        {
            if (!Visible || isDisposing) return;
            checkObjectsInTime();
            base.Update(tm);
        }

        internal override void UpdateControls()
        {
            for (int a = 0; a < Controls.Count; a++)
            {
                Controls[a].Update();

                if(Controls[a] is HitBase)
                {
                    HitBase ctr = (HitBase)Controls[a];

                    if (ctr.Died)
                    {
                        Controls.Remove(ctr);
                    }
                }
            }
        }

       
    }
}
