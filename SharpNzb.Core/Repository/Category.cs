using SharpNzb.Core.Model.Nzb;
using SubSonic.SqlGeneration.Schema;

namespace SharpNzb.Core.Repository
{
    public class Category
    {
        [SubSonicPrimaryKey]
        public string Name { get; set; }
        public PostProcessing PostProcessing { get; set; }
        public Priority Priority { get; set; }
        [SubSonicNullString]
        public string Script { get; set; }
        public string Path { get; set; }
        [SubSonicNullString]
        public string Groups { get; set; }
    }
}
