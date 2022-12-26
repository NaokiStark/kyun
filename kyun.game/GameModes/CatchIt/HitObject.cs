using kyun.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using kyun.Utils;
using kyun.Beatmap;
using Microsoft.Xna.Framework;
using kyun.Audio;
using kyun.Score;
using osuBMParser;
using kyun.Screen;
using kyun.game.Beatmap.Generators;

namespace kyun.game.GameModes.CatchIt
{
    public class HitObject : HitBase
    {
        internal CatchItMode i;

        internal IHitObj _hitButton;
        internal IBeatmap _beatmap;

        public HitObject _parent;

        public ScoreType score = ScoreType.Miss;

        public long CollisionAt = 0;
        public int PositionAtCollision = 1;

        ScreenMode a = ScreenModeManager.GetActualMode();

        public int HitSound
        {
            get
            {
                return _hitButton.HitSound;
            }
        }

        private TimingPoint _TimingPoint;

        public long startTimeTmp = 0;

        public long Time
        {
            get
            {
                if (startTimeTmp > 0)
                {
                    return startTimeTmp;
                }

                if (_longNote)
                {
                    if (_hitButton.EndTime >= KyunGame.Instance.Player.Length)
                    {
                        return (long)(_hitButton.StartTime + (decimal)_TimingPoint.MsPerBeat);
                    }
                    else
                    {
                        return (long)_hitButton.EndTime;
                    }
                }

                return (long)_hitButton.StartTime;
            }
            set
            {
                startTimeTmp = value;
            }
        }

        public int ReplayId = 1;

        bool _longNote;

        public int PositionInRow = 1;

        public HitObject(IHitObj hitObject, IBeatmap beatmap, CatchItMode Instance, bool shared = false, bool longNote = false, HitObject parent = null) : base(SpritesContent.Instance.CircleNote)
        {

            float msTemp = (((OsuUtils.OsuBeatMap)beatmap).osuBeatmapType == 3) ? hitObject.MsPerBeat / 1.5f : hitObject.MsPerBeat;
            //MsPerBeat = ((1680 - beatmap.ApproachRate * 150) * 10000) / ((60000 / msTemp) / 100);

            float preempt = 0;
            if (beatmap.ApproachRate < 5)
            {
                preempt = 1200 + 600 * (5 - (beatmap.ApproachRate / 10)) / 5;
            }/*
            else if(beatmap.ApproachRate == 5)
            {
                preempt = 1200;
            }*/
            else
            {
                preempt = 1200 - 750 * ((beatmap.ApproachRate / 10) - 5) / 5;
            }

            MsPerBeat = preempt * 10000;

            i = Instance;
            _hitButton = hitObject;
            _beatmap = beatmap;
            _longNote = longNote;

            if (_longNote)
            {
                parent.Texture = SpritesContent.Instance.Holder_Start;
                Texture = SpritesContent.Instance.Holder_End;
                chechAndAdd(hitObject, beatmap, Instance);
            }
            else
            {
                if(i._osuBeatmapSkin != null)
                {
                    if(i._osuBeatmapSkin.skinFiles["hitcircle"] != null)
                    {
                        Texture = i._osuBeatmapSkin.skinFiles["hitcircle"];
                    }
                }
            }

            Visible = true;
            Opacity = 1;

            _parent = parent;

            Vector2 oLoc = hitObject.OsuLocation;
            switch (i._osuMode)
            {
                case OsuGameMode.Taiko:
                    if (_parent == null)
                    {
                        //oLoc = RandomGenerator.MakeRandom(8);
                        oLoc = hitObject.OsuLocation;
                    }


                    //finalPos = getMiddleScreen();
                    break;
            }

            //384 = max value of Y in osu!

            int row = 384 / i.fieldSlots;

            int positionInRow = (int)Math.Floor((oLoc.X / (float)row));

            positionInRow = Math.Min(i.fieldSlots, positionInRow);
            positionInRow = Math.Max(1, positionInRow);

            if (_parent == null)
            {
                PositionInRow = positionInRow;
                if (i.objectIndx > 0)
                {
                    var lastpos = (i.HitObjects[i.objectIndx - 1] as HitObject).PositionInRow;
                    if (Math.Abs(lastpos - PositionInRow) > 2)
                    {
                        if (lastpos == 1)
                        {
                            PositionInRow = positionInRow = 3;
                        }
                        else
                        {
                            PositionInRow = positionInRow = 2;
                        }
                    }
                }
            }
            else
            {
                PositionInRow = _parent.PositionInRow;
            }


            int inFieldPosition = (int)(i.FieldSize.Y / i.fieldSlots);

            inFieldPosition *= positionInRow;

            Size = i.PlayerSize * .8f;

            Position = new Vector2(i.ActualScreenMode.Width, inFieldPosition + i.FieldPosition.Y + ((i.PlayerSize.Y / 2) - (Size.Y / 2)));
            if (_parent != null)
            {
                Position = new Vector2(i.ActualScreenMode.Width, _parent.Position.Y);
            }


            switch (hitObject.HitSound)
            {
                case 2:
                    //TextureColor = Color.FromNonPremultiplied(0, 203, 255, 255);
                    TextureColor = Color.White;
                    break;
                case 4:
                    //TextureColor = Color.FromNonPremultiplied(9, 229, 27, 255);
                    TextureColor = Color.White;
                    break;
                case 8:
                    TextureColor = Color.FromNonPremultiplied(255, 61, 58, 255);
                    break;
                default:
                    TextureColor = Color.White;
                    break;

            }

            _TimingPoint = i.Beatmap.GetTimingPointFor((long)_hitButton.StartTime, false);


        }

