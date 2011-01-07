using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public interface IYencProvider
    {
        void Decode(NzbFileModel file);
    }
}
