using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.Beatmap
{
    public class Mapset:List<ubeatBeatMap>
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Creator { get; set; }
        public string[] Tags { get; set; }
        public Mapset(string title, string artist, string creator,List<string> tags)
            : base()
        {
            this.Title = title;
            this.Artist = artist;
            this.Creator = creator;
            if (tags == null)
                this.Tags = new string[]{""};
            else
                this.Tags = tags.ToArray<string>();
        }
        public static Mapset OrderByDiff(Mapset mp)
        {
            var ss = mp.OrderBy(x => x.OverallDifficulty).ToList();
            mp.Clear();
            foreach (ubeatBeatMap gg in ss)
            {
                mp.Add(gg);
            }
            return mp;
        }
    }
}
