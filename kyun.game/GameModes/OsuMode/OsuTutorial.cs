using kyun.GameModes.OsuMode;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameModes.OsuMode
{
    public class OsuTutorial : ScreenBase
    {
        Image cicle;
        Image holder;
        public OsuTutorial()
        {
            cicle = new Image(SpritesContent.Instance.CircleNote);
            holder = new Image(SpritesContent.Instance.CircleNoteHolder);
        }
    }
}
