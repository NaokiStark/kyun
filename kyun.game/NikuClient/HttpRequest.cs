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

        /// <summary>
        /// Cookie container
        /// </summary>
        public static CookieContainer CookiesJar
        {
            get
            {
                if (_cookies == null)
                    _cookies = new CookieContainer();
                return _cookies;
            }
        }

        /// <summary>
        /// Get Request
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>Task of string</returns>
        public static async Task<string> Get(string url, bool cookies = false)
        {
            using (var handler = new HttpClientHandler{
                CookieContainer = (cookies) ? CookiesJar : new CookieContainer()
            })
            {
                using (HttpClient client = new HttpClient(handler))
                    return await client.GetStringAsync(url);
            }            
        }

        /// <summary>
        /// Get Request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cookies"></param>
        /// <returns>Task of string</returns>
        public static async Task<string> Post(string url, HttpQueryString data = null, bool cookies = false)
        {
            using (var handler = new HttpClientHandler{
                CookieContainer = (cookies)?CookiesJar:new CookieContainer()
            })
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