        private void chechAndAdd(IHitObj hitObject, IBeatmap beatmap, CatchItMode instance)
        {
            int repeats = (hitObject as kyun.Beatmap.HitHolder).osuRepeat;

            for (int a = 1; a < repeats; a++)
            {
                decimal stime = ((hitObject.EndTime - hitObject.StartTime) / (decimal)repeats) * (decimal)a;

                HitButton hbtn = new HitButton()
                {
                    StartTime = hitObject.StartTime + stime,
                    EndTime = hitObject.StartTime + stime,
                    BeatmapContainer = beatmap,
                    OsuLocation = hitObject.OsuLocation,
                    MsPerBeat = hitObject.MsPerBeat,
                    HitSound = hitObject.HitSound,
                };

                var cobj = new HitObject(hbtn, beatmap, instance);

                instance.HitObjects.Add(cobj);
                instance.HitObjectsRemain.Add(cobj);
                cobj.Texture = SpritesContent.instance.MenuSnow;
                cobj.Scale = .5f;

                instance.Controls.Add(cobj);
            }
        }

        internal void updateRowPos()
        {
            int inFieldPosition = (int)(i.FieldSize.Y / i.fieldSlots);

            inFieldPosition *= PositionInRow;

            Size = i.PlayerSize * .95f;

            Position = new Vector2(i.ActualScreenMode.Width, inFieldPosition + i.FieldPosition.Y + ((Size.Y / 2) - (Size.Y / 2)));
            if (_parent != null)
            {
                Position = new Vector2(i.ActualScreenMode.Width, _parent.Position.Y);
            }
        }

