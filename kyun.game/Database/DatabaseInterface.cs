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

            db = new LiteDatabase("kyun.db");

            beatmaps = db.GetCollection<Mapset>("mapsets");
            BMPS = db.GetCollection<ubeatBeatMap>("beatmaps");

            scores = db.GetCollection<Score.ScoreInfo>("scores");
            
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
            beatmaps.Delete(x => true);
            BMPS.Delete(x => true);
        }

        public async void SaveScore(Score.ScoreInfo scoreInfo)
        {
            await Task.Run(() => scores.Insert(scoreInfo));
        }

        public void SaveBeatmaps(List<Mapset> cbeatmaps)
        {
            beatmaps.Delete(x => true);
            BMPS.Delete(x => true);

            foreach (Mapset mp in cbeatmaps)
            {
                BMPS.InsertBulk(mp.Beatmaps);
                
            }
            beatmaps.InsertBulk(cbeatmaps);

        }

        public void InsertMapset(Mapset mpset)
        {
            try
            {
                BMPS.InsertBulk(mpset.Beatmaps);
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
            db?.Dispose();
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}
