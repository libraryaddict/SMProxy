using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Plugins
{
    public interface IPlugin
    {
        void GlobalInitialize();
        void SessionInitialize(Proxy proxy);
    }
}
