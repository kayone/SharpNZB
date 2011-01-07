using SharpNzb.Core.Model;

namespace SharpNzb.Core.Providers
{
    public interface IFilenameParseProvider
    {
        TvShowParseModel ParseTv(string title);
    }
}
