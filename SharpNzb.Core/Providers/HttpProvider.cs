using System;
using System.IO;
using System.Net;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public class HttpProvider : IHttpProvider
    {
        public void DownloadAsStream(NzbImportModel nzb)
        {
            WebClient wc = new WebClient();
            Stream nzbStream = wc.OpenRead(nzb.Location);
            WebHeaderCollection whc = wc.ResponseHeaders;

            if (GetConnectionResponse(whc))
            {
                nzb.Name = GetRemoteNzbFilename(whc);

                if (String.IsNullOrEmpty(nzb.Category))
                    nzb.Category = GetRemoteNzbCategory(whc);

                nzb.Stream = StreamToMemoryStream(nzbStream);
                nzbStream.Close();
            }
        }

        private bool GetConnectionResponse(WebHeaderCollection whc)
        {
            try
            {
                foreach (string key in whc.Keys)
                {
                    string value = whc.GetValues(key)[0];

                    if (key == "Connection")
                    {
                        if (value.Contains("close")) //Look if the connection was closed (NZB was not returned) - NZBMatrix returns close for Valid Requests
                            return false; //Will this always work??

                        return true; //Return true
                    }
                }
            }

            catch (Exception)
            {
                return false;
            }
            return false;
        }

        private string GetRemoteNzbFilename(WebHeaderCollection whc)
        {
            try
            {
                foreach (string key in whc.Keys)
                {
                    string value = whc.GetValues(key)[0];

                    if (key == "X-DNZB-Name")
                    {
                        return value;
                    }

                    if (key == "Content-Disposition")
                    {
                        int start = value.IndexOf("filename=") + 9;
                        int end = value.IndexOf(".nzb");
                        int length = end - start;

                        return value.Substring(start, length).TrimStart('\"', ' ').TrimEnd('\"', ' ');
                    }
                }
            }

            catch (Exception)
            {
                return "unknown";
            }
            return "unknown";
        }

        private string GetRemoteNzbCategory(WebHeaderCollection whc)
        {
            try
            {
                foreach (string key in whc.Keys)
                {
                    string value = whc.GetValues(key)[0];

                    if (key == "X-DNZB-Category")
                    {
                        return value;
                    }
                }
            }

            catch (Exception)
            {
                return String.Empty;
            }
            return String.Empty;
        }

        private MemoryStream StreamToMemoryStream(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] chunk = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) > 0)
            {
                ms.Write(chunk, 0, bytesRead);
            }
            return ms;
        }
    }
}
