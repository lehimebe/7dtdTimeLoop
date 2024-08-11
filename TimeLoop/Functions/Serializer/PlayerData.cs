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
        public string CrossplatformId;
        public bool SkipTimeLoop;

        public PlayerData (ClientInfo clientInfo)
        {
            this.CrossplatformId = clientInfo.CrossplatformId.CombinedString;
            this.SkipTimeLoop = false;
        }

        #region WORKAROUND
        public PlayerData(string crossplatformId, bool skipTimeLoop)
        {
            this.CrossplatformId = crossplatformId;
            this.SkipTimeLoop = skipTimeLoop;
        }
        #endregion
    }
}
