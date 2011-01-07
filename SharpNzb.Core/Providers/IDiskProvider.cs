using System;
using System.IO;

namespace SharpNzb.Core.Providers
{
    public interface IDiskProvider
    {
        bool FolderExists(string path);
        string[] GetDirectories(string path);
        String CreateDirectory(string path);
        string[] GetFiles(string path, string pattern, SearchOption searchOption);
        bool FileExists(string path);
        long GetSize(string path);
        Stream OpenAsStream(string path);
        string SimpleFilename(string path);
    }
}
