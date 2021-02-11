using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kyun.Beatmap;
using kyun.Score;
using kyun.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace kyun.game.GameModes.CatchIt
{
    public class FakeHitObject : HitObject
    {
        public bool alreadySounded = false;

        public FakeHitObject(IHitObj hitObject, IBeatmap beatmap, CatchItMode Instance, bool shared = false, bool longNote = false, HitObject parent = null) : base(hitObject, beatmap, Instance, shared, longNote, parent)
        {
            //TextureColor = Color.FromNonPremultiplied(154, 156, 42, 255);
            Texture = i.bombTx;
        }

        internal override void CheckScore()
        {
            
            Texture2D particle = SpritesContent.Instance.MissTx;

            var gnpr = i.particleEngine.AddNewHitObjectParticle(Texture,
               new Vector2(2f),
               new Vector2(Position.X, Position.Y),
               10,
               0,
               ((score & ScoreType.Miss) == ScoreType.Miss) ? Color.Violet : TextureColor
               );
            gnpr.Opacity = .6f;
            gnpr.Size = Size;

            //i._scoreDisplay.Add((((int)score / 50) * Math.Max(Combo.Instance.ActualMultiplier, 1)) / 2);
            //i._scoreDisplay.CalcAcc(score);

            i.particleEngine.AddNewScoreParticle(particle,
                new Vector2(.05f),
                new Vector2(0, Position.Y + (particle.Height + 10) * 1.5f),
                10,
                0,
                Color.White
                );

        }

        internal void checkOkScore()
        {
            score = ScoreType.Perfect;
            
            i._scoreDisplay.Add((((int)score / 50) * Math.Max(Combo.Instance.ActualMultiplier, 1)) / 2);
            i._scoreDisplay.CalcAcc(score);
        }

        internal override void updatePosition()
        {
            base.updatePosition();

            if (!alreadySounded)
            {
                Rectangle cthisRg = new Rectangle((int)Position.X, (int)i.Player.Position.Y, (int)Size.X, (int)Size.Y);
                Rectangle ccatcherRg = new Rectangle((int)i.Player.Position.X, (int)i.Player.Position.Y, (int)i.PlayerSize.X, (int)i.PlayerSize.Y);
                if (cthisRg.Intersects(ccatcherRg))
                {
                    alreadySounded = true;
                    playHitsound();
                }
            }

            if (/*i.catcherElapsed > Math.Max(i.NonInheritedPoint.MsPerBeat * 0.99f, 1000f / 150f)*/ false)
            {
                // whoea
               var gnpr = i.particleEngine.AddNewHitObjectParticle(Texture,
               new Vector2(2f),
               new Vector2(Position.X, Position.Y),
               10,
               0,
               TextureColor
               );
                gnpr.Opacity = .8f;
                gnpr.Size = Size;
            }
        }
    }

}
