using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.NikuClient
{
    public class HttpQueryString : List<KeyValuePair<string, string>>
    {
        public HttpQueryString AddQuery(string key, string value)
        {
            this.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public string BuildQuery()
        {
            string temp = "";

            int i = 0;

            foreach(KeyValuePair<string, string> data in this)
            {
                temp += $"{data.Key}={data.Value}";

                if (i != this.Count - 1)
                    temp += "&";
                i++;
            }

            return temp;
        }

        public byte[] GetUTF8Bytes()
        {
            string tmp = BuildQuery();
            
            return Encoding.UTF8.GetBytes(tmp);
        }

        public byte[] GetAsciiBytes()
        {
            string tmp = BuildQuery();

            return Encoding.ASCII.GetBytes(tmp);
        }
    }
}
