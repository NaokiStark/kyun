using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kyun.Beatmap;
using kyun.game.GameModes.CatchIt;
using Microsoft.Xna.Framework;
using kyun.GameModes;
using kyun.Score;
using Microsoft.Xna.Framework.Graphics;
using kyun.Utils;

namespace kyun.game.GameModes.CatchItCollab
{
    public class HitObjectCollab : HitObject
    {
        public HitObjectCollab(IHitObj hitObject, IBeatmap beatmap, CatchItMode Instance, bool shared = false, bool longNote = false, HitObject parent = null) : base(hitObject, beatmap, Instance, shared, longNote, parent)
        {
            Position = new Vector2(i.ActualScreenMode.Width - i.Player.Size.X, Position.Y);
        }

        internal override void updatePosition()
        {
            int inFieldPosition = (int)(i.FieldSize.Y / i.fieldSlots);

            inFieldPosition *= PositionInRow;

            float sizeT = i.ActualScreenMode.Width - i.Player.Size.X * 2 - i.Player.Position.X;

            MsPerBeat = ((1680 - i.Beatmap.ApproachRate * 100)) / ((60000 / i.Beatmap.HitObjects[0].MsPerBeat) * 5);
            float appr = ((i.gameMod & GameMod.DoubleTime) != GameMod.DoubleTime) ? MsPerBeat : MsPerBeat * 1.5f ;

            float pos = (Time + 100) - i.GamePosition;
            appr = (float)Math.Round(appr, 1);

            Position = new Vector2((int)Math.Round(pos * (1/appr)), inFieldPosition + i.FieldPosition.Y + ((i.PlayerSize.Y / 2) - (Texture.Height / 2)));



            Rectangle thisRg = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            Rectangle catcherRg = new Rectangle((int)i.Player.Position.X, (int)i.Player.Position.Y, (int)i.PlayerSize.X, (int)i.PlayerSize.Y);

            if ((i.gameMod & GameMod.Auto) == GameMod.Auto)
            {
                if (Position.X < catcherRg.Width + 10)
                {
                    //Emulates key
                    if (Math.Abs(i.playerLinePosition - PositionInRow) >= 2)
                        i.isKeyDownShift = true;

                    i.playerLinePosition = PositionInRow;
                    //i.movements.Add(new ReplayObject { PressedAt = PositionInRow, LeaveAt = i.GamePosition });
                }

            }

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

            if (Position.X < 0)
            {
                if (!((i.gameMod & GameMod.Replay) == GameMod.Replay))
                    PositionAtCollision = i.playerLinePosition;

                CollisionAt = i.GamePosition;
                CheckScore();
                i._healthbar.Substract((2 * _beatmap.OverallDifficulty) * i.FailsCount);
                Combo.Instance.Miss();
                i.catcherToIdle = 0;
                i.changeCatcherTx(PlayerTxType.Miss);
                //playHitsound();
                Died = true;
            }
        }

        internal override void CheckScore()
        {
            Texture2D particle = SpritesContent.Instance.MissTx; //Using a no assingned var

            if (CollisionAt >= Time - _beatmap.Timing300 * 2 && CollisionAt <= Time + _beatmap.Timing300 * 2)
            {
                //Perfect
                score = ScoreType.Perfect;
                particle = SpritesContent.Instance.PerfectTx;
                i._healthbar.Add(4);

            }
            else if (CollisionAt >= Time - _beatmap.Timing300 * 2 && CollisionAt <= Time + _beatmap.Timing100 * 2)
            {

                score = ScoreType.Excellent;
                particle = SpritesContent.Instance.ExcellentTx;
                i._healthbar.Add(2);
            }
            else if (CollisionAt >= Time - _beatmap.Timing100 * 2 && CollisionAt <= Time + _beatmap.Timing100 * 2)
            {
                //Excellent
                score = ScoreType.Excellent;
                particle = SpritesContent.Instance.ExcellentTx;
                i._healthbar.Add(2);
            }
            else if (CollisionAt >= Time - _beatmap.Timing50 * 2 && CollisionAt <= Time + _beatmap.Timing50 * 2)
            {
                //Bad
                score = ScoreType.Good;
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
    }
}
