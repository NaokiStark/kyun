using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.NikuClient
{
    public class ServerList : List<KeyValuePair<int, string>>
    {
        public int lastCheck = 0;

        public void AddServer(string Server)
        {
            this.Add(new KeyValuePair<int, string>(this.Count, Server));
        }

        public KeyValuePair<int, string> GetServer(int id)
        {
            return this[id];
        }
    }
}
