using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public interface IEmailProvider
    {
        bool Send(NzbModel nzb);
    }
}
