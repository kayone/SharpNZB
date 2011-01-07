using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public interface INzbImportProvider
    {
        void BeginImport(NzbImportModel import);
    }
}
