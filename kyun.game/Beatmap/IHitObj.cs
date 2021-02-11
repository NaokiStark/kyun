using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.UIObjs;

namespace kyun.Beatmap
{
    public interface IHitObj
    {
        decimal StartTime { get; set; }
        IBeatmap BeatmapContainer { get; set; }
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
        int HitSound { get; set; }
        Score.ScoreValue GetScoreValue();
        Score.ScoreType GetScore();
        float GetAccuracyPercentage();
        OldApproachObj apo { get; set; }
        Vector2 OsuLocation { get; set; }
        float MsPerBeat { get; set; }

        /* fake kyun obj*/

        bool isFakeOBj { get; set; }
    }
}
