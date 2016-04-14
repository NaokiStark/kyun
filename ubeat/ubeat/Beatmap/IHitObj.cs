using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.Beatmap
{
    public interface IHitObj
    {
         decimal StartTime { get; set; }
         ubeat.Beatmap.ubeatBeatMap BeatmapContainer { get; set; }
         int Location { get; set; }
        void Start(long Position);
        void Update(long Position);
        void Render(long ccc, Vector2 position);
        bool Died { get; set; }
        void AddTexture(Texture2D texture);
         decimal EndTime { get; set; }
         Texture2D Texture { get; set; }
         
        Score.ScoreValue GetScoreValue();
        Score.ScoreType GetScore();
        float GetAccuracyPercentage();
    }
}
