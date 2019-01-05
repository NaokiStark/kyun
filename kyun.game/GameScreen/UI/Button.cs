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
            
            if (Uping && Scale < 1.5f)
            {
                Scale = Math.Min(Scale + (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f), 1.1f);
            }
            else if (!Uping && Scale > 1f)
            {
                Scale = Math.Max(Scale - (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f), 1f);
            }
        }

        public override void Update()
        {
            if (!Visible) return;
            base.Update(); //Update Events
                      

            Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)this.Texture.Width, (int)this.Texture.Height);
            Rectangle cursor = new Rectangle((int)MouseHandler.GetState().X, (int)MouseHandler.GetState().Y, 1, 1);

            //oie
            if (System.Windows.Forms.Form.ActiveForm != KyunGame.WinForm) return;

            if (!KyunGame.Instance.IsActive) return; //Fix events
            updateScale();
            if (cursor.Intersects(rg) && !alredyIntersecs)
            {
                alredyIntersecs = true;
                Uping = true;
                

                if (alredyIntersecs)
                {
                    //AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.ButtonOver);
                    EffectsPlayer.PlayEffect(SpritesContent.Instance.ButtonOver);
                }
                                
            }
            else if(!cursor.Intersects(rg))
            {
                Uping = false;                            
                alredyIntersecs = false;

            }
        }
    }
}
