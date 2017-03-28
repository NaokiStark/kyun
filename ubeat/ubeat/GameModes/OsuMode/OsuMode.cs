using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ubeat.Beatmap;
using ubeat.GameScreen;
using System.Threading;

namespace ubeat.GameModes.OsuMode
{
    public class OsuMode : GameModeScreenBase
    {

        int lastIndex = 0;
        List<HitBase> hitbaseObjects = new List<HitBase>();
        bool End = false;


        public static OsuMode GetInstance()
        {
            return (OsuMode)Instance;
        }

        public OsuMode()
            : base("OsuMode")
        {
            ChangeBackground(UbeatGame.Instance.SelectedBeatmap.Background);
            onKeyPress += (obj, args) => {

                if (args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
                {
                    togglePause();
                }

                if (UbeatGame.Instance.Player.Paused && args.Key == Microsoft.Xna.Framework.Input.Keys.F2)
                {
                    ScreenManager.ChangeTo(BeatmapScreen.Instance);
                }

            };
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
            lastIndex = 0;
            ChangeBackground(UbeatGame.Instance.SelectedBeatmap.Background);
            hitbaseObjects.Clear();
           
        }

        private void togglePause()
        {
            UbeatGame.Instance.Player.Paused = !UbeatGame.Instance.Player.Paused;
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

            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - Beatmap.ApproachRate * 150f)+2000;

            long nextObjStart = (long)lastObject.StartTime - approachStart;


            if (actualTime > nextObjStart)
            {

                if (lastObject is HitButton)
                {
                    var obj = new HitSingle(lastObject, Beatmap, this);
                    obj.Opacity = 0;
                    hitbaseObjects.Add(obj);
                }
                else
                {
                    var obj = new HitHolder(lastObject, Beatmap, this);
                    obj.Opacity = 0;
                    hitbaseObjects.Add(obj);
                }

                lastIndex++;
            }


        }

        public override void Update(GameTime tm)
        {
            if (!Visible || isDisposing) return;
            checkObjectsInTime();
            base.Update(tm);

            if (lastIndex >= Beatmap.HitObjects.Count && hitbaseObjects.Count < 1)
            {
                End = true;
            }

            if (End)
            {
                ScreenManager.ChangeTo(BeatmapScreen.Instance);
                UbeatGame.Instance.Player.Play(Beatmap.SongPath);
                End = false;
            }
        }

        public override void Render()
        {
            base.Render();

            for (int a = hitbaseObjects.Count - 1; a > -1; a--)
            {
                hitbaseObjects[a].Render();
            }
        }

        internal override void UpdateControls()
        {
            //Controls in general
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

            //Hit objects
            for (int a = 0; a < hitbaseObjects.Count; a++)
            {
                hitbaseObjects[a].Update();

                HitBase ctr = (HitBase)hitbaseObjects[a];

                if (ctr.Died)
                {
                    hitbaseObjects.Remove(ctr);
                }

            }
        }

       
    }
}
