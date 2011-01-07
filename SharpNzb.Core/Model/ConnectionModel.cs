using System;
using SharpNzb.Core.Model.Nzb;
using SharpNzb.Core.Providers;

namespace SharpNzb.Core.Model
{
    public class ConnectionModel
    {
        //Contains information about connections
        public Guid Id { get; set; }
        public string Server { get; set; }
        public NzbSegmentModel Article { get; set; }
        public long Speed { get; set; }
        public INntpConnectionProvider Connection { get; set; }
    }
}
