using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace kyun.Utils
{
    public class SongInfo
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public string Song { get; set; }

        public static SongInfo FromFile(string filename)
        {
            JObject json = null;

            using (StreamReader sr = new StreamReader(File.Open(filename, FileMode.Open)))
            {
                json = JObject.Parse(sr.ReadToEnd()); //Another thread will deal with this
            }

            if (json == null)
                return null;

            return new SongInfo()
            {
                Artist = (string)json["artist"],
                Title = (string)json["title"],
                Cover = Path.Combine(Path.GetDirectoryName(filename), (string)json["cover"]),
                Song = Path.Combine(Path.GetDirectoryName(filename), (string)json["song"])
            };
        }
    }
}
