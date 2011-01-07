using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;
using SharpNzb.Core.Helper;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public class YencProvider : IYencProvider
    {
        private IDiskProvider _disk;

        private List<NzbFileModel> _list;
        private Thread _decodeThread;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public YencProvider(IDiskProvider disk)
        {
            _disk = disk;
            _list = new List<NzbFileModel>();
        }

        #region IYencProvider Members

        public void Decode(NzbFileModel file)
        {
            //Add to Queue and then Start Processing
            _list.Add(file);
            file.Status = NzbFileStatus.DecodeQueued;

            Logger.Debug("NzbFile to be Decoded: {0}", file.Name);
            if (_decodeThread == null || !_decodeThread.IsAlive)
            {
                Logger.Debug("Initializing background decode of NzbFiles.");
                _decodeThread = new Thread(StartDecode)
                {
                    Name = "Decode NzbFile",
                    Priority = ThreadPriority.Lowest
                };

                _decodeThread.Start();
            }
            else
            {
                Logger.Warn("NzbFile decoding already in progress");
            }
        }

        #endregion

        private void StartDecode()
        {
            var file = _list.GetRange(0, 1).First(); //Get the nzbImport at the index found above
            _list.RemoveAt(0);
            file.Status = NzbFileStatus.Decoding;

            FileStream output = null;
            StreamReader input = null;

            string line;
            string decoder = "";
            string sdecoder = "";

            string outputfile = "";
            long outputsize = -1;

            bool crcfailed = false;
            Crc32Helper crc = new Crc32Helper();

            int decodedsegments = 0;

            foreach (NzbSegmentModel segment in file.Segments)
            {
                if (segment.Storage == null) //Make sure Storage isn't null... Null Exception Avoidance
                    continue;

                // Check if the storage is 0 (It will be empty then)
                if (segment.Storage.Length == 0)
                    continue;

                input = new StreamReader(segment.Storage, Encoding.GetEncoding("iso-8859-1"));

                // If uudecode is used, each file is automaticly a new segment
                if (decoder == "uudecode" || (decoder == "mime" && sdecoder == "base64"))
                    decodedsegments++;

                line = input.ReadLine();
                while (line != null)
                {
                    if (decoder == "mime")
                    {
                        if (line.StartsWith("Content-Transfer-Encoding:"))
                            sdecoder = line.Remove(0, 27);

                        if (line.StartsWith("--=") || line.StartsWith("Content-Type:"))
                        {
                            decoder = "";
                            sdecoder = "";

                            outputfile = "";

                            output.Close();
                            output = null;
                        }

                        // Perhaps get filename out of this, but its also in the content type
                        if (line.StartsWith("Content-Disposition:"))
                            line = "";

                        if (sdecoder == "base64")
                        {
                            if (line.Length % 4 > 0)
                                line = "";

                            if (line != "")
                            {
                                byte[] buffer;
                                buffer = Convert.FromBase64String(line);

                                output.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }

                    if (decoder == "uudecode")
                    {
                        if (line != "" && line != "end")
                        {
                            char[] buffer = line.ToCharArray();
                            if (uudecode_checkline(buffer))
                            {
                                int p = 0;
                                int n = 0;
                                byte ch;

                                n = uudecode_dec(buffer[p]);
                                for (++p; n > 0; p += 4, n -= 3)
                                {
                                    if (n >= 3)
                                    {
                                        // Error ?
                                        if (!(uudecode_is_dec(buffer[p]) && uudecode_is_dec(buffer[p + 1]) && uudecode_is_dec(buffer[p + 2]) && uudecode_is_dec(buffer[p + 3])))
                                            throw new Exception("33");

                                        ch = (byte)(uudecode_dec(buffer[p + 0]) << 2 | uudecode_dec(buffer[p + 1]) >> 4);
                                        output.WriteByte(ch);
                                        ch = (byte)(uudecode_dec(buffer[p + 1]) << 4 | uudecode_dec(buffer[p + 2]) >> 2);
                                        output.WriteByte(ch);
                                        ch = (byte)(uudecode_dec(buffer[p + 2]) << 6 | uudecode_dec(buffer[p + 3]));
                                        output.WriteByte(ch);

                                    }
                                    else
                                    {
                                        if (n >= 1)
                                        {
                                            if (!(uudecode_is_dec(buffer[p]) && uudecode_is_dec(buffer[p + 1])))
                                                throw new Exception("34");

                                            ch = (byte)(uudecode_dec(buffer[p + 0]) << 2 | uudecode_dec(buffer[p + 1]) >> 4);
                                            output.WriteByte(ch);
                                        }
                                        if (n >= 2)
                                        {
                                            if (!(uudecode_is_dec(buffer[p + 1]) && uudecode_is_dec(buffer[p + 2])))
                                                throw new Exception("35");

                                            ch = (byte)(uudecode_dec(buffer[p + 1]) << 4 | uudecode_dec(buffer[p + 2]) >> 2);
                                            output.WriteByte(ch);
                                        }
                                        if (n >= 3)
                                        {
                                            if (!(uudecode_is_dec(buffer[p + 2]) && uudecode_is_dec(buffer[p + 3])))
                                                throw new Exception("36");

                                            ch = (byte)(uudecode_dec(buffer[p + 2]) << 6 | uudecode_dec(buffer[p + 3]));
                                            output.WriteByte(ch);
                                        }
                                    }
                                }
                            }
                        }

                        if (line == "end")
                        {
                            decoder = "";
                            outputfile = "";

                            output.Close();
                            output = null;
                        }
                    }

                    if (decoder == "yenc")
                    {
                        if (line.StartsWith("=ypart "))
                        {
                            // Part description
                            string[] ypart = line.Split(" ".ToCharArray());
                            foreach (string s in ypart)
                            {
                                if (s.StartsWith("begin"))
                                {
                                    output.Seek(long.Parse(s.Remove(0, 6)) - 1, SeekOrigin.Begin);
                                }
                            }
                        }
                        else
                        {
                            if (line.StartsWith("=yend "))
                            {
                                // End of the Yenc part, do CRC check
                                decoder = "";

                                string[] yend = line.Split(" ".ToCharArray());
                                foreach (string s in yend)
                                {
                                    if (s.StartsWith("pcrc32"))
                                    {
                                        long opcrc = Convert.ToInt64(s.Remove(0, 7), 16);
                                        long cpcrc = crc.EndByteCRC();

                                        if (opcrc != cpcrc)
                                            crcfailed = true;
                                    }
                                }
                                decodedsegments++;

                                if (outputsize == output.Length)
                                    outputsize = -1;

                                if (outputsize == -1)
                                {
                                    output.Close();
                                    output = null;

                                    outputfile = "";
                                }
                            }
                            else
                            {
                                // Yenc Encoded part
                                bool escape = false;

                                if (line.StartsWith(".."))
                                    line = line.Remove(0, 1);

                                foreach (char c in line.ToCharArray())
                                {
                                    if (c == '=' && !escape)
                                    {
                                        escape = true;
                                    }
                                    else
                                    {
                                        byte nc = (byte)c;
                                        if (escape)
                                        {
                                            nc = (byte)(nc - 64);
                                            escape = false;
                                        }

                                        nc = (byte)(nc - 42);
                                        output.WriteByte(nc);
                                        crc.AddByteCRC(nc);
                                    }
                                }
                            }
                        }
                    }

                    if (decoder == "")
                    {
                        if (line.StartsWith("=ybegin "))
                        {
                            decoder = "yenc";
                            crc.StartByteCRC();

                            // Check if its a valid ybegin line, as per 1.2 line, size and name have to be present
                            if (line.IndexOf("line=") != -1 && line.IndexOf("size=") != -1 && line.IndexOf("name=") != -1)
                            {
                                int b, e;
                                b = line.IndexOf("size=");
                                e = line.IndexOf(" ", b);
                                outputsize = long.Parse(line.Substring(b + 5, e - b - 5));

                                b = line.IndexOf("name=");
                                if (outputfile != line.Substring(b + 5))
                                {
                                    outputfile = line.Substring(b + 5);
                                    if (file.Filename == "")
                                        file.Filename = outputfile;
                                    //why add second filename?
                                    //article.Filename = article.Filename + outputfile;

                                    //string path = GetDirectory(article);

                                    try
                                    {
                                        if (!_disk.FolderExists(Path.GetFullPath(file.Path)))
                                            _disk.CreateDirectory(Path.GetFullPath(file.Path));
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.ErrorException("Unable to create directory [" + file.Path + "]", ex);
                                    }

                                    if (output != null)
                                    {
                                        output.Close();
                                        output = null;
                                    }

                                    output = new FileStream(Path.GetFullPath(file.Path) + "\\" + outputfile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 1024 * 1024);
                                }
                            }
                        }

                        if (line.StartsWith("begin 644 "))
                        {
                            decodedsegments++;

                            decoder = "uudecode";

                            outputfile = line.Remove(0, 10);
                            if (file.Filename == "")
                                file.Filename = outputfile;
                            //why add second file name?
                            //article.Filename = article.Filename + outputfile;

                            //string path = GetDirectory(article);

                            try
                            {
                                if (!_disk.FolderExists(Path.GetFullPath(file.Path)))
                                    _disk.CreateDirectory(Path.GetFullPath(file.Path));
                            }
                            catch (Exception ex)
                            {
                                throw (ex);
                            }

                            if (output != null)
                            {
                                output.Close();
                                output = null;
                            }

                            output = new FileStream(Path.GetFullPath(file.Path) + "\\" + outputfile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 1024 * 1024);
                        }

                        if (line.StartsWith("Content-Type: application/octet-stream;"))
                        {
                            decodedsegments++;

                            decoder = "mime";
                            sdecoder = "";

                            outputfile = line.Substring(line.IndexOf("name=") + 5);
                            if (outputfile[0] == '\"' && outputfile[outputfile.Length - 1] == '\"')
                                outputfile = outputfile.Substring(1, outputfile.Length - 2);

                            if (outputfile[0] == '\'' && outputfile[outputfile.Length - 1] == '\'')
                                outputfile = outputfile.Substring(1, outputfile.Length - 2);

                            if (file.Filename == "")
                                file.Filename = outputfile;
                            //why add second file name?
                            //article.Filename = article.Filename + outputfile;

                            //string path = GetDirectory(article);

                            try
                            {
                                if (!_disk.FolderExists(Path.GetFullPath(file.Path)))
                                    _disk.CreateDirectory(Path.GetFullPath(file.Path));
                            }
                            catch (Exception ex)
                            {
                                //frmMain.LogWriteError("Unable to create directory [" + path + "]");
                                throw (ex);
                            }

                            if (output != null)
                            {
                                output.Close();
                                output = null;
                            }

                            output = new FileStream(Path.GetFullPath(file.Path) + "\\" + outputfile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 1024 * 1024);
                        }
                    }

                    line = input.ReadLine();
                }

                input.Close();
                input = null;

                if (output != null)
                    output.Flush();

                segment.Status = NzbSegmentStatus.Decoded;
                segment.Storage.Close();
                segment.Storage = null; //Clear out the MemoryStream so we get our RAM back
            }

            if (output != null)
            {
                output.Close();
                output = null;
            }

            if (crcfailed || decodedsegments == 0)
            {
                file.Status = NzbFileStatus.DecodeFailed;
                return;
            }

            file.Status = NzbFileStatus.Decoded; //File Decoded Properly
            return;
        }

        private int uudecode_dec(char c)
        {
            int result = ((c - 0x20) & 0x3F);
            return result;
        }

        private bool uudecode_is_dec(char c)
        {
            return (((c - 0x20) >= 0) && ((c - 0x20) <= (0x3F + 1)));
        }

        private bool uudecode_checkline(char[] buffer)
        {
            int n = 0;

            n = buffer[0] - 0x20;

            if (n <= 0)
                return false;

            if (n > 45) // max size
                return false;

            if (((n + 1) / 3) * 4 > (buffer.Length - 1))
                return false;

            foreach (char c in buffer)
                if (!uudecode_is_dec(c))
                    return false;

            return true;
        }
    }
}
