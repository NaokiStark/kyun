using kyun.game.GameModes.CatchIt;
using kyun.GameScreen.UI;
using kyun.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using kyun.GameModes;
using kyun.Audio;
using kyun.Beatmap;
using kyun.GameModes.Classic;

namespace kyun.game.GameModes.CatchItCollab
{
    public class CatchItCollabMode : CatchItMode
    {
        static new CatchItCollabMode Instance = null;
        public new static CatchItCollabMode GetInstance()
        {
            if (Instance == null)
                Instance = new CatchItCollabMode();
            return Instance;
        }

        public Image Player2;

        public Vector2 Player2Size { get; set; }

        internal int player2LinePosition = 1;

        public CatchItCollabMode() : base()
        {
            Player2 = new Image(SpritesContent.Instance.Catcher);

            Player2Size = new Vector2(Math.Max(Math.Min(Player2.Texture.Width, 100), 50), Math.Max(Math.Min(Player2.Texture.Height, 100), 50));

            Player2.Size = PlayerSize;
            Player2.Position = new Vector2(ActualScreenMode.Width - Player2.Size.X, 0);

            Controls.Add(Player2);

            onKeyPress += CatchItCollabMode_onKeyPress;
        }

        private void CatchItCollabMode_onKeyPress(object sender, kyun.GameScreen.InputEvents.KeyPressEventArgs args)
        {

            switch (args.Key)
            {
                case Keys.Up:
                case Keys.Left:
                case Keys.W:
                case Keys.A:
                    if (player2LinePosition > 1)
                    {
                        if (isKeyDownShift)
                            player2LinePosition = Math.Max(player2LinePosition - 2, 1);
                        else
                            player2LinePosition--;
                    }
                    //movements.Add(new ReplayObject { PressedAt = playerLinePosition, LeaveAt = GamePosition });
                    break;
                case Keys.Down:
                case Keys.Right:
                case Keys.S:
                case Keys.D:
                    if (player2LinePosition < fieldSlots)
                    {
                        if (isKeyDownShift)
                            player2LinePosition = Math.Min(player2LinePosition + 2, fieldSlots);
                        else
                            player2LinePosition++;
                    }
                    //movements.Add(new ReplayObject { PressedAt = playerLinePosition, LeaveAt = GamePosition });
                    break;
                case Keys.L:
                    throwObject(player2LinePosition);
                    break;
            }
        }

        public override void Update(GameTime tm)
        {
            updatePlayerPosition();
            base.Update(tm);
        }

        internal override void checkObjects()
        {
            //base.checkObjects();
            if (!InGame) return;

            if (InGame && GamePosition > Beatmap.SleepTime && KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
            {
                KyunGame.Instance.Player.Play(Beatmap.SongPath, ((gameMod & GameMod.DoubleTime) == GameMod.DoubleTime) ? 1.5f : 1f);

                KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume;

                if ((long)Beatmap.HitObjects.First().StartTime > 3500)
                    skipButton.Visible = true;
                else
                    skipButton.Visible = false;
            }

            if (lastIndex >= Beatmap.HitObjects.Count)
            {
                InGame = false;
                return;
            }

            long actualTime = GamePosition;

            if(lastIndex < Beatmap.HitObjects.Count)
            {
                IHitObj lastObject = Beatmap.HitObjects[lastIndex];
                long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - Beatmap.ApproachRate * 150f) + 1000;

                long nextObjStart = (long)lastObject.StartTime - approachStart;

                if (actualTime > nextObjStart)
                {
                    lastIndex++;
                }
            }
        }

        internal void throwObject(int pos)
        {

            float totalLengthInPixels = ActualScreenMode.Width - Player.Size.X * 2 - Player.Position.X;

            float MsPerBeat = ((1680 - Beatmap.ApproachRate * 100)) / ((60000 / Beatmap.HitObjects[lastIndex].MsPerBeat) * 5);
            float appr = 1f / (((gameMod & GameMod.DoubleTime) != GameMod.DoubleTime) ? MsPerBeat : MsPerBeat * 1.5f);
            appr = (float)Math.Round(appr, 1);

            kyun.Beatmap.HitButton hitObject = new kyun.Beatmap.HitButton()
            {
                MsPerBeat = Beatmap.HitObjects[lastIndex].MsPerBeat,
                HitSound = pos * 2,
                OsuLocation = Vector2.Zero,
                StartTime = GamePosition + (long)(totalLengthInPixels / appr)
            };


            var obj = new HitObjectCollab(hitObject, Beatmap, this)
            {
                PositionInRow = pos
            };

            obj.ReplayId = lastIndex;
            obj.PositionInRow = player2LinePosition;
            HitObjects.Add(obj);
            HitObjectsRemain.Add(obj);

            Controls.Add(obj);
        }

        internal void updatePlayerPosition()
        {
            int inFieldPosition = (int)(FieldSize.Y / fieldSlots);

            inFieldPosition *= player2LinePosition;

            Player2.Position = new Vector2(ActualScreenMode.Width - Player2.Size.X, inFieldPosition + FieldPosition.Y);
        }
    }
}
