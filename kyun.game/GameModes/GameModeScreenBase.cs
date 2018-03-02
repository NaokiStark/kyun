using System;
using System.Collections.Generic;
using kyun.Beatmap;
using kyun.GameModes.Classic;
using kyun.GameScreen;
using kyun.Score;
using kyun.Utils;
using kyun.GameScreen.UI;
using Microsoft.Xna.Framework;

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
        public Label replayLabel;

        public GameModeScreenBase(string GameModeName) 
            : base(GameModeName)
        {
            var mes = SpritesContent.Instance.TitleFont.MeasureString("REPLAY");
            replayLabel = new Label()
            {
                Text = "REPLAY",
                Font = SpritesContent.Instance.TitleFont,
                Visible = false,
                Centered = true,
                Position = new Vector2(ActualScreenMode.Width / 2/* - (mes.X / 2)*/, 100)
            };

            
        }

        public virtual void Play(IBeatmap Beatmap, GameMod GameMods = GameMod.None)
        {
           
        }

        internal virtual GameModeScreenBase GetInstance()
        {
            return this;
        }

        public virtual void Play(IBeatmap beatmap, GameMod gameMod, Replay rpl)
        {
            
        }
    }
}
