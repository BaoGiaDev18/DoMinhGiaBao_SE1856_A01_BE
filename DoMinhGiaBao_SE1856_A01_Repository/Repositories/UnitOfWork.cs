using DoMinhGiaBao_SE1856_A01_Repository.Entities;
using System;
using System.Threading.Tasks;

namespace DoMinhGiaBao_SE1856_A01_Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FUNewsManagementContext _context;
        private IGenericRepository<SystemAccount>? _systemAccounts;
        private IGenericRepository<NewsArticle>? _newsArticles;
        private IGenericRepository<Category>? _categories;
        private IGenericRepository<Tag>? _tags;

        public UnitOfWork(FUNewsManagementContext context)
        {
            _context = context;
        }

        public IGenericRepository<SystemAccount> SystemAccounts
        {
            get
            {
                return _systemAccounts ??= new GenericRepository<SystemAccount>(_context);
            }
        }

        public IGenericRepository<NewsArticle> NewsArticles
        {
            get
            {
                return _newsArticles ??= new GenericRepository<NewsArticle>(_context);
            }
        }

        public IGenericRepository<Category> Categories
        {
            get
            {
                return _categories ??= new GenericRepository<Category>(_context);
            }
        }

        public IGenericRepository<Tag> Tags
        {
            get
            {
                return _tags ??= new GenericRepository<Tag>(_context);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
