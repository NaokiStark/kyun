using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.Beatmap.Generators
{
    public class RandomGenerator
    {
        static int MaxX = 530;
        static int MaxY = 310;

        public static Vector2 MakeRandom(int maxX, int maxY, float diff = 1)
        {
            OsuUtils.OsuBeatMap.rnd.Seed = (uint)DateTime.Now.Ticks;
            float x = OsuUtils.OsuBeatMap.rnd.Next((int)(maxX * diff));
            float y = OsuUtils.OsuBeatMap.rnd.Next((int)(maxY * diff));

            x = x + Math.Abs(maxX * diff / 4) ;
            y = y + Math.Abs(maxY * diff / 4) ;

            return new Vector2(x, y);
        }

        public static Vector2 MakeRandom()
        {
            return MakeRandom(MaxX, MaxY);
        }

        public static Vector2 MakeRandom(float diff)
        {
            float dividedDiff = diff / 10f;
            return MakeRandom((int)(MaxX), (int)(MaxY), dividedDiff);
        }
    }
}
