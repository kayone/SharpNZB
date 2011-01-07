using System.IO;

namespace SharpNzb.Core.Model.Nzb
{
    public class NzbSegmentModel
    {
        public string SegmentId { get; set; }
        public int Number { get; set; }
        public long Size { get; set; }
        public long DownloadTime { get; set; } //Download time in ms
        public NzbSegmentStatus Status { get; set; }
        public MemoryStream Storage { get; set; }
        public string NzbFileName { get; set; } //Stores the name of the parent NzbFile
        public string Group { get; set; }
    }
}
