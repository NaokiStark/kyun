using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.GameScreen.UI.Buttons;
using kyun.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.Overlay
{
    public class QuestionOverlay : OverlayScreen
    {
        static QuestionOverlay instance;
        private Label ltitle;
        private Label ladditional;
        public ButtonStandard btnYes;
        private ButtonStandard btnNo;
        private static List<EventHandler> deleg = new List<EventHandler>();

        public static QuestionOverlay Instance
        {
            get
            {
                if (instance == null)
                    instance = new QuestionOverlay();

                return instance;
            }
            set
            {
                instance = value;
            }
        }
        public QuestionOverlay() : base(OverlayType.Normal)
        {
            Visible = false;
            ltitle = new Label(0)
            {
                Text = "",
                Position = new Vector2(ActualScreenMode.Width / 2, ActualScreenMode.Height * 0.2f),
                Centered = true,
                Font = SpritesContent.Instance.ScoreBig
            };

            ladditional = new Label(0)
            {
                Text = "",
                Position = new Vector2(ActualScreenMode.Width / 2, ltitle.Position.Y + 70),
                Centered = true,
                Font = SpritesContent.Instance.ListboxFont
            };

            btnYes = new ButtonStandard(Color.ForestGreen)
            {
                Caption = "Yes!",
                Position = new Vector2(ActualScreenMode.Width / 2 - SpritesContent.Instance.ButtonStandard.Width / 2, ladditional.Position.Y + 100),
                
            };

            btnNo = new ButtonStandard(Color.Red)
            {
                Caption = "Pls no",
                Position = new Vector2(ActualScreenMode.Width / 2 - SpritesContent.Instance.ButtonStandard.Width / 2, btnYes.Position.Y + SpritesContent.Instance.ButtonStandard.Height + 20),
            };

            btnNo.Click += (obj, args) => {
                Visible = false;
                ScreenManager.RemoveOverlay();
            };

            Controls.Add(ltitle);
            Controls.Add(ladditional);
            Controls.Add(btnYes);
            Controls.Add(btnNo);
            //Controls.Add();

        }

        public static QuestionOverlay ShowAlert(EventHandler CallbackYes, string Title, string Text = "")
        {
            
            Instance.ltitle.Text = Title;
            Instance.ladditional.Text = Text;
            
            Instance.btnYes.Click += CallbackYes;
            deleg.Add(CallbackYes);
            Instance.btnYes.Click += (e, arg) => {Instance.Visible = false; ScreenManager.RemoveOverlay(); RemoveEvents(); }; //Nomorexd
            Instance.Visible = true;
            return Instance;
        }

        public static void RemoveEvents()
        {
            foreach (EventHandler d in deleg)
            {
                Instance.btnYes.Click -= d;
            }
        }
    }
}
