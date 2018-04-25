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
using Microsoft.Xna.Framework.Input;
using kyun.GameScreen.UI.Particles;

namespace kyun.GameModes.OsuMode
{
    public class OsuMode : GameModeScreenBase
    {

        int lastIndex = 0;
        List<HitBase> hitbaseObjects = new List<HitBase>();
        bool End = false;

        static OsuMode Instance;
        private int timeToLeave;
        private bool mousePressedLeft;
        private bool mousePressedRight;
        private bool keypressedZ;
        private bool keypressedX;
        public ParticleEngine _particleEngine { get; private set; }


        public static OsuMode GetInstance()
        {
            if (Instance == null)
                Instance = new OsuMode();
            return Instance;
        }


        public OsuMode()
            : base("OsuMode")
        {

            Instance = this;

            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            onKeyPress += (obj, args) =>
            {

                if (args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
                {
                    togglePause();
                }

                if (KyunGame.Instance.Player.Paused && args.Key == Microsoft.Xna.Framework.Input.Keys.F2)
                {
                    ScreenManager.ChangeTo(BeatmapScreen.Instance);
                }

            };
            _particleEngine = new ParticleEngine();
            Controls.Add(_particleEngine);
        }


        /// <summary>
        /// Start game
        /// </summary>
        /// <param name="beatmap"></param>
        public override void Play(IBeatmap beatmap, GameMod GameMods = GameMod.None)
        {
            AllowVideo = true;
            End = false;

            BackgroundDim = ((ScreenBase)game.GameScreen.GameLoader.GetInstance()).BackgroundDim;
            clearObjects();
            base.gameMod = GameMods;
            KyunGame.Instance.Player.Stop();
            Beatmap = beatmap;
            GamePosition = 0;
            InGame = true;
            lastIndex = 0;
            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            hitbaseObjects.Clear();
            if (game.Settings1.Default.Video)
            {
                if (!AVPlayer.videoplayer.Stopped)
                {
                    avp.videoplayer.vdc?.Dispose();
                    
                    avp.videoplayer.Play(beatmap.Video);
                }
            }
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

            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - Beatmap.ApproachRate * 150f) + 500;

            long nextObjStart = (long)lastObject.StartTime - approachStart;


            if (actualTime > nextObjStart)
            {

                if (lastObject is HitButton)
                {
                    var obj = new HitSingle(lastObject, Beatmap, this);
                    obj.Opacity = 0;
                    hitbaseObjects.Add(obj);
                    Controls.Add(obj);
                }
                else
                {
                    var obj = new HitHolder(lastObject, Beatmap, this);
                    obj.Opacity = 0;
                    hitbaseObjects.Add(obj);
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

            int leaveTime = 3000;

            if (lastIndex >= Beatmap.HitObjects.Count)
            {
                if (Beatmap.HitObjects.Last() is kyun.Beatmap.HitHolder)
                {
                    if (GamePosition > ((HitHolder)hitbaseObjects.Last()).EndTime)
                    {
                        End = true;
                    }
                }
                else
                {
                    if (GamePosition > ((HitSingle)hitbaseObjects.Last()).Time)
                    {
                        End = true;
                    }
                }
            }



            if (End)
            {
                timeToLeave += tm.ElapsedGameTime.Milliseconds;
            }

            if (End && timeToLeave > leaveTime)
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
                    if (obj == null)
                        continue; //wtf
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

        }

        private bool keypressed(int key)
        {
            switch (key)
            {
                case 1:
                    mousePressedLeft = MouseHandler.GetState().LeftButton == ButtonState.Pressed;
                    return mousePressedLeft;
                    break;
                case 2:
                    mousePressedRight = MouseHandler.GetState().RightButton == ButtonState.Pressed;
                    return mousePressedRight;
                    break;
                case 3:
                    keypressedZ = Keyboard.GetState().IsKeyDown(Keys.Z);
                    return keypressedZ;
                    break;
                case 4:
                    keypressedX = Keyboard.GetState().IsKeyDown(Keys.X);
                    return keypressedX;
                    break;
            }

            return false;
        }

        internal override void UpdateControls()
        {
            Controls.RemoveAll(item => item == null); //wtf
            Controls.RemoveAll(item => item.Died);
            bool first = false;
            foreach (UIObjectBase control in Controls)
            {              

                if(control is HitBase)
                {
                    
                    if (!first)
                    {
                        if (!control.Died)
                            ((HitSingle)control).IsFirst = first = true;
                    }
                }

                control.Update();

                if(control is HitBase)
                {
                    ((HitSingle)control).keyPressed = Keyboard.GetState().GetPressedKeys().ToList();
                }
            }

            
        }


    }
}
