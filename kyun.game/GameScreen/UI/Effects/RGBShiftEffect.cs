using kyun.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameScreen.UI.Effects
{
    public class RGBShiftEffect : EffectParametersBase
    {
        private float peak;
        private float magic;
        private float dPeak;

        public RGBShiftEffect()
        {
            Effect = SpritesContent.Instance.RGBShiftEffect;
            Parameters.Add(new KeyValuePair<string, dynamic>("xColoredTexture", null));
            Parameters.Add(new KeyValuePair<string, dynamic>("DisplacementDist", 0f));
            Parameters.Add(new KeyValuePair<string, dynamic>("DisplacementScroll", 0f));
        }

        public override void Update()
        {
            base.Update();

            magic = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(-4d, 4d);

            if (peak > 0)
            {
                peak = (float)Math.Max(peak - KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f, 0);
            }
            else if (peak < 0)
            {
                peak = (float)Math.Min(peak + KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f, 0);
            }


            if (peak == 0 || float.IsNaN(peak))
            {

                if (KyunGame.Instance.Player.PeakVol >= InstanceManager.MaxPeak - 0.11)
                    peak = 2 * (magic / -magic);
            }

            dPeak = (float)Math.Max(dPeak - KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.01f, 0);

            if (KyunGame.Instance.Player.PeakVol >= InstanceManager.MaxPeak - 0.01)
            {
                dPeak = KyunGame.Instance.Player.PeakVol;
            }

            float distorsion = Math.Max(0, Math.Min(.1f, dPeak / 5));

            for(int i = 0; i < Parameters.Count; i++)
            {
                KeyValuePair<string, dynamic> pair = Parameters[i];
                switch (pair.Key)
                {
                    case "DisplacementDist":
                        Parameters[i] = new KeyValuePair<string, dynamic>("DisplacementDist", -distorsion);
                        break;
                    case "DisplacementScroll":
                        Parameters[i] = new KeyValuePair<string, dynamic>("DisplacementScroll", (dPeak <= 0f) ? 0f : (peak / 1000) * magic);
                        break;
                }
            }
        }
    }
}
