using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using kyun.GameScreen;
using kyun.UIObjs;
using kyun.GameModes.Classic;
using System;
using kyun.Audio;

namespace kyun.Score
{
    public class HealthBar : UIObjectBase
    {
        public delegate void GmEv();
        public event GmEv OnFail;
        
        public Texture2D BgBar;

        float overallDiff;
        public HealthBar(int width = 0, int height = 0)
        {

            if(width == 0)
            {
                width = KyunGame.Instance.GraphicsDevice.Viewport.Width / 2;
                width = width - (width / 3);
            }

            if(height == 0)
            {
                height = 40;
            }
            
            
            this.Texture = new Texture2D(KyunGame.Instance.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.WhiteSmoke;
            this.Texture.SetData(data);


            BgBar = new Texture2D(KyunGame.Instance.GraphicsDevice, width + 20, height + 20);
            Color[] dataBar = new Color[(width + 20) * (height + 20)];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.Black;
            this.BgBar.SetData(dataBar);

            this.IsActive = false;
        }

        void HltTmr_Tick()
        {
            if (!IsActive || !ClassicModeScreen.Instance.InGame || ScreenBase.AVPlayer.audioplayer.PlayState != BassPlayState.Playing) return;

            this.Value -= (KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds / 100f) * ((overallDiff / 20f)); //Test if is hard c:

            Value = Math.Max(0.009f, Value); //Limit to 0.009

        }

        public void Start(float OverallDiff)
        {

            OverallDiff = Math.Max(OverallDiff, 3);
            this.Value = 100;
            IsActive = true;
            Enabled = true;
            this.overallDiff = OverallDiff;
        }

    
        public void Stop()
        {
            IsActive = false;
        }

        public void Reset()
        {
            Value = 100;

        }

        public void Add(float value)
        {
            value -= overallDiff / 10;
            if (value + this.Value > 100)
                this.Value = 100;
            else
                this.Value += value;
        }

        public void Substract(float value)
        {
            value += overallDiff / 10;
            if ((this.Value - value) < 0)
                this.Value = 0;
            else
                this.Value -= value;
        }

        public override void Update()
        {
            HltTmr_Tick();/*
            if (Grid.Instance.inGame && !Grid.Instance.Paused && IsActive)
                if (Value < 0.1f)
                    if(!Grid.Instance.autoMode)
                        if(!Grid.Instance.NoFailMode)
                            OnFail?.Invoke();*/

            if (ClassicModeScreen.Instance.InGame && (ScreenBase.AVPlayer.audioplayer.PlayState == BassPlayState.Playing) && IsActive)
                if (Value < 0.1f)
                    if (((ClassicModeScreen)ClassicModeScreen.Instance).gameMod != GameModes.GameMod.Auto)
                        if (((ClassicModeScreen)ClassicModeScreen.Instance).gameMod != GameModes.GameMod.NoFail)
                            OnFail?.Invoke();
        }

        public override void Render()
        {

            if (!Enabled) return;
            float res = (float)Value * (float)this.Texture.Width / 100;

            int colorBar = (int)(Value * 255f / 100f);
            Color colbr = Color.FromNonPremultiplied(255, colorBar, colorBar, 255);


            //Rect
            //Rectangle size = new Rectangle((int)Grid.GetPositionFor(1).X-40-10, (int)Grid.GetPositionFor(1).Y+ Game1.Instance.buttonDefault.Height+30, this.Texture.Bounds.Width, (int)res);
            //Rectangle sizeBar = new Rectangle((int)Grid.GetPositionFor(7).X - 100, (int)Grid.GetPositionFor(7).Y - 40, BgBar.Bounds.Width, BgBar.Bounds.Height);
            //Draw 

            Rectangle size = new Rectangle((int)Position.X + 10, (int)Position.Y + 10, (int)res, Texture.Height);
            Rectangle sizeBar = new Rectangle((int)Position.X, (int)Position.Y, this.BgBar.Width, this.BgBar.Height);
            //bg          


            KyunGame.Instance.SpriteBatch.Draw(this.BgBar,
                sizeBar,
                null,
                Color.White * 0.75f,
                0f,
                new Vector2(0),
                SpriteEffects.None,
                0);

            //bar
            KyunGame.Instance.SpriteBatch.Draw(this.Texture,
                size,
                null,
                colbr,
                0f,
                new Vector2(0),
                SpriteEffects.None,
                0);

        }

        public bool Enabled { get; set; }
        public float Value { get; private set; }
    }
}
