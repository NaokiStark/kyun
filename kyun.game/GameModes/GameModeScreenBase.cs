using System;
using System.Collections.Generic;
using kyun.Beatmap;
using kyun.GameModes.Classic;
using kyun.GameScreen;
using kyun.Score;

namespace kyun.GameModes
{
    /// <summary>
    /// Base for GameMode Screen
    /// </summary>
    public class GameModeScreenBase : ScreenBase, IScreenGameMode
    {
        public IBeatmap Beatmap;

        

        internal GameMod gameMod { get; set; }

        public bool InGame { get; set; }

        public long GamePosition { get; set; }
        public List<HitBase> HitObjects { get; set; }

        public static IScreenGameMode Instance;
        public ScoreDisplay _scoreDisplay;
        internal bool onBreak;

        public GameModeScreenBase(string GameModeName) 
            : base(GameModeName)
        {
            
        }

        public virtual void Play(IBeatmap Beatmap, GameMod GameMods = GameMod.None)
        {
           
        }

        internal virtual GameModeScreenBase GetInstance()
        {
            return this;
        }
    }
}
