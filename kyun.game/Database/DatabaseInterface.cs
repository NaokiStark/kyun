using kyun.Beatmap;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.Database
{
    public class DatabaseInterface : IDisposable
    {
        LiteDatabase db;
        LiteCollection<Mapset> beatmaps;
        LiteCollection<ubeatBeatMap> BMPS;
        LiteCollection<Score.ScoreInfo> scores;

        static DatabaseInterface instance = null;

        public static DatabaseInterface Instance
        {
            get
            {
                if (instance == null)
                    instance = new DatabaseInterface();

                return instance;
            }
        }

        public DatabaseInterface()
        {
            var mapper = BsonMapper.Global;

            mapper.Entity<Mapset>()
                .DbRef(x => x.Beatmaps, "beatmaps");

            mapper.Entity<Score.ScoreInfo>().DbRef(x => x.Beatmap, "beatmaps");

            //db = new LiteDatabase("kyun.db");

            db = new LiteDatabase("filename=kyun.db;upgrade=true");
            
            beatmaps = (LiteCollection<Mapset>)db.GetCollection<Mapset>("mapsets");
            BMPS = (LiteCollection<ubeatBeatMap>)db.GetCollection<ubeatBeatMap>("beatmaps");

            scores = (LiteCollection<Score.ScoreInfo>)db.GetCollection<Score.ScoreInfo>("scores");
            
        }

        public List<Mapset> GetBeatmaps()
        {
            return beatmaps.Include(x => x.Beatmaps).FindAll().ToList();
        }

        public List<ubeatBeatMap> GetAllBeatmaps()
        {
            return BMPS.FindAll().ToList();
        }

        public List<Score.ScoreInfo> GetScoresFor(ubeatBeatMap beatmap)
        {
            return scores.Find(x => x.BeatmapName == beatmap.Title
                                 && x.BeatmapArtist == beatmap.Artist
                                 && x.BeatmapDiff == beatmap.Version).ToList();
        }

        public void DeleteBeatmaps()
        {
            beatmaps.DeleteAll();
            BMPS.DeleteAll();
        }

        public async void SaveScore(Score.ScoreInfo scoreInfo)
        {
            await Task.Run(() => scores.Insert(scoreInfo));
        }

        public void SaveBeatmaps(List<Mapset> cbeatmaps)
        {
            beatmaps.DeleteAll();
            BMPS.DeleteAll();

            foreach (Mapset mp in cbeatmaps)
            {
                //BMPS.InsertBulk(mp.Beatmaps);
                try {
                    BMPS.Insert(mp.Beatmaps);
                }
                catch { }
                
                
            }
            //beatmaps.InsertBulk(cbeatmaps);
            beatmaps.Insert(cbeatmaps);
        }

        public void InsertMapset(Mapset mpset)
        {
            try
            {
                //BMPS.InsertBulk(mpset.Beatmaps);
                BMPS.Insert(mpset.Beatmaps);
            }
            catch
            {

            }

            try
            {
                beatmaps.Insert(mpset);
            }
            catch
            {

            }
        }

        ~DatabaseInterface()
        {

            try { db?.Dispose(); } catch { }
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}
