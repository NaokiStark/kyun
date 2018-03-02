using kyun.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using kyun.Utils;
using kyun.Beatmap;
using Microsoft.Xna.Framework;
using kyun.Audio;
using kyun.Score;

namespace kyun.game.GameModes.CatchIt
{
    public class HitObject : HitBase
    {
        CatchItMode i;

        internal IHitObj _hitButton;
        internal IBeatmap _beatmap;

        public HitObject _parent;

        public Score.ScoreType score = ScoreType.Miss;

        public long CollisionAt = 0;
        public int PositionAtCollision = 1;

        Screen.ScreenMode a = Screen.ScreenModeManager.GetActualMode();

        public int HitSound
        {
            get
            {
                return _hitButton.HitSound;
            }
        }

        public long Time
        {
            get
            {
                return (_longNote)? (long)_hitButton.EndTime : (long)_hitButton.StartTime;
            }
        }

        public int ReplayId = 1;

        bool _longNote;

        public int PositionInRow = 1;

        public HitObject(IHitObj hitObject, IBeatmap beatmap, CatchItMode Instance, bool shared = false, bool longNote = false, HitObject parent = null) : base(SpritesContent.Instance.CatchObject)
        {
            float msTemp = (((OsuUtils.OsuBeatMap)beatmap).osuBeatmapType == 3)? hitObject.MsPerBeat / 1.5f: hitObject.MsPerBeat;
            MsPerBeat = ((1680 - beatmap.ApproachRate * 150) * 10000) / ((60000 / msTemp) / 50);

            i = Instance;
            _hitButton = hitObject;
            _beatmap = beatmap;
            _longNote = longNote;

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

            Position = new Vector2(i.ActualScreenMode.Width, inFieldPosition + i.FieldPosition.Y + ((i.PlayerSize.Y / 2) - (Texture.Height / 2)));

            

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
                    TextureColor = Color.FromNonPremultiplied(255, 229, 244, 255);
                    break;

            }
        }

        internal void updatePosition()
        {

            //Note: 1024 is for universal velocity, cuz, in higher resolution speeds are imposible
            float twidth = 1024 - i.PlayerSize.X;

            

            float appr = ((i.gameMod & GameMod.DoubleTime) != GameMod.DoubleTime)?MsPerBeat:MsPerBeat * 1.5f /*(int)(1000f * ((float)a.Width / 500f))*/;


            float stime = ((float)Time - (float)i.GamePosition);

            float gtime = (stime / appr);
            float percen = gtime * twidth;
            
            
            Position = new Vector2((twidth * percen) + i.PlayerSize.X, Position.Y);

            Rectangle thisRg = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            Rectangle catcherRg = new Rectangle((int)i.Player.Position.X, (int)i.Player.Position.Y, (int)i.PlayerSize.X, (int)i.PlayerSize.Y);

            if((i.gameMod & GameMod.Auto) == GameMod.Auto)
            {
                if (Position.X < catcherRg.Width + 10)
                {
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
                if(!((i.gameMod & GameMod.Replay) == GameMod.Replay))
                    PositionAtCollision = PositionInRow;

                Combo.Instance.Add();
                CheckScore();
                playHitsound();
                Died = true;
                return;
            }

            if (Position.X < 0)
            {
                if (!((i.gameMod & GameMod.Replay) == GameMod.Replay))
                    PositionAtCollision = i.playerLinePosition;

                CollisionAt = i.GamePosition;
                CheckScore();
                i._healthbar.Substract((2 * _beatmap.OverallDifficulty) * i.FailsCount);
                Combo.Instance.Miss();
                //playHitsound();
                Died = true;
            }

            
        }

        internal void updateOpacity()
        {
            Opacity = Math.Max(KyunGame.Instance.maxPeak, .7f);
        }

        internal virtual void CheckScore()
        {

            Texture2D particle = SpritesContent.Instance.MissTx; //Using a no assingned var

            if (CollisionAt >= Time - _beatmap.Timing300 && CollisionAt <= Time + _beatmap.Timing300)
            {
                //Perfect
                score = Score.ScoreType.Perfect;
                particle = SpritesContent.Instance.PerfectTx;
                i._healthbar.Add(4);
                
            }
            else if (CollisionAt >= Time - _beatmap.Timing300 && CollisionAt <= Time + _beatmap.Timing100)
            {

                score = Score.ScoreType.Excellent;
                particle = SpritesContent.Instance.ExcellentTx;
                i._healthbar.Add(2);
            }
            else if (CollisionAt >= Time - _beatmap.Timing100 && CollisionAt <= Time + _beatmap.Timing100)
            {
                //Excellent
                score = Score.ScoreType.Excellent;
                particle = SpritesContent.Instance.ExcellentTx;
                i._healthbar.Add(2);
            }
            else if (CollisionAt >= Time - _beatmap.Timing50 && CollisionAt <= Time + _beatmap.Timing50)
            {
                //Bad
                score = Score.ScoreType.Good;
                particle = SpritesContent.Instance.GoodTx;
            }

            i._scoreDisplay.Add((((int)score / 50) * Math.Max(Combo.Instance.ActualMultiplier, 1)) / 2);

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
            if(_longNote && ((OsuUtils.OsuBeatMap)_beatmap).osuBeatmapType <= 1)
                if(Position.X > i.Player.Position.X + i.PlayerSize.X)
                    KyunGame.Instance.SpriteBatch.Draw(i.LongTail, new Rectangle((int)Position.X + Texture.Width/2, (int)Position.Y + (Texture.Height / 2) / 2, ((int)_parent.Position.X + Texture.Width) - (int)Position.X - Texture.Width, Texture.Height / 2), TextureColor * Opacity);
            base.Render();
        }
    }
}
