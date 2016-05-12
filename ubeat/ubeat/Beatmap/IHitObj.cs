using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.UIObjs;

namespace ubeat.Beatmap
{
    public interface IHitObj
    {
        decimal StartTime { get; set; }
        ubeat.Beatmap.ubeatBeatMap BeatmapContainer { get; set; }
        int Location { get; set; }
        void Start(long Position);
        void Update(long Position, Vector2 position);
        void Render(long ccc, Vector2 position);
        bool Died { get; set; }
        void AddTexture(Texture2D texture);
        void Reset();
        decimal EndTime { get; set; }
        Texture2D Texture { get; set; }
        long PressedAt { get; set; }
        Score.ScoreValue GetScoreValue();
        Score.ScoreType GetScore();
        float GetAccuracyPercentage();
        ApproachObj apo { get; set; }
    }
}
