using System.Linq;
using SharpNzb.Core.Repository;
using SubSonic.Repository;

namespace SharpNzb.Core.Providers
{
    public class ServerProvider : IServerProvider
    {
        private IRepository _sonicRepo;
        
        public ServerProvider(IRepository sonicRepo)
        {
            _sonicRepo = sonicRepo;
        }

        #region IServerProvder Members

        public void Add(Server server)
        {
            _sonicRepo.Add(server);
        }

        public void Delete(string name)
        {
            _sonicRepo.Delete<Server>(name);
        }

        public IQueryable<Server> GetAll()
        {
            return _sonicRepo.All<Server>();
        }

        public IQueryable<Server> GetEnabled()
        {
            return _sonicRepo.All<Server>().Where(s => s.Enabled);
        }

        public void Update(Server server)
        {
            _sonicRepo.Update(server);
        }

        #endregion
    }
}
