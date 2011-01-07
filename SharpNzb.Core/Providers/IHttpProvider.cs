using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public interface IHttpProvider
    {
        //Interface for Http Operations (getting NZB files, etc)
        void DownloadAsStream(NzbImportModel nzb);
    }
}
