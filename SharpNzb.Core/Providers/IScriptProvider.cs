using System.Collections.Generic;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public interface IScriptProvider
    {
        List<string> AllScripts();
        bool Run(string name, NzbModel nzb);
    }
}
