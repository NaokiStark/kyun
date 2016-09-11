using System;
using System.Collections.Generic;
using System.Linq;

namespace ubeat.Utils
{
    public static class BeatmapSearchEngine
    {
        public static List<Beatmap.Mapset> SearchBeatmaps(string keyword)
        {
            if (keyword == null)
                return Game1.Instance.AllBeatmaps;

            Logger.Instance.Info("Keyword: {0}", keyword);

            keyword = keyword.Trim().ToLower();

            if (keyword != "")
            {                
                // I don't sure 
                // Maybe this returns null
                // And world could be destroyed
                try
                {
                    var maps = new List<Beatmap.Mapset>();

                    Beatmap.Mapset[] mapsetArr = Game1.Instance.AllBeatmaps.ToArray<ubeat.Beatmap.Mapset>();

                    var searchedMaps = from Mapset in mapsetArr
                                       where Mapset.Title.ToLower().Contains(keyword)
                                       || Mapset.Creator.ToLower().Contains(keyword)
                                       || Mapset.Artist.ToLower().Contains(keyword)
                                       || Mapset.Tags.Contains(keyword)
                                       select Mapset;


                    //var AllMapsWORepeats = searchedMaps.ToList().Distinct();

                    foreach (Beatmap.Mapset mapset in searchedMaps)
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
