using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using kyun.UIObjs;
using kyun.Beatmap;
using kyun.GameModes;
using kyun.GameModes.Classic;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

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

        /// <summary>
        /// Mouse Positions in osu mode
        /// </summary>
        public List<KeyValuePair<long, Vector2>> MousePositions { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        public int Mode { get; set; }

        /// <summary>
        /// Build replay for ubeat mode
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="Beatmap"></param>
        /// <param name="classic"></param>
        /// <returns></returns>
        public static Replay Build(List<HitBase> hits, IBeatmap Beatmap, bool classic = true)
        {
            var tmp = new Replay();
            tmp.Hits = new List<ReplayObject>();
            foreach (HitBase h in hits)
            {
                if (classic)
                {
                    HitSingle hitSingle = (HitSingle)h;
                    var ho = new ReplayObject()
                    {
                        PressedAt = hitSingle.pressedTime,
                        LeaveAt = (h is GameModes.Classic.HitHolder) ? ((GameModes.Classic.HitHolder)h).leaveTime : 0
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


            if (game.NikuClient.NikuClientApi.User != null)
            {
                if (!game.NikuClient.NikuClientApi.isLogout)
                {
                    tmp.Username = game.NikuClient.NikuClientApi.User.Username;
                }
            }
            return tmp;
        }

        /// <summary>
        /// Build replay for CatchIt mode
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="Beatmap"></param>
        /// <param name="classic"></param>
        /// <returns></returns>
        public static Replay Build(List<ReplayObject> hits, IBeatmap Beatmap, bool classic = true)
        {


            var tmp = new Replay();
            tmp.Hits = hits;
            tmp.Mode = (classic) ? 1 : 0;

            if (game.NikuClient.NikuClientApi.User != null)
            {
                if (!game.NikuClient.NikuClientApi.isLogout)
                {
                    tmp.Username = game.NikuClient.NikuClientApi.User.Username;
                }
            }
            return tmp;
        }

        /// <summary>
        /// Build replay for osu Mode (bad)
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="beatmap"></param>
        /// <param name="mouseMoves"></param>
        /// <returns></returns>
        public static Replay Build(List<ReplayObject> hits, IBeatmap beatmap, List<KeyValuePair<long, Vector2>> mouseMoves)
        {
            var tmp = new Replay();
            tmp.Hits = hits;
            tmp.MousePositions = mouseMoves;


            if (game.NikuClient.NikuClientApi.User != null)
            {
                if (!game.NikuClient.NikuClientApi.isLogout)
                {
                    tmp.Username = game.NikuClient.NikuClientApi.User.Username;
                }
            }
            return tmp;
        }

        /// <summary>
        /// Build replay for osu Mode (Good)
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="beatmap"></param>
        /// <param name="mouseMoves"></param>
        /// <returns></returns>
        public static Replay Build(List<HitBase> hitObjects, IBeatmap beatmap, List<KeyValuePair<long, Vector2>> recordedMousePositions)
        {
            var tmp = new Replay();
            tmp.Hits = new List<ReplayObject>();
            tmp.Mode = 2;
            foreach (HitBase h in hitObjects)
            {
                ReplayObject ho = null;
                if (h is kyun.GameModes.OsuMode.HitHolder)
                {
                    ho = new ReplayObject()
                    {
                        PressedAt = ((kyun.GameModes.OsuMode.HitSingle)h).pressedTime,
                        LeaveAt = 0
                    };
                }
                else
                {
                    ho = new ReplayHitHolder()
                    {
                        PressedAt = ((kyun.GameModes.OsuMode.HitSingle)h).pressedTime,
                        LeaveAt = 0
                    };
                }


                tmp.Hits.Add(ho);
            }

            if (game.NikuClient.NikuClientApi.User != null)
            {
                if (!game.NikuClient.NikuClientApi.isLogout)
                {
                    tmp.Username = game.NikuClient.NikuClientApi.User.Username;
                }
            }

            tmp.MousePositions = recordedMousePositions.Distinct(new CurDist()).ToList();
            return tmp;
        }

        public static Replay ReBuild(string data)
        {
            var tmp = new Replay();

            var hits = new List<ReplayObject>();

            var mouse = new List<KeyValuePair<long, Vector2>>();

            List<string> d = data.Split('\n').ToList();

            string kr = string.Join("", data.Split(new string[] { "@k:\n" }, StringSplitOptions.None)[1]);

            string[] k = kr.Split('\n');

            foreach (string ki in k)
            {
                string[] ks = ki.Split(':');

                hits.Add(new ReplayObject
                {
                    PressedAt = int.Parse(ks[0]),
                    LeaveAt = int.Parse(ks[1])
                });
            }

            string mr = string.Join("", data.Split(new string[] { "@m:\n" }, StringSplitOptions.None)[1]);

            string[] m = kr.Split('\n');

            foreach (string mi in m)
            {
                string[] mk = mi.Split(':')[1].Split('|');

                mouse.Add(new KeyValuePair<long, Vector2>(
                    long.Parse(mi.Split(':')[0]),
                    new Vector2(float.Parse(mk[0]), float.Parse(mk[1]))
                ));
            }

            tmp.Hits = hits;
            tmp.MousePositions = mouse;

            return tmp;
        }

        public new async Task<string> ToString()
        {
            return await Task.Run(() =>
            {
                string main = $"modetype:{Mode}";

                string raw = "m:0";
                raw = "@k:\n";

                foreach (ReplayObject @object in Hits)
                {
                    raw += $"{@object.PressedAt}:{@object.LeaveAt}\n";
                }

                raw = "@m:\n";

                if (MousePositions != null)
                {
                    foreach (KeyValuePair<long, Vector2> @object in MousePositions)
                    {
                        raw += $"{@object.Key}:{@object.Value.X}|{@object.Value.Y}\n";
                    }
                }

                return raw;
            });
        }
    }

    public class CurDist : IEqualityComparer<KeyValuePair<long, Vector2>>
    {
        public bool Equals(KeyValuePair<long, Vector2> x, KeyValuePair<long, Vector2> y)
        {
            if (x.Key == y.Key)
                return true;

            if (x.Value.X == y.Value.X && x.Value.Y == y.Value.Y)
                return true;

            return false;
        }

        public int GetHashCode(KeyValuePair<long, Vector2> obj)
        {
            return obj.Key.GetHashCode();
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
