using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using NLog;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public class NntpConnectionProvider : INntpConnectionProvider
    {
        private long _speed;
        private long _speedLimit;
        private long _byteCount; //Bytes transferred since the last throttle
        private long _start; //The start time of the last throttle
        private TcpClient _tcp;
        private Stream _stream;
        private Guid _connectionId;
        private Thread _connectionThread;
        private NzbSegmentModel _segment;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public NntpConnectionProvider(Guid connectionId)
        {
            _connectionId = connectionId;
        }

        #region INntpConnectionProvider Members

        public event ArticleFinishedHandler ArticleFinished;

        public bool Connect(string hostname, int port, bool ssl, string username, string password)
        {
            _tcp = new TcpClient(hostname, port);

            if (!ssl)
            {
                NetworkStream networkStream = _tcp.GetStream();
                _stream = networkStream;
            }
            else
            {
                SslStream sslStream = new SslStream(_tcp.GetStream(), false,
                                                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                                                    null);

                try
                {
                    sslStream.AuthenticateAsClient(hostname);
                    //The server name must match the name on the server certificate.
                }
                catch (AuthenticationException e)
                {
                    //  who cares if it's not confirmed? it's still encrypted between you and server!


                    //Console.WriteLine("Exception: {0}", e.Message);
                    if (e.InnerException != null)
                    {
                        //Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                    }
                    //Console.WriteLine("Authentication failed - closing the connection.");
                    //tcp.Close();
                    //return;
                }

                _stream = sslStream;
            }

            //Read the Welcome Message
            string startNntp = ReadLine();
            if (!startNntp.StartsWith("200"))
                return false;

            if (username != null && password != null)
            {
                if (!AuthenticateUser(username, password))
                {
                    _stream.Close();
                }
            }

            return true;
        }

        public void Disconnect()
        {
            SendBytes("QUIT\r\n", _stream);
        }

        public void GetArticle(NzbSegmentModel segment)
        {
            _segment = segment;
            _connectionThread = new Thread(DoGetArticle)
            {
                Name = "ConnectionThread " + _connectionId,
                Priority = ThreadPriority.Normal
            };

            _start = CurrentMilliseconds;
            _connectionThread.Start();
        }

        public long SpeedLimit
        {
            get { return _speedLimit; }
            set { _speedLimit = value; }
        }

        public long Speed
        {
            get { return _speed; }
        }

        #endregion

        private void DoGetArticle()
        {
            //Time the download
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //If SendGroupBeforeDownload is true, then listen
            if (!SelectGroup(_segment.Group))
            {
                Logger.Debug("Failed to Select the Group - Terminating Connection");
                throw new NotImplementedException("NntpConnectionProvider - Select Group");
            }

            SendCommand("BODY <" + _segment.SegmentId + ">");
            ServerResponse(_segment, "222");
            sw.Stop();
            _segment.DownloadTime = sw.ElapsedMilliseconds;
        }

        private bool AuthenticateUser(string username, string password)
        {
            string r = null;

            r = ShortCmd("AUTHINFO USER " + username);
            if (r.StartsWith("381 "))
            {
                r = ShortCmd("AUTHINFO PASS " + password);

                if (r.StartsWith("281 "))
                {
                    return true;
                }
            }
            else if (r.StartsWith("281 "))
            {
                return true;
            }

            return false;
        }

        private string ShortCmd(string cmd)
        {
            SendBytes(cmd + "\r\n", _stream);

            string r = new StreamReader(_stream).ReadLine();        // store for other commands just incase
            return r;
        }

        public bool SelectGroup(string groupname)
        {
            string r = ShortCmd("GROUP " + groupname);

            return true;
        }

        private string ReadLine()
        {
            var sr = new StreamReader(_stream, Encoding.GetEncoding("iso-8859-1"), false, 1024 * 5);
            return sr.ReadLine();
        }

        private long CurrentMilliseconds
        {
            get
            {
                return Environment.TickCount;
            }
        }

        //The actual throttling is done here
        private void Throttle(int bufferSizeInBytes)
        {
            // Make sure the buffer isn't empty.
            if (_speedLimit <= 0 || bufferSizeInBytes <= 0)
            {
                return;
            }

            _byteCount += bufferSizeInBytes;
            long elapsedMilliseconds = CurrentMilliseconds - _start;

            if (elapsedMilliseconds > 0)
            {
                // Calculate the current bps.
                long bps = _byteCount * 1000L / elapsedMilliseconds;
                _speed = bps; //Set the speed of this connecttion to bps (the calculated speed)

                // If the bps are more then the maximum bps, try to throttle.
                if (bps > _speedLimit)
                {
                    // Calculate the time to sleep.
                    long wakeElapsed = _byteCount * 1000L / _speedLimit;
                    int toSleep = (int)(wakeElapsed - elapsedMilliseconds);

                    if (toSleep > 1)
                    {
                        try
                        {
                            // The time to sleep is more then a millisecond, so sleep.
                            Thread.Sleep(toSleep);
                        }
                        catch (ThreadAbortException)
                        {
                            // Eatup ThreadAbortException.
                        }

                        // A sleep has been done, reset.
                        Reset();
                    }
                }
            }
        }

        //Resets the bytecount to 0 and reset the start time to the current time.
        private void Reset()
        {
            long difference = CurrentMilliseconds - _start;

            // Only reset counters when a known history is available of more then 1 second.
            if (difference > 1000)
            {
                _byteCount = 0;
                _start = CurrentMilliseconds;
            }
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
                                              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // Do not allow this client to communicate with unauthenticated servers.
            // Allow anyways for now
            return true;
        }

        private void SendCommand(string line)
        {
            SendBytes(line + "\r\n", _stream);
        }

        private void SendBytes(string bytes, Stream stream)
        {
            byte[] send = S2B(bytes);
            stream.Write(send, 0, send.Length);
            stream.Flush();
        }

        private static byte[] S2B(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private void ServerResponse(NzbSegmentModel segment, string expect = null)
        {
            byte[] buffer = new byte[1024 * 3];
            int i = 0;
            var bytes = -1;

            segment.Storage = new MemoryStream();

            do
            {
                i += 1;
                bytes = _stream.Read(buffer, 0, buffer.Length); // read from server with buffer
                segment.Storage.Write(buffer, 0, bytes); // write network buffer to memorystream

                if (i == 1 && expect != null)
                {
                    if (!Encoding.ASCII.GetString(buffer).StartsWith(expect))
                    {
                        segment.Storage.Close();
                        return;
                    }
                }
                Throttle(buffer.Length);
            } while (StillDownloading(ref bytes, ref buffer));

            if (segment.Storage.Length > 5)
            {
                segment.Storage.SetLength(segment.Storage.Length - 5); //Trim out "\r\n/\r\n" from end of Stream 
                segment.Storage.Position = 0; //Set Position to 0 before using, or Stream will be incomplete.

                //Testing
                SaveMemoryStream(segment.Storage, "storage\\" + segment.SegmentId);
                segment.Storage.Position = 0; //Set Position to 0 before using, or Stream will be incomplete.
                //End Testing

                segment.Status = NzbSegmentStatus.Downloaded;
                OnArticleFinished(); //Trigger the Event saying this connection is finished with the current segment
            }
        }

        private static bool StillDownloading(ref int bytes, ref byte[] buffer)
        {
            if (bytes == 0)
                return false;

            if (bytes >= 5)
            {
                if ((buffer[bytes - 5] == 13 && buffer[bytes - 4] == 10 && buffer[bytes - 3] == 46 &&
                     buffer[bytes - 2] == 13 && buffer[bytes - 1] == 10))
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual void OnArticleFinished()
        {
            if (ArticleFinished != null)
                ArticleFinished(_connectionId);
        }

        //Used for Testing
        public void SaveMemoryStream(MemoryStream ms, string fileName)
        {
            using (var outStream = File.OpenWrite(fileName))
            {
                ms.WriteTo(outStream);
            }
        }
    }
}
