using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Troschuetz.Random;
using Troschuetz.Random.Generators;
using ubeat.Beatmap;
using ubeat.GameScreen;
using ubeat.OsuUtils;
using ubeat.Score;

namespace ubeat.UIObjs
{
    public class HitHolder : IHitObj
    {
        #region PublicVars
        public Texture2D Texture { get; set; }
        public long PressedAt { get; set; }
        public long LeaveAt { get; set; }
        public bool isActive { get; set; }
        public long ActualPos { get; set; }
        public bool Died { get; set; }
        public int Y = 0;
        public int X = 0;
        public bool hasAlredyPressed { get; set; }
        public decimal Length { get; set; }
        public System.Windows.Forms.Timer tmrApproachOpacity { get; set; }
        public float opacity { get; set; }
        public ApproachObj apo { get; set; }
        public decimal StartTime { get; set; }
        public ubeatBeatMap BeatmapContainer { get; set; }
        public int Location { get; set; }
        public decimal EndTime { get; set; }
        #endregion

        #region PrivateVars
        SoundEffectInstance holdFld;
        bool ticked;
        bool isFilling;
        #endregion

        #region Texture
        public void AddTexture(Texture2D texture)
        {
            this.Texture = texture;
        }
        #endregion  

        #region GameEvents
        public void Start(long Position)
        {
            tmrApproachOpacity = null;
            tmrApproachOpacity = new System.Windows.Forms.Timer()
            {
                Interval = 2
            };
            tmrApproachOpacity.Start();
            tmrApproachOpacity.Tick += tmrApproachOpacity_Tick;
            isActive = true;
            Died = false;
            isFilling = false;
            ActualPos = Position;
            PressedAt = 0;
            LeaveAt = 0;
            hasAlredyPressed = false;
            apo = null;

        }

        public void Reset()
        {
            isActive = false;
            Died = true;
            isFilling = false;
            ActualPos = 0;
            PressedAt = 0;
            LeaveAt = 0;
            hasAlredyPressed = false;
            apo = null;
        }

        private void tmrApproachOpacity_Tick(object sender, EventArgs e)
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

