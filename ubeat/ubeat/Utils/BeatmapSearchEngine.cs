using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.Utils
{
    public static class BeatmapSearchEngine
    {
        public static List<ubeat.Beatmap.Mapset> SearchBeatmaps(string keyword)
        {
            
            if (keyword == null)
                return Game1.Instance.AllBeatmaps;

            Console.WriteLine("KEYWORD: " + keyword);
            keyword = keyword.Trim().ToLower();

            if (keyword != "")
            {                
                // I don't sure 
                // Maybe this returns null
                // And world could be destroyed
                try
                {
                    var maps = new List<ubeat.Beatmap.Mapset>();

                    ubeat.Beatmap.Mapset[] mapsetArr = Game1.Instance.AllBeatmaps.ToArray<ubeat.Beatmap.Mapset>();

                    var searchedMaps = from Mapset in mapsetArr
                                       where Mapset.Title.ToLower().Contains(keyword)
                                       || Mapset.Creator.ToLower().Contains(keyword)
                                       || Mapset.Artist.ToLower().Contains(keyword)
                                       || Mapset.Tags.Contains<string>(keyword)
                                       select Mapset;


                    //var AllMapsWORepeats = searchedMaps.ToList().Distinct();

                    foreach (ubeat.Beatmap.Mapset mapset in searchedMaps)
                    {
                        maps.Add(mapset);
                    }

                    return maps; //Nice?
                }
                catch
                {
                    return new List<Beatmap.Mapset>(); //Length 0
                }
            }
            else
            {
                return Game1.Instance.AllBeatmaps; //genius c:
            }
        }
    }
}
