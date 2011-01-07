using System.Linq;

namespace SharpNzb.Core.Instrumentation
{
    public interface ILogProvider
    {
        IQueryable<Log> GetAllLogs();
        void DeleteAll();
    }
}