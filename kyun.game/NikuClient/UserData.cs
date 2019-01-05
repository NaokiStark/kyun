using kyun.Utils;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.NikuClient
{
    public class UserData : IDisposable
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Token { get; set; }
        public string WsHash { get; set; }
        public bool Banned { get; set; }
        public DateTime LastLogin { get; set; }
        public string AvatarUrl { get; set; }
        public string CoverUrl { get; set; } //I dunno
        public string BackgroundUrl { get; set; } //Ammm, yes, custom background from server
        public Texture2D AvatarTexture { get; set; }
        public Texture2D BackgroundTexture { get; set; }
        


        public static async Task<UserData> Load(string user, string password, string server)
        {
            var tmpData = new UserData
            {
                Username = user,
                Password = password,
                Server = server
            };

            string dataResult = await HttpRequest.Get($"http://{server}/login/kyunLogin?user={user}&pass={password}");

            JObject jsRes = JObject.Parse(dataResult);

            int resultStatus = (int)jsRes["status"];

            if (resultStatus == 0)
                return null; //Invalid data

            tmpData.Token = (string)jsRes["data"]["tokens"]["token"];

            string userDetails = await HttpRequest.Get($"http://{server}/user/GetUser?token={tmpData.Token}&user={user}");

            JObject jsonDetails = JObject.Parse(userDetails);
            
            resultStatus = (int)jsonDetails["status"];
            //Weird case
            if (resultStatus == 0)
                return null; //Invalid data

            tmpData.AvatarUrl = (string)jsonDetails["data"]["avatar"];
            tmpData.Username = (string)jsonDetails["data"]["user"];
            tmpData.CoverUrl = (string)jsonDetails["data"]["cover"];
            tmpData.BackgroundUrl = (string)jsonDetails["data"]["background"];

            tmpData.Banned = false;
            tmpData.WsHash = (string)jsonDetails["data"]["wshash"];
            
            tmpData.LastLogin = DateTime.Now;

            Stream avatarstream = await GetAvatarStream(tmpData.AvatarUrl);

            tmpData.AvatarTexture = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, avatarstream);

            try
            {
                WebRequest req = WebRequest.Create(tmpData.BackgroundUrl);
                WebResponse response = req.GetResponse();
                MemoryStream stream = new MemoryStream();
                response.GetResponseStream().CopyTo(stream);
                System.Drawing.Bitmap im = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(stream);

                tmpData.BackgroundTexture = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, SpritesContent.BitmapToStream(im));
            }
            catch (Exception ex)
            {
                tmpData.BackgroundTexture = SpritesContent.Instance.DefaultBackground;
            }


            return tmpData;
        }

        public static async Task<UserData> LoadToken(string user, object token, string server)
        {
            var tmpData = new UserData
            {
                Username = user,
                //Password = password,
                Server = server
            };

            string dataResult = await HttpRequest.Get($"http://{server}/login/checkToken?user={user}&token={token}");

            JObject jsRes = JObject.Parse(dataResult);

            int resultStatus = (int)jsRes["status"];

            if (resultStatus == 0)
                return null; //Invalid data

            tmpData.Token = (string)jsRes["data"]["tokens"]["token"];

            string userDetails = await HttpRequest.Get($"http://{server}/user/GetUser?token={tmpData.Token}&user={user}");

            JObject jsonDetails = JObject.Parse(userDetails);

            resultStatus = (int)jsonDetails["status"];
            //Weird case
            if (resultStatus == 0)
                return null; //Invalid data

            tmpData.AvatarUrl = (string)jsonDetails["data"]["avatar"];
            tmpData.Username = (string)jsonDetails["data"]["user"];
            tmpData.CoverUrl = (string)jsonDetails["data"]["cover"];
            tmpData.BackgroundUrl = (string)jsonDetails["data"]["background"];

            tmpData.Banned = false;
            tmpData.WsHash = (string)jsonDetails["data"]["wshash"];

            tmpData.LastLogin = DateTime.Now;

            Stream avatarstream = await GetAvatarStream(tmpData.AvatarUrl);
            tmpData.AvatarTexture = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, avatarstream);

            try
            {
                WebRequest req = WebRequest.Create(tmpData.BackgroundUrl);
                WebResponse response = await req.GetResponseAsync();
                MemoryStream stream = new MemoryStream();
                //response.GetResponseStream().CopyTo(stream);
                //System.Drawing.Bitmap im = new System.Drawing.Bitmap(response.GetResponseStream());

                //tmpData.BackgroundTexture = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, SpritesContent.BitmapToStream(im));
                tmpData.BackgroundTexture = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, response.GetResponseStream(), true);
            }
            catch(Exception ex)
            {
                tmpData.BackgroundTexture = SpritesContent.Instance.DefaultBackground;
            }

            return tmpData;
        }

        public static async Task<Stream> GetAvatarStream(string url)
        {
            System.Drawing.Image cimg = null;

            if (url == "")
            {
                cimg = System.Drawing.Image.FromFile(SpritesContent.Instance.defaultbg);
            }
            else
            {
                try
                {
                    WebRequest req = WebRequest.Create(url);
                    WebResponse response = await req.GetResponseAsync();
                    Stream stream = response.GetResponseStream();
                    
                    cimg = new System.Drawing.Bitmap(stream);
                }
                catch
                {
                    Logger.Instance.Warn("Failed to download avatar image");
                    cimg = System.Drawing.Image.FromFile(SpritesContent.Instance.defaultbg);
                }

            }

            System.Drawing.Bitmap cbimg = SpritesContent.ResizeImage(cimg, (int)(((float)cimg.Width / (float)cimg.Height) * 75), (int)75);

            System.Drawing.Bitmap ccbimg;
            MemoryStream istream;
            if (cbimg.Width != cbimg.Height)
            {
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)((cbimg.Width - 75) / 2), 0, (int)75, (int)75));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }

            return istream;
        }

        public override string ToString()
        {            
            return $"[{Username}]\r\n Token: {Token}\r\n WebSocketHash: {WsHash}\r\n Avatar: {AvatarUrl}\r\n Background: {BackgroundUrl}\r\n";
        }

        public void Dispose()
        {
            BackgroundTexture?.Dispose();
            AvatarTexture.Dispose();
            WsHash = null;
            Token = null;
            Username = null;
            Password = null;
        }
    }
}

