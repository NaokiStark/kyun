﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.Beatmap
{
    public interface IBeatmap
    {

        string FilePath { get; set; } //Mod: loads objects when beatmap is being played

        string Title { get; set; }
        string Artist { get; set; }
        string Creator { get; set; }
        string Version { get; set; }
        List<string> Tags { get; set; }
        float BPM { get; set; }
        long MSPB { get; set; }
        List<IHitObj> HitObjects { get; set; }
        string SongPath { get; set; }
        float HPDrainRate { get; set; }
        float OverallDifficulty { get; set; }
        float ApproachRate { get; set; }
        string Background { get; set; }
        string Video { get; set; }
        long VideoStartUp { get; set; }
        int SleepTime { get; set; }
        List<Break> Breaks { get; set; }
        int Timing300 { get; }
        int Timing100 { get; }
        int Timing50 { get; }
    }


}