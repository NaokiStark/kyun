using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.Score
{
    public class Combo
    {

        public static Combo Instance { 
            get {
                if (instance == null)
                    return new Combo();
                else
                    return instance;
            }
        }

        static Combo instance = null;

        public Combo()
        {
            instance = this;
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
