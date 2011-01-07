using System;
using System.IO;
using System.Security.AccessControl;

namespace SharpNzb.Core.Providers
{
    class DiskProvider : IDiskProvider
    {
        #region IDiskProvider Members

        public bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path, string pattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, pattern, searchOption);
        }

        public long GetSize(string path)
        {
            return new FileInfo(path).Length;
        }

        public String CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path).FullName;
        }

        public Stream OpenAsStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public string SimpleFilename(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        #endregion
    }
}
