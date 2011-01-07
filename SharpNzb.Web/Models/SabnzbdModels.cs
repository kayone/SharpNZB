using System.Collections.Generic;
using System.Linq;
using SharpNzb.Core.Model.Nzb;
using SharpNzb.Core.Repository;

namespace SharpNzb.Web.Models
{
    public class SabnzbdModels
    {
        public IQueryable<NzbModel> Queue { get; set; }
        public IQueryable<History> History { get; set; }
        public List<string> QueueTest { get; set; }
    }
}