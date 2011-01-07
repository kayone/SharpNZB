using System.Linq;
using SharpNzb.Core.Repository;

namespace SharpNzb.Core.Providers
{
    public interface IServerProvider
    {
        IQueryable<Server> GetEnabled();
        IQueryable<Server> GetAll();
        void Add(Server server);
        void Delete(string name);
        void Update(Server server);
    }
}
