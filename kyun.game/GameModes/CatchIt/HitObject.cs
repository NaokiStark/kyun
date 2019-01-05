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

        public long Time
        {
            get
            {
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
                parent.Texture = Texture = SpritesContent.Instance.CircleNoteHolder;
            }

            Visible = true;
            Opacity = 1;

            _parent = parent;

            //384 = max value of Y in osu!

            int row = 384 / i.fieldSlots;

            int positionInRow = (int)Math.Floor((hitObject.OsuLocation.X / (float)row));

            positionInRow = Math.Min(i.fieldSlots, positionInRow);
            positionInRow = Math.Max(1, positionInRow);

            PositionInRow = positionInRow;

            int inFieldPosition = (int)(i.FieldSize.Y / i.fieldSlots);

            inFieldPosition *= positionInRow;

            Size = i.PlayerSize * .95f;

            Position = new Vector2(i.ActualScreenMode.Width, inFieldPosition + i.FieldPosition.Y + ((Size.Y / 2) - (Size.Y / 2)));



            switch (hitObject.HitSound)
            {
                case 2:
                    TextureColor = Color.FromNonPremultiplied(0, 203, 255, 255);
                    break;
                case 4:
                    TextureColor = Color.FromNonPremultiplied(9, 229, 27, 255);
                    break;
                case 8:
                    TextureColor = Color.FromNonPremultiplied(255, 61, 58, 255);
                    break;
                default:
                    TextureColor = /*Color.FromNonPremultiplied(255, 229, 244, 255)*/ Color.White;
                    break;

            }

            _TimingPoint = i.Beatmap.GetTimingPointFor((long)_hitButton.StartTime, false);
        }

        internal virtual void updatePosition()
        {

            //Note: 1024 is for universal velocity, cuz, in higher resolution speeds are imposible
            float twidth = 1024 - (i.PlayerSize.X + i.Player.Position.X);


            //experimental zone


            //eof experimental zone

            float appr = ((i.gameMod & GameMod.DoubleTime) != GameMod.DoubleTime) ? MsPerBeat : MsPerBeat * 1.5f /*(int)(1000f * ((float)a.Width / 500f))*/;

            float stime = ((float)Time - (float)i.GamePosition);

            float gtime = (stime / appr) * (i.SmoothVelocity * (Math.Max(1.5f, Math.Min(_hitButton.MsPerBeat, 400) / 100)));
            float percen = gtime * twidth;


            Position = new Vector2((twidth * percen) + i.PlayerSize.X, Position.Y);

            Rectangle thisRg = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            Rectangle catcherRg = new Rectangle((int)i.Player.Position.X, (int)i.Player.Position.Y, (int)i.PlayerSize.X, (int)i.PlayerSize.Y);

            if ((i.gameMod & GameMod.Auto) == GameMod.Auto)
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

                CollisionAt = i.GamePosition;
                if (!((i.gameMod & GameMod.Replay) == GameMod.Replay))
                    PositionAtCollision = PositionInRow;

                Combo.Instance.Add();
                CheckScore();
                playHitsound();
                Died = true;
                return;
            }

            if (Position.X <= 0)
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

        internal void updateOpacity()
        {
            //Opacity = Math.Max(KyunGame.Instance.maxPeak, .7f);
        }

        internal virtual void CheckScore()
        {

            Texture2D particle = SpritesContent.Instance.MissTx; //Using a no assingned var

            if (/*CollisionAt >= Time - (_beatmap.Timing300 * 2) &&*/ CollisionAt <= Time + (_beatmap.Timing300 * 2))
            {
                //Perfect
                score = ScoreType.Perfect;
                particle = SpritesContent.Instance.PerfectTx;
                i._healthbar.Add(4);

            }
            else if (CollisionAt >= Time - _beatmap.Timing300 && CollisionAt <= Time + (_beatmap.Timing100 * 2))
            {

                score = ScoreType.Excellent;
                particle = SpritesContent.Instance.ExcellentTx;
                i._healthbar.Add(2);
            }
            else if (CollisionAt >= Time - (_beatmap.Timing100 * 2) && CollisionAt <= Time + (_beatmap.Timing100 * 2))
            {
                //Excellent
                score = ScoreType.Excellent;
                particle = SpritesContent.Instance.ExcellentTx;
                i._healthbar.Add(2);
            }
            else if (CollisionAt >= Time - (_beatmap.Timing50 * 2) && CollisionAt <= Time + (_beatmap.Timing50 * 2))
            {
                //Bad
                score = ScoreType.Good;
                particle = SpritesContent.Instance.GoodTx;
            }



            var gnpr = i.particleEngine.AddNewHitObjectParticle(Texture,
               new Vector2(2f),
               new Vector2(Position.X, Position.Y),
               10,
               0,
               ((score & ScoreType.Miss) == ScoreType.Miss) ? Color.Violet : TextureColor
               );
            gnpr.Opacity = .6f;
            gnpr.Size = i.PlayerSize;

            i._scoreDisplay.Add((((int)score / 50) * Math.Max(Combo.Instance.ActualMultiplier, 1)) / 2);
            i._scoreDisplay.CalcAcc(score);

            i.particleEngine.AddNewScoreParticle(particle,
                new Vector2(.05f),
                new Vector2(0, Position.Y + (particle.Height + 10) * 1.5f),
                10,
                0,
                Color.White
                );

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

            EffectsPlayer.PlayEffect(hsound);
        }


        public override void Update()
        {
            updateOpacity();
            updatePosition();
            base.Update();
        }

        public override void Render()
        {


            if (_longNote && ((OsuUtils.OsuBeatMap)_beatmap).osuBeatmapType <= 1)
            {
                if (Position.X > i.Player.Position.X + i.PlayerSize.X)
                {
                    if (_parent != null)
                    {
                       
                      

                        KyunGame.Instance.SpriteBatch.Draw(i.LongTail, new Rectangle((int)Position.X + (int)Size.X / 2, (int)Position.Y + (((int)Size.Y / 2)) / 2, ((int)_parent.Position.X + (int)Size.X) - (int)Position.X - (int)Size.X, ((int)Size.Y / 2)), TextureColor * Opacity);
                        
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
