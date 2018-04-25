using kyun.game.NikuClient;
using kyun.game.Winforms;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameScreen.UI
{
    public class UserBox : UIObjectBase
    {
        FilledRectangle displayBg;
        Image userAvatar;
        Label nickLabel;

        static UserBox instance;

        public new Vector2 Position {
            get {
                return userAvatar.Position;
            }
            set {
                userAvatar.Position = value;
                displayBg.Position = new Vector2(userAvatar.Position.X + 75, userAvatar.Position.Y);
                nickLabel.Position = new Vector2(displayBg.Position.X + 5, nickLabel.Position.Y);
            }
        }

        public static UserBox GetInstance()
        {
            if (instance == null)
                instance = new UserBox();

            return instance;
        }

        public UserBox()
        {
            displayBg = new FilledRectangle(new Vector2(200, 75), Color.Black * .8f);



            userAvatar = new Image((NikuClientApi.User != null) ? NikuClientApi.User.AvatarTexture:ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, UserData.GetAvatarStream("")))
            {
                BeatReact = false
            };

            nickLabel = new Label(0)
            {
                Text = (NikuClientApi.User == null) ? "Login" : NikuClientApi.User.Username,
                Font = SpritesContent.Instance.ListboxFont,
                Scale = .8f,
                ForegroundColor = Color.White
            };

            userAvatar.Click += UserAvatar_Click;
            displayBg.Click += UserAvatar_Click;
            nickLabel.Click += UserAvatar_Click;            
        }

        private void UserAvatar_Click(object sender, EventArgs e)
        {
            var isFullScreen = KyunGame.Instance.Graphics.IsFullScreen;
            if (isFullScreen)
                KyunGame.Instance.ToggleFullscreen(false);

            new LoginForm().Show();
        }

        public void ReloadAvatar()
        {
            userAvatar.Texture = (NikuClientApi.User != null) ? NikuClientApi.User.AvatarTexture : ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, UserData.GetAvatarStream(""));
            nickLabel.Text = (NikuClientApi.User == null) ? "Login" : NikuClientApi.User.Username;
        }

        public override void Update()
        {
            if(NikuClientApi.User == null && !NikuClientApi.isLogout)
            {
                NikuClientApi.isLogout = true;
                userAvatar.Texture = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, UserData.GetAvatarStream(""));
                nickLabel.Text = "Login";
            }

            displayBg.Opacity = nickLabel.Opacity = userAvatar.Opacity = Opacity;
            displayBg.Update();
            userAvatar.Update();
            nickLabel.Update();
        }

        public override void Render()
        {
            displayBg.Render();
            userAvatar.Render();
            nickLabel.Render();
        }
    }
}
