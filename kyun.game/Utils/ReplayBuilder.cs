using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using kyun.UIObjs;
using kyun.Beatmap;
using kyun.GameModes;
using kyun.GameModes.Classic;

namespace kyun.Utils
{
    public class ReplayBuilder
    {
        public static bool SaveReplay(List<HitBase> hits, ubeatBeatMap BeatmapComplete)
        {
            //I NEED MONEY
            /*
            try
            {
                string fDir = Path.Combine(System.Windows.Forms.Application.StartupPath, "Replays");
                string fName = Path.Combine(fDir, Logger.ConvertToTimestamp(DateTime.Now) + "-" + BeatmapComplete.Artist + "-" + BeatmapComplete.Title + " [" + BeatmapComplete.Version + "].ubr");
                if (!(new DirectoryInfo(fDir).Exists))
                {
                    new DirectoryInfo(fDir).Create();
                }

                StreamWriter fl = new StreamWriter(fName);

                fl.WriteLine(BeatmapComplete.Artist + "|" + BeatmapComplete.Title + "|" + BeatmapComplete.Version + "|" + BeatmapComplete.Creator);
                fl.WriteLine("=");

                for (int a = 0; a < BeatmapComplete.HitObjects.Count; a++)
                {
                    if (BeatmapComplete.HitObjects[a] is HitButton)
                    {
                        fl.WriteLine(string.Format("B:{0}", ((HitButton)BeatmapComplete.HitObjects[a]).PressedAt));
                    }
                    else if (BeatmapComplete.HitObjects[a] is HitHolder)
                    {
                        fl.WriteLine(string.Format("H:{0}:{1}", ((HitHolder)BeatmapComplete.HitObjects[a]).PressedAt, ((HitHolder)BeatmapComplete.HitObjects[a]).LeaveAt));
                    }

                }
                fl.Flush();

                fl.Close();
            }
            catch
            {
                Logger.Instance.Warn("I can't save this replay");
                return false;
            }*/

            return true;
        }
    }

    //REPLAY OBJECT XD

    // This needs refactoring later
    // I'm doing this shit cuz I'm a fucking faggot person who have autism

    public class Replay
    {
        /// <summary>
        /// Obviusly, Hits
        /// </summary>
        public List<ReplayObject> Hits { get; set; }

        /// <summary>
        /// Fucking Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Fucking artist
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// Fucking Version of beatmap
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// I think I will need it sometime
        /// </summary>
        public int Id { get; set; }

        public static Replay Build(List<HitBase> hits, IBeatmap Beatmap, bool classic = true)
        {
            var tmp = new Replay();
            tmp.Hits = new List<ReplayObject>();
            foreach(HitBase h in hits)
            {
                if (classic)
                {
                    HitSingle hitSingle = (HitSingle)h;
                    var ho = new ReplayObject()
                    {
                        PressedAt = hitSingle.pressedTime,
                        LeaveAt = (h is GameModes.Classic.HitHolder) ?((GameModes.Classic.HitHolder)h).leaveTime : 0
                    };

                    tmp.Hits.Add(ho);
                }
                else
                {
                    game.GameModes.CatchIt.HitObject hitobj = (game.GameModes.CatchIt.HitObject)h;

                    var ho = new ReplayObject()
                    {
                        PressedAt = hitobj.PositionAtCollision,
                        LeaveAt = hitobj.CollisionAt
                    };
                    tmp.Hits.Add(ho);
                }                
            }
            return tmp;
        }

        public static Replay Build(List<ReplayObject> hits, IBeatmap Beatmap, bool classic = true)
        {
            var tmp = new Replay();
            tmp.Hits = hits;
            return tmp;
        }
    }

    public class ReplayObject
    {
        /// <summary>
        /// Hit pressed at fucking specified time
        /// </summary>
        public long PressedAt { get; set; }
        public long LeaveAt { get; set; }
    }

    public class ReplayHitHolder : ReplayObject
    {
        /// <summary>
        /// HitHolder key-up. Fuck.
        /// </summary>
       
        //shit
    }

}