        internal virtual void updatePosition()
        {

            //Note: 1024 is for universal velocity, cuz, in higher resolution speeds are imposible
            float twidth = 1024 - (i.PlayerSize.X + i.Player.Position.X);



            float appr = ((i.gameMod & GameMod.DoubleTime) != GameMod.DoubleTime) ? MsPerBeat : MsPerBeat * 1.5f /*(int)(1000f * ((float)a.Width / 500f))*/;

            float stime = ((float)Time - (float)i.GamePosition);

            float gtime = (stime / appr) * (i.SmoothVelocity * (Math.Max(1.5f, Math.Min(_hitButton.MsPerBeat, 400) / 100)));
            float percen = gtime * twidth;


            Position = new Vector2((twidth * percen) + i.PlayerSize.X, Position.Y);

            Rectangle thisRg = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            Rectangle catcherRg = new Rectangle((int)i.Player.Position.X, (int)i.Player.Position.Y, (int)i.PlayerSize.X, (int)i.PlayerSize.Y);

            if ((i.gameMod & GameMod.Auto) == GameMod.Auto && !(this is FakeHitObject))
            {
                if (Position.X < catcherRg.Width + 10)
                {
                    //Emulates key
                    if (Math.Abs(i.playerLinePosition - PositionInRow) >= 2)
                        i.isKeyDownShift = true;

                    i.playerLinePosition = PositionInRow;
                    i.movements.Add(new ReplayObject { PressedAt = PositionInRow, LeaveAt = i.GamePosition });
                }

            }

            if ((i.gameMod & GameMod.Auto) == GameMod.Auto && this is FakeHitObject)
            {
                if (Position.X < catcherRg.Width + 10)
                {
                    if (i.playerLinePosition == PositionInRow)
                    {
                        if (i.playerLinePosition >= 2)
                        {
                            if (i.playerLinePosition == 4)
                            {
                                i.playerLinePosition = PositionInRow - 1;
                            }
                            else if (i.playerLinePosition == 2)
                            {
                                i.playerLinePosition = OsuUtils.OsuBeatMap.rnd.NextBoolean() ? 1 : 3;
                            }
                            else if (i.playerLinePosition == 3)
                            {
                                i.playerLinePosition = OsuUtils.OsuBeatMap.rnd.NextBoolean() ? 2 : 4;
                            }

                        }
                        else if (i.playerLinePosition < 2)
                        {
                            i.playerLinePosition = PositionInRow + 1;
                        }
                    }
                }
            }
            /*
            if ((i.gameMod & GameMod.Replay) == GameMod.Replay)
            {
                var playerPosition = (int)i.replay.Hits[ReplayId].PressedAt;
                var collisionat = i.replay.Hits[ReplayId].LeaveAt;
                if (i.GamePosition > collisionat)
                    i.playerLinePosition = playerPosition;
            }*/

            if (thisRg.Intersects(catcherRg))
            {
                if (this is FakeHitObject)
                {
                    CollisionAt = i.GamePosition;
                    if (!((i.gameMod & GameMod.Replay) == GameMod.Replay))
                        PositionAtCollision = PositionInRow;

                    if (!(CollisionAt <= Time + (_beatmap.Timing300 / 2)) || (i.gameMod & GameMod.Auto) == GameMod.Auto)
                    {
                        // ok, not toooo hard, this mean doesnt hit in front
                        //Died = true;
                        return;
                    }
                    else
                    {
                        i._healthbar.Substract((2 * _beatmap.OverallDifficulty) * i.FailsCount);
                        Combo.Instance.Miss();
                        i.catcherToIdle = 0;
                        i.changeCatcherTx(PlayerTxType.Miss);
                        CheckScore();
                        //playHitsound();
                        Died = true;
                        return;
                    }
                }
                else
                {
                    CollisionAt = i.GamePosition;
                    if (!((i.gameMod & GameMod.Replay) == GameMod.Replay))
                        PositionAtCollision = PositionInRow;

                    Combo.Instance.Add();
                    CheckScore();
                    playHitsound();
                    Died = true;
                    return;
                }

            }

            if (Position.X <= 0 - Size.X)
            {
                if (this is FakeHitObject)
                {

                    CollisionAt = Time;
                    (this as FakeHitObject).checkOkScore();
                    Combo.Instance.Add();

                    Died = true;

                }
                else
                {
                    if (!((i.gameMod & GameMod.Replay) == GameMod.Replay))
                        PositionAtCollision = i.playerLinePosition;

                    CollisionAt = i.GamePosition + 1000;
                    CheckScore();
                    i._healthbar.Substract((2 * _beatmap.OverallDifficulty) * i.FailsCount);
                    Combo.Instance.Miss();
                    i.catcherToIdle = 0;
                    i.changeCatcherTx(PlayerTxType.Miss);
                    //playHitsound();
                    Died = true;
                }
            }

            // ToDo: debug this 
            if(i.playerLinePosition != PositionInRow && _longNote && _parent != null && !Died)
            {
                if (_parent.Died) { 
                    Combo.Instance.Miss();
                    i.catcherToIdle = 0;
                    i.changeCatcherTx(PlayerTxType.Miss);
                }
            }
        }

        internal void updateOpacity()
        {
            //Opacity = Math.Max(KyunGame.Instance.maxPeak, .7f);
        }

