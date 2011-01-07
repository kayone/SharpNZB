using System;
using System.Linq;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public interface INzbQueueProvider
    {
        //Holds the NZB Queue
        bool MoveUp(Guid id);
        bool MoveToTop(Guid id);
        bool MoveDown(Guid id);
        bool MoveToBottom(Guid id);
        bool Swap(Guid idOne, Guid idTwo);
        bool Remove(Guid id);
        bool Purge();
        bool Add(NzbModel nzb);
        bool Insert(NzbModel nzb, int index);
        bool Sort();
        int Index(Guid id);
        IQueryable<NzbModel> AllItems();
        IQueryable<NzbModel> Range(int start, int count);
    }
}
