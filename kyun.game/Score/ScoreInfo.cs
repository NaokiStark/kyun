using kyun.Beatmap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.Score
{
    public class ScoreInfo
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int Combo { get; set; }
        public string Username { get; set; }
        public long UserId { get; set; }
        public string BeatmapName { get; set; }
        public string BeatmapArtist { get; set; }
        public string BeatmapDiff { get; set; }
        public ubeatBeatMap Beatmap { get; set; }

        //ToDo: make this better

        public string RawMovements { get; set; }
    }
}
