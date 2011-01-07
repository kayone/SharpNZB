using SubSonic.SqlGeneration.Schema;

namespace SharpNzb.Core.Repository
{
    public class Server
    {
        [SubSonicPrimaryKey]
        public string Hostname { get; set; }
        public int Port { get; set; }
        public int Connections { get; set; }
        public bool Ssl { get; set; }
        public bool Enabled { get; set; }
        public int Retention { get; set; } 
        public bool Backup { get; set; }
        public bool Optional { get; set; }
        public double Downloaded { get; set; } //Data Downloaded in KB
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
