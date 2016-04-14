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

        public Mapset(string title, string artist, string creator)
            : base()
        {
            this.Title = title;
            this.Artist = artist;
            this.Creator = creator;
        }
    }
}
