﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ubeat.Beatmap;
using ubeat.Score;

namespace ubeat.GameScreen
{
    public partial class ScoreScreen
    {
        public ScoreScreen()
        {
            LoadInterface();

        }

        void ScoreScreen_OnLoad(object sender, EventArgs e)
        {

            ChangeBeatmapDisplay(Game1.Instance.SelectedBeatmap);
            this.lblTitleDesc.Text = string.Format("{0} - {1} [{2}]", Grid.Instance.bemap.Artist, Grid.Instance.bemap.Title, Grid.Instance.bemap.Version);
            int perfect = 0;
            int excellent = 0;
            int good = 0;
            int bad = 0;
            int miss = 0;
            ulong total = 0;
            float acc = 0;
            for (int a = 0; a < Grid.Instance.bemap.HitObjects.Count; a++)
            {
                IHitObj ho = Grid.Instance.bemap.HitObjects[a];
                switch (ho.GetScore())
                {
                    case Score.ScoreType.Perfect:
                        perfect++;
                        break;
                    case Score.ScoreType.Excellent:
                        excellent++;
                        break;
                    case Score.ScoreType.Good:
                        good++;
                        break;
                    case Score.ScoreType.Miss:
                        miss++;
                        break;
                }
                //total += (ulong)((long)ho.GetScoreValue() * Combo.Instance.ActualMultiplier);
                acc += ho.GetAccuracyPercentage();
            }

                lblPerfect.Text = "Perfect: " + perfect.ToString();perfect.ToString();
                lblExcellent.Text = "Excellent: " + excellent.ToString();
                lblGood.Text = "Good: " + good.ToString();
                lblCombo.Text = "Max. Combo: " + Combo.Instance.MaxMultiplier.ToString();
                lblMiss.Text = "Miss: " + miss.ToString();
                lblAccuracy.Text = "Accuracy: " + Math.Round((float)acc / (float)Grid.Instance.bemap.HitObjects.Count, 2).ToString() + "%";
                lblScore.Text = "Score: " + Grid.Instance.ScoreDispl.TotalScore.ToString();
                                
            
        }

        void ChangeBeatmapDisplay(ubeatBeatMap bm)
        {
            if (Game1.Instance.SelectedBeatmap.SongPath != bm.SongPath)
            {
                Game1.Instance.Player.Play(bm.SongPath);
                Game1.Instance.Player.soundOut.Volume = Game1.Instance.GeneralVolume;
            }

            Game1.Instance.SelectedBeatmap = bm;

            try
            {
                FileStream bgFstr = new FileStream(bm.Background, FileMode.Open);
                Background = Texture2D.FromStream(Game1.Instance.GraphicsDevice, bgFstr);
                bgFstr.Close();
            }
            catch
            {
                Logger.Instance.Warn("BACKGROUND NOT FOUND!!");
            }
        }
    }
}
