using System;
using SubSonic.SqlGeneration.Schema;

namespace SharpNzb.Core.Repository
{
    [SubSonicTableNameOverride("History")]
    public class History
    {
        [SubSonicPrimaryKey]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public TimeSpan DownloadTime { get; set; }
        public TimeSpan RepairTime { get; set; }
        public TimeSpan UnpackTime { get; set; }
        public int Category { get; set; }
        public string ScriptOutput { get; set; }
        public string FinalDirectory { get; set; }
    }
}
