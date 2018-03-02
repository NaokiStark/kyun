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
using kyun.Audio;
using kyun.Beatmap;
using kyun.GameScreen;
using kyun.OsuUtils;
using kyun.Score;
using kyun.Screen;
using kyun.UIObjs;
using kyun.Utils;

namespace kyun.Beatmap
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
        public int HitSound { get; set; }

        public float MsPerBeat { get; set; }
        #endregion

        #region PrivateVars
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
            if (opacity + (KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * .004f) < 1)
            {
                opacity += (KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * .004f);
            }
            else
            {
                opacity = 1;
            }
        }

        public void Update(long Position, Vector2 p)
        { }
        
        public void Render(long ccc, Vector2 position)
        {

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
