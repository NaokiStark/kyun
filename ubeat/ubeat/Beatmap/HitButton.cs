﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ubeat.Beatmap;
using ubeat.GameScreen;
using Troschuetz.Random;
using Troschuetz.Random.Generators;
using Microsoft.Xna.Framework.Audio;
using ubeat.OsuUtils;
using ubeat.Score;
namespace ubeat.UIObjs
{
    public class HitButton : IHitObj
    {

        public Texture2D Texture { get; set; }
        public long PressedAt { get; set; }
        public bool isActive { get; set; }
        public long ActualPos { get; set; }
        public bool Died { get; set; }
        public bool hasAlredyPressed { get; set; }
        public int Y = 0;
        public int X = 0;
        Timer tmrApproachOpacity;
        float opacity=0;

        public void AddTexture(Texture2D texture)
        {
            this.Texture = texture;
        }
        
        #region GameEvents
        public void Start(long Position)
        {
            isActive = true;
            Died = false;
            tmrApproachOpacity = new Timer()
            {
                Interval = 2
            };
            tmrApproachOpacity.Tick += tmrApproachOpacity_Tick;
            ActualPos = Position;
            tmrApproachOpacity.Start();
            ActualPos = Position;
            PressedAt = 0;
            apo = null;
            hasAlredyPressed = false;
        }

        void tmrApproachOpacity_Tick(object sender, EventArgs e)
        {
            int appr = (int)(1950 - BeatmapContainer.ApproachRate * 150);
            float percentg = (float)(1f / ((float)appr)) * 100f;

            if (opacity + percentg > 1)
            {
                tmrApproachOpacity.Stop();
                return;
            }


            this.opacity = opacity + percentg;
        }
        public void Update(long Position)
        {
            
            if (isActive)
            {
                if (apo == null)
                {
                    apo = new ApproachObj(Grid.GetPositionFor(this.Location - 96), BeatmapContainer.ApproachRate,this.StartTime);
                    Grid.Instance.objs.Add(apo);

                }
                if (Grid.Instance.autoMode)
                {
                    if (Position > StartTime /*+ OsuBeatMap.rnd.Next(-(BeatmapContainer.Timing300), BeatmapContainer.Timing300)*/)
                    {
                        hasAlredyPressed = true;
                        isActive = false;
                        SoundEffectInstance ins = Game1.Instance.soundEffect.CreateInstance();
                        ins.Volume = Game1.Instance.player.Volume;
                        ins.Play();
                        PressedAt = (long)StartTime;
                    }
                }
                else
                {
                    if (Position > StartTime + BeatmapContainer.Timing50 && !hasAlredyPressed)
                    {
                        isActive = false;
                        PressedAt = Position;
                    }
                    if (Keyboard.GetState().IsKeyDown((Microsoft.Xna.Framework.Input.Keys)Location) && !hasAlredyPressed)
                    {
                        if (Position > StartTime - BeatmapContainer.Timing50)
                        {
                            hasAlredyPressed = true;
                            PressedAt = Position;
                            isActive = false;
                        }
                        return;
                    }
                    if (Keyboard.GetState().IsKeyUp((Microsoft.Xna.Framework.Input.Keys)Location) && hasAlredyPressed)
                    {
                        isActive = false;

                    }
                }
            }

        }
        ApproachObj apo;
        public void Render(long ccc, Vector2 position)
        {
            if (Died)
            {
               
                return;
            }
                        
            if (!isActive)
            {
                if (apo != null)
                {
                    apo.Died = true;
                    Grid.Instance.objs.Remove(apo);
                    apo = null;
                }
                Score.ScoreValue getScore = GetScoreValue();
                if ((int)getScore > (int)Score.ScoreValue.Miss)
                {
                    Grid.Instance.Health.Add(2f * (float)(((decimal)GetScoreValue() - (decimal)BeatmapContainer.OverallDifficulty)*100));
                    SoundEffectInstance ins = Game1.Instance.soundEffect.CreateInstance();
                    ins.Volume = Game1.Instance.player.Volume;
                    ins.Play();
                    Combo.Instance.Add();
                }
                else
                {
                    Combo.Instance.Miss();
                    Grid.Instance.Health.Substract(2 * BeatmapContainer.OverallDifficulty);
                }
                Grid.Instance.ScoreDispl.Add(((long)getScore * ((Combo.Instance.ActualMultiplier > 0) ? Combo.Instance.ActualMultiplier : 1))/2);
                Grid.Instance.objs.Add(new ScoreObj(GetScore(), new Vector2(position.X + (Texture.Bounds.Width / 2), position.Y + (Texture.Bounds.Height / 2))));

                Stop(ccc);
            }
            else
            {
                if (ccc >= StartTime - BeatmapContainer.Timing100)
                {
                    float opac = 0;
                    if (hasAlredyPressed)
                    {
                        opac = 1f;
                    }
                    else
                    {
                        opac = .6f;
                    }
                    Game1.Instance.spriteBatch.Draw(Game1.Instance.radiance, new Microsoft.Xna.Framework.Rectangle((int)position.X-2, (int)position.Y-2, Game1.Instance.radiance.Bounds.Width+2, Game1.Instance.radiance.Bounds.Height+2), Color.White * opac);

                }
                Game1.Instance.spriteBatch.Draw(this.Texture, new Microsoft.Xna.Framework.Rectangle((int)position.X, (int)position.Y, Texture.Bounds.Width, Texture.Bounds.Height), Color.White * opacity);

            }
        }
        public void Stop(long Position)
        {
            isActive = false;
            Died = true;
        }
        #endregion


