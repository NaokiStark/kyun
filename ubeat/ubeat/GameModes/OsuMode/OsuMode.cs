using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using kyun.Beatmap;
using kyun.GameScreen;
using System.Threading;
using kyun.Utils;
using kyun.Audio;

namespace kyun.GameModes.OsuMode
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
            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            onKeyPress += (obj, args) => {

                if (args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
                {
                    togglePause();
                }

                if (KyunGame.Instance.Player.Paused && args.Key == Microsoft.Xna.Framework.Input.Keys.F2)
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
            clearObjects();
            base.gameMod = GameMods;
            KyunGame.Instance.Player.Stop();
            Beatmap = beatmap;
            GamePosition = 0;
            InGame = true;
            lastIndex = 0;
            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            //hitbaseObjects.Clear();
           
        }

        private void clearObjects()
        {
            Controls.RemoveAll(item => item is HitBase);
        }

        private void togglePause()
        {
            KyunGame.Instance.Player.Paused = !KyunGame.Instance.Player.Paused;
        }

        private void checkObjectsInTime()
        {

            if (KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
                GamePosition += (long)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
            else
                GamePosition = KyunGame.Instance.Player.Position + Beatmap.SleepTime;

            if (!InGame) return;

            if (InGame && GamePosition > Beatmap.SleepTime && KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
            {
                KyunGame.Instance.Player.Play(Beatmap.SongPath);
                KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume;
            }
            

            if (lastIndex >= Beatmap.HitObjects.Count)
            {
                InGame = false;
                return;
            }

            long actualTime = GamePosition;

            
            IHitObj lastObject = Beatmap.HitObjects[lastIndex];

            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - Beatmap.ApproachRate * 150f)+500;

            long nextObjStart = (long)lastObject.StartTime - approachStart;


            if (actualTime > nextObjStart)
            {

                if (lastObject is HitButton)
                {
                    var obj = new HitSingle(lastObject, Beatmap, this);
                    obj.Opacity = 0;
                    //hitbaseObjects.Add(obj);
                    Controls.Add(obj);
                }
                else
                {
                    var obj = new HitHolder(lastObject, Beatmap, this);
                    obj.Opacity = 0;
                    //hitbaseObjects.Add(obj);
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

            if (lastIndex >= Beatmap.HitObjects.Count && hitbaseObjects.Count < 1)
            {
                End = true;
            }

            if (End)
            {
                ScreenManager.ChangeTo(BeatmapScreen.Instance);
                KyunGame.Instance.Player.Play(Beatmap.SongPath);
                End = false;
            }
        }

        internal override void RenderObjects()
        {
            try
            {
                foreach (UIObjectBase obj in Controls.Reverse<UIObjectBase>())
                {
                    if (obj.Texture != null)
                    {
                        if (obj.Texture == SpritesContent.Instance.TopEffect)
                        {
                            continue;
                        }
                        else
                        {
                            obj.Render();
                        }
                    }
                    else
                    {
                        obj.Render();
                    }
                }

            }
            catch
            {

            }
        }

        public override void Render()
        {
            base.Render();

            /*
            for (int a = hitbaseObjects.Count - 1; a > -1; a--)
            {
                hitbaseObjects[a].Render();
            }*/
            /*
            foreach (HitBase hitObject in hitbaseObjects.Reverse<HitBase>())
            {
                hitObject.Render();
            }*/
        }

        internal override void UpdateControls()
        {
            //Controls in general
            /*
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
            }*/

            foreach (UIObjectBase control in Controls)
            {
                control.Update();
            }

            Controls.RemoveAll(item => item.Died);

            /*
            //Hit objects
                for (int a = 0; a < hitbaseObjects.Count; a++)
            {
                hitbaseObjects[a].Update();

                HitBase ctr = (HitBase)hitbaseObjects[a];

                if (ctr.Died)
                {
                    hitbaseObjects.Remove(ctr);
                }

            }*/
        }

       
    }
}
