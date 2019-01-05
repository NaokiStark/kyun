using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.Beatmap
{
    public class Mapset
    {
        

        public Mapset(string title, string artist, string creator, List<string> tags)
        {
            Beatmaps = new List<ubeatBeatMap>();

            Title = title;
            Artist = artist;
            Creator = creator;

            if (tags == null)
                Tags = new string[]{""};
            else
                Tags = tags.ToArray<string>();
        }

        public Mapset() 
        {
            Beatmaps = new List<ubeatBeatMap>();
            Title = Creator = string.Empty;
            Artist = "You have no beatmaps";
            Tags = new string[]{ "" };
        }

        public static Mapset OrderByDiff(Mapset mp)
        {
            var ss = mp.Beatmaps.OrderBy(x => x.OverallDifficulty).ToList();
            mp.Beatmaps.Clear();

            foreach (ubeatBeatMap gg in ss)
                mp.Beatmaps.Add(gg);

            return mp;
        }

        public void Add(ubeatBeatMap bm)
        {
            Beatmaps.Add(bm);
        }


        public int Count
        {
            get
            {
                return Beatmaps.Count;
            }
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Creator { get; set; }
        public string[] Tags { get; set; }

        public List<ubeatBeatMap> Beatmaps { get; set; }
    }
}
