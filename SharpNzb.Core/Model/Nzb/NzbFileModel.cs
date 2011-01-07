using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpNzb.Core.Model.Nzb
{
    public class NzbFileModel
    {
        //Need to support sorting (Auto and manual)

        public string Name { get; set; }
        public NzbFileStatus Status { get; set; }
        public string Filename { get; set; } //Used for Decoding...
        public string NzbName { get; set; } //Used for Decoding
        public DateTime DatePosted { get; set; }
        public Guid NzbId { get; set; } //Stores the ID of the parent Nzb
        public string Path { get; set; } //Stores the path of the file (where it was decoded to)

        public long Size //In Bytes
        {
            get
            {
                var size = from s in Segments select s.Size;
                return size.Sum();
            }
        }

        public long DownloadTime //Download time in ms
        {
            get
            {
                var time = from t in Segments select t.DownloadTime;
                return time.Sum();
            }
        }

        public TimeSpan Age
        {
            get
            {
                return DateTime.Now.Subtract(DatePosted);
            }
        } //TimeSpan between DateTime.Now and DatePosted
        public List<string> Groups { get; set; } //Groups that this NzbFile has been Posted To
        public List<NzbSegmentModel> Segments { get; set; }

        public long Remaining
        {
            get
            {
                var finished = from s in Segments where s.Status >= NzbSegmentStatus.Downloaded select s.Size; //Select all segments where the Status is Downloaded or Greater
                long completedSize = finished.Sum();
                return Size - completedSize; //Return total size minus size already downloaded (in bytes)
            }
        }

        public long Downloaded
        {
            get
            {
                var finished = from s in Segments where s.Status >= NzbSegmentStatus.Downloaded select s.Size; //Select all segments where the Status is Downloaded or Greater
                return finished.Sum(); //Return Total downloaded in bytes
            }
        }

        public double Progress
        {
            get
            {
                //Get Total Number of Segments completed
                var finished = from s in Segments where s.Status >= NzbSegmentStatus.Downloaded select s.Size; //Select all segments where the Status is Downloaded or Greater

                //Store the size of completed files
                long completedSize = finished.Sum();
                return completedSize/Size*100; //Return the size of already downloaded by total size * 100 (as percentage)
            }


        }
    }
}
