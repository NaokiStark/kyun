using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Beatmap;

namespace kyun.GameModes
{
    public interface IScreenGameMode
    {
        bool InGame { get; set; }
        long GamePosition { get; set; }

        void Play(IBeatmap Beatmap, GameMod GameMods = GameMod.None);
    }

    /// <summary>
    /// GameMods
    /// </summary>
    public enum GameMod
    {
        None = 0,
        Auto = 1,
        NoFail = 2
    }
}
