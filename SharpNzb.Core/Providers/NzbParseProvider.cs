using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public class NzbParseProvider : INzbParseProvider
    {
        private ICategoryProvider _category;

        public NzbParseProvider(ICategoryProvider category)
        {
            _category = category;
        }

        #region IImportProvider Members

        public NzbModel Process(NzbImportModel import)
        {
            XNamespace ns = "http://www.newzbin.com/DTD/2003/nzb";
            import.Stream.Seek(0, SeekOrigin.Begin);
            XDocument xDoc = XDocument.Load(import.Stream);

            var nzb = from n in xDoc.Descendants(ns + "nzb") select n;

            if (nzb.Count() != 1)
                return null;

            NzbModel newNzb = new NzbModel();
            newNzb.Name = !String.IsNullOrEmpty(import.NewName) ? import.NewName : import.Name;
            newNzb.Id = Guid.NewGuid();
            newNzb.Status = NzbStatus.Queued;
            newNzb.Priority = (Priority)import.Priority;
            newNzb.Script = import.Script;
            newNzb.Category = import.Category;
            newNzb.PostProcessing = GetPostProcessing(import);

            var nzbFileList = new List<NzbFileModel>();

            //Get all the files for this NZB
            var files = from f in nzb.Elements(ns + "file") select f;
            foreach (var file in files)
            {
                var nzbFile = new NzbFileModel();
                nzbFile.Status = NzbFileStatus.Queued;
                nzbFile.NzbId = newNzb.Id;
                var segmentList = new List<NzbSegmentModel>();

                //Get the Age of the File and Convert to DateTime
                var date = Convert.ToInt64((from d in file.Attributes("date") select d.Value).FirstOrDefault());
                nzbFile.DatePosted = TicksToDateTime(date);

                //Get the Subject and set the NzbFile's Filename
                var subject = (from s in file.Attributes("subject") select s).FirstOrDefault();
                int fileNameStart = subject.Value.IndexOf("\"") + 1;
                int fileNameEnd = subject.Value.LastIndexOf("\"") - fileNameStart;
                nzbFile.Filename = subject.Value.Substring(fileNameStart, fileNameEnd);

                //Get the groups for the NzbFile
                nzbFile.Groups = (from g in file.Descendants(ns + "group") select g.Value).ToList();

                //Get the Segments for this file
                var segments = from s in file.Descendants(ns + "segment") select s;
                foreach (var segment in segments)
                {
                    var nzbFileSegment = new NzbSegmentModel();
                    nzbFileSegment.Status = NzbSegmentStatus.Queued;
                    nzbFileSegment.NzbFileName = nzbFile.Name;
                    nzbFileSegment.Number = Convert.ToInt32((from n in segment.Attributes("number") select n.Value).FirstOrDefault());
                    nzbFileSegment.Size = Convert.ToInt64((from n in segment.Attributes("bytes") select n.Value).FirstOrDefault());
                    nzbFileSegment.SegmentId = segment.Value;
                    segmentList.Add(nzbFileSegment);
                }
                nzbFile.Segments = segmentList;
                nzbFileList.Add(nzbFile);
            }
            newNzb.Files = nzbFileList;
            return newNzb;
        }
        #endregion

        private DateTime TicksToDateTime(long linuxDate)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(linuxDate);
        }

        private PostProcessing GetPostProcessing(NzbImportModel import)
        {
            if (Enum.IsDefined(typeof (PostProcessing), import.PostProcessing))
            {
                if (import.PostProcessing == -100)
                    return _category.Default().PostProcessing;

                return (PostProcessing)import.PostProcessing; //If not supplied use default for this category/default
            }

            if (!String.IsNullOrEmpty(import.Category))
            {
                var category = _category.Find(import.Category);

                if (category != null)
                {
                    var pp = category.PostProcessing;

                    if (pp != null)
                        return pp;
                }
            }

            return _category.Default().PostProcessing;
        }
    }
}
