using System;
using System.Collections.Generic;
using System.Linq;
using SharpNzb.Core.Model;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public class NntpProvider : INntpProvider
    {
        private INzbQueueProvider _nzbQueue;
        private IServerProvider _server;
        private List<ConnectionModel> _connections;
        private bool _isAlive;
        private long _speedLimit;

        public NntpProvider(INzbQueueProvider nzbQueue, IServerProvider server)
        {
            _nzbQueue = nzbQueue;
            _server = server;
            _connections = new List<ConnectionModel>();
        }

        #region INntpProvider Members

        public bool Connect()
        {
            if (IsAlive)
                return true;

            //Get Number of connections that should be created
            //Create a ConnectionModel 

            var servers = _server.GetEnabled().Where(s => !s.Backup);

            foreach (var server in servers)
            {
                for (int i = 0; i < server.Connections; i++)
                {
                    var connection = new ConnectionModel();
                    connection.Server = server.Hostname;
                    connection.Id = Guid.NewGuid();
                    connection.Connection = new NntpConnectionProvider(Guid.NewGuid());

                    if (connection.Connection.Connect(server.Hostname, server.Port, server.Ssl, server.Username, server.Password))
                    {
                        connection.Connection.ArticleFinished += new ArticleFinishedHandler(Connection_ArticleFinished);
                    }
                    _connections.Add(connection);
                }
            }

            if (_connections.Count > 0)
            {
                //Set _isAlive to true, calculate the speed limit for each connection, start downloading
                _isAlive = true;
                if (SpeedLimit > 0)
                {
                    var limit = CalculateSpeedLimit();
                    _connections.ForEach(c => SpeedLimit = limit);
                }

                _connections.ForEach(Download);
                return true;
            }

            return false;
        }

        public bool Disconnect()
        {
            if (_connections.Count < 1)
                return false;

            _connections.ForEach(c => c.Connection.Disconnect());
            _connections.Clear();
            _isAlive = false;

            return true;
        }

        public long Speed
        {
            get
            {
                //Get the current speed of each connection
                long speed = 0;
                _connections.ForEach(c => speed += c.Connection.Speed);
                return speed;
            }
        }

        public long SpeedLimit
        {
            get
            {
                //Get the speed limit for each connection and return the total value
                long limit = 0;
                _connections.ForEach(c => limit += c.Connection.SpeedLimit);
                return limit;
            }
            set
            {
                //Get Number of connections to get limit per connection and send that limit to each connection
                _speedLimit = value;

                var limit = CalculateSpeedLimit();
                _connections.ForEach(c => c.Connection.SpeedLimit = limit);
            }
        }

        public List<ConnectionModel> Connections()
        {
            return _connections;
        }

        public bool IsAlive
        {
            get { return _isAlive; }
        }

        #endregion

        private NzbSegmentModel GetNextArticle()
        {
            var queue = _nzbQueue.AllItems(); //Get the NZB Queue and store it

            var nzb = queue.Where(n => n.Status == NzbStatus.Queued || n.Status == NzbStatus.Downloading).FirstOrDefault(); //Get the first nzb where status is queued or downloading

            if (nzb == null)
                return null;

            var file = nzb.Files.Where(f => f.Status == NzbFileStatus.Queued || f.Status == NzbFileStatus.Downloading).FirstOrDefault(); //Get the first file from the nzb where status is queued or downloading

            if (file == null)
                return null;

            var segment = file.Segments.Where(s => s.Status == NzbSegmentStatus.Queued).FirstOrDefault(); //Get the first segment from the file where the status is queued
            file.Status = NzbFileStatus.Downloading; //Set the current file's status to Downloading
            nzb.Status = NzbStatus.Downloading; //Set the current Nzb's status to Downloading

            return segment;
        }

        private string GetGroup(NzbSegmentModel segment)
        {
            var queue = _nzbQueue.AllItems();

            foreach (var nzb in queue)
            {
                return (from f in nzb.Files where f.Name == segment.NzbFileName select f.Groups[0]).FirstOrDefault();
            }

            throw new NotImplementedException("NntpProvider - GetGroup");
        }

        private void Download(ConnectionModel connection)
        {
            //Get the next article to download
            var article = GetNextArticle();

            if (article == null)
            {
                connection.Connection.Disconnect();
                _connections.Remove(connection);
            }

            connection.Article = article;
            connection.Connection.GetArticle(article);
        }

        private void Connection_ArticleFinished(Guid connectionId)
        {
            var connection = _connections.Where(c => c.Id == connectionId).FirstOrDefault();
            var currentArticle = connection.Article;
            Download(connection);
            CheckIfFileHasDownloaded(currentArticle.NzbFileName);

        }

        private long CalculateSpeedLimit()
        {
            if (_connections.Count < 1)
                return 0;

            return _speedLimit / _connections.Count();
        }

        private void CheckIfFileHasDownloaded(string nzbFilename)
        {
            //var nzb = from f in _nzbQueue.AllItems() where f.Files.Contains(nzbFilename)  
        }
    }
}