using SubSonic.SqlGeneration.Schema;

namespace SharpNzb.Core.Repository
{
    [SubSonicTableNameOverride("Config")]
    public class Config
    {
        [SubSonicPrimaryKey]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}