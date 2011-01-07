using System.Collections.Generic;
using SharpNzb.Core.Model;

namespace SharpNzb.Core.Providers
{
    public interface INntpProvider
    {
        bool Connect();
        bool Disconnect();
        long Speed { get; }
        long SpeedLimit { get; set; }
        List<ConnectionModel> Connections();
        bool IsAlive { get; }
    }
}
