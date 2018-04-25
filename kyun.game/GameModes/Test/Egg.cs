using kyun.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using kyun.Utils;
using Microsoft.Xna.Framework;
using kyun.Audio;

namespace kyun.game.GameModes.Test
{
    public class Egg : HitBase
    {

        TestScreen i;

        public float randomVelocity;



        public Egg() : base(SpritesContent.Instance.CatchObject)
        {
            i = TestScreen.GetInstance();
            randomVelocity = OsuUtils.OsuBeatMap.rnd.Next(10, 20) / 10f;
        }

        public override void Update()
        {
            if (Died || !Visible)
                return;

            base.Update();

            Rectangle rg = new Rectangle((int)i.pl.Position.X, (int)i.pl.Position.Y, (int)i.pl.playerSize.X, (int)i.pl.playerSize.Y);
            Rectangle tspos = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            Scale = Opacity = Math.Max(.8f, KyunGame.Instance.maxPeak);

            Position = new Vector2(Position.X - (Elapsed.Milliseconds * .5f * randomVelocity * KyunGame.Instance.maxPeak), Position.Y);

            if (Position.X < -tspos.Width)
                Died = true;

            if (!rg.Intersects(tspos))
                return;

            //Collision

            Died = true;

            EffectsPlayer.PlayEffect(SpritesContent.Instance.ButtonHit);
            i.score++;
        }
    }
}