        public void Update(long Position,Vector2 p)
        {
            if (holdFld != null)
            {
                if (isFilling && Grid.Instance.Paused)
                {
                    if (holdFld.State == SoundState.Playing)
                        holdFld.Pause();

                }
                if (isFilling && !Grid.Instance.Paused)
                {
                    if (holdFld.State != SoundState.Playing)
                        holdFld.Play();
                }
                if (!isFilling && holdFld.State == SoundState.Playing)
                    holdFld.Stop();
                if (!isActive && holdFld.State == SoundState.Playing)
                    holdFld.Stop();
            }

            if (isActive)
            {
                if (apo == null)
                {
                    apo = new ApproachObj(Grid.GetPositionFor(this.Location - 96), BeatmapContainer.ApproachRate, this.StartTime);
                    Grid.Instance.objs.Add(apo);
                }
                if (Grid.Instance.autoMode)
                {
                    if (Position >= StartTime /*+ OsuBeatMap.rnd.Next(-(BeatmapContainer.Timing300), BeatmapContainer.Timing300) */&& !hasAlredyPressed)
                    {
                        PressedAt = (long)StartTime;
                        
                        SoundEffectInstance ins = Game1.Instance.HolderHit.CreateInstance();
                        ins.Volume = Game1.Instance.GeneralVolume;
                        ins.Play();
                        hasAlredyPressed = true;
                        isFilling = true;
                        holdFld = Game1.Instance.HitHolderFilling.CreateInstance();
                        holdFld.Volume = Game1.Instance.GeneralVolume;
                        holdFld.IsLooped = true;
                        holdFld.Play();
                    }
                    else if (Position >= EndTime)
                    {
                        LeaveAt = Position;

                        isActive = false;
                        /*
                        SoundEffectInstance ins = Game1.Instance.HolderHit.CreateInstance();
                        ins.Volume = Game1.Instance.GeneralVolume;
                        ins.Play();*/
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
                            isFilling = true;
                            holdFld = Game1.Instance.HitHolderFilling.CreateInstance();
                            holdFld.Volume = Game1.Instance.GeneralVolume;
                            holdFld.IsLooped = true;
                            holdFld.Play();
                        }
                        return;
                    }
                    if (Keyboard.GetState().IsKeyUp((Microsoft.Xna.Framework.Input.Keys)Location) && hasAlredyPressed)
                    {
                        isFilling = false;
                        LeaveAt = Position;
                        isActive = false;

                    }
                    if (Position > EndTime)
                    {
                        isFilling = false;
                        LeaveAt = (long)EndTime; //Easy Easy Easy Easy Easy Easy Easy Easy Easy
                        isActive = false;
                    }
                }
                if (Position > (this.Length / 2 + StartTime) && !ticked)
                {
                    ticked = true;
                    SoundEffectInstance TickSnd = Game1.Instance.HolderTick.CreateInstance();
                    Grid.Instance.Health.Add(.5f);
                    Combo.Instance.Add();
                    TickSnd.Volume = Game1.Instance.GeneralVolume;
                    TickSnd.Play();
                }
            }
            else
            {
                if (apo != null)
                {
                    apo.Died = true;
                    Grid.Instance.objs.Remove(apo);
                    apo = null;
                }

                if (holdFld != null)
                    holdFld.Stop();
                Score.ScoreValue score = GetScoreValue();
                if ((int)score > (int)Score.ScoreValue.Miss)
                {
                    Grid.Instance.FailsCount = 0;
                    float healthToAdd = (BeatmapContainer.OverallDifficulty / 2) + Math.Abs(this.LeaveAt - PressedAt) / 100;
                    Grid.Instance.Health.Add(healthToAdd);
                    SoundEffectInstance ins = Game1.Instance.HolderHit.CreateInstance();
                    ins.Volume = Game1.Instance.GeneralVolume;
                    ins.Play();
                    Combo.Instance.Add();
                }
                else
                {
                    Grid.Instance.FailsCount++;
                    if (Combo.Instance.ActualMultiplier > 10)
                    {
                        SoundEffectInstance ins = Game1.Instance.ComboBreak.CreateInstance();
                        ins.Volume = Game1.Instance.GeneralVolume;
                        ins.Play();
                    }
                    Combo.Instance.Miss();
                    Grid.Instance.Health.Substract((4 * BeatmapContainer.OverallDifficulty) * Grid.Instance.FailsCount);
                }

                Grid.Instance.ScoreDispl.Add(((long)score * ((Combo.Instance.ActualMultiplier > 0) ? Combo.Instance.ActualMultiplier : 1)) / 2);

                Grid.Instance.objs.Add(new ScoreObj(GetScore(), new Vector2(p.X + (Texture.Bounds.Width / 2), p.Y + (Texture.Bounds.Height / 2))));

                Stop(Position);
            }
        }
        public void Render(long ccc,Vector2 position)
        {
            if (Died)
            {
                
                return;
            }
            
            
            if (!isActive)
            {
                
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
                    Game1.Instance.spriteBatch.Draw(Game1.Instance.radiance, new Microsoft.Xna.Framework.Rectangle((int)position.X - 4, (int)position.Y - 4, Game1.Instance.radiance.Bounds.Width + 4, Game1.Instance.radiance.Bounds.Height + 4), Color.White * opac);

                }
                Game1.Instance.spriteBatch.Draw(this.Texture, new Microsoft.Xna.Framework.Rectangle((int)position.X, (int)position.Y, Texture.Bounds.Width, Texture.Bounds.Height), Color.White * opacity);
                //Game1.Instance.spriteBatch.DrawString(Game1.Instance.fontDefault, (Location - 96).ToString(), new Vector2(position.X + (Texture.Bounds.Width / 2), position.Y + (Texture.Bounds.Height / 2)), Color.Black * opacity);
                if (isFilling)
                {
                    float stime = ccc - (float)StartTime;
                    float gtime = stime / (float)Length;
                    float percentgg = gtime * Game1.Instance.HolderFillDeff.Bounds.Height;

                    var angle = (float)Math.PI;
                    Game1.Instance.spriteBatch.Draw(Game1.Instance.HolderFillDeff,
                        new Microsoft.Xna.Framework.Rectangle((int)position.X + Game1.Instance.HolderFillDeff.Bounds.Height + 1, (int)position.Y + (Game1.Instance.HolderFillDeff.Bounds.Height), Game1.Instance.HolderFillDeff.Bounds.Width, (int)percentgg),
                        new Rectangle(0, 0, Game1.Instance.HolderFillDeff.Bounds.Width, (int)percentgg),
                        Color.White,
                        angle,
                        new Vector2(0, 0),
                        Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                        0);
                }
               
            }
        }
        public void Stop(long Position)
        {
            isFilling = false;
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
            if (LeaveAt > EndTime - BeatmapContainer.Timing50)
            {
                if (PressedAt >= StartTime - BeatmapContainer.Timing300 && PressedAt <= StartTime + BeatmapContainer.Timing300)
                {
                    //Perfect
                    return Score.ScoreType.Perfect;
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
            else
            {
                //rip?
                if (PressedAt >= StartTime - BeatmapContainer.Timing300 && PressedAt <= StartTime + BeatmapContainer.Timing300)
                {
                    //ño
                    return Score.ScoreType.Good;
                }
                else if (PressedAt >= StartTime - BeatmapContainer.Timing100 && PressedAt <= StartTime + BeatmapContainer.Timing100)
                {
                    //ño
                    return Score.ScoreType.Good;
                }

                else if (PressedAt >= StartTime - BeatmapContainer.Timing50 && PressedAt <= StartTime + BeatmapContainer.Timing50)
                {
                    //Bad
                    return Score.ScoreType.Miss;
                }
                else
                {
                    return Score.ScoreType.Miss;
                }
            }
           
        }
        public Score.ScoreValue GetScoreValue()
        {
            Score.ScoreValue sscv=Score.ScoreValue.Miss;
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
    }
}
