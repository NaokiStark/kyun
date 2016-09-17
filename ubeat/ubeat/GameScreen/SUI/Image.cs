using Microsoft.Xna.Framework;
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
            Game1.Instance.Player.OnStopped += () => {
                nextBeat = 0;
            };
        }


        void updateScale()
        {
            if (!BeatReact) return;
            //ToDo: Ms per beat

           

            if (Game1.Instance.Player.PlayState == NAudio.Wave.PlaybackState.Playing)
            {
                if (Game1.Instance.SelectedBeatmap != null)
                {
                    if (nextBeat == 0) nextBeat += (long)Game1.Instance.SelectedBeatmap.BPM;

                    if (Game1.Instance.Player.Position > nextBeat)
                    {
                        Scale = 1.1f;
                        nextBeat += (long)Game1.Instance.SelectedBeatmap.BPM;
                    }
                }

            }
            
            if (Scale > 1f)
            {
                Scale -= (float)(Game1.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.0001f);
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
            Game1.Instance.spriteBatch.Draw(this.Texture, rg, null, Color.White, 0, new Vector2((this.Texture.Width / 2), (this.Texture.Height / 2)), SpriteEffects.None, 0);
        }
    }
}
