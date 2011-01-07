using System;
using System.Linq;
using SharpNzb.Core.Repository;
using SubSonic.Repository;

namespace SharpNzb.Core.Providers
{
    public class HistoryProvider : IHistoryProvider
    {
        private IRepository _sonicRepo;

        public HistoryProvider(IRepository repo)
        {
            _sonicRepo = repo;
        }

        #region IHistoryProvider Members

        public void Add(History history)
        {
            _sonicRepo.Add(history);
        }

        public void Delete(Guid id)
        {
            _sonicRepo.Delete<History>(id);
        }

        public IQueryable<History> AllItems()
        {
            return _sonicRepo.All<History>();
        }

        public IQueryable<History> Range(int start, int count)
        {
            var list = _sonicRepo.All<History>().ToList();
            return list.GetRange(start, count).AsQueryable();
        }

        public void Purge()
        {
            _sonicRepo.DeleteMany(AllItems());
        }

        #endregion
    }
}
