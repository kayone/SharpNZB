using System.Linq;
using SharpNzb.Core.Repository;

namespace SharpNzb.Core.Providers
{
    public interface ICategoryProvider
    {
        Category Default();
        Category Find(string name);
        void Add(Category category);
        void Delete(Category category);
        IQueryable<Category> AllItems();
    }
}
