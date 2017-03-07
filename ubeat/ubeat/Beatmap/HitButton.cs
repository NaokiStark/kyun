using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Windows.Forms;
using ubeat.UIObjs;
using ubeat.GameScreen;
using ubeat.Score;
using ubeat.Audio;
using ubeat.Screen;
using ubeat.Utils;

namespace ubeat.Beatmap
{
    public class HitButton : IHitObj
    {
        #region PublicVars
        public Texture2D Texture { get; set; }
        public long PressedAt { get; set; }
        public bool isActive { get; set; }
        public long ActualPos { get; set; }
        public bool Died { get; set; }
        public bool hasAlredyPressed { get; set; }
        public int Y = 0;
        public int X = 0;
        public decimal StartTime { get; set; }
        public IBeatmap BeatmapContainer { get; set; }
        public int Location { get; set; }
        public decimal EndTime { get; set; }
        public OldApproachObj apo { get; set; }

        #endregion

        #region PrivateVars
        Timer tmrApproachOpacity;
        float opacity = 0;

        #endregion

        #region Textures
        public void AddTexture(Texture2D texture)
        {
            this.Texture = texture;
        }
        #endregion

        #region GameEvents
        public void Start(long Position)
        {
            isActive = true;
            Died = false;
          
            ActualPos = Position;

            PressedAt = 0;
            apo = null;
            hasAlredyPressed = false;
        }

        public void Reset()
        {
            isActive = false;
            Died = true;
            ActualPos = 0;
            PressedAt = 0;
            hasAlredyPressed = false;
            apo = null;
        }

        void tmrApproachOpacity_Tick()
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

            if(opacity + (UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * .004f) < 1)
            {
                opacity += (UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * .004f);
            }
            else
            {
                opacity = 1;
            }

        }

        public void Update(long Position, Vector2 ps)
        {

            if (isActive)
            {
                tmrApproachOpacity_Tick();
                if (apo == null)
                {
                    apo = new OldApproachObj(Grid.GetPositionFor(this.Location - 96), BeatmapContainer.ApproachRate, this.StartTime, Grid.Instance);
                    Grid.Instance.objs.Add(apo);

                }
                if (Grid.Instance.autoMode)
                {
                    if (Position > StartTime)
                    {
                        hasAlredyPressed = true;
                        isActive = false;
                        PressedAt = (long)StartTime;
                    }
                }
                else
                {
                    bool mouseDown = (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) || (Mouse.GetState().RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);

                    //Rectangle MousePos = new Rectangle((int)UbeatGame.Instance.touchHandler.LastPosition.X, (int)UbeatGame.Instance.touchHandler.LastPosition.Y, 10, 10);

                    Rectangle ActualPos = new Rectangle((int)ps.X, (int)ps.Y, Texture.Bounds.Width, Texture.Bounds.Height);

                    bool intersecs = UbeatGame.Instance.touchHandler.TouchIntersecs(ActualPos);

                    bool mouseUp = Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released || Mouse.GetState().RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released;




                    if (Position > StartTime + BeatmapContainer.Timing50 && !hasAlredyPressed)
                    {
                        isActive = false;
                        PressedAt = Position;
                    }
                    if ((Keyboard.GetState().IsKeyDown((Microsoft.Xna.Framework.Input.Keys)Location)) && !hasAlredyPressed)
                    {

                        if (Position > StartTime - BeatmapContainer.Timing50)
                        {
                            hasAlredyPressed = true;
                            PressedAt = Position;
                            isActive = false;
                        }
                        return;
                    }
                    else if(/*UbeatGame.Instance.touchHandler.TouchDown &&*/ intersecs)
                    {
                        if (Position > StartTime - BeatmapContainer.Timing50)
                        {
                            hasAlredyPressed = true;
                            PressedAt = Position;
                            isActive = false;
                        }
                        return;
                    }

                    if ((Keyboard.GetState().IsKeyUp((Microsoft.Xna.Framework.Input.Keys)Location) || (!intersecs)) && hasAlredyPressed)
                    {
                        isActive = false;

                    }
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
                Score.ScoreValue getScore = GetScoreValue();
                if ((int)getScore > (int)Score.ScoreValue.Miss)
                {

                    Grid.Instance.FailsCount = 0;
                    float healthToAdd = (BeatmapContainer.OverallDifficulty / 2) + Math.Abs(PressedAt - (long)this.StartTime) / 100;
                    Grid.Instance.Health.Add(healthToAdd);
                    /*
                        SoundEffectInstance ins = Game1.Instance.soundEffect.CreateInstance();
                        ins.Volume = Game1.Instance.GeneralVolume;
                        ins.Play();*/
                    AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.HitButton);

                    Combo.Instance.Add();
                }
                else
                {

                    Grid.Instance.FailsCount++;
                    if (Combo.Instance.ActualMultiplier > 10)
                    {
                        AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.ComboBreak);
                        /*
                        SoundEffectInstance ins = Game1.Instance.ComboBreak.CreateInstance();
                        ins.Volume = Game1.Instance.GeneralVolume;
                        ins.Play();*/
                    }
                    Combo.Instance.Miss();
                    Grid.Instance.Health.Substract((2 * BeatmapContainer.OverallDifficulty) * Grid.Instance.FailsCount);
                }
                Grid.Instance.ScoreDispl.Add(((long)getScore * ((Combo.Instance.ActualMultiplier > 0) ? Combo.Instance.ActualMultiplier : 1)) / 2);
                Grid.Instance.objs.Add(new ScoreObj(GetScore(), new Vector2(ps.X+ (Texture.Width/4), ps.Y+(Texture.Height / 4))));

                Stop(Position);
            }

        }
        public void Render(long ccc, Vector2 position)
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
                    //UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.radiance, new Microsoft.Xna.Framework.Rectangle((int)position.X - 2, (int)position.Y - 2, UbeatGame.Instance.radiance.Bounds.Width + 2, UbeatGame.Instance.radiance.Bounds.Height + 2), Color.White * opac);

                }
                UbeatGame.Instance.SpriteBatch.Draw(this.Texture, new Microsoft.Xna.Framework.Rectangle((int)position.X, (int)position.Y, Texture.Bounds.Width, Texture.Bounds.Height), Color.White * opacity);
                if (ccc >= StartTime - BeatmapContainer.Timing50)
                {
                    ScreenMode mode = ScreenModeManager.GetActualMode();
                    bool isSmallRes = mode.Height < 720;

                    float perct = (float)(ccc / (StartTime - BeatmapContainer.Timing300)) * 1f;
                    UbeatGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.Push,
                        new Rectangle(
                            (int)position.X,
                            (int)position.Y,
                            (isSmallRes)? Texture.Width : SpritesContent.Instance.Push.Bounds.Width,
                             (isSmallRes) ? Texture.Height : SpritesContent.Instance.Push.Bounds.Height),
                        Color.White * perct);
                }
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

            if (PressedAt >= StartTime - BeatmapContainer.Timing300 && PressedAt <= StartTime + BeatmapContainer.Timing300)
            {
                //Perfect
                return Score.ScoreType.Perfect;
            }
            else if (PressedAt >= StartTime - BeatmapContainer.Timing300 && PressedAt <= StartTime + BeatmapContainer.Timing100)
            {

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


    }
}
