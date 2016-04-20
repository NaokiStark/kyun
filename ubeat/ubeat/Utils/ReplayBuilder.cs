using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ubeat.UIObjs;

namespace ubeat.Utils
{
    public class ReplayBuilder
    {
        public static bool SaveReplay(Beatmap.ubeatBeatMap BeatmapComplete)
        {
            try
            {
                string fDir = Path.Combine(System.Windows.Forms.Application.StartupPath, "Replays");
                string fName = Path.Combine(fDir, Logger.ConvertToTimestamp(DateTime.Now) + "-" + BeatmapComplete.Artist + "-" + BeatmapComplete.Title + " [" + BeatmapComplete.Version + "].ubr");
                if (!(new DirectoryInfo(fDir).Exists))
                {
                    new DirectoryInfo(fDir).Create();
                }

                StreamWriter fl = new StreamWriter(fName);

                fl.WriteLine(BeatmapComplete.Artist + BeatmapComplete.Title + BeatmapComplete.Version + BeatmapComplete.Creator);
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
            }

            return true;
        }
    }
}
