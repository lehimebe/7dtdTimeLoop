using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLoop.Functions
{
    [Serializable]
    public class PlayerData
    {
        public ClientInfo clientInfo;
        public bool skipTimeLoop;

        public PlayerData (ClientInfo clientInfo)
        {
            this.clientInfo = clientInfo;
            this.skipTimeLoop = false;
        }
    }
}
