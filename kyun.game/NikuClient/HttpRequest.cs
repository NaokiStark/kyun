using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.NikuClient
{
    public class HttpRequest
    {
        static CookieContainer _cookies = null;
        public static CookieContainer CookiesJar {
            get
            {
                if (_cookies == null)
                    _cookies = new CookieContainer();
                return _cookies;
            }
        }

        public static async Task<string> Get(string url)
        {            
            using (var handler = new HttpClientHandler() { /*CookieContainer = CookiesJar*/ })
                using (HttpClient client = new HttpClient(handler))
                    return await client.GetStringAsync(url);                            
        }

        public static async Task<string> Post(string url, HttpQueryString data = null)
        {           
            using (var handler = new HttpClientHandler() { /*CookieContainer = CookiesJar*/ })
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    HttpContent content = new FormUrlEncodedContent(data?.ToArray());
                    var result = await client.PostAsync(url, content);

                    return await result.Content.ReadAsStringAsync();
                }
            }            
        }
    }
}
