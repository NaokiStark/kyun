using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using kyun.Utils;
using kyun.Audio;
using System;
using System.Collections.Generic;

namespace kyun.GameScreen.UI
{
    public class Image : UIObjectBase
    {

        public bool BeatReact = false;

        long nextBeat = 0;
        long maxTimeMs = 1000 * 20;
        long actualTimeMs = 0;
        float maxPeak = 0;

        float ScaleSelect = 1;

        Vector2 sx = Vector2.Zero;
        public override Vector2 Size {
            get {
                if (sx == Vector2.Zero)
                    return new Vector2(Texture.Width, Texture.Height);
                return sx;
            }
            set
            {
                sx = value;
            }
        }



        public Image(Texture2D texture)
        {
            this.Texture = texture;
            Opacity = 1;
            BeatReact = true;
        }


        void updateScale()
        {
            if (!BeatReact) return;
            
                     
            float pScale = KyunGame.Instance.Player.PeakVol;
            if (KyunGame.Instance.Player.PlayState != BassPlayState.Playing)
            {
                pScale = 0;
            }

            if (pScale >= KyunGame.Instance.maxPeak - 0.0001) pScale = 1.15f * ScaleSelect;
            if (pScale > 1.15 * Scale) pScale = 1.15f * ScaleSelect;


            if (pScale < Scale) pScale = Scale;  


            Scale = pScale;

            if ((Scale) > ScaleSelect)
            {
                Scale -= (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.0004f);
            }

        }

        public void ChangeScale(float scale)
        {
            Scale = ScaleSelect = scale;
        }

        public override void Update()
        {
            if (!Visible)
                return;

            base.Update(); //Update Events

            updateScale(); //ToDo: Ms per beat
        }

        public override void Render()
        {

            if (!Visible)
                return;

            if (Effect != null && KyunGame.Instance.Graphics.GraphicsProfile == GraphicsProfile.HiDef)
            {
                KyunGame.Instance.SpriteBatch.End();
                KyunGame.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

                foreach (KeyValuePair<string, dynamic> parameter in EffectParameters)
                {
                    Effect.Parameters[parameter.Key].SetValue(parameter.Value);                    
                }

                Effect.CurrentTechnique = Effect.Techniques[0];
                foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }
            }

            Rectangle rg = new Rectangle();
            if (Size != Vector2.Zero)
            {
                rg = new Rectangle((int)this.Position.X + ((int)this.Size.X / 2), (int)this.Position.Y + ((int)this.Size.Y / 2), (int)(this.Size.X * Scale), (int)(this.Size.Y * Scale));
            }
            else
            {
                rg = new Rectangle((int)this.Position.X + (this.Texture.Width / 2), (int)this.Position.Y + (this.Texture.Height / 2), (int)(this.Texture.Width * RenderScale), (int)(this.Texture.Height * RenderScale));

            }
            if(Texture !=null && !Texture.IsDisposed)
                KyunGame.Instance.SpriteBatch.Draw(this.Texture, rg, null, TextureColor * Opacity, AngleRotation, new Vector2((this.Texture.Width / 2), (this.Texture.Height / 2)), SpriteEffects.None, 0);

            if (Effect != null && KyunGame.Instance.Graphics.GraphicsProfile == GraphicsProfile.HiDef)
            {
                KyunGame.Instance.SpriteBatch.End();
                KyunGame.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            }
            //Render over object
            if (Tooltip != null)
                Tooltip?.Render();
        }
    }
}
