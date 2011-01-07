using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpNzb.Core.Model.Nzb
{
    public class NzbModel
    {
        //Contains all the Files (Sorted List) for the NZB (once imported)
        //Location of files etc

        public Guid Id { get; set; } //Unique ID for this NZB (Guid seems to make the most sense?)
        public string Name { get; set; }
        public NzbStatus Status { get; set; } //Queued, Downloading, Paused, Verifying, Repairing, Extracting, Running Script
        public PostProcessingStatus PostProcessingStatus { get; set; } //Store whether or not Post Processing was successful.
        public string Category { get; set; } //What category does this NZB belong to? TV, Movies, etc
        public string Script { get; set; } //Which Script will be run on completion
        public string ScriptOutput { get; set; } //The output of the script (written to the command line)
        public Priority Priority { get; set; } //Priority in which to download the NZB (low, normal, high, forced)
        public PostProcessing PostProcessing { get; set; } //What actions to Perform after Post Processing?
        public string DownloadDirectory { get; set; } //Where are the files being stored while downloading?
        public string FinalDirectory { get; set; } //Where will the files be unpacked to (or copied if unpack fails)
        public string Par2File { get; set; } //Store the name of the smallest par2name here...
        public string RarFile { get; set; } //Store the name of the main .RAR file here

        public long Size //In Bytes
        {
            get
            {
                var size = from s in Files select s.Size;
                return size.Sum();
            }
        }
        public TimeSpan Age
        {
            get
            {
                //Get the Age of all Files, calculate Average Age

                var files = from f in Files select f.Age;
                var minutes = files.Sum(s => s.TotalMinutes); //Get age in Minutes
                return new TimeSpan(0, 0, Convert.ToInt32(minutes)/Files.Count, 0); //return TimeSpan made from all Files ages in minutes divided by number of files
            }
        }

        public long DownloadTime //Download time in ms
        {
            get
            {
                var time = from t in Files select t.DownloadTime;
                return time.Sum();
            }

        }

        public long Remaining { get; set; } //Size of download left
        public double Progress { get; set; } //What percent has been completed
        public double RepairProgress { get; set; } //Store the Percentage of the Repair Process
        public double UnpackProgress { get; set; } //Store the Percentage of the Unpack Process
        public int UnpackFileCount { get; set; } //Store which file number is currently being unpacked
        public List<NzbFileModel> Files { get; set; }

        //This is needed for telerik grid binding
        public string StatusLevel
        {
            get { return Status.ToString(); }
        }

        //Used for Post Processing
        public string ShowName { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string EpisodeName { get; set; }
    }
}
