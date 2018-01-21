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

        public Score.ScoreType score = ScoreType.Miss;

        long CollisionAt = 0;

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

        bool _longNote;

        public int PositionInRow = 1;

        public HitObject(IHitObj hitObject, IBeatmap beatmap, CatchItMode Instance, bool shared = false, bool longNote = false) : base(SpritesContent.Instance.SquareButton)
        {
            i = Instance;
            _hitButton = hitObject;
            _beatmap = beatmap;
            _longNote = longNote;

            Visible = true;
            Opacity = 1;


            //384 = max value of Y in osu!

            int row = 384 / i.fieldSlots;

            int positionInRow = (int)Math.Floor((hitObject.OsuLocation.X / (float)row));

            positionInRow = Math.Min(i.fieldSlots, positionInRow);
            positionInRow = Math.Max(1, positionInRow);

            PositionInRow = positionInRow;

            int inFieldPosition = (int)(i.FieldSize.Y / i.fieldSlots);

            inFieldPosition *= positionInRow;

            Position = new Vector2(i.ActualScreenMode.Width, inFieldPosition + i.FieldPosition.Y + ((i.PlayerSize.Y / 2) - (Texture.Height / 2)));
        }

        internal void updatePosition()
        {
            float twidth = i.ActualScreenMode.Width - i.PlayerSize.X;

            Screen.ScreenMode a = Screen.ScreenModeManager.GetActualMode();

            int appr = (int)(2000 - _beatmap.ApproachRate * 150) * (int)(1000f * ((float)a.Width / 500f));


            float stime = ((float)Time - (float)i.GamePosition);

            float gtime = (stime / appr);
            float percen = gtime * twidth;
            
            
            Position = new Vector2((twidth * percen) + i.PlayerSize.X, Position.Y);

            Rectangle thisRg = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            Rectangle catcherRg = new Rectangle((int)i.Player.Position.X, (int)i.Player.Position.Y, (int)i.PlayerSize.X, (int)i.PlayerSize.Y);

            if((i.gameMod & GameMod.Auto) == GameMod.Auto)
            {
                if (Position.X < catcherRg.Width + 10)
                    i.playerLinePosition = PositionInRow;
            }

            if (thisRg.Intersects(catcherRg))
            {
                CollisionAt = i.GamePosition;
                
                Combo.Instance.Add();
                CheckScore();
                playHitsound();
                Died = true;
                return;
            }

            if (Position.X < 0)
            {
                CollisionAt = 1;
                CheckScore();
                i._healthbar.Substract((2 * _beatmap.OverallDifficulty) * i.FailsCount);
                Combo.Instance.Miss();
                //playHitsound();
                Died = true;
            }
                
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
            updatePosition();
            base.Update();
        }
    }
}
