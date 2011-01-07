using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NLog;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public class NzbImportProvider : INzbImportProvider
    {
        private IDiskProvider _disk;
        private IHttpProvider _http;
        private IDecompressProvider _decompress;
        private INzbParseProvider _parse;
        private INzbQueueProvider _queue;
        private Thread _importThread;
        private List<NzbImportModel> _list;
        private INntpProvider _nntp;
        private IPreQueueProvider _preQueue;
        private IConfigProvider _config;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private object _lock = new object();

        public NzbImportProvider(IDiskProvider disk, IHttpProvider http, IDecompressProvider decompress, INzbParseProvider parse, INzbQueueProvider queue, INntpProvider nntp, IPreQueueProvider preQueue, IConfigProvider config)
        {
            _disk = disk;
            _http = http;
            _decompress = decompress;
            _parse = parse;
            _queue = queue;
            _list = new List<NzbImportModel>();
            _nntp = nntp;
            _preQueue = preQueue;
            _config = config;
        }

        #region IImportProvider Members
        public void BeginImport(NzbImportModel import)
        {
            //Add to Queue and then Start Processing
            _list.Add(import);

            Logger.Debug("NZB to be Imported: {0}", import.Location);
            if (_importThread == null || !_importThread.IsAlive)
            {
                Logger.Debug("Initializing background import of NZBs.");
                _importThread = new Thread(Import)
                {
                    Name = "ImportNzbs",
                    Priority = ThreadPriority.Lowest
                };

                _importThread.Start();
            }
            else
            {
                Logger.Warn("NZB Importing already in Progress");
            }

        }
        #endregion

        private void Import()
        {
            try
            {
                if (_list.Count < 1) //If less than 1 in queue, return
                    return;

                while (_list.Count > 0) //While there are still NZBs in the Import Queue, keep going
                {
                    var import = GrabNext();

                    if (import == null)
                    {
                        Thread.Sleep(30000); //Sleep for 30 seconds (All NzbImportModels must be in a waiting state)
                        continue;
                    }

                    if (import.ImportType == ImportType.Url)
                    {
                        var status = AddUrl(import);
                        if  (status == ImportStatus.Failed)
                        {
                            if (import.RetryCount == 4)
                            {
                                throw new NotImplementedException("Add Url - Max Attempts Reached");
                                //Max attempts reached, add to history as failed...
                                //Todo: Add to history as failed
                            }

                            import.RetryCount++;
                            import.WaitUntil = DateTime.Now.AddSeconds(30); //Do not try again for 30 seconds
                            _list.Add(import); //Re-add this to try again later

                            if (_list.Count == 1)
                                Thread.Sleep(30000);

                        }

                        if (status == ImportStatus.Invalid)
                        {
                            throw new NotImplementedException("Add Url - Bad NZB");
                            //Todo: Add this to history as failed
                        }
                    }

                    else
                    {
                        var localStatus = AddLocalFile(import);

                        if (localStatus == ImportStatus.Failed)
                        {
                            //Todo: Handle failed local file import
                            throw new NotImplementedException("Add Local File - Failure");
                        }

                        if (localStatus == ImportStatus.Invalid)
                        {
                            //Todo: Handle invalid local file import
                            throw new NotImplementedException("Add Local File - Invalid NZB");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex.Message, ex);
            }
        }

        private NzbImportModel GrabNext()
        {
            lock (_lock)
            {
                var index = _list.FindIndex(0, i => i.WaitUntil < DateTime.Now); //Find the first NzbImportModel from _list where WaitUntil has been passed

                if (index < 0) //return false if the idect could not be found
                    return null;

                var nzb = _list.GetRange(index, 1).First(); //Get the nzbImport at the index found above
                _list.RemoveAt(index); //Remove the nzbImportModel

                return nzb;
            }
        }

        private ImportStatus AddLocalFile(NzbImportModel import)
        {
            //Determine if the file is a RAW NZB (or is zipped, gzipped, rarred) and then Import

            if(!_disk.FileExists(import.Location))
            {
                Logger.Error("NZB: {0} does not exist", import.Location);
                return ImportStatus.Invalid;
            }

            import.Name = _disk.SimpleFilename(import.Location); //Get the Filename without path or Extension
            import.Stream = _disk.OpenAsStream(import.Location); //Get the NZB as a stream

            if (import.Stream == null)
            {
                return ImportStatus.Failed;
            }

            var nzb = _parse.Process(import);

            if (nzb == null) //If its null return
                return ImportStatus.Invalid;

            //Run PreQueue if one is set
            if (!String.IsNullOrEmpty(_config.GetValue("PreQueueScript", String.Empty, false)))
            {
                if (!_preQueue.Run(nzb)) return ImportStatus.Rejected; //Return rejected status if NZB is not wanted due to PreQueue
            }

            var position = _queue.AllItems().Count(q => (int)q.Priority >= (int)import.Priority); //Find all items with a higher or equal priority and insert it at that point (zero-indexed list mataches perfectly)
            _queue.Insert(nzb, position); //Do the insert!
            _nntp.Connect(); //Start Downloading if not already doing so

            return ImportStatus.Ok;
        }

        private ImportStatus AddUrl(NzbImportModel import)
        {
            //Determine if the file is a RAW NZB (or is zipped, gzipped, rarred) and then Import
            _http.DownloadAsStream(import);

            if (import.Stream == null)
            {
                import.RetryCount++;
                import.WaitUntil = DateTime.Now.AddMinutes(1);

                return ImportStatus.Failed;
            }

            var nzb = _parse.Process(import);

            if (nzb == null) //If its null return
                return ImportStatus.Invalid;

            //Run PreQueue if one is set
            if (!String.IsNullOrEmpty(_config.GetValue("PreQueueScript", String.Empty, false)))
            {
                if (!_preQueue.Run(nzb)) return ImportStatus.Rejected; //Return rejected status if NZB is not wanted due to PreQueue
            }

            var position = _queue.AllItems().Count(q => (int)q.Priority >= (int)import.Priority); //Find all items with a higher or equal priority and insert it at that point (zero-indexed list mataches perfectly)
            _queue.Insert(nzb, position); //Do the insert!
            _nntp.Connect(); //Start Downloading if not already doing so

            return ImportStatus.Ok;
        }
    }
}
