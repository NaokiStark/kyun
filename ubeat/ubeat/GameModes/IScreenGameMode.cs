using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.Beatmap;

namespace ubeat.GameModes
{
    public interface IScreenGameMode
    {
        bool InGame { get; set; }

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
