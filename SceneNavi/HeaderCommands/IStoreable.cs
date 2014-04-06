using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.HeaderCommands
{
    interface IStoreable
    {
        void Store(byte[] databuf, int baseadr);
    }
}
