using kyun.GameScreen;
using kyun.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.NikuClient
{
    public class NikuClientApi
    {
        public static ServerList svrs = new ServerList();

        public static UserData User = null;

        public static event EventHandler LoginSuccess;
        public static event EventHandler LoginFail;

        public static bool isLogout = false;

        public NikuClientApi(string user, string pass, bool tokenLogin = true)
        {
            //Note: Less hardcoded?
            //svrs.AddServer("localhost/onics2");
            svrs.AddServer("onics.club");

            if (!tokenLogin)
                CheckConnection(user, pass, svrs.GetServer(0).Value);
            else
                CheckTokenConnection(user, pass, svrs.GetServer(0).Value);
        }

        private async void CheckTokenConnection(string user, string token, string value)
        {
            try
            {
                User = await UserData.LoadToken(user.ToLower(), token, value);

                if (User == null)
                {
                    LoginFail?.Invoke(this, new EventArgs());                    
                }
                else
                {
                    LoginSuccess?.Invoke(this, new EventArgs());
                    SpritesContent.Instance.DefaultBackground = User.BackgroundTexture;
                    if (ScreenManager.ActualScreen != null && !(ScreenManager.ActualScreen is LogoScreen))
                        ((ScreenBase)ScreenManager.ActualScreen).ChangeBackground(User.BackgroundTexture);
                }
                    

                Logger.Instance.Debug(User.ToString());

            }
            catch (Exception ex)
            {
                Logger.Instance.Warn("User can't fetched");
                Logger.Instance.Warn(ex.Message);
                Logger.Instance.Warn(ex.StackTrace);
                LoginFail?.Invoke(this, new EventArgs());
            }
        }

        private async void CheckConnection(string user, string pass, string value)
        {
            try
            {
                User = await UserData.Load(user.ToLower(), pass, value);

                if (User == null)
                {
                    LoginFail?.Invoke(this, new EventArgs());
                }                    
                else
                {
                    LoginSuccess?.Invoke(this, new EventArgs());
                    SpritesContent.Instance.DefaultBackground = User.BackgroundTexture;
                    if (ScreenManager.ActualScreen != null && !(ScreenManager.ActualScreen is LogoScreen))
                        ((ScreenBase)ScreenManager.ActualScreen).ChangeBackground(User.BackgroundTexture);
                }



                Logger.Instance.Debug(User.ToString());
            }
            catch(Exception ex)
            {
                Logger.Instance.Warn("User can't fetched");
                Logger.Instance.Warn(ex.Message);
                Logger.Instance.Warn(ex.StackTrace);
                LoginFail?.Invoke(this, new EventArgs());
            }
        }

        public void Logout()
        {
            Settings1.Default.User = "";
            Settings1.Default.Token = "";
            Settings1.Default.Save();
            isLogout = false;
            User.Dispose();
            User = null;
            SpritesContent.Instance.DefaultBackground = ContentLoader.LoadTexture(SpritesContent.Instance.defaultbg);
            ((ScreenBase)ScreenManager.ActualScreen).ChangeBackground(SpritesContent.Instance.DefaultBackground);            
        }
    }
}
