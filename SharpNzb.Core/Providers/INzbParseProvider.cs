using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public interface INzbParseProvider
    {
        NzbModel Process(NzbImportModel import);
    }
}
