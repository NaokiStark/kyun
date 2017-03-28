﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Troschuetz.Random;
using Troschuetz.Random.Generators;
using ubeat.Audio;
using ubeat.Beatmap;
using ubeat.GameScreen;
using ubeat.OsuUtils;
using ubeat.Score;
using ubeat.Screen;
using ubeat.UIObjs;
using ubeat.Utils;

namespace ubeat.Beatmap
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
        public OldApproachObj apo { get; set; }
        public decimal StartTime { get; set; }
        public IBeatmap BeatmapContainer { get; set; }
        public int Location { get; set; }
        public decimal EndTime { get; set; }
        public Vector2 OsuLocation { get; set; }
        #endregion

        #region PrivateVars
        NPlayer holdFld;
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

            ticked = false;
           
            //tmrApproachOpacity.Tick += tmrApproachOpacity_Tick;
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

        private void tmrApproachOpacity_Tick()
        {
            /*
            int appr = (int)(1950 - BeatmapContainer.ApproachRate * 150);
            float percentg = (float)(1f / ((float)appr)) * 100f;
            if (opacity + percentg > 1)
            {
                return;
            }
            this.opacity = opacity + percentg;
            */
            if (opacity + (UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * .004f) < 1)
            {
                opacity += (UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * .004f);
            }
            else
            {
                opacity = 1;
            }
        }

        public void Update(long Position, Vector2 p)
        {
            tmrApproachOpacity_Tick();
            if (holdFld != null)
            {

                if (isFilling && Grid.Instance.Paused)
                {
                    if (holdFld.PlayState == NAudio.Wave.PlaybackState.Playing)
                        holdFld.Paused = true;

                }
                if (isFilling && !Grid.Instance.Paused)
                {
                    if (holdFld.PlayState != NAudio.Wave.PlaybackState.Playing)
                        holdFld.Paused = false;
                }
                /*
                if (isFilling && holdFld.PlayState == NAudio.Wave.PlaybackState.Stopped)
                {
                    holdFld.Position = 0;
                    holdFld.WaveOut.Play();
                }*/

                if (!isFilling)
                    holdFld.Dispose();
                if (!isActive)
                    holdFld.Dispose();
            }

            


            if (isActive)
            {
                if (apo == null)
                {
                    apo = new OldApproachObj(Grid.GetPositionFor(this.Location - 96), BeatmapContainer.ApproachRate, this.StartTime, Grid.Instance);
                    Grid.Instance.objs.Add(apo);
                }
                if (Grid.Instance.autoMode)
                {
                    if (Position >= StartTime /*+ OsuBeatMap.rnd.Next(-(BeatmapContainer.Timing300), BeatmapContainer.Timing300) */&& !hasAlredyPressed)
                    {
                        PressedAt = (long)StartTime;

                        /*
                        SoundEffectInstance ins = Game1.Instance.soundEffect.CreateInstance();
                        ins.Volume = Game1.Instance.GeneralVolume;
                        ins.Play();
                         */
                        AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.HitHolder);

                        hasAlredyPressed = true;
                        isFilling = true;

                        float healthToAdd = (BeatmapContainer.OverallDifficulty / 2) + Math.Abs((float)this.StartTime - PressedAt) / 100;
                        Grid.Instance.Health.Add(healthToAdd);
                        Combo.Instance.Add();

                        /*
                        holdFld = Game1.Instance.HitHolderFilling.CreateInstance();
                        holdFld.Volume = Game1.Instance.GeneralVolume;
                        holdFld.IsLooped = true;
                        holdFld.Play();
                         */
                        //holdFld = EffectsPlayer.PlayEffectLooped(Game1.Instance.HolderFilling, Game1.Instance.GeneralVolume);


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
                    bool mouseDown = (Mouse.GetState().LeftButton == ButtonState.Pressed) || (Mouse.GetState().RightButton == ButtonState.Pressed);
                    //Rectangle MousePos = new Rectangle((int)UbeatGame.Instance.touchHandler.LastPosition.X, (int)UbeatGame.Instance.touchHandler.LastPosition.Y, 10, 10);

                    Rectangle ActualPos = new Rectangle((int)p.X, (int)p.Y, Texture.Bounds.Width, Texture.Bounds.Height);

                    bool intersecs = UbeatGame.Instance.touchHandler.TouchIntersecs(ActualPos);

                    bool mouseUp = Mouse.GetState().LeftButton == ButtonState.Released || Mouse.GetState().RightButton == ButtonState.Released;


                    if (Position > StartTime + BeatmapContainer.Timing50 && !hasAlredyPressed)
                    {
                        isActive = false;
                        PressedAt = Position;
                    }
                    if ((Keyboard.GetState().IsKeyDown((Keys)Location) || (/*UbeatGame.Instance.touchHandler.TouchDown &&*/ intersecs)) && !hasAlredyPressed)
                    {
                        if (Position > StartTime - BeatmapContainer.Timing50)
                        {

                            /*
                            SoundEffectInstance ins = Game1.Instance.soundEffect.CreateInstance();
                            ins.Volume = Game1.Instance.GeneralVolume;
                            ins.Play();
                             * 
                             */
                            AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.HitHolder);

                            hasAlredyPressed = true;
                            PressedAt = Position;
                            isFilling = true;

                            float healthToAdd = (BeatmapContainer.OverallDifficulty / 2) + Math.Abs((float)this.StartTime - PressedAt) / 100;
                            Grid.Instance.Health.Add(healthToAdd);
                            Combo.Instance.Add();

                            //holdFld = EffectsPlayer.PlayEffectLooped(Game1.Instance.HolderFilling);

                        }
                        return;
                    }

                    mouseUp = Mouse.GetState().LeftButton == ButtonState.Released;
                    bool kup = Keyboard.GetState().IsKeyUp((Keys)Location);
                    if (kup && hasAlredyPressed)
                    {
                        if (/*UbeatGame.Instance.touchHandler.TouchUp*/ !intersecs)
                        {
                            isFilling = false;
                            LeaveAt = Position;
                            isActive = false;
                        }                        

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

                    AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.HolderTick);

                    //SoundEffectInstance TickSnd = Game1.Instance.HolderTick.CreateInstance();
                    Grid.Instance.Health.Add(1f * BeatmapContainer.OverallDifficulty);
                    Combo.Instance.Add();
                    //TickSnd.Volume = Game1.Instance.GeneralVolume;
                    //TickSnd.Play();
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
                    AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.HitHolder);
                    Grid.Instance.FailsCount = 0;
                    float healthToAdd = (BeatmapContainer.OverallDifficulty / 2) + Math.Abs(this.LeaveAt - PressedAt) / 100;
                    Grid.Instance.Health.Add(healthToAdd);

                    /*
                    SoundEffectInstance ins = Game1.Instance.HolderHit.CreateInstance();
                    ins.Volume = Game1.Instance.GeneralVolume;
                    ins.Play();*/


                    Combo.Instance.Add();
                }
                else
                {
                    Grid.Instance.FailsCount++;
                    if (Combo.Instance.ActualMultiplier > 10)
                    {
                        /*
                        SoundEffectInstance ins = Game1.Instance.ComboBreak.CreateInstance();
                        ins.Volume = Game1.Instance.GeneralVolume;
                        ins.Play();*/
                        AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.ComboBreak);

                    }
                    Combo.Instance.Miss();
                    Grid.Instance.Health.Substract((4 * BeatmapContainer.OverallDifficulty) * Grid.Instance.FailsCount);
                }

                Grid.Instance.ScoreDispl.Add(((long)score * ((Combo.Instance.ActualMultiplier > 0) ? Combo.Instance.ActualMultiplier : 1)) / 2);

                Grid.Instance.objs.Add(new ScoreObj(GetScore(), new Vector2(p.X + (Texture.Width / 4), p.Y + (Texture.Height / 4))));

                Stop(Position);
            }
        }
        public void Render(long ccc, Vector2 position)
        {
            if (Died)
            {

                return;
            }

            float opac = 0;
            if (!isActive)
            {

            }
            else
            {

                if (ccc >= StartTime - BeatmapContainer.Timing100)
                {

                    
                    if (hasAlredyPressed)
                    {
                        opac = 1f;
                    }
                    else
                    {
                        opac = .6f;
                    }

                }
                UbeatGame.Instance.SpriteBatch.Draw(this.Texture, new Rectangle((int)position.X, (int)position.Y, Texture.Bounds.Width, Texture.Bounds.Height), Color.White * opacity);
                if (ccc >= StartTime - BeatmapContainer.Timing50 && !isFilling)
                {
                    float perct = (float)(ccc / (StartTime - BeatmapContainer.Timing300)) * 1f;

                    ScreenMode mode = ScreenModeManager.GetActualMode();
                    bool isSmallRes = mode.Height < 720;

                    UbeatGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.Hold,
                        new Rectangle(
                            (int)position.X,
                            (int)position.Y,
                            (isSmallRes) ? Texture.Width : SpritesContent.Instance.Hold.Bounds.Width,
                            (isSmallRes) ? Texture.Height : SpritesContent.Instance.Hold.Bounds.Height),
                        Color.White * perct);
                }
                //Game1.Instance.spriteBatch.DrawString(Game1.Instance.fontDefault, (Location - 96).ToString(), new Vector2(position.X + (Texture.Bounds.Width / 2), position.Y + (Texture.Bounds.Height / 2)), Color.Black * opacity);
                if (isFilling)
                {

                    //float stime = ccc - (float)StartTime;
                    //float gtime = stime / (float)Length;
                    //float percentgg = gtime * UbeatGame.Instance.HolderFillDeff.Bounds.Height;

                    //var angle = (float)Math.PI;
                    //UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.HolderFillDeff,
                    //    new Microsoft.Xna.Framework.Rectangle((int)position.X + UbeatGame.Instance.HolderFillDeff.Bounds.Height + 1, (int)position.Y + (UbeatGame.Instance.HolderFillDeff.Bounds.Height), UbeatGame.Instance.HolderFillDeff.Bounds.Width, (int)percentgg),
                    //    new Rectangle(0, 0, UbeatGame.Instance.HolderFillDeff.Bounds.Width, (int)percentgg),
                    //    Color.White,
                    //    angle,
                    //    new Vector2(0, 0),
                    //    Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                    //    0);

                    ScreenMode mode = ScreenModeManager.GetActualMode();
                    bool isSmallRes = mode.Height < 720;


                    float initSize = (isSmallRes)? SpritesContent.Instance.HolderFillDeff_0.Width : SpritesContent.Instance.HolderFillDeff.Width;

                    long positionOnHolder = ccc - (long)StartTime;
                    if (positionOnHolder < 0) positionOnHolder = 0;

                    float positionPercent = positionOnHolder * 100f / (float)Length;

                    float percen = initSize * positionPercent / 100f;

                    if(!(isSmallRes))
                        UbeatGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.Radiance, new Rectangle((int)position.X , (int)position.Y, SpritesContent.Instance.Radiance.Bounds.Width, SpritesContent.Instance.Radiance.Bounds.Height), Color.White * 0.5f);

                    UbeatGame.Instance.SpriteBatch.Draw((isSmallRes) ? SpritesContent.Instance.HolderFillDeff_0 : SpritesContent.Instance.HolderFillDeff,
                      new Rectangle((int)position.X + (this.Texture.Width / 2) - (int)(percen/2), (int)position.Y + (this.Texture.Height/2) - (int)(percen / 2), (int)percen, (int)percen),
                      null,
                      Color.White,
                      0,
                      Vector2.Zero,
                      Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                      0);

                    string secondsTo = ((EndTime - ccc) / 100).ToString("0");
                    Vector2 measureSize = SpritesContent.Instance.DefaultFont.MeasureString(secondsTo);

                    Vector2 bgS = measureSize * 1.1f;


                    UbeatGame.Instance.SpriteBatch.DrawString(SpritesContent.Instance.DefaultFont, secondsTo, new Vector2((position.X + Texture.Width / 2) + 1, (position.Y + Texture.Height / 2) + 1), Color.Black, 0, bgS / 2, 1.1f, SpriteEffects.None, 0);
                    UbeatGame.Instance.SpriteBatch.DrawString(SpritesContent.Instance.DefaultFont, secondsTo, new Vector2(position.X + Texture.Width / 2, position.Y + Texture.Height / 2), Color.White, 0, measureSize / 2, 1f, SpriteEffects.None, 0);


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
                    acc = 50f;
                    break;
                case Score.ScoreType.Miss:
                    acc = 0;
                    break;
            }

            return acc;
        }

        public Score.ScoreType GetScore()
        {

            float fillPerc = (((float)LeaveAt - (float)StartTime) / (float)Length) * 100f;

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
            else if (fillPerc <= 30)
            {
                return Score.ScoreType.Miss;
            }
            else
            {
                //rip?
                if (PressedAt >= StartTime - BeatmapContainer.Timing300 && PressedAt <= StartTime + BeatmapContainer.Timing300)
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
    }
}
