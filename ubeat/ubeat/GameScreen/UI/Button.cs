using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Audio;
using kyun.Utils;

namespace kyun.GameScreen.UI
{
    public class Button : InputControl
    {

        bool alredyIntersecs = false;

        public bool PlayHit { get; set; }

        bool Uping = false;

        float currT = 0;

        public Button(Texture2D Textur)
        {
            this.Texture = Textur;
            PlayHit = true;
            this.Click += Button_Click;
        }

        void Button_Click(object sender, EventArgs e)
        {
            if (PlayHit)
            {
                //AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.ButtonHit);
                EffectsPlayer.PlayEffect(SpritesContent.Instance.ButtonHit);
            }

        }

        void updateScale()
        {
            
            if (Uping && Scale < 1.1f)
            {
                Scale += (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f);
            }
            else if (!Uping && Scale > 1f)
            {
                Scale -= (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f);
            }
            else if (Uping && Scale + (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f) > 1.1f)
            {
                Scale = 1.1f;
            }
            else
            {
                Scale = 1f;
            }

            
        }

        public override void Update()
        {
            if (!Visible) return;
            base.Update(); //Update Events

          

            Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)this.Texture.Width, (int)this.Texture.Height);
            Rectangle cursor = new Rectangle((int)Mouse.GetState().X, (int)Mouse.GetState().Y, 1, 1);

            //oie
            if (System.Windows.Forms.Form.ActiveForm != (System.Windows.Forms.Control.FromHandle(KyunGame.Instance.Window.Handle) as System.Windows.Forms.Form)) return;

            if (!KyunGame.Instance.IsActive) return; //Fix events

            if (cursor.Intersects(rg))
            {
                Uping = true;
                updateScale();

                if (!alredyIntersecs)
                {
                    //AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.ButtonOver);
                    EffectsPlayer.PlayEffect(SpritesContent.Instance.ButtonOver);
                }

                alredyIntersecs = true;
            }
            else
            {
                Uping = false;
                updateScale();               
                alredyIntersecs = false;
            }
        }
    }
}
