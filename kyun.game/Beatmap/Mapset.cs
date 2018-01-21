using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.Beatmap
{
    public class Mapset : List<ubeatBeatMap>
    {
        public Mapset(string title, string artist, string creator, List<string> tags)
            : base()
        {
            Title = title;
            Artist = artist;
            Creator = creator;

            if (tags == null)
                Tags = new string[]{""};
            else
                Tags = tags.ToArray<string>();
        }

        public Mapset() : base()
        {
            Title = Creator = string.Empty;
            Artist = "You have no beatmaps";
            Tags = new string[]{ "" };
        }

        public static Mapset OrderByDiff(Mapset mp)
        {
            var ss = mp.OrderBy(x => x.OverallDifficulty).ToList();
            mp.Clear();

            foreach (ubeatBeatMap gg in ss)
                mp.Add(gg);

            return mp;
        }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string Creator { get; set; }
        public string[] Tags { get; set; }
    }
}
