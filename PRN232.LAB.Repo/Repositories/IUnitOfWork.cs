using PRN232.LAB.Repo.Entities;
using System;
using System.Threading.Tasks;

namespace PRN232.LAB.Repo.Repositories
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
