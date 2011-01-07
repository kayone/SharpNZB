namespace SharpNzb.Core.Providers
{
    public interface IDecompressProvider
    {
        bool Unrar(string path);
    }
}
