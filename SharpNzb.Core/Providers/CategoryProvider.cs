using System.Linq;
using SharpNzb.Core.Repository;
using SubSonic.Repository;

namespace SharpNzb.Core.Providers
{
    public class CategoryProvider : ICategoryProvider
    {
        private readonly IRepository _sonicRepo;

        public CategoryProvider(IRepository repo)
        {
            _sonicRepo = repo;
        }

        #region ICategoryProvider Members

        public Category Default()
        {
            return _sonicRepo.Single<Category>("default");
        }

        public Category Find(string name)
        {
            return _sonicRepo.Single<Category>(name.ToLower());
        }

        public void Add(Category category)
        {
            _sonicRepo.Add(category);
        }

        public void Delete(Category category)
        {
            _sonicRepo.Delete<Category>(category);
        }

        public IQueryable<Category> AllItems()
        {
            return _sonicRepo.All<Category>();
        }

        #endregion
    }
}
