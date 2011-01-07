using System;
using System.Linq;
using SharpNzb.Core.Repository;

namespace SharpNzb.Core.Providers
{
    public interface IHistoryProvider
    {
        IQueryable<History> AllItems();
        void Add(History history);
        void Delete(Guid id);
        void Purge();
        IQueryable<History> Range(int start, int count);
    }
}
