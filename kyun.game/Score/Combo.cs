using kyun.Audio;
using kyun.Utils;

namespace kyun.Score
{
    public class Combo
    {
        public Combo()
        {
            instance = this;
        }

        private static Combo instance = null;
        public static Combo Instance
        {
            get
            {
                if (instance == null)
                    return new Combo();
                else
                    return instance;
            }
        }

        public long MaxMultiplier { get; private set; }
        public long ActualMultiplier { get; private set; }

        public void ResetAll()
        {
            MaxMultiplier = 0;
            ActualMultiplier = 0;
        }

        public void Miss()
        {
            if (ActualMultiplier >= 10)
                EffectsPlayer.PlayEffect(SpritesContent.Instance.ComboBreak);

            ActualMultiplier = 0;
        }

        public void Add()
        {
            ActualMultiplier++;
            if (ActualMultiplier > MaxMultiplier)
                MaxMultiplier = ActualMultiplier;
        }
    }
}
