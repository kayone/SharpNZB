using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public interface IPreQueueProvider
    {
        bool Run(NzbModel nzb);
    }
}