        #region Score
        public float GetAccuracyPercentage()
        {
            float acc = 0;

            switch (GetScore())
            {
                case Score.ScoreType.Perfect:
                    acc = 100;
                    break;
                case Score.ScoreType.Excellent:
                    acc = 75;
                    break;
                case Score.ScoreType.Good:
                    acc = 35.2f;
                    break;
                case Score.ScoreType.Miss:
                    acc = 0;
                    break;
            }

            return acc;

        }
        public Score.ScoreType GetScore()
        {
            
            if (PressedAt >= StartTime - BeatmapContainer.Timing300 && PressedAt <= StartTime + BeatmapContainer.Timing300)
            {
                 //Perfect
                 return Score.ScoreType.Perfect;
            }
            else if(PressedAt >= StartTime - BeatmapContainer.Timing300 && PressedAt <= StartTime + BeatmapContainer.Timing100){

                return Score.ScoreType.Excellent;
            }
            else if (PressedAt >= StartTime - BeatmapContainer.Timing100 && PressedAt <= StartTime + BeatmapContainer.Timing100)
            {
                 //Excellent
                 return Score.ScoreType.Excellent;
            }

            else if (PressedAt >= StartTime - BeatmapContainer.Timing50 && PressedAt <= StartTime + BeatmapContainer.Timing50)
            {
                 //Bad
                 return Score.ScoreType.Good;
             }
             else
             {
                 return Score.ScoreType.Miss;
             }

        }
        public Score.ScoreValue GetScoreValue()
        {
            Score.ScoreValue sscv = Score.ScoreValue.Miss;
            switch (GetScore())
            {
                case Score.ScoreType.Perfect:
                    sscv = Score.ScoreValue.Perfect;
                    break;
                case Score.ScoreType.Excellent:
                    sscv = Score.ScoreValue.Excellent;
                    break;
                case Score.ScoreType.Good:
                    sscv = Score.ScoreValue.Good;
                    break;
                
            }
            return sscv;
        }
        #endregion

        public decimal StartTime
        {
            get;
            set;
        }

        public ubeatBeatMap BeatmapContainer
        {
            get;
            set;
        }

        public int Location
        {
            get;
            set;
        }

        public decimal EndTime
        {
            get;
            set;
        }
    }
}
