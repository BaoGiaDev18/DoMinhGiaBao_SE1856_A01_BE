using DoMinhGiaBao_SE1856_A01_Repository.Entities;
using System;
using System.Threading.Tasks;

namespace DoMinhGiaBao_SE1856_A01_Repository.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<SystemAccount> SystemAccounts { get; }
        IGenericRepository<NewsArticle> NewsArticles { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Tag> Tags { get; }
        
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
