using System;
using kyun.Beatmap;
using kyun.GameScreen;

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

        public static IScreenGameMode Instance;

        public GameModeScreenBase(string GameModeName) 
            : base(GameModeName)
        {
            
        }

        public virtual void Play(IBeatmap Beatmap, GameMod GameMods = GameMod.None)
        {
           
        }
    }
}
