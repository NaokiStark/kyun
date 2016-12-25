﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ubeat.GameScreen.UI
{
    public class Image : ScreenUIObject
    {

        public bool BeatReact = false;

        long nextBeat = 0;


        public Image(Texture2D texture)
        {
            this.Texture = texture;
            UbeatGame.Instance.Player.OnStopped += () => {
                nextBeat = 0;
            };
            BeatReact = true;
        }


        void updateScale()
        {
            if (!BeatReact) return;
            //ToDo: Ms per beat

            /*

             if (UbeatGame.Instance.Player.PlayState == NAudio.Wave.PlaybackState.Playing)
             {
                 if (UbeatGame.Instance.SelectedBeatmap != null)
                 {
                     if (nextBeat == 0) nextBeat += (long)UbeatGame.Instance.SelectedBeatmap.BPM;

                     if (UbeatGame.Instance.Player.Position > nextBeat)
                     {
                         Scale = 1.1f;
                         nextBeat += (long)UbeatGame.Instance.SelectedBeatmap.BPM;
                     }
                 }

             }*/

            float pScale = UbeatGame.Instance.Player.PeakVol;
            if (pScale > 0.7f) pScale = 1.15f;
            if (pScale > 1.15) pScale = 1.15f;


            if (pScale < Scale) pScale = Scale;  


            Scale = pScale;

            if (Scale > 1f)
            {
                Scale -= (float)(UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.0004f);
            }

        }

        public override void Update()
        {
            base.Update(); //Update Events

            updateScale(); //ToDo: Ms per beat
        }

        public override void Render()
        {
            Rectangle rg = new Rectangle((int)this.Position.X + (this.Texture.Width/2), (int)this.Position.Y + (this.Texture.Height/2), (int)(this.Texture.Width * Scale), (int)(this.Texture.Height * Scale));
            UbeatGame.Instance.SpriteBatch.Draw(this.Texture, rg, null, Color.White, 0, new Vector2((this.Texture.Width / 2), (this.Texture.Height / 2)), SpriteEffects.None, 0);
        }
    }
}
