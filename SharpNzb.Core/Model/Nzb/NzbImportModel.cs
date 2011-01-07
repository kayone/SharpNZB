using System;
using System.IO;

namespace SharpNzb.Core.Model.Nzb
{
    public class NzbImportModel
    {
        public string Location { get; set; } //Location of the NZB file (url, file on disk)
        public string Category { get; set; } //Category for the download
        public string Script { get; set; } //Script to run after post-processing finishes
        public string NewName { get; set; } //New Name for the NZB (Ignore the one from the filename/headers)
        public string Name { get; set; } //Stores the name of the NZB when Importing (Blank until It is processed)
        public int Priority { get; set; } //Priority that the NZB should be added in
        public int PostProcessing { get; set; } //What to do after the Download Finishes (Repair, Unpack Delete)
        public Stream Stream { get; set; } //Used to store the stream of the nzb
        public ImportType ImportType { get; set; } //Disk or Url Import

        public DateTime WaitUntil { get; set; } //DateTime to store CurrentTime +60 seconds to wait before trying that NZB again
        public int RetryCount { get; set; } //Store the number of retry counts for this NZB
    }
}