        internal virtual void CheckScore()
        {

            Texture2D particle = SpritesContent.Instance.MissTx; //Using a no assingned var
            if (i._osuBeatmapSkin != null)
            {
                particle = i._osuBeatmapSkin.skinFiles["hit0"] == null ? SpritesContent.Instance.MissTx : i._osuBeatmapSkin.skinFiles["hit0"];
            }

            if (/*CollisionAt >= Time - (_beatmap.Timing300 * 2) &&*/ CollisionAt <= Time + (_beatmap.Timing300 * 2))
            {
                //Perfect
                score = ScoreType.Perfect;
                if (i._osuBeatmapSkin != null)
                {
                    particle = i._osuBeatmapSkin.skinFiles["hit300"] == null ? SpritesContent.Instance.PerfectTx : i._osuBeatmapSkin.skinFiles["hit300"];
                }
                else
                {
                    particle = SpritesContent.Instance.PerfectTx;
                }
                i._healthbar.Add(4);

            }
            else if (CollisionAt >= Time - _beatmap.Timing300 && CollisionAt <= Time + (_beatmap.Timing100 * 2))
            {

                score = ScoreType.Excellent;
                if (i._osuBeatmapSkin != null)
                {
                    particle = i._osuBeatmapSkin.skinFiles["hit100"] == null ? SpritesContent.Instance.ExcellentTx : i._osuBeatmapSkin.skinFiles["hit100"];
                }
                else
                {
                    particle = SpritesContent.Instance.ExcellentTx;
                }
                i._healthbar.Add(2);
            }
            else if (CollisionAt >= Time - (_beatmap.Timing100 * 2) && CollisionAt <= Time + (_beatmap.Timing100 * 2))
            {
                //Excellent
                score = ScoreType.Excellent;
                if (i._osuBeatmapSkin != null)
                {
                    particle = i._osuBeatmapSkin.skinFiles["hit100"] == null ? SpritesContent.Instance.ExcellentTx : i._osuBeatmapSkin.skinFiles["hit100"];
                }
                else
                {
                    particle = SpritesContent.Instance.ExcellentTx;
                }
                i._healthbar.Add(2);
            }
            else if (CollisionAt >= Time - (_beatmap.Timing50 * 2) && CollisionAt <= Time + (_beatmap.Timing50 * 2))
            {
                //Bad
                score = ScoreType.Good;
                if (i._osuBeatmapSkin != null)
                {
                    particle = i._osuBeatmapSkin.skinFiles["hit50"] == null ? SpritesContent.Instance.GoodTx : i._osuBeatmapSkin.skinFiles["hit50"];
                }
                else
                {
                    particle = SpritesContent.Instance.GoodTx;
                }
            }

            Texture2D donePartice = Texture;
            if (_longNote || Texture == SpritesContent.Instance.Holder_Start)
            {
                donePartice = SpritesContent.Instance.CircleNoteHolder;
            }

            var gnpr = i.particleEngine.AddNewHitObjectParticle(donePartice,
               new Vector2(2f),
               new Vector2((score == ScoreType.Good || score == ScoreType.Miss) ? Position.X : i.PlayerSize.X, Position.Y),
               10,
               0,
               ((score & ScoreType.Miss) == ScoreType.Miss) ? Color.Black : TextureColor
               );
            gnpr.Opacity = .6f;
            gnpr.Size = Size;

            i._scoreDisplay.Add((((int)score / 50) * Math.Max(Combo.Instance.ActualMultiplier, 1)) / 2);
            i._scoreDisplay.CalcAcc(score);

            if(score != ScoreType.Miss && HitSound == 8)
            {
                i.showGuide();
            }

            if (particle == SpritesContent.instance.MissTx || particle == SpritesContent.instance.ExcellentTx || particle == SpritesContent.instance.PerfectTx || particle == SpritesContent.instance.GoodTx)
            {
                i.particleEngine.AddNewScoreParticle(particle,
                    new Vector2(.05f),
                    new Vector2(0, Position.Y + (particle.Height + 10) * 1.5f),
                    10,
                    0,
                    Color.White
                    );
            }
            else
            {
                var cpart = i.particleEngine.AddNewScoreParticle(particle,
                    new Vector2(.05f),
                    new Vector2(2, Position.Y),
                    10,
                    0,
                    Color.White
                    );
                cpart.Size = i.PlayerSize;
            }
        }

        internal override ScoreType GetScore()
        {
            return score;
        }

        internal virtual void playHitsound()
        {
            int hsound = 0;
            switch (HitSound)
            {
                case 2:
                    hsound = SpritesContent.Instance.Hitwhistle;
                    break;
                case 4:
                    hsound = SpritesContent.Instance.Hitfinish;
                    break;
                case 8:
                    hsound = SpritesContent.Instance.Hitclap;
                    break;
                default:
                    hsound = SpritesContent.Instance.HitHolder;
                    break;
            }

            TimingPoint tm = i.ActualTimingPoint;

            if (i.sampleSet != null)
            {
                if (tm.SampleType > 0 && tm.SampleSet > 0)
                {
                    var tempSound = i.sampleSet.GetSample(tm.SampleType, HitSound, tm.SampleSet);
                    if (tempSound != 0)
                    {
                        hsound = tempSound;
                    }
                }

            }

            EffectsPlayer.PlayEffect(hsound, tm.Volume / 100f);
        }


        public override void Update()
        {
            updateOpacity();
            updatePosition();
            base.Update();
        }

        public override void Render()
        {

            if (Position.X > ScreenMode.Width + Size.X * 2 && !_longNote)
            {
                return;
            }

            if (_longNote)
            {
                int posx = (int)(_parent.Position.X + _parent.Size.X / 2) - 1;
                int posy = (int)_parent.Position.Y;
                int elmWidth = (int)Math.Abs(posx - (Position.X + (Size.X / 2))) + 1;
                int elmHeigth = (int)Size.Y;

                var rg = new Rectangle(posx, posy, elmWidth, elmHeigth);
                if (Position.X > i.Player.Position.X + i.PlayerSize.X)
                {
                    if (_parent != null)
                    {
                        KyunGame.Instance.SpriteBatch.Draw(
                            SpritesContent.Instance.Holder_Middle,
                            rg,
                            TextureColor * Opacity);

                        if (_parent.Died)
                        {
                            _parent.Position = new Vector2(i.PlayerSize.X, _parent.Position.Y);
                        }
                        _parent?.Render();

                        base.Render();
                        base.Render();

                    }
                }
            }
            else
            {
                base.Render();
            }

        }
    }
}
