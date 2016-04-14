using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.UIObjs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using ubeat.GameScreen;
namespace ubeat.Score
{
    public class HealthBar:IUIObject
    {

        public Microsoft.Xna.Framework.Vector2 Position { get; set; }

        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get; set; }

        public bool isActive { get; set; }

        public bool Died { get; set; }

        public float Value { get; private set; }

        public delegate void GmEv();
        public event GmEv OnFail;

        Texture2D BgBar;

        float overallDiff;
        Timer HltTmr;
        public HealthBar()
        {
            /*
            int width = 600;
            int height = 40;*/
            int width = 40;
            int height = -(((int)Grid.GetPositionFor(1).Y ) - ((int)Grid.GetPositionFor(7).Y) - Game1.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight) +35;
            this.Texture = new Texture2D(Game1.Instance.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            this.Texture.SetData(data);

            float op = .5f;
            BgBar = new Texture2D(Game1.Instance.GraphicsDevice, width + 20, height + 20);
            Color[] dataBar = new Color[(width+20) * (height+20)];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.Black * op;
            this.BgBar.SetData(dataBar);

            HltTmr = new Timer() { Interval = 1 };
            HltTmr.Tick += HltTmr_Tick;
            
            //Later
            HltTmr.Start();
            
            this.isActive = false;
        }

        void HltTmr_Tick(object sender, EventArgs e)
        {
            if (!isActive || !Grid.Instance.inGame || Grid.Instance.Paused) return;

            this.Value -= ((overallDiff / 10) + .5f)/20; //Test if is hard c:

        }

        public void Start(float OverallDiff)
        {
            this.Value = 100;
            isActive = true;
            this.overallDiff = OverallDiff;
        }

        public void Stop()
        {
            isActive = false;
        }
        
        public void Reset()
        {
            Value = 100;

        }

        public void Add(float value) {
            value = value - (overallDiff/10);
            if (value + this.Value > 100)
                this.Value = 100;
            else
                this.Value += value;
        }
        
        public void Substract(float value) {
            value = value + (overallDiff / 10);
            if ((this.Value - value) < 0)
                this.Value = 0;
            else
                this.Value -= value;
        }

        public void Update()
        {
            if(Grid.Instance.inGame && !Grid.Instance.Paused && isActive)
                if (Value < 1)
                    if (OnFail != null)
                        OnFail();
        }

        public void Render()
        {

            float res = (float)Value * (float)this.Texture.Height / 100;

            //Rect
            Rectangle size = new Rectangle((int)Grid.GetPositionFor(1).X-40-10, (int)Grid.GetPositionFor(1).Y+ Game1.Instance.buttonDefault.Height+30, this.Texture.Bounds.Width, (int)res);
            Rectangle sizeBar = new Rectangle((int)Grid.GetPositionFor(7).X - 100, (int)Grid.GetPositionFor(7).Y - 40, BgBar.Bounds.Width, BgBar.Bounds.Height);
            //Draw 

            //bg          
            

            Game1.Instance.spriteBatch.Draw(this.BgBar,
                sizeBar,
                null,
                Color.White,
                0f,
                new Vector2(0),
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                0);

            var angle = (float)Math.PI;
            //bar
            Game1.Instance.spriteBatch.Draw(this.Texture,
                size,
                new Rectangle(0, 0, this.Texture.Width, (int)res),
                Color.White,
                angle,
                new Vector2(0),
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                0);

        }
    }
}
